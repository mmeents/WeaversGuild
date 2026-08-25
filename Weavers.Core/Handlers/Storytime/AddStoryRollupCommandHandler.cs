using MediatR;
using System.Text;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;


namespace Weavers.Core.Handlers.Storytime {
  public record AddStoryRollupCommand(int storyId, string realm, int todoId) : IMcpRequest, IRequest<ItemDto?>;
  public class AddStoryRollupCommandHandler : IRequestHandler<AddStoryRollupCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public AddStoryRollupCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<ItemDto?> Handle(AddStoryRollupCommand request, CancellationToken cancellationToken) {

      var storyItem = await _context.GetItemDtoById(request.storyId);
      if (storyItem == null) { throw new Exception($"Story with id {request.storyId} not found"); }
      if (storyItem.ItemTypeId != (int)WeItemType.StoryModel) {
        throw new Exception($"Invalid item type {(WeItemType)storyItem.ItemTypeId}; requires a {WeItemType.StoryModel} type {(int)WeItemType.StoryModel}.");
      }
      var realmId = storyItem.GetParentId();
      var realmItem = realmId > 0 ? await _context.GetItemDtoById(realmId) : null;

      if (realmItem == null || realmItem.ItemTypeId != (int)WeItemType.RealmModel) {
        throw new Exception($"Parent realm for story {storyItem.Id} not found.");
      }

      ResolveAttributionQueryResult? attribution = null;
      if (request.todoId > 0) {
        attribution = await _mediator.Send(new ResolveAttributionQuery(request.todoId));
      }

      var nextRollupCount = realmItem.Relations.Count(r => r.RelatedItemTypeId == (int)WeItemType.StoryRollupModel && r.RelatedItemId.HasValue) + 1;
      var rollupName = $"{storyItem.Name}";
      var credits = "";

      var sb = new StringBuilder();
      var cr = new StringBuilder();
      var beatCredits = new HashSet<string>();
      var directorCredits = new HashSet<string>();
      var characterNames = new Dictionary<int, string>();
      var actorCredits = new Dictionary<string, string>();
      var observerCredits = new HashSet<string>();

      var storyAddedByProp = storyItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
      if (storyAddedByProp != null) {
        cr.Append($"{storyItem.Name} Credits\n\n");
        cr.Append($"Story by {storyAddedByProp.Value}\n\n");
      }

      var scenes = storyItem.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.SceneModel && r.RelatedItemId.HasValue)
        .OrderBy(r => r.Rank);

