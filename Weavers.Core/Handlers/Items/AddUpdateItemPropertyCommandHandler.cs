using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {

  public record AddUpdateItemPropertyCommand(
    int Id,
    int ItemId,
    string PropertyName,
    string? PropertyValue = null,
    int? PropertyValueTypeId = null,
    int? PropertyEditorTypeId = null,
    int? ReferenceItemTypeId = null
  ) : IRequest<ItemPropertyDto>;

  public class AddUpdateItemPropertyCommandHandler : IRequestHandler<AddUpdateItemPropertyCommand, ItemPropertyDto> {
    private readonly FabricDbContext _context;    

    public AddUpdateItemPropertyCommandHandler(
      FabricDbContext context ) {
      _context = context;
    }

    public async Task<ItemPropertyDto> Handle(AddUpdateItemPropertyCommand request, CancellationToken cancellationToken) {
      DateTime markUtc = DateTime.UtcNow;
      
      ItemProperty? property;
      int id = 0;

      if (request.Id == 0) {
        // Create new property
        property = new ItemProperty(          
          request.ItemId,
          request.PropertyName,
          request.PropertyValue,
          request.PropertyValueTypeId,
          request.ReferenceItemTypeId,
          request.PropertyEditorTypeId);

        await _context.ItemProperties.AddAsync(property, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
      } else {
        // Update existing property
        id = request.Id;
        property = await _context.ItemProperties.FindAsync(new object[] { request.Id }, cancellationToken);
        if (property == null) {
          throw new InvalidOperationException($"ModelProperty with id {request.Id} not found");
        }

        property.Update(request.PropertyValue, request.PropertyValueTypeId, request.ReferenceItemTypeId,  request.PropertyEditorTypeId);
        _context.ItemProperties.Update(property);
        await _context.SaveChangesAsync(cancellationToken);
      }

      property = await _context.ItemProperties.FindAsync(new object[] { id }, cancellationToken);
      
      var response = property?.ToDto() ?? throw new Exception("Property not found after update");
      
      return response;
    }
  }
}
