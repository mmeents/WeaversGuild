using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {

  public record UpdateItemPropertyNamespaceRecursiveCommand(int ItemId, string OldNamespace, string NewNamespace) : IRequest;

  public class UpdateItemPropertyNamespaceRecursiveCommandHandler(FabricDbContext context) :IRequestHandler<UpdateItemPropertyNamespaceRecursiveCommand> {
    private readonly FabricDbContext _context = context;    
    public async Task Handle(UpdateItemPropertyNamespaceRecursiveCommand request, CancellationToken cancellationToken) {
      await WalkAndUpdate(request.ItemId, request.OldNamespace, request.NewNamespace, cancellationToken);
    }
    private async Task WalkAndUpdate(int itemId, string oldBase, string newBase, CancellationToken ct) {
      var item = await _context.GetItemDtoById(itemId, ct);
      if (item == null) return;
      string? propKey = item.ItemTypeId switch {
        (int)WeItemType.LibraryModel => Cx.ItNamespaceRoot,
        (int)WeItemType.DependencyInjectionModel => Cx.ItNamespace,
        (int)WeItemType.NamespaceModel => Cx.ItNamespace,
        (int)WeItemType.InterfaceModel => Cx.ItNamespace,
        (int)WeItemType.RecordModel => Cx.ItNamespace,
        (int)WeItemType.StructModel => Cx.ItNamespace,
        (int)WeItemType.ClassModel => Cx.ItNamespace,        
        (int)WeItemType.EntityClassModel => Cx.ItNamespace,
        (int)WeItemType.EntityConfigurationModel => Cx.ItNamespace,        
        _ => null
      };      
      if (propKey != null) {
        var prop = item.Properties.FirstOrDefault(p => p.Name == propKey);
        if (prop != null && prop.Value?.StartsWith(oldBase) == true) {       
          var fullNamespace = newBase + prop.Value.Substring(oldBase.Length);          
          await UpdateNamespaceProperty(prop, fullNamespace);
        }
      }
      var childIds = item.Relations
        .Where(r => r.RelationTypeId == (int)WeRelationTypes.Contains)
        .Select(r => r.RelatedItemId)
        .Where(id => id.HasValue)
        .Select(id => id!.Value)
        .ToList();

      foreach (var childId in childIds) {
        await WalkAndUpdate(childId, oldBase, newBase, ct);
      }  
    }

    private async Task UpdateNamespaceProperty(ItemPropertyDto prop, string namespaceName) { 
      var itemProp = await _context.ItemProperties.FindAsync(prop.Id);
      if (itemProp != null) {
        itemProp.Value = namespaceName;
        _context.ItemProperties.Update(itemProp);
        await _context.SaveChangesAsync();
      }
    }



  }
}
