using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Models;
using Weavers.Core.Entities;
using Weavers.Core.Extensions;

namespace Weavers.Core.Extensions {
  public static class ItemResolveExt {

    public static string ResolveParentFolderPath(this ItemDto? item, string defaultPath) {
      if (item == null) return defaultPath;
      string propertyKey = item.ItemTypeId.GetFolderPropertyName();                  
      string Value = item.Properties.FirstOrDefault(p => p.Name == propertyKey)?.Value ?? defaultPath;
      if (item.ItemTypeId.IsFileNameType()) {
        Value = Path.GetDirectoryName(Value) ?? defaultPath; 
      }
      return Value;
    }

    public static string ResolveItemsNamespace(this ItemDto? item, string defaultNamespace) {
      if (item == null) return defaultNamespace;
      var propKey = item.ItemTypeId.GetNamespacePropertyName();
      return item.Properties.FirstOrDefault(p => p.Name == propKey)?.Value ?? defaultNamespace;
    }

    public static bool IsValidFolderParent(this ItemDto item) =>
      item.ItemTypeId == (int)WeItemType.OrganizationModel ||
      item.ItemTypeId == (int)WeItemType.OrgDocFolderModel ||
      item.ItemTypeId == (int)WeItemType.ProjectFolderModel ||
      item.ItemTypeId == (int)WeItemType.RelativeFolderModel;

    public static bool IsValidNamespaceParent(this ItemDto item) {       
      var propKey = item.ItemTypeId.GetNamespacePropertyName();
      return propKey != "";
    }

    public static string GetFileName(this ItemDto item) {
      if (item.ItemTypeId.IsFileNameType()) {
        var result = item.ItemTypeId switch {
          (int)WeItemType.OrgDocModel => item.Name.UrlSafe() + ".md",
          (int)WeItemType.FileMdModel => item.Name.UrlSafe() + ".md",
          (int)WeItemType.FileHtmlModel => item.Name.UrlSafe() + ".html",
          (int)WeItemType.FileConfigModel => item.Name.UrlSafe() + ".json",
          (int)WeItemType.SolutionModel => item.Name.UrlSafe() + ".sln",
          (int)WeItemType.LibraryModel => item.Name.UrlSafe() + ".csproj",
          (int)WeItemType.NamespaceModel => item.Name.UrlSafe(),
          (int)WeItemType.DependencyInjectionModel => "DependencyInjection.cs",
          (int)WeItemType.DbContextModel => item.Name.UrlSafe() + ".cs",
          _ => item.Name.Contains('.') ? item.Name : item.Name.UrlSafe() + ".cs"
        };
        return result;
      }
      return "";
    }



  }
}
