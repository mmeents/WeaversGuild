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
  public record AddCallSheetRoleCommand(int callSheetId, string name, string instruction, int todoId) : IMcpRequest, IRequest<ItemDto?>;
  public class AddCallSheetRoleCommandHandler : IRequestHandler<AddCallSheetRoleCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddCallSheetRoleCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(AddCallSheetRoleCommand request, CancellationToken cancellationToken) {

      var callSheetItem = await _context.GetItemDtoById(request.callSheetId);
      if (callSheetItem == null) { throw new Exception($"Parent call sheet with id {request.callSheetId} not found"); }
      if (callSheetItem.ItemTypeId != (int)WeItemType.CallSheetModel) { throw new Exception($"Parent item {request.callSheetId} with type {(WeItemType)callSheetItem.ItemTypeId} is not a {WeItemType.CallSheetModel}"); }


      var beatId = callSheetItem.GetParentId();
      var beatItem = await _context.GetItemDtoById(beatId);
      if (beatItem == null) { throw new Exception($"Beat item with id {beatId} not found."); }
      var sceneId = beatItem.GetParentId();
      var sceneItem = await _context.GetItemDtoById(sceneId);
      if (sceneItem == null) { throw new Exception($"Scene item with id {sceneId} not found."); }

      ItemDto? charItem = null;
      var charItemRel = sceneItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.CharacterModel 
        && string.Compare(r.RelatedItemName, request.name, true) == 0);
      if (charItemRel == null) {
        charItem = await _mediator.Send(
          new CreateRelatedItemCommand(sceneItem.Id, (int)WeRelationTypes.Contains,
            (int)WeItemType.CharacterModel, request.name, "", "{}"));
      } else {
        charItem = await _context.GetItemDtoById(charItemRel.RelatedItemId ?? 0);
      }
      if (charItem == null) { throw new Exception($"error getting character details."); }

      var script = string.IsNullOrWhiteSpace(callSheetItem.Data) || callSheetItem.Data == "{}" ?
        new CallSheetScript()
        : JsonSerializer.Deserialize<CallSheetScript>(callSheetItem.Data) ?? new CallSheetScript();
      var nextRank = script.Script.Any() ? script.Script.Max(s => s.Rank) + 1 : 1;

      script.Script.Add(new CallSheetScriptItem {
        Rank = nextRank,
        Type = Cx.RoleType,
        CharacterId = charItem.Id,
        Name = request.name,
        Instruction = request.instruction
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
