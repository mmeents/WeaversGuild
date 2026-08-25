using MediatR;
using System.Text.Json;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.ItemSummaries;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Storytime {
  public record AddPerformanceActionCommand(int actorPerformanceId, string action, string line, int todoId) : IMcpRequest, IRequest<ItemDto?>;
  public class AddPerformanceActionCommandHandler : IRequestHandler<AddPerformanceActionCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;

    public AddPerformanceActionCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<ItemDto?> Handle(AddPerformanceActionCommand request, CancellationToken cancellationToken) {
      
      var actorPerformanceItem = await _context.GetItemDtoById(request.actorPerformanceId);
      if (actorPerformanceItem == null) {
        return null;
      }
      if (string.IsNullOrEmpty(request.action)) {
        return await AppendEntry(actorPerformanceItem, request.line, Cx.LineType, request.todoId);
      } else {
        return await AppendEntry(actorPerformanceItem, request.action, Cx.ActionType, request.todoId);
      }      
    }

    private async Task<ItemDto?> AppendEntry(ItemDto actorPerformanceItem, string text, string type, int todoId) {     
      var script = string.IsNullOrWhiteSpace(actorPerformanceItem.Data) || actorPerformanceItem.Data == "{}"
        ? new PerformanceScript()
        : JsonSerializer.Deserialize<PerformanceScript>(actorPerformanceItem.Data) ?? new PerformanceScript();

      var nextRank = script.Entries.Any() ? script.Entries.Max(s => s.Rank) + 1 : 1;

      var characterId = actorPerformanceItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCharacter)?.Value.AsInt();
      if (characterId is null or <= 0) throw new Exception("CharacterId not found in actorPerformanceItem properties");

      var charItem = await _context.GetItemDtoById(characterId.Value)
        ?? throw new Exception($"Character with id {characterId.Value} not found");

      script.Entries.Add(new PerformanceEntry {
        Rank = nextRank,
        Type = type,
        CharacterId = characterId,
        CharacterName = charItem.Name,
        Text = text
      });
      actorPerformanceItem.Data = JsonSerializer.Serialize(script);

      var addedByProp = actorPerformanceItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
      if (addedByProp != null && todoId > 0) {
        var attribution = await _mediator.Send(new ResolveAttributionQuery(todoId));
        addedByProp.Value = attribution.PresenceModelKey;
        await addedByProp.SaveProp(actorPerformanceItem, _mediator);
      }

      return await _mediator.Send(actorPerformanceItem!.ToUpdateCmd());
    }
  }
}
