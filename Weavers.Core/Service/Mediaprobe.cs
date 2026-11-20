using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.Icc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Weavers.Core.Entities;
using MdDirectory = MetadataExtractor.Directory;

namespace Weavers.Core.Service {
  public enum CapturedSource : byte { None = 0, Exif = 1, QuickTime = 2, Xmp = 3, FileName = 4, FileMtime = 5 }

  public sealed record ProbeResult(MediaAsset? Asset, string? Error);

  /// <summary>
  /// One read of a file: SHA-256 over the full stream, then MetadataExtractor over the header.
  /// Returns a detached MediaAsset ready to insert (or to discard if the hash already exists).
  /// </summary>
  public static partial class MediaProbe {
    const int MaxTagValue = 512;

    static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase) {
    ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".webp", ".heic", ".heif", ".avif",
    ".dng", ".cr2", ".cr3", ".nef", ".arw", ".raf", ".orf", ".rw2",
    ".mov", ".mp4", ".m4v", ".3gp", ".avi", ".mkv", ".mp3"
  };

    static readonly JsonSerializerOptions JsonOpts = new() {
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,   // no \uXXXX bloat; stored, never rendered as HTML
    };

    static readonly HashSet<string> WidthNames = new(StringComparer.Ordinal) { "Image Width", "Exif Image Width", "Width" };
    static readonly HashSet<string> HeightNames = new(StringComparer.Ordinal) { "Image Height", "Exif Image Height", "Height" };

    public static bool IsMediaExtension(string path) => Extensions.Contains(Path.GetExtension(path));

