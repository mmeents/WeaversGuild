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
using Weavers.Core.Service;


namespace Weavers.Core.Handlers.Items {
  public record UpdateItemPropertyPathRecursiveCommand(int ItemId, string NewPath) : IRequest;
  public class UpdateItemPropertyPathRecursiveCommandHandler(
    FabricDbContext context, IMediator mediator, ISessionItemCacheService sessionCache
  ) : IRequestHandler<UpdateItemPropertyPathRecursiveCommand> {
    private readonly FabricDbContext _context = context;
    private readonly IMediator _mediator = mediator;
    private readonly ISessionItemCacheService _sessionCache = sessionCache;
    public async Task Handle(UpdateItemPropertyPathRecursiveCommand request, CancellationToken cancellationToken) {
      await WalkAndUpdate(request.ItemId, request.NewPath, cancellationToken);
    }

    private async Task WalkAndUpdate(int itemId, string newBase, CancellationToken ct) {      
      var item = await _sessionCache.GetItemAsync(itemId, ct);
      if (item == null) return;

      string? parentPath = null;
      if (item.ItemTypeId == (int)WeItemType.OrganizationModel) {
        parentPath = newBase;
      } else {
        parentPath = await _mediator.Send(new GetParentItemPathPropertyQuery(itemId));
      }
      if (parentPath == null) return;

      string propKey = item.ItemTypeId.GetFolderPropertyName();
      if (propKey == "") return;

      var prop = item.Properties.FirstOrDefault(p => p.Name == propKey);
      if (prop != null) {
        if (item.ItemTypeId.IsAParentFolder()) {
          var fullPath = Path.Combine(parentPath, item.Name.UrlSafe());
          await UpdatePathProperty(prop, fullPath);
        } else if (propKey == Cx.ItFilePath) {
          var fullPath = "";
          var fileName = "";
          if (propKey == Cx.ItFilePath) {
            fileName = item.GetFileName();
            fullPath = Path.Combine(parentPath, fileName);
          } else {
            fullPath = parentPath;
          }
          await UpdatePathProperty(prop, fullPath);
        } else {
          await UpdatePathProperty(prop, parentPath);
        }
      }
      
      var childIds = item.Relations
        .Where(r => r.RelationTypeId == (int)WeRelationTypes.Contains)
        .Select(r => r.RelatedItemId)
        .Where(id => id.HasValue)
        .Select(id => id!.Value)
        .ToList();

      foreach (var childId in childIds)
        await WalkAndUpdate(childId, newBase, ct);
      
    }

    private async Task UpdatePathProperty(Models.ItemPropertyDto prop, string path) {
      var itemProp = await _context.ItemProperties.FindAsync(prop.Id);
      if (itemProp != null) {
        itemProp.Value = path;
        _context.ItemProperties.Update(itemProp);
        await _context.SaveChangesAsync();
        await _context.MarkItemUpdated(itemProp.ItemId);
        _sessionCache.RemoveCacheItem(prop.ItemId); 
      }
    }

  }
}
