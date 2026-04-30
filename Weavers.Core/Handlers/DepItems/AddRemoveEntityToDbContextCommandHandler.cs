using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.DepItems {

  public record AddRemoveEntityToDbContextCommand(int EntityItemId, bool Add) : IRequest<bool>;
  public class AddRemoveEntityToDbContextCommandHandler : IRequestHandler<AddRemoveEntityToDbContextCommand, bool> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public AddRemoveEntityToDbContextCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }


    public async Task<bool> Handle(AddRemoveEntityToDbContextCommand request, CancellationToken cancellationToken) {

      var LibraryItem = await _mediator.Send(new GetLibDiModelCommand(request.EntityItemId), cancellationToken);
      if (LibraryItem == null) { return false; }
      var dbContextItem = await _context.ResolveDbContextFromLib(LibraryItem, cancellationToken);
      if (dbContextItem == null) { return false; }

      var entityItem = await _context.GetItemDtoById(request.EntityItemId);
      if (entityItem == null) { return false; }

      if (request.Add) {         
        var existingImportId = await GetDbContextEntityImportIdForEntity(dbContextItem, request.EntityItemId, cancellationToken);
        if (existingImportId == null) {
          var command = new CreateRelatedItemCommand(dbContextItem.Id, 
            (int)WeRelationTypes.Contains, 
            (int)WeItemType.DbContextEntityImportModel, 
            entityItem.Name, "", "");
          var result = await _mediator.Send(command);
          if (result != null) {
            var classProp = result.Properties.Where(p => p.Name == Cx.ItRegisterObject).FirstOrDefault();
            if (classProp != null) {
              if (classProp.Value != entityItem.Id.ToString()) {
                var prop = _context.ItemProperties.Where(p => p.Id == classProp.Id).FirstOrDefault();
                if (prop != null) {
                  prop.Value = entityItem.Id.ToString();
                  _context.ItemProperties.Update(prop);
                  await _context.SaveChangesAsync(cancellationToken);
                }
              }
            }
            
          }
        }        
      } else {
        var itemToRemove = await this.GetDbContextEntityImportIdForEntity(dbContextItem, request.EntityItemId, cancellationToken);
        if (itemToRemove != null) {
          await _mediator.Send(new DeleteItemCommand(itemToRemove.Value), cancellationToken);
        }
      }
      return true;
    }


    private async Task<int?> GetDbContextEntityImportIdForEntity(ItemDto DbConextItem, int entityItemId, CancellationToken cancellationToken) {
      foreach (var importRel in DbConextItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.DbContextEntityImportModel)) {
        if (importRel.RelatedItemId != null) {
          var importItem = await _context.GetItemDtoById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            var prop = importItem?.Properties
              .FirstOrDefault(p => p.Name == Cx.ItRegisterObject && p.Value == entityItemId.ToString());
            if (prop != null) {
              return importItem?.Id;
            }
          }
        }
      }
      return null;
    }







    // below is end of name space and class.  do not change.
  }
}