    public static async Task<ProbeResult> ProbeAsync(string path, CancellationToken ct) {
      try {
        await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
                                            bufferSize: 1 << 20, FileOptions.Asynchronous | FileOptions.SequentialScan);
        var asset = new MediaAsset {
          FileHash = await SHA256.HashDataAsync(fs, ct),
          FileSize = fs.Length,
          MediaType = MimeFromExtension(path),
        };

        fs.Position = 0;
        IReadOnlyList<MdDirectory> dirs;
        try { 
          dirs = ImageMetadataReader.ReadMetadata(fs); 
        } catch (Exception) { // bad/unsupported metadata must not block cataloguing
          dirs = Array.Empty<MdDirectory>(); 
        }   

        Fill(asset, dirs, path);
        return new ProbeResult(asset, null);
      } catch (OperationCanceledException) { throw; } catch (Exception ex) { return new ProbeResult(null, ex.Message); }   // locked, still syncing, vanished...
    }

    static void Fill(MediaAsset a, IReadOnlyList<MdDirectory> dirs, string path) {
      var mime = dirs.OfType<FileTypeDirectory>().FirstOrDefault()?.GetDescription(FileTypeDirectory.TagDetectedFileMimeType);
      if (!string.IsNullOrWhiteSpace(mime)) a.MediaType = Trunc(mime, 50)!;

      var ifd0 = dirs.OfType<ExifIfd0Directory>().FirstOrDefault();
      var sub = dirs.OfType<ExifSubIfdDirectory>().FirstOrDefault();

      a.CameraModel = Trunc(ifd0?.GetDescription(ExifDirectoryBase.TagModel)?.Trim(), 100);
      if (ifd0 is not null && ifd0.TryGetInt32(ExifDirectoryBase.TagOrientation, out var o) && o is >= 1 and <= 8)
        a.Orientation = (byte)o;

      (a.Width, a.Height) = FindDimensions(dirs);
      a.DurationMs = FindDurationMs(dirs);

      // Works whether GetGeoLocation returns a class or a nullable struct in your package version.
      if (dirs.OfType<GpsDirectory>().FirstOrDefault()?.GetGeoLocation() is { } geo &&
          !(geo.Latitude == 0 && geo.Longitude == 0)) {
        a.Lat = geo.Latitude;
        a.Lon = geo.Longitude;
      }

      SetCaptured(a, sub, dirs, path);
      a.MediaTags = BuildTagsJson(dirs);
    }

    // ── Capture date: EXIF -> QuickTime -> filename -> mtime, recording which won ──
    static void SetCaptured(MediaAsset a, ExifSubIfdDirectory? sub, IReadOnlyList<MdDirectory> dirs, string path) {
      const int TagOffsetTimeOriginal = 0x9011;   // "+05:00", written by most phones since ~2017

      if (sub is not null &&
          (sub.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var exif) ||
           sub.TryGetDateTime(ExifDirectoryBase.TagDateTimeDigitized, out exif)) &&
          exif.Year > 1970) {
        a.CapturedLocal = DateTime.SpecifyKind(exif, DateTimeKind.Unspecified);
        a.CapturedOffsetMin = ParseOffsetMinutes(sub.GetString(TagOffsetTimeOriginal));
        a.CapturedSource = (byte)CapturedSource.Exif;
        return;
      }

      // Apple: com.apple.quicktime.creationdate, e.g. "2019-07-04T15:22:11-0500" (has the zone).
      foreach (var d in dirs.Where(d => d.Name.StartsWith("QuickTime", StringComparison.Ordinal))) {
        var tag = d.Tags.FirstOrDefault(t => t.Name == "Creation Date");
        if (tag is null) continue;
        var s = d.GetString(tag.Type);
        if (s is null) continue;
        s = TrailingOffset().Replace(s.Trim(), "$1:$2");
        if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto) && dto.Year > 1970) {
          a.CapturedLocal = DateTime.SpecifyKind(dto.DateTime, DateTimeKind.Unspecified);
          a.CapturedOffsetMin = (short)dto.Offset.TotalMinutes;
          a.CapturedSource = (byte)CapturedSource.QuickTime;
          return;
        }
      }

      // Movie header "Created" is UTC by spec (many cameras write local anyway; can't tell).
      foreach (var d in dirs.Where(d => d.Name == "QuickTime Movie Header" || d.Name.StartsWith("MP4", StringComparison.Ordinal))) {
        var tag = d.Tags.FirstOrDefault(t => t.Name is "Created" or "Creation Time");
        if (tag is not null && d.TryGetDateTime(tag.Type, out var utc) && utc.Year > 1970) {
          a.CapturedLocal = DateTime.SpecifyKind(utc, DateTimeKind.Unspecified);
          a.CapturedOffsetMin = 0;
          a.CapturedSource = (byte)CapturedSource.QuickTime;
          return;
        }
      }

      // IMG_20190704_152211.jpg, PXL_20230101_120000123.jpg, 2019-07-04 15.22.11.jpg ...
      var m = FileNameStamp().Match(Path.GetFileNameWithoutExtension(path));
      if (m.Success && DateTime.TryParseExact(
            string.Concat(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value, m.Groups[5].Value, m.Groups[6].Value),
            "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var named)) {
        a.CapturedLocal = named;
        a.CapturedOffsetMin = null;
        a.CapturedSource = (byte)CapturedSource.FileName;
        return;
      }

      // Last resort. For iCloud downloads this is often the download time, hence the source flag.
      var mtimeLocal = File.GetLastWriteTime(path);
      a.CapturedLocal = DateTime.SpecifyKind(mtimeLocal, DateTimeKind.Unspecified);
      a.CapturedOffsetMin = (short)TimeZoneInfo.Local.GetUtcOffset(mtimeLocal).TotalMinutes;
      a.CapturedSource = (byte)CapturedSource.FileMtime;
    }

    static short? ParseOffsetMinutes(string? s) {
      if (string.IsNullOrWhiteSpace(s)) return null;
      s = s.Trim();
      if (s.Length < 6 || (s[0] != '+' && s[0] != '-')) return null;
      if (!int.TryParse(s.AsSpan(1, 2), out var h) || !int.TryParse(s.AsSpan(4, 2), out var mm)) return null;
      var total = h * 60 + mm;
      return (short)(s[0] == '-' ? -total : total);
    }

    // ── Dimensions / duration: name-based so one routine covers JPEG, PNG, HEIC, TIFF, QuickTime ──
    static (int?, int?) FindDimensions(IReadOnlyList<MdDirectory> dirs) {
      foreach (var d in dirs) {
        if (d is ExifThumbnailDirectory) continue;
        int? w = null, h = null;
        foreach (var t in d.Tags) {
          if (w is null && WidthNames.Contains(t.Name) && d.TryGetInt32(t.Type, out var wv) && wv > 0) w = wv;
          if (h is null && HeightNames.Contains(t.Name) && d.TryGetInt32(t.Type, out var hv) && hv > 0) h = hv;
        }
        if (w is not null && h is not null) return (w, h);
      }
      return (null, null);
    }

    static int? FindDurationMs(IReadOnlyList<MdDirectory> dirs) {
      foreach (var d in dirs)
        foreach (var t in d.Tags) {
          if (t.Name is not ("Duration" or "Duration in Seconds")) continue;
          switch (d.GetObject(t.Type)) {
            case TimeSpan ts when ts > TimeSpan.Zero:
              return (int)Math.Min(int.MaxValue, ts.TotalMilliseconds);
            case double sec when t.Name == "Duration in Seconds" && sec > 0:
              return (int)(sec * 1000);
          }
        }
      return null;
    }

    // ── MediaTags: {"Exif IFD0": {"Make": "Apple", ...}, "GPS": {...}} ──
    static string BuildTagsJson(IReadOnlyList<MdDirectory> dirs) {
      var root = new Dictionary<string, Dictionary<string, string>>();
      foreach (var d in dirs) {
        if (d is ExifThumbnailDirectory or IccDirectory) continue;    // the bulk, with no search value
        var tags = new Dictionary<string, string>();
        foreach (var t in d.Tags) {
          if (t.Name.StartsWith("Unknown tag", StringComparison.Ordinal)) continue;
          string? v;
          try { v = t.Description; } catch { continue; }
          if (string.IsNullOrWhiteSpace(v)) continue;
          tags[t.Name] = v.Length > MaxTagValue ? v[..MaxTagValue] + "…" : v;
        }
        if (tags.Count == 0) continue;
        var key = d.Name;
        for (var n = 2; root.ContainsKey(key); n++) key = $"{d.Name} #{n}";
        root[key] = tags;
      }
      return JsonSerializer.Serialize(root, JsonOpts);
    }

    static string MimeFromExtension(string path) => Path.GetExtension(path).ToLowerInvariant() switch {
      ".jpg" or ".jpeg" => "image/jpeg",
      ".png" => "image/png",
      ".gif" => "image/gif",
      ".heic" or ".heif" => "image/heic",
      ".mov" => "video/quicktime",
      ".mp4" or ".m4v" => "video/mp4",
      _ => "application/octet-stream",
    };

    static string? Trunc(string? s, int max) => s is null || s.Length <= max ? s : s[..max];

    [GeneratedRegex(@"([+-]\d{2})(\d{2})$")]
    private static partial Regex TrailingOffset();

    [GeneratedRegex(@"(?<!\d)((?:19|20)\d{2})[-_.]?(\d{2})[-_.]?(\d{2})[-_ T.]?(\d{2})[-_.:]?(\d{2})[-_.:]?(\d{2})")]
    private static partial Regex FileNameStamp();
  }

}
