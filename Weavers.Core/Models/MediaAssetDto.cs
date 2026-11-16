using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;

namespace Weavers.Core.Models {
  public class MediaAssetDto {
    public int Id { get; set; }
    public byte[] FileHash { get; set; } = [];      // binary(16) for xxHash128, binary(32) for SHA-256. Fixed width so it can be indexed.
    public long FileSize { get; set; }
    public string MediaType { get; set; } = "";     // "image/jpeg", from FileTypeDetector, nvarchar(50)
    public DateTime? CapturedLocal { get; set; }    // nullable: "unknown" is a real answer
    public short? CapturedOffsetMin { get; set; }   // from OffsetTimeOriginal when present
    public byte CapturedSource { get; set; }        // Exif / Xmp / QuickTime / FileName / Mtime
    public int? Width { get; set; }
    public int? Height { get; set; }
    public byte Orientation { get; set; }           // EXIF 1–8; or store post-rotation display dims
    public int? DurationMs { get; set; }            // null for stills
    public string? CameraModel { get; set; }        // common filter, worth a real column
    public double? Lat { get; set; }
    public double? Lon { get; set; }
    public long? PHash { get; set; }
    public string? GroupKey { get; set; }           // Live Photo HEIC+MOV, RAW+JPG pairing
    public string MediaTags { get; set; } = "";     // JSON, see below

  }

  public static class MediaAssetDtoExtensions {
    public static MediaAssetDto ToDto(this MediaAsset asset) {
      return new MediaAssetDto {
        Id = asset.Id,
        FileHash = asset.FileHash,
        FileSize = asset.FileSize,
        MediaType = asset.MediaType,
        CapturedLocal = asset.CapturedLocal,
        CapturedOffsetMin = asset.CapturedOffsetMin,
        CapturedSource = asset.CapturedSource,
        Width = asset.Width,
        Height = asset.Height,
        Orientation = asset.Orientation,
        DurationMs = asset.DurationMs,
        CameraModel = asset.CameraModel,
        Lat = asset.Lat,
        Lon = asset.Lon,
        PHash = asset.PHash,
        GroupKey = asset.GroupKey,
        MediaTags = asset.MediaTags
      };
    }
  }

}
