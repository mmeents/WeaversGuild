using AngleSharp.Dom;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Weavers.Core.Handlers.Builds {
  public record WriteDocumentCommand(int ItemId) : IRequest<WriteDocumentCmdResult>;

  public class WriteDocumentCmdResult {
    public bool Success { get; set; }
    public string Message { get; set; } = "";
  }

  public class WriteDocumentCommandHandler : IRequestHandler<WriteDocumentCommand, WriteDocumentCmdResult> {
    private readonly FabricDbContext _context;
    public WriteDocumentCommandHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<WriteDocumentCmdResult> Handle(WriteDocumentCommand request, CancellationToken cancellationToken) {

      var item = await _context.GetItemDtoById(request.ItemId, cancellationToken);
      if (item == null) return new WriteDocumentCmdResult { Success = false, Message = $"Failed to lookup id {request.ItemId}" };
      if (!item.ItemTypeId.IsContentType()) return new WriteDocumentCmdResult { 
        Success = false, 
        Message = $"Item type {(WeItemType)item.ItemTypeId} is not document type." 
      };

      if (item.ItemTypeId == (int)WeItemType.RealmModel) {
        try {
          await WriteRealm(item);
          return new WriteDocumentCmdResult { Success = true, Message = $"Realm documents written successfully." };
        } catch (Exception ex) {
          return new WriteDocumentCmdResult { Success = false, Message = $"Failed writing realm documents: {ex.Message}" };
        }
      } else if (item.ItemTypeId == (int)WeItemType.StoryRollupModel) {
        try {
          await WriteStoryRollup(item);
          return new WriteDocumentCmdResult { Success = true, Message = $"Story rollup document written successfully." };
        } catch (Exception ex) {
          return new WriteDocumentCmdResult { Success = false, Message = $"Failed writing story rollup document: {ex.Message}" };
        }
      }

      var templateContent = item.Description;
    
      var folderProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (folderProp == null) return new WriteDocumentCmdResult { Success = false, Message = $"Failed to find filename property in item" };

      var fileNamePath = folderProp.Value;
      if (string.IsNullOrEmpty(fileNamePath)) return new WriteDocumentCmdResult { Success = false, Message = $"Filename is empty." };

      var filesFolder = Path.GetDirectoryName(fileNamePath);
      if (filesFolder != null && !Directory.Exists(filesFolder)) {
        try {
          Directory.CreateDirectory(filesFolder);
        } catch (Exception ex) {
          return new WriteDocumentCmdResult { Success = false, Message = $"Failed Ex creating directory for {filesFolder} {ex.Message} " };
        }
      } 

      try {
        if (File.Exists(fileNamePath)) {
          File.Delete(fileNamePath);
        }

        File.WriteAllText(fileNamePath, templateContent);

        return new WriteDocumentCmdResult { Success = true, Message = "Document written successfully." };
      } catch (Exception ex) {
        return new WriteDocumentCmdResult { Success = false, Message = $"Failed Exception {ex.Message}" };
      }

    }

    public async Task WriteRealm(ItemDto realmItem) {
      if (realmItem == null) throw new Exception($"Realm item is null");

      var realmParentId = realmItem?.IncomingRelations.FirstOrDefault(r => r.ItemTypeId.IsFolderType())?.ItemId;
      if (realmParentId == null || realmParentId == 0) {
        throw new Exception($"Realm parent folder not found for realm {realmItem?.Name} (id {realmItem?.Id}) needed for folder location.");
      }
      var realmParentItem = await _context.GetItemDtoById(realmParentId.Value, CancellationToken.None);
      var realmFolderProp = realmParentItem.ResolveParentFolderPath("0");
      if (realmFolderProp == null || realmFolderProp.StartsWith("0")) { 
        throw new Exception($"Failed to resolve folder path for realm parent {realmParentItem?.Name} (id {realmParentItem?.Id})");
      } 
      
      var storyIds = realmItem!.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.StoryRollupModel)
        .Select(r => r.RelatedItemId).Where(id => id != null && id != 0).ToList();
      var indexList = new IndexList();
      foreach (var storyId in storyIds) { 
        var storyItem = await _context.GetItemDtoById(storyId!.Value, CancellationToken.None);
        if (storyItem == null) continue;
        var indexEntry = await BuildIndexEntry(realmItem, storyItem, realmFolderProp);
        indexList.Stories.Add(indexEntry);
        await WriteStoryRollup(storyItem);
      }

      if (indexList.Stories.Count == 0) { throw new Exception($"No index entries found for realm item {realmItem.Name} (id {realmItem.Id})"); }

      var fileNamePath = Path.Combine(realmFolderProp, $"index.json");
      if (string.IsNullOrEmpty(fileNamePath)) throw new Exception($"Filename is empty for realm item {realmItem.Name} (id {realmItem.Id})");
      var filesFolder = Path.GetDirectoryName(fileNamePath);
      if (filesFolder != null && !Directory.Exists(filesFolder)) {
        try {
          Directory.CreateDirectory(filesFolder);
        } catch (Exception ex) {
          throw new Exception($"Failed creating directory for {filesFolder} {ex.Message}");
        }
      }
      try {
        if (File.Exists(fileNamePath)) {
          File.Delete(fileNamePath);
        }
        var dataOut = JsonSerializer.Serialize(indexList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileNamePath, dataOut);
      } catch (Exception ex) {
        throw new Exception($"Failed writing realm file {fileNamePath} Exception: {ex.Message}");
      }
    }

    private async Task<IndexEntry> BuildIndexEntry(ItemDto realmItem, ItemDto storyItem, string destinationFolder) {
      if (storyItem == null) throw new Exception($"StoryItem is null");
      if (realmItem == null) throw new Exception($"RealmItem is null");      
      return new IndexEntry {
        Slug = storyItem.Name.UrlSafe(),
        Name = storyItem.Name,        
        Published = storyItem.Established.ToString("yyyy-MM-dd"),
        Blurb = StringExt.Blurb(storyItem.Description),
        Words = StringExt.WordCount(storyItem.Description),
        RealmName = realmItem.Name,
      };
    }

    public async Task WriteStoryRollup(ItemDto StoryItem) { 
      if (StoryItem == null) throw new Exception($"StoryItem is null");

      var parentRealmId = StoryItem.IncomingRelations.FirstOrDefault(r => r.ItemTypeId == (int)WeItemType.RealmModel)?.ItemId;
      var realmItem = await _context.GetItemDtoById(parentRealmId ?? 0, CancellationToken.None);

      var realmParentId = realmItem?.IncomingRelations.FirstOrDefault(r => r.ItemTypeId.IsFolderType())?.ItemId;
      if (realmParentId == null || realmParentId == 0) {
        throw new Exception($"Realm parent folder not found for realm {realmItem?.Name} (id {realmItem?.Id}) needed for folder location.");
      }
      var realmParentItem = await _context.GetItemDtoById(realmParentId.Value, CancellationToken.None);
      var realmFolderProp = realmParentItem.ResolveParentFolderPath("0");
      if (realmFolderProp == null || realmFolderProp.StartsWith("0")) { 
        throw new Exception($"Failed to resolve folder path for realm parent {realmParentItem?.Name} (id {realmParentItem?.Id})");
      }
      var outputFilePath = Path.Combine(realmFolderProp, $"{StoryItem.Name.UrlSafe()}.json");
      var creditsProp = StoryItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCredits);
      var credits = creditsProp?.Value ?? "";

      var realmProp = StoryItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRealm);
      var realmNote = realmProp?.Value ?? "";
      var indexEntry = new ItemEntry {
        Slug = StoryItem.Name.UrlSafe(),
        Name = StoryItem.Name,        
        Published = StoryItem.Established.ToString("yyyy-MM-dd"),
        Blurb = StringExt.Blurb(StoryItem.Description),
        Words = StringExt.WordCount(StoryItem.Description),
        Content = StoryItem.Description,
        RealmName = realmItem!.Name,
        RealmNote = realmNote,
        RealmProse = realmItem.Description,
        Credits = credits
      };

      if (string.IsNullOrEmpty(outputFilePath)) throw new Exception($"Filename is empty for realm item {realmItem.Name} (id {realmItem.Id})");
      var filesFolder = Path.GetDirectoryName(outputFilePath);
      if (filesFolder != null && !Directory.Exists(filesFolder)) {
        try {
          Directory.CreateDirectory(filesFolder);
        } catch (Exception ex) {
          throw new Exception($"Failed creating directory for {filesFolder} {ex.Message}");
        }
      }
      try {
        if (File.Exists(outputFilePath)) {
          File.Delete(outputFilePath);
        }
        var dataOut = JsonSerializer.Serialize(indexEntry, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputFilePath, dataOut);
      } catch (Exception ex) {
        throw new Exception($"Failed writing realm file {outputFilePath} Exception: {ex.Message}");
      }

    }

  }

  public class ItemEntry {
    [JsonPropertyName("slug")]
    public string Slug { get; init; } = "";

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("published")]
    public string Published { get; init; } = "";

    [JsonPropertyName("blurb")]
    public string Blurb { get; init; } = "";

    [JsonPropertyName("content")]
    public string Content { get; init; } = "";

    [JsonPropertyName("realmName")]
    public string RealmName { get; init; } = "";

    [JsonPropertyName("realmNote")]
    public string RealmNote { get; init; } = "";

    [JsonPropertyName("realmProse")]
    public string RealmProse { get; init; } = "";

    [JsonPropertyName("words")]
    public int Words { get; init; }

    [JsonPropertyName("credits")]
    public string Credits { get; init; } = "";
  }


  public class IndexList {
    [JsonPropertyName("generated")]
    public string Generated { get; init; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

    [JsonPropertyName("stories")]
    public List<IndexEntry> Stories { get; init; } = new List<IndexEntry>();
  }

  public class IndexEntry {
    [JsonPropertyName("slug")] 
    public string Slug { get; init; } = "";

    [JsonPropertyName("name")] 
    public string Name { get; init; } = "";    

    [JsonPropertyName("published")] 
    public string Published { get; init; } = "";

    [JsonPropertyName("blurb")] 
    public string Blurb { get; init; } = "";
   
    [JsonPropertyName("realmName")]
    public string RealmName { get; init; } = "";    

    [JsonPropertyName("words")] 
    public int Words { get; init; }
    
  }

}
