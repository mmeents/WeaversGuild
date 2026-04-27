using Azure.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.DepItems {
  public record AddRemoveClassToLibDiCommand(int ClassItemId, bool Add, bool GenerateInterface) : IRequest<bool>;
  public class AddRemoveClassToLibDiCommandHandler : IRequestHandler<AddRemoveClassToLibDiCommand, bool> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;

    public AddRemoveClassToLibDiCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }


    public async Task<bool> Handle(AddRemoveClassToLibDiCommand request, CancellationToken cancellationToken) {
      
      var LibraryItem = await _mediator.Send(new GetLibDiModelCommand(request.ClassItemId), cancellationToken);
      if (LibraryItem == null) {
        return false;
      }
      var diItemid = LibraryItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.DependencyInjectionModel).Select(r => r.RelatedItemId).FirstOrDefault();
      if (diItemid == null) {
        return false;
      }
      var DiItem = await _context.GetItemDtoById(diItemid.Value);
      if (DiItem == null) {
        return false;
      }
      var classItem = await _context.GetItemDtoById(request.ClassItemId);
      if (classItem == null) {
        return false;
      }

      if (request.Add) {
        var existingImportId = await this.GetLibDiImportIdForClass(DiItem, request.ClassItemId, cancellationToken);
        if (existingImportId == null) {        
          var command = new CreateRelatedItemCommand(DiItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.DiImportModel, classItem.Name, "", "");
          var result = await _mediator.Send(command);
          if (result != null) { 
            var classProp = result.Properties.Where(p => p.Name == Cx.ItRegisterObject).FirstOrDefault();
            if (classProp != null) {
              if (classProp.Value != classItem.Id.ToString()) {
                var prop = _context.ItemProperties.Where(p => p.Id == classProp.Id).FirstOrDefault();
                if (prop != null) {
                    prop.Value = classItem.Id.ToString();
                    _context.ItemProperties.Update(prop);
                    await _context.SaveChangesAsync(cancellationToken);
                }                
              }
            }            
            var interfaceProp = result.Properties.Where(p => p.Name == Cx.ItRegisterInterface).FirstOrDefault();
            if (interfaceProp != null) {
              if (interfaceProp.Value != request.GenerateInterface.ToString()) {
                var prop = _context.ItemProperties.Where(p => p.Id == interfaceProp.Id).FirstOrDefault();
                if (prop != null) {
                    prop.Value = request.GenerateInterface.ToString();
                    _context.ItemProperties.Update(prop);
                    await _context.SaveChangesAsync(cancellationToken);
                }
              }
            }
          }
        }

      } else {
        var itemToRemove = await this.GetLibDiImportIdForClass(DiItem, request.ClassItemId, cancellationToken);
        if (itemToRemove != null) {
          await _mediator.Send(new DeleteItemCommand(itemToRemove.Value), cancellationToken);
        }
      }
      return true;
    }

    private async Task<int?> GetLibDiImportIdForClass(ItemDto DepItem, int classItemId, CancellationToken cancellationToken) {
      foreach (var importRel in DepItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.DiImportModel)) {
        if (importRel.RelatedItemId != null) {
          var importItem = await _context.GetItemDtoById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            var prop = importItem?.Properties
              .FirstOrDefault(p => p.Name == Cx.ItRegisterObject && p.Value == classItemId.ToString());
            if (prop != null) {
              return importItem?.Id;              
            }
          }
        }
      }
      return null;
    }
  }
}