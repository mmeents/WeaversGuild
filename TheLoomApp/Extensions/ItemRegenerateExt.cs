
using System.IO;
using System.Xml.Linq;
using TheLoomApp.Models;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Models;
using Weavers.Core.Extensions;
using Microsoft.VisualBasic.Logging;

namespace TheLoomApp.Extensions {
  public static class ItemRegenerateExt {
    public static async Task<List<string>> Regenerate(this ItemNode node, List<string> log) {
      if (log == null) log = new List<string>();
      if (node?.Item == null) return log;
      await DispatchRegenerateAsync(node, log);
      foreach (ItemNode child in node.Nodes)
        await child.Regenerate(log);
      return log;
    }

    

    private static async Task DispatchRegenerateAsync(this ItemNode node, List<string> log) {
      if (node?.Item == null) return;
      switch (node.Item.ItemTypeId) {
        // Tier 1 — folders
        case (int)WeItemType.ProjectFolderModel:
          EnsureFolderExistsAsync(node.Item, log); break;
        case (int)WeItemType.RelativeFolderModel:
          EnsureFolderExistsAsync(node.Item, log); break;
        case (int)WeItemType.NamespaceModel:
          EnsureNamespaceFolderAsync(node, log); break;
        // Tier 2 — composites
        case (int)WeItemType.LibraryModel:
          RegenerateLibraryAsync(node, log); break;
        case (int)WeItemType.DependencyInjectionModel:
          await RegenerateDiFileAsync( node, log); break;        
        case (int)WeItemType.HandlerModel:
          await RegenerateHandlerFileAsync(node, log); break; 
        // Tier 3 — leaves
        case (int)WeItemType.FileModel:
          RegenerateFileModelAsync(node.Item, log); break;
        case (int)WeItemType.InterfaceModel:
          await RegenerateInterfaceFileAsync(node, log); break;
        case (int)WeItemType.ClassModel:
          await RegenerateClassFileAsync(node, log); break;
        case (int)WeItemType.RecordModel:
        case (int)WeItemType.StructModel:
          await RegenerateSimpleTypeFileAsync(node, log); break;

        // Tier 4 — fragments, skip (parent handles)  
        default: break;
      }
    }

    public static void EnsureFolderExistsAsync(ItemDto item, List<string> log) {
      if (item == null) return;
      string parentFolderPath = item.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
      if (string.IsNullOrEmpty(parentFolderPath)) {
        log.Add($"Cannot determine parent folder path for item '{item.Name}' (Id: {item.Id}). Skipping generation.");
        return;
      }
      if (!Directory.Exists(parentFolderPath)) {
        try {
          Directory.CreateDirectory(parentFolderPath);
          log.Add($"Created folder at '{parentFolderPath}' for item '{item.Name}' (Id: {item.Id}).");
        } catch (Exception ex) {
          log.Add($"Failed to create folder at '{parentFolderPath}' for item '{item.Name}' (Id: {item.Id}). Error: {ex.Message}");
        }
      } else {
        log.Add($"Folder already exists at '{parentFolderPath}' for item '{item.Name}' (Id: {item.Id}).");
      }
    }

    public static void RegenerateFileModelAsync(ItemDto item, List<string> log) {
      var filePathProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (filePathProp == null || string.IsNullOrEmpty(filePathProp.Value)) {
        log.Add($"Item '{item.Name}' (Id: {item.Id}) is missing a valid file path property. Skipping file generation.");
        return;
      }
      try {
        var filePath = filePathProp.Value;
        if (string.IsNullOrEmpty(filePath)) {
          log.Add($"Item '{item.Name}' (Id: {item.Id}) has an empty file path. Skipping file generation.");
          return;
        }
        if (item.Description.Length <= 0) {
          log.Add($"Item '{item.Name}' (Id: {item.Id}) has an empty description. Skipping file generation.");
        } else {
          File.WriteAllText(filePath, $"{item.Description}");
          log.Add($"Generated file for item '{item.Name}' at '{filePath}'.");
        }
        if (item.Data.Length > "{  }".Length) { // added space to simulate hard returns.
          var dataFilePath = Path.GetFileNameWithoutExtension(filePath) + ".json";
          File.WriteAllText(dataFilePath, item.Data);
          log.Add($"Generated data file for item '{item.Name}' at '{dataFilePath}'.");
        }

      } catch (Exception ex) {
        log.Add($"Failed to generate file for item '{item.Name}' at '{filePathProp.Value}'. Error: {ex.Message}");
      }
    }

    public static void EnsureNamespaceFolderAsync(ItemNode node, List<string> log) { }
    public static void RegenerateLibraryAsync(ItemNode node, List<string> log) {
      var item = node.Item;
      if (item == null) { 
        log.Add($"Node is missing an associated item. Skipping library file generation.");
        return;  
      }
      var filePathProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (filePathProp == null || string.IsNullOrEmpty(filePathProp.Value)) {
        log.Add($"Item '{item.Name}' (Id: {item.Id}) is missing a valid file path property. Skipping library file generation.");
        return;      
      }
      try {         
        var filePath = filePathProp.Value;
        if (string.IsNullOrEmpty(filePath)) {
          log.Add($"Item '{item.Name}' (Id: {item.Id}) has an empty file path. Skipping library file generation.");
          return;
        }
        var libraryContent = $"{item.Description}";
        File.WriteAllText(filePath, libraryContent);
        log.Add($"Generated library file for item '{item.Name}' at '{filePath}'.");
      } catch (Exception ex) {
        log.Add($"Failed to generate library file for item '{item.Name}' at '{filePathProp.Value}'. Error: {ex.Message}");
      }
    }
    public static async Task RegenerateDiFileAsync(ItemNode node, List<string> log) {
      await Task.Delay(10);
    }
    public static async Task RegenerateEntityFileAsync(ItemNode node, List<string> log) {
      await Task.Delay(10);
    }
    public static async Task RegenerateHandlerFileAsync(ItemNode node, List<string> log) {
      await Task.Delay(10);    
      }
    public static async Task RegenerateInterfaceFileAsync(ItemNode node, List<string> log) {
      await Task.Delay(10);    
      }
    public static async Task RegenerateClassFileAsync(ItemNode node, List<string> log) {
      await Task.Delay(10);
    }
    public static async Task RegenerateSimpleTypeFileAsync(ItemNode node, List<string> log) {
      await Task.Delay(10);
    }






  }
}
