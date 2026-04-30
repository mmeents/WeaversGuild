using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ItemPropsExt {
    public static string GetFileName(this ItemDto item) => item.ItemTypeId switch {
      (int)WeItemType.FileModel => item.Name.UrlSafe() + ".md",
      (int)WeItemType.SolutionModel => item.Name.UrlSafe() + ".sln",
      (int)WeItemType.LibraryModel => item.Name.UrlSafe() + ".csproj",
      (int)WeItemType.NamespaceModel => item.Name.UrlSafe(),
      (int)WeItemType.DependencyInjectionModel => "DependencyInjection.cs",
      (int)WeItemType.DbContextModel => item.Name.UrlSafe() + ".cs",
      _ => item.Name.Contains('.') ? item.Name : item.Name.UrlSafe() + ".cs"
    };

    public static void AddOrUpdateProperty(this ItemDto item, ItemPropertyDto property) {
      var existingProperty = item.Properties.FirstOrDefault(p => p.Name == property.Name);
      if (existingProperty != null) {
        existingProperty.Value = property.Value;
      } else {
        item.Properties.Add(property);
      }
    }

    public static async Task SaveProp(this ItemPropertyDto property, ItemDto item, IMediator mediator) {
      var updatedProp = await mediator.Send(new AddUpdateItemPropertyCommand(
        property.Id,
        property.ItemId,
        property.Name,
        property.Value,
        property.ValueDataTypeId,
        property.EditorTypeId,
        property.ReferenceItemTypeId
      ));
      if (updatedProp != null) {        
        if (item != null) {
          item.AddOrUpdateProperty(updatedProp);
        }
      }
    }
    




  }
}
