using AngleSharp.Css.Dom;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

namespace Weavers.Core.Service {

  public interface IStorytimeService {
    Task<ItemDto?> AddRealm(ItemDto parentItem, string name, string description, string tone);
    Task<ItemDto?> AddStory(ItemDto parentItem, string name, string description, int povTypeId, int sceneCount);
    Task<ItemDto?> AddScene(ItemDto parentItem, string name, string description, int povTypeId, string entryState, string exitState);

    Task<ScheduleBeatWriterResult> ScheduleBeatWriters(ItemDto storyItem, ItemDto handlerDesk, int? fromTodoId);

    Task<ItemDto?> AddBeat(ItemDto parentItem, string name, string description);
    Task<ItemDto?> AddCharacter(ItemDto parentItem, string name, string description);

    Task<ScheduleBeatDirectorResults> ScheduleBeatDirectors(ItemDto sceneItem, ItemDto handlerDesk, int? fromTodoId);

    Task<ItemDto?> AddCallSheet(ItemDto parentItem, string name, string description);
    Task<ItemDto?> AddCallSheetRole(ItemDto callSheetItem, string name, string instruction);
    Task<ItemDto?> AddCallSheetNarration(ItemDto callSheetItem, string name, string narration);

    Task<ScheduleActorPerformanceResults> ScheduleActorPerformances(ItemDto performanceItem, ItemDto handlerDesk, int? fromTodoId);

    Task<ItemDto?> AddPerformance(ItemDto parentItem, string name, string description);
    Task<ItemDto?> AddPerformanceAction(ItemDto actorPerformanceItem, string action);
    Task<ItemDto?> AddPerformanceLine(ItemDto actorPerformanceItem, string line);

    Task<GetPerformanceRollupResult> GetPerformanceRollup(ItemDto performanceItem);