      var sceneIds = storyItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.SceneModel && r.RelatedItemId.HasValue)
         .Select(r => r.RelatedItemId!.Value).ToList();
      var allScenes = await _mediator.Send(new GetItemsByIdsQuery(sceneIds));

      foreach (var sceneRel in scenes) {
        var scene = allScenes.FirstOrDefault(s => s.Id == sceneRel.RelatedItemId!.Value);
        if (scene == null) continue;

        var sceneAddedByProp = scene.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
        if (sceneAddedByProp != null) {
          cr.Append($"Scene {scene.Name} by {sceneAddedByProp.Value}\n");
        }

        // table of names for cast below.
        var sceneCharacterRels = scene.Relations
          .Where(r => r.RelatedItemTypeId == (int)WeItemType.CharacterModel)
          .ToDictionary(r => r.RelatedItemId!.Value, v => v.RelatedItemName);

        foreach (int id in sceneCharacterRels.Keys) {
          characterNames[id] = sceneCharacterRels[id];
        }

        var beats = scene.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.BeatModel).OrderBy(r => r.Rank);
        var beatIds = beats.Where(r => r.RelatedItemId.HasValue).Select(r => r.RelatedItemId!.Value).ToList();
        var allBeats = await _mediator.Send(new GetItemsByIdsQuery(beatIds));
        List<int> callSheets = new List<int>(); 
        foreach (var beatRel in beats) {
          var beat = allBeats.FirstOrDefault(b => b.Id == beatRel.RelatedItemId!.Value);
          if (beat == null) continue;
          var addedBy = beat.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
          if (addedBy != null) {
            var beatCredit = addedBy?.Value ?? "";
            if (beatCredit != "" && !beatCredits.Contains(beatCredit)) {
              beatCredits.Add(beatCredit);
            }
          }
          var callSheetRel = beat.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.CallSheetModel)
            .OrderByDescending(r => r.RelatedItemId!.Value).FirstOrDefault();
          if (callSheetRel != null) {
            callSheets.Add(callSheetRel.RelatedItemId!.Value);         
          }
        }

        var allCallSheets = await _mediator.Send(new GetItemsByIdsQuery(callSheets));
        foreach(var callSheet in allCallSheets) {
          var dirAddedBy = callSheet.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
          if (dirAddedBy != null) {
            var dirCredit = dirAddedBy?.Value ?? "";
            if (dirCredit != "" && !directorCredits.Contains(dirCredit)) {
              directorCredits.Add(dirCredit);
            }
          }
        }


        // latest performance wins — a re-run supersedes
        var perfRel = scene.Relations
          .Where(r => r.RelatedItemTypeId == (int)WeItemType.PerformanceModel && r.RelatedItemId.HasValue)
          .OrderByDescending(r => r.RelatedItemId!.Value)
          .FirstOrDefault();
        if (perfRel == null) continue;

        var perf = await _context.GetItemDtoById(perfRel.RelatedItemId!.Value);
        var obsRel = perf?.Relations
          .Where(r => r.RelatedItemTypeId == (int)WeItemType.ObservationModel && r.RelatedItemId.HasValue)
          .OrderByDescending(r => r.RelatedItemId!.Value)
          .FirstOrDefault();

        var actorPerformancesRelations = perf.Relations
          .Where(r => r.RelatedItemTypeId == (int)WeItemType.ActorPerformanceModel && r.RelatedItemId.HasValue)
          .OrderByDescending(r => r.RelatedItemId!.Value);
        var actorPerformancesIds = actorPerformancesRelations.Select(r => r.RelatedItemId!.Value).ToList();
        var allActorPerformances = await _mediator.Send(new GetItemsByIdsQuery(actorPerformancesIds));

        foreach (var actorPerfRel in actorPerformancesRelations) {
          var actorPerf = allActorPerformances.FirstOrDefault(b => b.Id == actorPerfRel.RelatedItemId!.Value);
          if (actorPerf == null) continue;
          var actorCharId = actorPerf.Properties.FirstOrDefault(p => p.Name == Cx.ItCharacter)?.Value ?? "";
          var actorModel = actorPerf.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy)?.Value ?? "";
          if (!string.IsNullOrEmpty(actorCharId) && !string.IsNullOrEmpty(actorModel) && !actorCredits.ContainsKey(actorCharId)) {
            var id = actorCharId.AsInt();
            var charName = characterNames.ContainsKey(id) ? characterNames[id] : "";
            if (charName != "") {
              actorCredits[charName] = actorModel;
            }
          }
        }

        if (obsRel == null) continue;

        var obs = await _context.GetItemDtoById(obsRel.RelatedItemId!.Value);
        if (obs == null) continue;

        var obsWriter = obs.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy)?.Value ?? "";
        if (!string.IsNullOrEmpty(obsWriter) && !observerCredits.Contains(obsWriter)) {
          observerCredits.Add(obsWriter);
        }

        sb.Append($"\n## {scene.Name}\n\n");
        sb.Append(obs.Description + "\n");
      }

      var sceneRollup = sb.ToString();

      if (beatCredits.Count > 0) {
        cr.Append("\nScene Beats by\n");
        foreach (var beatcr in beatCredits) {
          cr.Append(beatcr + "\n");
        }
      }

      if (directorCredits.Count > 0) {
        cr.Append("\nBeats Directed by\n");
        foreach (var dirCr in directorCredits) {
          cr.Append(dirCr + "\n");
        }
      }

      if (actorCredits.Count > 0) {
        cr.Append("\nCast\n");
        foreach (var kv in actorCredits) {
          var charName = characterNames.TryGetValue(kv.Key.AsInt(), out var n) ? n : $"{kv.Key}";
          cr.Append($"{charName} performed by {kv.Value}\n");
        }
      }

      if (observerCredits.Count > 0) {
        cr.Append("\nFinal Story Prose\n");
        foreach (var obCr in observerCredits) {
          cr.Append(obCr + "\n");
        }
      }

      if (attribution != null) {
        cr.Append($"\nPost production {attribution.PresenceModelKey}\n");
      }
      cr.Append($"\n\nA WeaversGuild production.");
      credits = cr.ToString();

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(realmItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.StoryRollupModel, rollupName, sceneRollup, "{}"));

      if (newItem != null) {
        var creditsProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCredits);
        if (creditsProp != null) {
          creditsProp.Value = credits;
          await creditsProp.SaveProp(newItem, _mediator);
        }

        var realmProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRealm);
        if (realmProp != null) {
          realmProp.Value = request.realm;
          await realmProp.SaveProp(newItem, _mediator);
        }

        var addedByProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
        if (addedByProp != null && attribution != null) {
          addedByProp.Value = attribution.PresenceModelKey;
          await addedByProp.SaveProp(newItem, _mediator);
        }

        newItem = await _context.GetItemDtoById(newItem.Id);
      }

      return newItem;


    }
  }
}
