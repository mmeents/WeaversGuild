using MediatR;
using System.Text.Json;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Storytime {
  public record GetPerformanceRollupQuery(int performanceId) : IMcpRequest, IRequest<GetPerformanceRollupResult>;
  public class GetPerformanceRollupQueryHandler : IRequestHandler<GetPerformanceRollupQuery, GetPerformanceRollupResult> {
    private readonly FabricDbContext _context;    
    private readonly IMediator _mediator;
    public GetPerformanceRollupQueryHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<GetPerformanceRollupResult> Handle(GetPerformanceRollupQuery request, CancellationToken cancellationToken) {

      var result = new GetPerformanceRollupResult();
      var performanceItem = await _context.GetItemDtoById(request.performanceId);
      if (performanceItem == null) { throw new Exception($"Parent scene with id {request.performanceId} not found"); }
      if (performanceItem.ItemTypeId != (int)WeItemType.PerformanceModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)performanceItem.ItemTypeId}; requires a {WeItemType.PerformanceModel} type {(int)WeItemType.PerformanceModel} parent.");
      }
      result.Performance.Id = performanceItem.Id;
      result.Performance.Name = performanceItem.Name;

      var scene = await _context.GetItemDtoById(performanceItem.GetParentId());
      if (scene != null) {
        result.Scene.Id = scene.Id;
        result.Scene.Name = scene.Name;
        result.Scene.Rank = scene.IncomingRelations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.StoryModel)?.Rank ?? 0;
        result.Scene.EntryState = scene.Properties.FirstOrDefault(p => p.Name == Cx.ItEntryState)?.Value ?? "";
        result.Scene.ExitState = scene.Properties.FirstOrDefault(p => p.Name == Cx.ItExitState)?.Value ?? "";
        result.Scene.Pov = scene.Properties.FirstOrDefault(p => p.Name == Cx.ItPov)?.Value.GetPOVString() ?? "";
        result.Characters = scene.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.CharacterModel && r.RelatedItemId.HasValue)
          .Select(r => new CharacterDto {
            Id = r.RelatedItemId!.Value,
            Name = r.RelatedItemName ?? "",
          }).ToList();

        var story = await _context.GetItemDtoById(scene.GetParentId());
        if (story != null) {
          result.Story.Id = story.Id;
          result.Story.Name = story.Name;
          result.Story.Card = story.Description;
          result.Story.TargetSceneCount = story.Properties.FirstOrDefault(p => p.Name == Cx.ItTargetSceneCount)?.Value.AsInt() ?? 0;
          var realm = await _context.GetItemDtoById(story.GetParentId());
          if (realm != null) {
            result.Realm.Id = realm.Id;
            result.Realm.Name = realm.Name;
            result.Realm.Tone = realm.Properties.FirstOrDefault(p => p.Name == Cx.ItTone)?.Value ?? "";
          }
        }
      }

      // pre-load actor performances once, keyed by their performance-rank
      var actorByRank = new Dictionary<int, PerformanceScript>();
      List<int> apList = new List<int>();
      foreach (var r in performanceItem.Relations.Where(r =>
          r.RelatedItemTypeId == (int)WeItemType.ActorPerformanceModel && r.RelatedItemId.HasValue)) {
        apList.Add(r.RelatedItemId!.Value);
      }
      var apItems = await _mediator.Send(new GetItemsByIdsQuery(apList));
      foreach (var ap in apItems) { 
        if (ap == null || string.IsNullOrWhiteSpace(ap.Data) || ap.Data == "{}") continue;
        var apRank = ap.Properties.FirstOrDefault(p => p.Name == Cx.ItRank)?.Value.AsInt() ?? -1;   // whatever your Rank-prop const is
        var apScript = JsonSerializer.Deserialize<PerformanceScript>(ap.Data);
        if (apRank >= 0 && apScript?.Entries.Any() == true) actorByRank[apRank] = apScript;
      }

      var outList = new List<EntryDto>();
      var outRank = 0;
      var actorsPerformed = false;

      var script = string.IsNullOrWhiteSpace(performanceItem.Data) || performanceItem.Data == "{}" ?
        new PerformanceScript()
        : JsonSerializer.Deserialize<PerformanceScript>(performanceItem.Data) ?? new PerformanceScript();

      var perfList = script.Entries.OrderBy(e => e.Rank).Select(e => new EntryDto {
        Rank = e.Rank,
        Type = e.Type,
        CharacterId = e.CharacterId,
        CharacterName = e.CharacterName,
        Text = e.Text,
        Source = "Director",
      }).ToList();

      foreach (var entry in perfList) {
        if (entry.Type == Cx.ActionType && actorByRank.TryGetValue(entry.Rank, out var apScript)) {
          foreach (var ae in apScript.Entries.OrderBy(e => e.Rank)) {
            outList.Add(new EntryDto {
              Rank = outRank++,
              Type = ae.Type,
              CharacterId = ae.CharacterId,
              CharacterName = ae.CharacterName,
              Text = ae.Text,
              Source = "Actor",
            });
          }
          actorsPerformed = true;
        } else {
          entry.Rank = outRank++;   // renumber on emit
          outList.Add(entry);
        }
      }

      result.Performance.ActorPerformed = actorsPerformed;
      result.Performance.Entries = outList;

      return result;
    }
  }
}
