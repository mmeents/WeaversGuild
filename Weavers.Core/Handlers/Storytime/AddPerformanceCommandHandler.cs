using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.ItemSummaries;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;
using Weavers.Core.Service;
using System.Text.Json;

namespace Weavers.Core.Handlers.Storytime {
  public record AddPerformanceCommand(int sceneId, string name) : IMcpRequest, IRequest<ItemDto?>;
  public class AddPerformanceCommandHandler : IRequestHandler<AddPerformanceCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddPerformanceCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }

    public async Task<ItemDto?> Handle(AddPerformanceCommand request, CancellationToken cancellationToken) {
      var sceneItem = await _context.GetItemDtoById(request.sceneId);
      if (sceneItem == null) { throw new Exception($"Parent scene with id {request.sceneId} not found"); }
      if (sceneItem.ItemTypeId != (int)WeItemType.SceneModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)sceneItem.ItemTypeId}; requires a {WeItemType.SceneModel} type {(int)WeItemType.SceneModel} parent.");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(sceneItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.PerformanceModel, request.name, "", "{}"));
      if (newItem != null) {
        var script = new PerformanceScript();
        var callSheetList = await _mediator.Send(new GetKidsByTypeRecQuery(sceneItem.Id, (int)WeItemType.CallSheetModel));
        var pRank = 0;
        foreach (var callSheet in callSheetList) {

          var callSheetScript = string.IsNullOrWhiteSpace(callSheet.Data) || callSheet.Data == "{}" ?
            new CallSheetScript()
            : JsonSerializer.Deserialize<CallSheetScript>(callSheet.Data) ?? new CallSheetScript();

          foreach (var entry in callSheetScript.Script) {

            if (entry.Type == Cx.NarrationType) {
              script.Entries.Add(new PerformanceEntry {
                Rank = pRank,
                Type = Cx.NarrationType,
                CharacterId = null,
                CharacterName = entry.Name,
                Text = entry.Instruction
              });
              pRank++;
            } else {  // role entry
              if (entry.CharacterId.HasValue && entry.CharacterId.Value > 0) {
                var characterItem = await _mediator.Send(new GetItemByIdQuery(entry.CharacterId.Value));
                if (characterItem != null) {
                  script.Entries.Add(new PerformanceEntry {
                    Rank = pRank,
                    Type = Cx.ActionType,
                    CharacterId = characterItem.Id,
                    CharacterName = characterItem.Name,
                    Text = entry.Instruction
                  });
                  pRank++;
                }
              }
            }

          } // end foreach entry
        }  // end foreach callSheet

        newItem.Data = JsonSerializer.Serialize(script);
        newItem = await _mediator.Send(newItem.ToUpdateCmd());

      }

      return newItem;


    }
  }
}
