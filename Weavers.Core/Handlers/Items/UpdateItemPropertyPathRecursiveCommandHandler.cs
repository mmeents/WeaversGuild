using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Items {
  public record UpdateItemPropertyPathRecursiveCommand(int ItemId, string OldPath, string NewPath) : IRequest;
  public class UpdateItemPropertyPathRecursiveCommandHandler(
    FabricDbContext context, IMediator mediator
  ) : IRequestHandler<UpdateItemPropertyPathRecursiveCommand> {
    private readonly FabricDbContext _context = context;
    private readonly IMediator _mediator = mediator;
    public async Task Handle(UpdateItemPropertyPathRecursiveCommand request, CancellationToken cancellationToken) {
      await WalkAndUpdate(request.ItemId, request.OldPath, request.NewPath, cancellationToken);
    }

    private async Task WalkAndUpdate(int itemId, string oldBase, string newBase, CancellationToken ct) {
      var item = await _context.GetItemDtoById(itemId, ct);
      if (item == null) return;

      string? parentPath = null;
      if (item.ItemTypeId == (int)WeItemType.ProjectFolderModel) { 
        parentPath = newBase; 
      } else {
        parentPath = await _mediator.Send(new GetParentItemPathPropertyQuery(itemId));
      }      
      if (parentPath == null) return;

      string propKey = item.ItemTypeId.GetFolderPropertyName();
      if (propKey == "") return;      
      if (propKey != null) {
        var prop = item.Properties.FirstOrDefault(p => p.Name == propKey);
        if (prop != null ) {      
          var fullPath = "";
          var fileName = "";
          if (propKey == Cx.ItFilePath) {            
            fileName = item.GetFileName();
            fullPath = Path.Combine(parentPath, fileName);
          } else if (item.ItemTypeId == (int)WeItemType.RelativeFolderModel) {
            fullPath = Path.Combine(parentPath, item.Name.UrlSafe());
          } else {
            fullPath = parentPath;
          }
          await UpdatePathProperty(prop, fullPath);
        }
      }     
      
      var childIds = item.Relations
        .Where(r => r.RelationTypeId == (int)WeRelationTypes.Contains)
        .Select(r => r.RelatedItemId)
        .Where(id => id.HasValue)
        .Select(id => id!.Value)
        .ToList();

      foreach (var childId in childIds)
        await WalkAndUpdate(childId, oldBase, newBase, ct);
      
    }

    private async Task UpdatePathProperty(Models.ItemPropertyDto prop, string path) {
      var itemProp = await _context.ItemProperties.FindAsync(prop.Id);
      if (itemProp != null) {
        itemProp.Value = path;
        _context.ItemProperties.Update(itemProp);
        await _context.SaveChangesAsync();
      }
    }

  }
}
