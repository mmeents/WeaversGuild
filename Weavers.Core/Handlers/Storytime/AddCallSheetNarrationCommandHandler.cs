using MediatR;
using System.Text.Json;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Storytime {
  public record AddCallSheetNarrationCommand(int callSheetId, string section, string narration, int todoId) : IMcpRequest, IRequest<ItemDto?>;
  public class AddCallSheetNarrationCommandHandler : IRequestHandler<AddCallSheetNarrationCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddCallSheetNarrationCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(AddCallSheetNarrationCommand request, CancellationToken cancellationToken) {
      var callSheetItem = await _context.GetItemDtoById(request.callSheetId);
      if (callSheetItem == null) { 
        throw new Exception($"Parent call sheet with id {request.callSheetId} not found"); 
      }
      if (callSheetItem.ItemTypeId != (int)WeItemType.CallSheetModel) { 
        throw new Exception($"Parent item {request.callSheetId} with type {(WeItemType)callSheetItem.ItemTypeId} is not a {WeItemType.CallSheetModel}"); 
      }

      var script = string.IsNullOrWhiteSpace(callSheetItem.Data) || callSheetItem.Data == "{}" ?
        new CallSheetScript()
        : JsonSerializer.Deserialize<CallSheetScript>(callSheetItem.Data) ?? new CallSheetScript();

      var nextRank = script.Script.Any() ? script.Script.Max(s => s.Rank) + 1 : 1;
      script.Script.Add(new CallSheetScriptItem {
        Rank = nextRank,
        Type = Cx.NarrationType,
        Name = request.section,
        Instruction = request.narration
      });

      callSheetItem.Data = JsonSerializer.Serialize(script);
      
      var addedByProp = callSheetItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
      if (addedByProp != null && request.todoId > 0) {
        var attribution = await _mediator.Send(new ResolveAttributionQuery(request.todoId));
        addedByProp.Value = attribution.PresenceModelKey;
        await addedByProp.SaveProp(callSheetItem, _mediator);
      }
        
      var updated = await _mediator.Send(callSheetItem.ToUpdateCmd());
      return updated;
    }
  }
}
