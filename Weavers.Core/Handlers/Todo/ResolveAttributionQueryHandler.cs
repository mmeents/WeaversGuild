using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Presence;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Extensions;

namespace Weavers.Core.Handlers.Todo {

  public record ResolveAttributionQuery(int TodoId) : IRequest<ResolveAttributionQueryResult>;

  public record ResolveAttributionQueryResult(int DeskId, int OperatorId, int PresenceId, string PresenceModelKey);
  public class ResolveAttributionQueryHandler : IRequestHandler<ResolveAttributionQuery, ResolveAttributionQueryResult> {
    private readonly FabricDbContext _context;
    public ResolveAttributionQueryHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<ResolveAttributionQueryResult> Handle(ResolveAttributionQuery request, CancellationToken cancellationToken) {

      var todoItem = await _context.GetItemDtoById(request.TodoId, cancellationToken);  // todo
      if (todoItem == null) throw new Exception($"Todo item with ID {request.TodoId} not found.");            
      
      // Desk from todo item        
      var desk = await _context.TryGetParentDesk(todoItem, cancellationToken);
      if (desk == null) throw new Exception($"Desk not found for Todo item with ID {request.TodoId}.");
      var deskId = desk.Id;          

      // Operator from desk
      var operatorId = desk.Properties.FirstOrDefault(p => p.Name == Cx.ItOperator)?.Value.AsInt();
      if (operatorId == null || operatorId == 0) throw new Exception($"Operator not configured for Desk with ID {deskId}.");

      var operatorItem = await _context.GetItemDtoById(operatorId.Value, cancellationToken);
      if (operatorItem == null || operatorItem.ItemTypeId != (int)WeItemType.DigitalOperatorModel) {
        throw new Exception($"Operator with ID {operatorId.Value} not found.");
      }     

      var presId = operatorItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPresence)?.Value.AsInt();
      if (presId == null || presId == 0) {
        throw new Exception($"Presence not configured for Operator with ID {operatorId.Value}.");
      }
      
      var presenceItem = await _context.GetItemDtoById(presId.Value, cancellationToken);
      if (presenceItem == null || (
        presenceItem.ItemTypeId != (int)WeItemType.PresModelLmStudioModel 
        && presenceItem.ItemTypeId != (int)WeItemType.PresModelClaudeModel 
        && presenceItem.ItemTypeId != (int)WeItemType.PresModelHumanModel)) {
        throw new Exception($"Presence with ID {presId.Value} not found.");
      }

      var presenceModel = "";
      if (presenceItem.ItemTypeId == (int)WeItemType.PresModelLmStudioModel
        || presenceItem.ItemTypeId == (int)WeItemType.PresModelClaudeModel) {
        presenceModel = presenceItem.Properties.FirstOrDefault(p => p.Name == Cx.ItModelKey)?.Value ?? string.Empty;
      } else {
        presenceModel = presenceItem.Name ?? string.Empty;
      }
        
      
      return new ResolveAttributionQueryResult(deskId, operatorId.Value, presId.Value, presenceModel);
    }
  }
}