    Task<ItemDto?> AddObservation(ItemDto parentItem, string name, string description);

  }
  public class StorytimeService : IStorytimeService {
    private readonly IServiceScopeFactory _scopeFactory;
    public StorytimeService(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;
    }
    public async Task<ItemDto?> AddRealm(ItemDto parentItem, string name, string description, string tone) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.RealmModel, name, description, "{}"));
      if (newItem != null) {
        var toneProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItTone);
        if (toneProp != null) {
          toneProp.Value = tone;
          await toneProp.SaveProp(newItem, mediator);
        }
      }
      return newItem;
    }
    public async Task<ItemDto?> AddStory(ItemDto parentItem, string name, string description, int povTypeId, int sceneCount) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.StoryModel, name, description, "{}"));
      if (newItem != null) {
        if (povTypeId >= (int)WeItemType.PovUndefined && povTypeId <= (int)WeItemType.PovThirdPersonOmniscient) {
          var povDefaultProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPovDefault);
          if (povDefaultProp != null) {
            povDefaultProp.Value = povTypeId.ToString();
            await povDefaultProp.SaveProp(newItem, mediator);
          }
        }

        var sceneCountProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItTargetSceneCount);
        if (sceneCountProp != null) {
          if (sceneCount <= 0 || sceneCount > 10) {
            sceneCountProp.Value = 5.ToString();
          } else {
            sceneCountProp.Value = sceneCount.ToString();
          }
          await sceneCountProp.SaveProp(newItem, mediator);
        }
      }
      return newItem;
    }
    public async Task<ItemDto?> AddScene(ItemDto parentItem, string name, string description, int povTypeId, string entryState, string exitState) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.SceneModel, name, description, "{}"));
      if (newItem != null) {
        if (povTypeId >= (int)WeItemType.PovUndefined && povTypeId <= (int)WeItemType.PovThirdPersonOmniscient) {
          var povDefaultProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPov);
          if (povDefaultProp != null) {
            povDefaultProp.Value = povTypeId.ToString();
            await povDefaultProp.SaveProp(newItem, mediator);
          }
        }

        var entryStateProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItEntryState);
        if (entryStateProp != null) {
          entryStateProp.Value = entryState;
          await entryStateProp.SaveProp(newItem, mediator);
        }

        var exitStateProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItExitState);
        if (exitStateProp != null) {
          exitStateProp.Value = exitState;
          await exitStateProp.SaveProp(newItem, mediator);
        }

      }
      return newItem;
    }

    public async Task<ScheduleBeatWriterResult> ScheduleBeatWriters(ItemDto storyItem, ItemDto handlerDesk, int? fromTodoId) {

      var result = new ScheduleBeatWriterResult {
        StoryId = storyItem.Id,
        AddedTodoIds = new List<int>(),
        Errors = new List<string>()
      };

      using var scope = _scopeFactory.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var appGraphOrgService = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();

      if (storyItem.ItemTypeId != (int)WeItemType.StoryModel) {
        result.Errors.Add($"Story item {storyItem.Id} is not a StoryModel.");
        return result;
      }

      if (handlerDesk.ItemTypeId != (int)WeItemType.DeskModel) {
        result.Errors.Add($"Handler desk item {handlerDesk.Id} is not a Desk.");
        return result;
      }

      var scenes = storyItem.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.SceneModel && r.RelatedItemId.HasValue)
        .OrderBy(r => r.Rank)
        .ToList();

      if (scenes.Count == 0) {
        result.Errors.Add($"Story does not appear to have any scenes. No work was done.");
        return result;
      }

      ItemDto? fromTodo = null;
      if (fromTodoId.HasValue && fromTodoId.Value > 0) {  // set overrides if a fromTodo was provided. if not leave original values in place.   
        fromTodo = await context.GetItemDtoById(fromTodoId.Value);
        if (fromTodo == null) {
          result.Errors.Add($"fromTodoId {fromTodoId.Value} does not exist. No work was done.");
          return result;
        }
        if (fromTodo.ItemTypeId != (int)WeItemType.TodoModel) {
          result.Errors.Add($"fromTodoId {fromTodoId.Value} is not a Todo. No work was done.");
          return result;
        }
      }

      foreach (var scene in scenes) {
        try {
          var sceneId = scene.RelatedItemId.HasValue ? scene.RelatedItemId.Value : 0;
          if (sceneId > 0) {
            result.SceneIds.Add(sceneId);
            var sceneDto = await context.GetItemDtoById(sceneId);
            if (sceneDto == null) {
              result.Errors.Add($"Scene relation for storyId {storyItem.Id} has related item id {sceneId} but that item does not exist.");
              continue;
            }
            if (sceneDto != null && sceneDto.Relations.Any(r => r.RelatedItemTypeId == (int)WeItemType.BeatModel)) {
              result.Skipped.Add(sceneId);
              continue;
            }
            var beatWrittenProp = sceneDto!.Properties.FirstOrDefault(p => p.Name == Cx.ItBeatsRequested);
            if (beatWrittenProp != null) {
              if (beatWrittenProp.Value.AsBoolean()) {
                result.Skipped.Add(sceneId);
                continue;
              }
            }

            var todoName = $"Write Beats for storyId {storyItem.Id}, sceneId {sceneId}: {scene.RelatedItemName}";
            var promptTemplate = $"TodoId: {{{{model.todo.id}}}}: Write the Beats for storyId {storyItem.Id}, sceneId {sceneId}.";
            // Do the add to the desk 
            var addedTodo = await appGraphOrgService.AddDeskTodo(handlerDesk, todoName, sceneId, promptTemplate);
            if (addedTodo != null) {

              if (fromTodo != null) {
                var fromTodoProp = addedTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItFromTodo);
                if (fromTodoProp != null) {
                  fromTodoProp.Value = fromTodo.Id.ToString();
                  await fromTodoProp.SaveProp(addedTodo, mediator);
                }

                var existingDepth = int.TryParse(fromTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth)?.Value ?? "0", out var depth) ? depth : 0;
                var addedDepthProp = addedTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth);
                if (addedDepthProp != null) {
                  addedDepthProp.Value = (existingDepth + 1).ToString();
                  await addedDepthProp.SaveProp(addedTodo, mediator);
                }
              }


              try {
                var itemDto = await mediator.Send(new SetTodoReadyCommand(addedTodo.Id));
              } catch (Exception ex) {
                result.Errors.Add($"Failed to set todo ready for todoId {addedTodo.Id}: " + ex.Message);
              }

              result.AddedTodoIds.Add(addedTodo.Id);

              if (beatWrittenProp != null) {
                beatWrittenProp.Value = "1";
                await beatWrittenProp.SaveProp(sceneDto, mediator);
              } else {
                ItemPropertyDto newBeatWrittenProp = new ItemPropertyDto {
                  ItemId = sceneDto.Id,
                  Name = Cx.ItBeatsRequested,
                  Value = "1",
                  IsRequired = false,
                  IsReadOnly = false,
                  IsVisible = true,
                  ValueDataTypeId = (int)WeDataType.Boolean,
                  EditorTypeId = (int)WeEditorType.Boolean
                };
                await newBeatWrittenProp.SaveProp(sceneDto, mediator);
              }

            } else {
              result.Errors.Add($"Add Desk Todo returned a null added item. target desk {handlerDesk.Id} for scene:{sceneId}");
            }

          } else {
            result.Errors.Add($"Scene relation for storyId {storyItem.Id} has no related item id.");
          }
        } catch (Exception ex) {
          result.Errors.Add($"Failed to schedule beat writers for sceneId {scene.RelatedItemId}: " + ex.Message);
        }
      }

      return result;
    }

    public async Task<ItemDto?> AddBeat(ItemDto parentItem, string name, string description) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.BeatModel, name, description, "{}"));
      return newItem;
    }
    public async Task<ItemDto?> AddCharacter(ItemDto parentItem, string name, string description) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.CharacterModel, name, description, "{}"));
      return newItem;
    }

    public async Task<ScheduleBeatDirectorResults> ScheduleBeatDirectors(ItemDto sceneItem, ItemDto handlerDesk, int? fromTodoId) {
      var result = new ScheduleBeatDirectorResults {
        StoryId = sceneItem.GetParentId(),
        SceneId = sceneItem.Id,
        AddedTodoIds = new List<int>(),
        Errors = new List<string>()
      };

      using var scope = _scopeFactory.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var appGraphOrgService = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();

      if (sceneItem.ItemTypeId != (int)WeItemType.SceneModel) {
        result.Errors.Add($"Story item {sceneItem.Id} is not a SceneModel.");
        return result;
      }

      if (handlerDesk.ItemTypeId != (int)WeItemType.DeskModel) {
        result.Errors.Add($"Handler desk item {handlerDesk.Id} is not a Desk.");
        return result;
      }

      var beats = sceneItem.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.BeatModel && r.RelatedItemId.HasValue)
        .OrderBy(r => r.Rank)
        .ToList();

      if (beats.Count == 0) {
        result.Errors.Add($"Story does not appear to have any scenes. No work was done.");
        return result;
      }

      ItemDto? fromTodo = null;
      if (fromTodoId.HasValue && fromTodoId.Value > 0) {  // set overrides if a fromTodo was provided. if not leave original values in place.   
        fromTodo = await context.GetItemDtoById(fromTodoId.Value);
        if (fromTodo == null) {
          result.Errors.Add($"fromTodoId {fromTodoId.Value} does not exist. No work was done.");
          return result;
        }
        if (fromTodo.ItemTypeId != (int)WeItemType.TodoModel) {
          result.Errors.Add($"fromTodoId {fromTodoId.Value} is not a Todo. No work was done.");
          return result;
        }
      }

      foreach (var scenesBeat in beats) {
        try {
          var beatId = scenesBeat.RelatedItemId.HasValue ? scenesBeat.RelatedItemId.Value : 0;
          if (beatId > 0) {
            result.BeatIds.Add(beatId);
            var beatDto = await context.GetItemDtoById(beatId);
            if (beatDto == null) {
              result.Errors.Add($"Beat relation for sceneId {sceneItem.Id} has related item id {beatId} but that item does not exist.");
              continue;
            }
            if (beatDto != null && beatDto.Relations.Any(r => r.RelatedItemTypeId == (int)WeItemType.CallSheetModel)) {
              result.Skipped.Add(beatId);
              continue;
            }
            var callSheetWrittenProp = beatDto!.Properties.FirstOrDefault(p => p.Name == Cx.ItCallSheetRequested);
            if (callSheetWrittenProp != null) {
              if (callSheetWrittenProp.Value.AsBoolean()) {
                result.Skipped.Add(beatId);
                continue;
              }
            }

            // add the call sheet.
            var callSheet = await mediator.Send(
              new CreateRelatedItemCommand(beatDto.Id, (int)WeRelationTypes.Contains,
                (int)WeItemType.CallSheetModel, $"CallSheet for BeatId {beatDto.Id}", "", "{}"));
            if (callSheet == null) {
              result.Errors.Add($"Failed to create CallSheet for beatId: {beatDto.Id}");
              continue;
            }

            var todoName = $"Direct the beat for beatId: {beatDto.Id}, sceneId {beatId}: {scenesBeat.RelatedItemName}";
            var promptTemplate = $"TodoId: {{{{model.todo.id}}}}:" + Environment.NewLine +
              $"Direct the beat for beatId: {beatDto.Id} within the sceneId {beatId};" + Environment.NewLine +
              $"The Call Sheet has already been created. Use Call Sheet Id: {callSheet.Id}.";
            var addedTodo = await appGraphOrgService.AddDeskTodo(handlerDesk, todoName, beatId, promptTemplate); // Do the add to the desk 
            if (addedTodo != null) {

              if (fromTodo != null) {
                var fromTodoProp = addedTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItFromTodo);
                if (fromTodoProp != null) {
                  fromTodoProp.Value = fromTodo.Id.ToString();
                  await fromTodoProp.SaveProp(addedTodo, mediator);
                }

                var existingDepth = int.TryParse(fromTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth)?.Value ?? "0", out var depth) ? depth : 0;
                var addedDepthProp = addedTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth);
                if (addedDepthProp != null) {
                  addedDepthProp.Value = (existingDepth + 1).ToString();
                  await addedDepthProp.SaveProp(addedTodo, mediator);
                }
              }


              try {
                var itemDto = await mediator.Send(new SetTodoReadyCommand(addedTodo.Id));
              } catch (Exception ex) {
                result.Errors.Add($"Failed to set todo ready for todoId {addedTodo.Id}: " + ex.Message);
              }

              result.AddedTodoIds.Add(addedTodo.Id);

              if (callSheetWrittenProp != null) {
                callSheetWrittenProp.Value = "1";
                await callSheetWrittenProp.SaveProp(beatDto, mediator);
              } else {
                ItemPropertyDto newBeatWrittenProp = new ItemPropertyDto {
                  ItemId = beatDto.Id,
                  Name = Cx.ItCallSheetRequested,
                  Value = "1",
                  IsRequired = false,
                  IsReadOnly = false,
                  IsVisible = true,
                  ValueDataTypeId = (int)WeDataType.Boolean,
                  EditorTypeId = (int)WeEditorType.Boolean
                };
                await newBeatWrittenProp.SaveProp(beatDto, mediator);
              }

            } else {
              result.Errors.Add($"Add Desk Todo returned a null added item. target desk {handlerDesk.Id} for beat:{beatId}");
            }

          } else {
            result.Errors.Add($"Scene relation for sceneId {sceneItem.Id} has no related item id.");
          }
        } catch (Exception ex) {
          result.Errors.Add($"Failed to schedule beat directors for beatId {scenesBeat.RelatedItemId}: " + ex.Message);
        }
      }

      return result;
    }
    public async Task<ItemDto?> AddCallSheet(ItemDto parentItem, string name, string description) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.CallSheetModel, name, description, "{}"));
      return newItem;
    }
    public async Task<ItemDto?> AddCallSheetNarration(ItemDto callSheetItem, string section, string narration) {
      using var scope = _scopeFactory.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var script = string.IsNullOrWhiteSpace(callSheetItem.Data) || callSheetItem.Data == "{}" ?
        new CallSheetScript()
        : JsonSerializer.Deserialize<CallSheetScript>(callSheetItem.Data) ?? new CallSheetScript();
      var nextRank = script.Script.Any() ? script.Script.Max(s => s.Rank) + 1 : 1;
      script.Script.Add(new CallSheetScriptItem {
        Rank = nextRank,
        Type = Cx.NarrationType,
        Name = section,
        Instruction = narration
      });
      callSheetItem.Data = JsonSerializer.Serialize(script);
      var updated = await mediator.Send(callSheetItem.ToUpdateCmd());
      return updated;
    }
    public async Task<ItemDto?> AddCallSheetRole(ItemDto callSheetItem, string name, string instruction) {
      using var scope = _scopeFactory.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

      var beatId = callSheetItem.GetParentId();
      var beatItem = await context.GetItemDtoById(beatId);
      if (beatItem == null) { throw new Exception($"Beat item with id {beatId} not found."); }
      var sceneId = beatItem.GetParentId();
      var sceneItem = await context.GetItemDtoById(sceneId);
      if (sceneItem == null) { throw new Exception($"Scene item with id {sceneId} not found."); }

      ItemDto? charItem = null;
      var charItemRel = sceneItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.CharacterModel && string.Compare(r.RelatedItemName, name, true) == 0);
      if (charItemRel == null) {
        charItem = await mediator.Send(
          new CreateRelatedItemCommand(sceneItem.Id, (int)WeRelationTypes.Contains,
            (int)WeItemType.CharacterModel, name, "", "{}"));
      } else {
        charItem = await context.GetItemDtoById(charItemRel.RelatedItemId ?? 0);
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
        Name = name,
        Instruction = instruction
      });

      callSheetItem.Data = JsonSerializer.Serialize(script);
      var updated = await mediator.Send(callSheetItem.ToUpdateCmd());

      return updated;
    }


    public async Task<ScheduleActorPerformanceResults> ScheduleActorPerformances(ItemDto performanceItem, ItemDto handlerDesk, int? fromTodoId) {
      var result = new ScheduleActorPerformanceResults {
        SceneId = performanceItem.GetParentId(),
        PerformanceId = performanceItem.Id,
        AddedTodoIds = new List<int>(),
        Errors = new List<string>()
      };

      using var scope = _scopeFactory.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var appGraphOrgService = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
      ItemDto? perfItem = await context.GetItemDtoById(performanceItem.Id) ?? throw new Exception($"Performance item with id {performanceItem.Id} not found.");

      if (perfItem.ItemTypeId != (int)WeItemType.PerformanceModel) {
        result.Errors.Add($"Story item {perfItem.Id} is not a PerformanceModel.");
        return result;
      }

      if (handlerDesk.ItemTypeId != (int)WeItemType.DeskModel) {
        result.Errors.Add($"Handler desk item {handlerDesk.Id} is not a Desk.");
        return result;
      }

      var sceneId = perfItem.GetParentId();
      var sceneItem = await context.GetItemDtoById(sceneId);
      if (sceneItem == null) {
        result.Errors.Add($"Performance items require a Scene type parent.");
        return result;
      }

      var script = string.IsNullOrWhiteSpace(perfItem.Data) || perfItem.Data == "{}" ?
       new PerformanceScript() :
       JsonSerializer.Deserialize<PerformanceScript>(perfItem.Data) ?? new PerformanceScript();

      var actorPerfs = perfItem.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.ActorPerformanceModel && r.RelatedItemId.HasValue)
        .OrderBy(r => r.Rank)
        .ToList();

      foreach (var scriptNode in script.Entries.OrderBy(e => e.Rank)) { // add missing actor performances for each script node. if it already exists, skip it.
        ItemDto? newPerf = null;
        if (scriptNode != null && scriptNode.Type == Cx.ActionType) {   // got to here 

          var characterId = scriptNode.CharacterId;
          var rank = scriptNode.Rank;
          var actorPerfName = $"Rank: {rank}; Character {scriptNode.CharacterName};";

          var existingPerf = actorPerfs.FirstOrDefault(ap => ap.RelatedItemName == actorPerfName);
          if (existingPerf == null) {

            newPerf = await mediator.Send(
              new CreateRelatedItemCommand(performanceItem.Id, (int)WeRelationTypes.Contains,
                (int)WeItemType.ActorPerformanceModel, actorPerfName, "", "{}"));

            if (newPerf != null) {
              result.ActorPerformanceIds.Add(newPerf.Id);

              var characterProp = newPerf.Properties.FirstOrDefault(p => p.Name == Cx.ItCharacter);
              if (characterProp != null) {
                characterProp.Value = characterId.ToString();
                await characterProp.SaveProp(newPerf, mediator);
              }

              var rankProp = newPerf.Properties.FirstOrDefault(p => p.Name == Cx.ItRank);
              if (rankProp != null) {
                rankProp.Value = rank.ToString();
                await rankProp.SaveProp(newPerf, mediator);
              }

              var InstructionsProp = newPerf.Properties.FirstOrDefault(p => p.Name == Cx.ItInstructions);
              if (InstructionsProp != null) {
                InstructionsProp.Value = scriptNode.Text;
                await InstructionsProp.SaveProp(newPerf, mediator);
              }
            }

          } else { // already exists             
            result.ActorPerformanceIds.Add(existingPerf.RelatedItemId!.Value); // add for the counts.
            result.Skipped.Add(existingPerf.RelatedItemId!.Value);  // skip if it exists already. we don't want to create duplicates.
          }
        }
      }

      if (result.ActorPerformanceIds.Count == 0) {
        result.Errors.Add($"Story does not appear to have any actor performances.");
        return result;
      }

      ItemDto? fromTodo = null;
      if (fromTodoId.HasValue && fromTodoId.Value > 0) {  // set overrides if a fromTodo was provided. if not leave original values in place.   
        fromTodo = await context.GetItemDtoById(fromTodoId.Value);
        if (fromTodo == null) {
          result.Errors.Add($"fromTodoId was specified but {fromTodoId.Value} does not exist. No work was done.");
          return result;
        }
        if (fromTodo.ItemTypeId != (int)WeItemType.TodoModel) {
          result.Errors.Add($"fromTodoId was specified but {fromTodoId.Value} is not a Todo. No work was done.");
          return result;
        }
      }
      foreach (var actorPerfId in result.ActorPerformanceIds) { 
        try {          
          if (actorPerfId > 0) {

            var actorPerfDto = await context.GetItemDtoById(actorPerfId);
            if (actorPerfDto == null) {
              result.Errors.Add($"Failed to find actor performance by id {actorPerfId}.");
              continue;
            }
            if (actorPerfDto != null && result.Skipped.Contains(actorPerfId)) {
              continue;  // was called skipped above.
            }

            var todoName = $"Act for actorPerformanceId: {actorPerfId}; {actorPerfDto!.Name}";
            var promptTemplate = $"TodoId: {{{{model.todo.id}}}}:" + Environment.NewLine +              
              $"{todoName}" + Environment.NewLine +
              $"Note: This is a component of the parent performanceId: {perfItem.Id};";

            var addedTodo = await appGraphOrgService.AddDeskTodo(handlerDesk, todoName, actorPerfId, promptTemplate); // Do the add to the desk 

            if (addedTodo != null) {

              if (fromTodo != null) {
                var fromTodoProp = addedTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItFromTodo);
                if (fromTodoProp != null) {
                  fromTodoProp.Value = fromTodo.Id.ToString();
                  await fromTodoProp.SaveProp(addedTodo, mediator);
                }

                var existingDepth = int.TryParse(fromTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth)?.Value ?? "0", out var depth) ? depth : 0;
                var addedDepthProp = addedTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth);
                if (addedDepthProp != null) {
                  addedDepthProp.Value = (existingDepth + 1).ToString();
                  await addedDepthProp.SaveProp(addedTodo, mediator);
                }
              }


              try {
                var itemDto = await mediator.Send(new SetTodoReadyCommand(addedTodo.Id));
              } catch (Exception ex) {
                result.Errors.Add($"Failed to set todo ready for todoId {addedTodo.Id}: " + ex.Message);
              }

              result.AddedTodoIds.Add(addedTodo.Id);

            } else {
              result.Errors.Add($"Add Desk Todo returned a null added item. target desk {handlerDesk.Id} for actor performance:{actorPerfId}");
            }

          } else {
            result.Errors.Add($"performance relation to actor performance {actorPerfId} has no related item id.");
          }
        } catch (Exception ex) {
          result.Errors.Add($"Failed to schedule actor performances Id: {actorPerfId}: " + ex.Message);
        }
      }

      return result;
    }


    public async Task<ItemDto?> AddPerformance(ItemDto sceneItem, string name, string description) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(sceneItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.PerformanceModel, name, description, "{}"));
      if (newItem != null) {
        var script = new PerformanceScript();
        var callSheetList = await mediator.Send(new GetKidsByTypeRecQuery(sceneItem.Id, (int)WeItemType.CallSheetModel));
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
                var characterItem = await mediator.Send(new GetItemByIdQuery(entry.CharacterId.Value));
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
        newItem = await mediator.Send(newItem.ToUpdateCmd());

      }

      return newItem;
    }

    public Task<ItemDto?> AddPerformanceAction(ItemDto actorPerformanceItem, string action)
      => AppendEntry(actorPerformanceItem, action, Cx.ActionType);

    public Task<ItemDto?> AddPerformanceLine(ItemDto actorPerformanceItem, string line)
      => AppendEntry(actorPerformanceItem, line, Cx.LineType);

    private async Task<ItemDto?> AppendEntry(ItemDto actorPerformanceItem, string text, string type) {
      using var scope = _scopeFactory.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      
      var script = string.IsNullOrWhiteSpace(actorPerformanceItem.Data) || actorPerformanceItem.Data == "{}"
        ? new PerformanceScript()
        : JsonSerializer.Deserialize<PerformanceScript>(actorPerformanceItem.Data) ?? new PerformanceScript();

      var nextRank = script.Entries.Any() ? script.Entries.Max(s => s.Rank) + 1 : 1;

      var characterId = actorPerformanceItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCharacter)?.Value.AsInt();
      if (characterId is null or <= 0) throw new Exception("CharacterId not found in actorPerformanceItem properties");

      var charItem = await context.GetItemDtoById(characterId.Value)
        ?? throw new Exception($"Character with id {characterId.Value} not found");

      script.Entries.Add(new PerformanceEntry {
        Rank = nextRank,
        Type = type,
        CharacterId = characterId,
        CharacterName = charItem.Name,
        Text = text
      });

      actorPerformanceItem.Data = JsonSerializer.Serialize(script);
      return await mediator.Send(actorPerformanceItem.ToUpdateCmd());
    }

    
    public async Task<GetPerformanceRollupResult> GetPerformanceRollup(ItemDto performanceItem) {

      var result = new GetPerformanceRollupResult(); 
      if (performanceItem.ItemTypeId != (int)WeItemType.PerformanceModel) {
        throw new Exception($"Item {performanceItem.Id} is not a PerformanceModel.");
      }
      result.Performance.Id = performanceItem.Id; 
      result.Performance.Name = performanceItem.Name;

      using var scope = _scopeFactory.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
      var scene = await context.GetItemDtoById(performanceItem.GetParentId());
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

        var story = await context.GetItemDtoById(scene.GetParentId());
        if (story != null) { 
          result.Story.Id = story.Id;
          result.Story.Name = story.Name;
          result.Story.Card = story.Description;
          result.Story.TargetSceneCount = story.Properties.FirstOrDefault(p => p.Name == Cx.ItTargetSceneCount)?.Value.AsInt() ?? 0;
          var realm = await context.GetItemDtoById(story.GetParentId());
          if (realm != null) {
            result.Realm.Id = realm.Id;
            result.Realm.Name = realm.Name;
            result.Realm.Tone = realm.Properties.FirstOrDefault(p => p.Name == Cx.ItTone)?.Value ?? "";
          }
        }
      }

      // pre-load actor performances once, keyed by their performance-rank
      var actorByRank = new Dictionary<int, PerformanceScript>();
      foreach (var r in performanceItem.Relations.Where(r =>
          r.RelatedItemTypeId == (int)WeItemType.ActorPerformanceModel && r.RelatedItemId.HasValue)) {
        var ap = await context.GetItemDtoById(r.RelatedItemId!.Value);
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
    public async Task<ItemDto?> AddObservation(ItemDto parentItem, string name, string description) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.ObservationModel, name, description, "{}"));
      return newItem;
    }

  }
}
