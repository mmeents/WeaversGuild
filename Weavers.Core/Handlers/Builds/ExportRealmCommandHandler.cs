using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Builds {
  public class ExportResult {  
    public bool Success { get; set; } = true;
    public required string OutputFolder { get; init; }
    public string MediaFolderName => "media";
    public string MediaFolder => Path.Combine(OutputFolder, MediaFolderName);
    public HashSet<string> Written { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> Warnings { get; } = new();    
    public FolderEntryClass TheFolder { get; init; } = new();
    private static readonly HashSet<string> ReservedSlugs = new(StringComparer.OrdinalIgnoreCase) { "index", "loom", "realm", "story", "album", "media" };
    private readonly Dictionary<int, string> _slugById = new();
    private readonly HashSet<string> _slugs = new(ReservedSlugs, StringComparer.OrdinalIgnoreCase);

    // Stable within one export: an item related twice gets the same slug.
    public string SlugFor(int id, string kind) {
      if (_slugById.TryGetValue(id, out var existing)) return existing;
      var baseSlug = kind.ToUpper().Substring(0, 1) + id.ToString();
      if (string.IsNullOrWhiteSpace(baseSlug)) baseSlug = "item";
      var slug = _slugs.Add(baseSlug) ? baseSlug : $"{baseSlug}-{id}";
      _slugs.Add(slug);
      _slugById[id] = slug;
      return slug;
    }

    public ExportResult Fail(string message) {
      Success = false;
      Warnings.Add(message);
      return this;
    }
  }

  public record ExportItemCommand(int folderItemId) : IRequest<ExportResult>;
  public class ExportRealmCommandHandler : IRequestHandler<ExportItemCommand, ExportResult> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public ExportRealmCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<ExportResult> Handle(ExportItemCommand request, CancellationToken cancellationToken) {

      var folderItem = await _context.GetItemDtoById(request.folderItemId, cancellationToken);

      if (folderItem == null) return (new ExportResult() { OutputFolder = "0"})
          .Fail( $"Failed to lookup id {request.folderItemId}" );
      if (!folderItem.ItemTypeId.IsFolderType()) return (new ExportResult() { OutputFolder = "0" })
          .Fail($"Item {folderItem.Name} (id {folderItem.Id}) is not a folder type");
      
      string folderPath = folderItem.ResolveParentFolderPath("0");
      if (!folderPath.ValidatePath()) {
        return (new ExportResult() { OutputFolder = "0"})
          .Fail( $"Failed to resolve folder path for item {folderItem.Name} (id {folderItem.Id})" );
      }          

      var result = new ExportResult {
        OutputFolder = folderPath,      
        TheFolder = new FolderEntryClass {
          Id = folderItem.Id,
          Name = folderItem.Name,
          File = $"index.json"
        }
      };

      string mediaPath = result.MediaFolder;
      if (!mediaPath.ValidatePath()) {  // validate also creates if not there.
        return result.Fail($"Failed to resolve media folder path for item {folderItem.Name} (id {folderItem.Id})");
      }

      var relatedFolderItemIds = folderItem.Relations
        .Where(r => r.RelatedItemTypeId.HasValue && r.RelatedItemTypeId.Value.IsFolderType())
        .OrderBy(r => r.Rank)
        .Select(r => r.RelatedItemId!.Value)
        .ToList();

      if (relatedFolderItemIds.Count > 0) {
        foreach (var relatedFolderItemId in relatedFolderItemIds) {
          if (relatedFolderItemId <= 0) continue;
          var subResult = await _mediator.Send(new ExportItemCommand(relatedFolderItemId), cancellationToken);
          result.Warnings.AddRange(subResult.Warnings);
          result.Written.UnionWith(subResult.Written);
          if (!subResult.Success) {
            result.Fail($"Failed to export related folder item id {relatedFolderItemId}");
            continue;
          }
          var rel = Path.GetRelativePath(result.OutputFolder, subResult.OutputFolder).Replace('\\', '/');
          result.TheFolder.Folders.AddRange(subResult.TheFolder.LiftInto(rel));
        }
      }

      List<int> relatedRealmIds = folderItem.Relations
        .Where(r => r.RelatedItemTypeId.HasValue && r.RelatedItemTypeId.Value == (int)WeItemType.RealmModel)
        .OrderBy(r => r.Rank)
        .Select(r => r.RelatedItemId!.Value)
        .ToList();

      bool realmsExported = false;
      if (relatedRealmIds.Count > 0) {
        var realms = await _mediator.Send(new GetItemsByIdsQuery(relatedRealmIds));
        foreach (var realmId in relatedRealmIds) { 
          var realm = realms.FirstOrDefault(r => r.Id == realmId);
          if (realm != null) {
            if (realm.Properties.FirstOrDefault(p => p.Name == Cx.ItExportOnWrite)?.Value.AsBoolean() == true) {
              realmsExported = true;

              var mediaItems = realm.Relations
              .Where(r => r.RelatedItemTypeId.HasValue && r.RelatedItemTypeId.Value == (int)WeItemType.ComfyMediaFileModel)
              .OrderBy(r => r.Rank)
              .Select(r => r.RelatedItemId!.Value)
              .ToList();

              var mediaEntries = await mediaItems.ToMediaList(result, _mediator, cancellationToken);
              var coverImg = mediaEntries.FirstOrDefault(m => m.Kind == "image");

              result.TheFolder.Realms.Add(new RealmIndexEntry {
                Id = realm.Id,
                Name = realm.Name,                
                File = $"R{realm.Id}.json",
                CoverImage = coverImg?.Src ?? "",
                Blurb = StringExt.Blurb(realm.Description)
              });

              var linkEntryIds = realm.Relations
                .Where(r => r.RelatedItemTypeId.HasValue && r.RelatedItemTypeId.Value == (int)WeItemType.LinkedOnModel)
                .OrderBy(r => r.Rank)
                .Select(r => r.RelatedItemId!.Value).ToList();
              var links = await linkEntryIds.ToLinksList(result, _mediator, cancellationToken);

              var entryIds = realm.Relations
                .Where(r => r.RelatedItemTypeId.HasValue && (
                  r.RelatedItemTypeId.Value == (int)WeItemType.MusicAlbumModel 
                  || r.RelatedItemTypeId.Value == (int)WeItemType.StoryRollupModel))
                .OrderBy(r => r.Rank)
                .Select(r => r.RelatedItemId!.Value).ToList();
              var entries = await entryIds.ToEntriesList(realm, $"R{realm.Id}.json", result, _mediator, cancellationToken);

              RealmEntryClass realmEntry = new RealmEntryClass {
                Id = realm.Id,
                Name = realm.Name,
                File = $"R{realm.Id}.json",
                Blurb = StringExt.Blurb(realm.Description),
                Content = realm.Description ?? "",
                Images = mediaEntries,
                Links = links,
                Entries = entries
              };
              result.Written.Add(realmEntry.WriteRealmEntry(result.OutputFolder));


            }
            
          }
        }
        if (realmsExported) {
          result.Written.Add(result.TheFolder.WriteLoomCss(result.OutputFolder));
          result.Written.Add(result.TheFolder.WriteLoomJs(result.OutputFolder));
          result.Written.Add(result.TheFolder.WriteFolderEntry(result.OutputFolder));
          result.Written.Add(result.TheFolder.WriteIndexHtml(result.OutputFolder));
          result.Written.Add(result.TheFolder.WriteRealmHtml(result.OutputFolder));
          result.Written.Add(result.TheFolder.WriteStoryHtml(result.OutputFolder));
          result.Written.Add(result.TheFolder.WriteAlbumHtml(result.OutputFolder));

        }

      }

      // folder exported is a index.html and index.json for folder for index, -- ok
      // write nested folders  -- ok
      // write related realms  -- here.
      // Combine result from folder and realm exports into a single result

      return result;
    }




  }


  public static class ExportExts {

    public static async Task<List<MediaEntryClass>> ToMediaList(this List<int>? mediaItemIds, ExportResult context, IMediator mediator, CancellationToken cancellationToken) {
      var result = new List<MediaEntryClass>();
      if (mediaItemIds == null || mediaItemIds.Count == 0) return result;

      var mediaItems = await mediator.Send(new GetItemsByIdsQuery(mediaItemIds), cancellationToken);
      foreach (var mediaItemId in mediaItemIds) {
        var mediaItem = mediaItems.FirstOrDefault(m => m.Id == mediaItemId);
        if (mediaItem == null) continue;

        var filePath = mediaItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value ?? "";
        var publishedUrl = mediaItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPublishedUrl)?.Value ?? "";
        var kind = filePath.GetKindFromPathExt();

        if (!string.IsNullOrWhiteSpace(publishedUrl)) {
          result.Add(new MediaEntryClass { Id = mediaItem.Id, Kind = kind, Src = publishedUrl });
          continue;
        }

        // unset => type default: images ship, audio/video stay home
        var flag = mediaItem.Properties.FirstOrDefault(p => p.Name == Cx.ItExportOnWrite)?.Value;
        bool export = string.IsNullOrWhiteSpace(flag) ? kind == "image" : flag.AsBoolean();
        if (!export) continue;

        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) {
          context.Warnings.Add($"Media {mediaItem.Id} ({mediaItem.Name}) file not found; skipped");
          continue;
        }

        var fileName = $"m{mediaItem.Id}{Path.GetExtension(filePath).ToLowerInvariant()}";
        var dest = Path.Combine(context.MediaFolder, fileName);
        if (context.Written.Add(dest)) File.Copy(filePath, dest, true);   // same media under realm + entry copies once

        result.Add(new MediaEntryClass { Id = mediaItem.Id, Kind = kind, Src = $"{context.MediaFolderName}/{fileName}" });
      }
      return result;
    }

    public static async Task<List<LinkEntryClass>> ToLinksList(this List<int>? linkItemIds, ExportResult context, IMediator mediator, CancellationToken cancellationToken) { 
      var result = new List<LinkEntryClass>();
      if (linkItemIds != null && linkItemIds.Count > 0) {
        var linkItems = await mediator.Send(new GetItemsByIdsQuery(linkItemIds), cancellationToken);
        foreach (var id in linkItemIds) {
          var linkItem = linkItems.FirstOrDefault(i => i.Id == id);
          if (linkItem == null) continue;
          var urlProp = linkItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPublishedUrl);
          var iconProp = linkItem.Properties.FirstOrDefault(p => p.Name == Cx.ItLinkIconUrl);
          result.Add(new LinkEntryClass {
            Id = linkItem.Id,
            Name = linkItem.Name,
            Url = urlProp?.Value ?? "",
            IconUrl = iconProp?.Value ?? "",
            Embed = linkItem.Properties.FirstOrDefault(p => p.Name == Cx.ItLinkAsEmbed)?.Value ?? ""
          });
        }
      }
      return result;
    }

    public static async Task<List<AlbumTrackEntry>> ToTracksList(this List<int>? trackIds, ExportResult context, IMediator mediator, CancellationToken cancellationToken) {
      var result = new List<AlbumTrackEntry>();
      if (trackIds == null || trackIds.Count == 0) return result;

      var tracks = await mediator.Send(new GetItemsByIdsQuery(trackIds), cancellationToken);
      foreach (var id in trackIds) {
        var track = tracks.FirstOrDefault(t => t.Id == id);
        if (track == null) continue;

        var flag = track.Properties.FirstOrDefault(p => p.Name == Cx.ItExportOnWrite)?.Value;
        if (!string.IsNullOrWhiteSpace(flag) && !flag.AsBoolean()) continue;   // unset = export

        result.Add(new AlbumTrackEntry {
          Id = track.Id,
          Name = track.Name,
          Content = track.Description ?? "",
          Credits = track.Properties.FirstOrDefault(p => p.Name == Cx.ItCredits)?.Value ?? "",
          Media = await track.RelatedIds(WeItemType.ComfyMediaFileModel).ToMediaList(context, mediator, cancellationToken),
          Links = await track.RelatedIds(WeItemType.LinkedOnModel).ToLinksList(context, mediator, cancellationToken)
        });
      }
      return result;
    }

    public static List<int> RelatedIds(this ItemDto item, WeItemType type) =>
      item.Relations
        .Where(r => r.RelatedItemTypeId == (int)type && r.RelatedItemId.HasValue)
        .OrderBy(r => r.Rank)
        .Select(r => r.RelatedItemId!.Value)
        .ToList();

    public static async Task<List<RealmItemEntryClass>> ToEntriesList(this List<int>? entryItemIds, ItemDto realm, string realmFile,
        ExportResult context, IMediator mediator, CancellationToken cancellationToken) {
      var result = new List<RealmItemEntryClass>();
      if (entryItemIds == null || entryItemIds.Count == 0) return result;

      var entryItems = await mediator.Send(new GetItemsByIdsQuery(entryItemIds), cancellationToken);
      foreach (var id in entryItemIds) {
        var entryItem = entryItems.FirstOrDefault(i => i.Id == id);
        if (entryItem == null) continue;

        var flag = entryItem.Properties.FirstOrDefault(p => p.Name == Cx.ItExportOnWrite)?.Value;
        if (!string.IsNullOrWhiteSpace(flag) && !flag.AsBoolean()) continue;   // unset = export

        var kind = entryItem.ItemTypeId switch {          
          (int)WeItemType.MusicAlbumModel => "album",
          (int)WeItemType.StoryRollupModel => "story",
          _ => "unknown"
        };
        var slug = context.SlugFor(entryItem.Id, kind);
        var storyFile = $"{slug}.json";        

        var media = await entryItem.RelatedIds(WeItemType.ComfyMediaFileModel).ToMediaList(context, mediator, cancellationToken);
        var cover = media.FirstOrDefault(m => m.Kind == "image")?.Src ?? "";
        var content = (entryItem.Description ?? "").ReplaceLineEndings("\n");
        var published = entryItem.Established.ToString("yyyy-MM-dd");
        var blurb = StringExt.Blurb(content);
        var words = kind == "story" ? StringExt.WordCount(content) : 0;

        if (kind == "story") {
          var links = await entryItem.RelatedIds(WeItemType.LinkedOnModel).ToLinksList(context, mediator, cancellationToken);
          var story = new StoryEntryClass {
            Id = entryItem.Id,
            Slug = slug,
            Name = entryItem.Name,
            File = storyFile,
            Published = published,
            Blurb = blurb,
            Words = words,
            Content = content.ReplaceLineEndings("\n"),
            Credits = entryItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCredits)?.Value ?? "",
            RealmName = realm.Name,
            RealmFile = realmFile,
            RealmNote = entryItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRealm)?.Value ?? "",
            Cover = cover,
            Media = media,
            Links = links
          };
          context.Written.Add(story.WriteJsonFile(context.OutputFolder, storyFile));
        } else if (kind == "album") {
          var tracks = await entryItem.RelatedIds(WeItemType.AlbumTrackModel).ToTracksList(context, mediator, cancellationToken);

          // no album art of its own -> borrow the first track image; flows to the realm shelf too
          if (string.IsNullOrEmpty(cover))
            cover = tracks.SelectMany(t => t.Media).FirstOrDefault(m => m.Kind == "image")?.Src ?? "";

          var album = new AlbumEntryClass {
            Id = entryItem.Id,
            Slug = slug,
            Name = entryItem.Name,
            File = storyFile,
            Published = published,
            Blurb = blurb,
            Content = content.ReplaceLineEndings("\n"),
            Credits = entryItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCredits)?.Value ?? "",
            RealmName = realm.Name,
            RealmFile = realmFile,
            Cover = cover,
            Media = media,
            Links = await entryItem.RelatedIds(WeItemType.LinkedOnModel).ToLinksList(context, mediator, cancellationToken),
            Tracks = tracks
          };
          context.Written.Add(album.WriteJsonFile(context.OutputFolder, storyFile));
        }

        result.Add(new RealmItemEntryClass {
          Id = entryItem.Id,
          Name = entryItem.Name,
          Kind = kind,
          File = storyFile,
          CoverImgUrl = cover,
          Published = published,
          Blurb = blurb,
          Words = words
        });
      }
      return result;
    }

    public static List<FolderEntryClass> FoldersWithRealms(this FolderEntryClass folder) { 
      var result = new List<FolderEntryClass>();
      if (folder.Realms.Count > 0) {
        result.Add(folder);
      }
      foreach (var subFolder in folder.Folders) {
        result.AddRange(subFolder.FoldersWithRealms());
      }
      return result;
    }

    public static string GetKindFromPathExt(this string path) {
      if (string.IsNullOrWhiteSpace(path)) return "unknown";
      var ext = Path.GetExtension(path).ToLowerInvariant();
      return ext switch {
        ".jpg" or ".jpeg" => "image",
        ".png" => "image",
        ".gif" => "image",
        ".bmp" => "image",
        ".tiff" => "image",
        ".webp" => "image",
        ".mp4" => "video",
        ".mov" => "video",
        ".avi" => "video",
        ".mkv" => "video",
        ".mp3" => "audio",
        ".wav" => "audio",
        ".flac" => "audio",
        _ => "unknown"
      };
    }

    // Each level is already flattened by its own export, so only one level needs re-prefixing.
    // Realm File/CoverImage paths stay relative to their folder's index.json.
    public static IEnumerable<FolderEntryClass> LiftInto(this FolderEntryClass sub, string relToParent) {
      static string Join(string a, string b) => a == "." ? b : $"{a}/{b}";
      if (sub.Realms.Count > 0)
        yield return new FolderEntryClass { Id = sub.Id, Name = sub.Name, File = Join(relToParent, sub.File), Realms = sub.Realms };
      foreach (var child in sub.Folders)
        yield return new FolderEntryClass { Id = child.Id, Name = child.Name, File = Join(relToParent, child.File), Realms = child.Realms };
    }

    private static readonly JsonSerializerOptions JsonOut = new() { WriteIndented = true };

    public static string WriteJsonFile<T>(this T obj, string outputFolder, string fileName) {
      if (obj == null) return "";
      var filePath = Path.Combine(outputFolder, fileName);
      File.WriteAllText(filePath, JsonSerializer.Serialize(obj, JsonOut));
      return filePath;
    }


  }

  // folder index level
 public class FolderEntryClass {

    [JsonPropertyName("id")]
    public int Id { get; init; } = 0;

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("file")]
    public string File { get; init; } = "";   

    [JsonPropertyName("folders")]
    public List<FolderEntryClass> Folders { get; init; } = new();

    [JsonPropertyName("realms")]
    public List<RealmIndexEntry> Realms { get; init; } = new();

  } 
  public class RealmIndexEntry {

    [JsonPropertyName("id")]
    public int Id { get; init; } = 0;

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("file")]
    public string File { get; init; } = "";


    [JsonPropertyName("coverImage")]
    public string CoverImage { get; init; } = "";

    [JsonPropertyName("blurb")]
    public string Blurb { get; init; } = "";

  }

  // realm level
  public class RealmEntryClass {

    [JsonPropertyName("id")]
    public int Id { get; init; } = 0;

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("file")]
    public string File { get; init; } = "";

   [JsonPropertyName("blurb")]
    public string Blurb { get; init; } = "";

    [JsonPropertyName("content")]
    public string Content { get; init; } = "";

    [JsonPropertyName("images")]
    public List<MediaEntryClass> Images { get; init; } = new();

    [JsonPropertyName("links")]
    public List<LinkEntryClass> Links { get; init; } = new();

    [JsonPropertyName("entries")]
    public List<RealmItemEntryClass> Entries { get; init; } = new();

  }
  
  // either a album or story exists in a realm
  public class RealmItemEntryClass {

    // item id of the album or story
    [JsonPropertyName("id")]
    public int Id { get; init; } = 0;

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    // album or story
    [JsonPropertyName("kind")]
    public string Kind { get; init; } = "";

    [JsonPropertyName("published")]
    public string Published { get; init; } = "";

    [JsonPropertyName("blurb")]
    public string Blurb { get; init; } = "";

    [JsonPropertyName("words")]
    public int Words { get; init; }

    // computed A for album or S for story followed by the Id with .json extension
    [JsonPropertyName("file")]
    public string File { get; init; } = "";

    [JsonPropertyName("cover")]
    public string CoverImgUrl { get; init; } = "";

  }

  public class StoryEntryClass {
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("slug")] public string Slug { get; init; } = "";
    [JsonPropertyName("name")] public string Name { get; init; } = "";
    [JsonPropertyName("kind")] public string Kind { get; init; } = "story";
    [JsonPropertyName("file")] public string File { get; init; } = "";
    [JsonPropertyName("published")] public string Published { get; init; } = "";
    [JsonPropertyName("blurb")] public string Blurb { get; init; } = "";
    [JsonPropertyName("words")] public int Words { get; init; }
    [JsonPropertyName("content")] public string Content { get; init; } = "";
    [JsonPropertyName("credits")] public string Credits { get; init; } = "";
    [JsonPropertyName("realmName")] public string RealmName { get; init; } = "";
    [JsonPropertyName("realmFile")] public string RealmFile { get; init; } = "";
    [JsonPropertyName("realmNote")] public string RealmNote { get; init; } = "";
    [JsonPropertyName("cover")] public string Cover { get; init; } = "";
    [JsonPropertyName("media")] public List<MediaEntryClass> Media { get; init; } = new();
    [JsonPropertyName("links")] public List<LinkEntryClass> Links { get; init; } = new();
  }

  // Album-Story level
  public class AlbumEntryClass {
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("slug")] public string Slug { get; init; } = "";
    [JsonPropertyName("name")] public string Name { get; init; } = "";
    [JsonPropertyName("kind")] public string Kind { get; init; } = "album";
    [JsonPropertyName("file")] public string File { get; init; } = "";
    [JsonPropertyName("published")] public string Published { get; init; } = "";
    [JsonPropertyName("blurb")] public string Blurb { get; init; } = "";
    [JsonPropertyName("content")] public string Content { get; init; } = "";   // liner notes (Description)
    [JsonPropertyName("credits")] public string Credits { get; init; } = "";
    [JsonPropertyName("realmName")] public string RealmName { get; init; } = "";
    [JsonPropertyName("realmFile")] public string RealmFile { get; init; } = "";
    [JsonPropertyName("cover")] public string Cover { get; init; } = "";
    [JsonPropertyName("media")] public List<MediaEntryClass> Media { get; init; } = new();
    [JsonPropertyName("links")] public List<LinkEntryClass> Links { get; init; } = new();
    [JsonPropertyName("tracks")] public List<AlbumTrackEntry> Tracks { get; init; } = new();
  }

  public class AlbumTrackEntry {
    [JsonPropertyName("id")]
    public int Id { get; init; } = 0;

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("credits")]
    public string Credits { get; init; } = "";

    [JsonPropertyName("content")]
    public string Content { get; init; } = "";   // lyrics / notes (Description)

    [JsonPropertyName("media")]
    public List<MediaEntryClass> Media { get; init; } = new();

    [JsonPropertyName("links")]
    public List<LinkEntryClass> Links { get; init; } = new();

  }


  // media level
  public class MediaEntryClass { 
    [JsonPropertyName("id")]
    public int Id { get; init; } = 0;

    [JsonPropertyName("kind")]
    public string Kind { get; init; } = "";

    [JsonPropertyName("src")]
    public string Src { get; init; } = "";
  }

  public class LinkEntryClass { 
    [JsonPropertyName("id")]
    public int Id { get; init; } = 0;

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("url")]
    public string Url { get; init; } = "";

    [JsonPropertyName("iconUrl")]
    public string IconUrl { get; init; } = "";

    [JsonPropertyName("embed")]
    public string Embed { get; init; } = "";
  }

}
