using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Service;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Storytime;

namespace Weavers.Core.Tools {
  public interface IStorytimeToolsHandler {
    Task<string> AddRealm(int id, string realmName, string realmDescription, string tone);
    Task<string> AddStory(int id, string name, string description, int povTypeId, int sceneCount, int todoId);
    Task<string> AddScene(int id, string name, string description, string entryState, string exitState, int todoId);
    Task<string> ScheduleBeatWriters(int storyId, int handlerDeskId, int? fromTodoId);
    Task<string> AddBeat(int id, string name, string description, int todoId);    
    Task<string> AddCharacter(int id, string name, string description);
    Task<string> ScheduleBeatDirectors(int sceneId, int handlerDeskId, int? fromTodoId);
    Task<string> AddCallSheet(int id, string name, string description, int todoId);
    Task<string> AddCallSheetRole(int callSheetId, string name, string instruction, int todoId);    
    Task<string> AddCallSheetNarration(int id, string name, string narration, int todoId);


    Task<string> ScheduleActorPerformances(int performanceId, int handlerDeskId, int? fromTodoId);

    Task<string> AddPerformance(int id, string name, string description);

    Task<string> AddPerformanceAction(int actorPerformanceId, string action, int todoId);
    Task<string> AddPerformanceLine(int actorPerformanceId, string line, int todoId);

    Task<string> GetPerformanceRollup(int performanceItemId);

    Task<string> AddObservation(int performanceId, string name, string description, int todoId);
    Task<string> AddStoryRollup(int storyId, string realm, int todoId);
  }


  public class StorytimeToolsHandler : IStorytimeToolsHandler {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StorytimeToolsHandler> _logger;
    public StorytimeToolsHandler(IServiceScopeFactory scopeFactory, ILogger<StorytimeToolsHandler> logger) {
      _scopeFactory = scopeFactory;
      _logger = logger;
    }

    public async Task<string> AddRealm(int id, string name, string description, string tone) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();        
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();        
        var addedItem = await mediator.Send(new AddRealmCommand(id, name, description, tone));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddRealm, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddRealm, await context.ToSummary(addedItem, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddRealm, 0, $"Failed to add realm {name} {ex.Message}");
      }
    }


    public async Task<string> AddStory(int id, string name, string description, int povTypeId, int sceneCount, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var addedItem = await mediator.Send(new AddStoryCommand(id, name, description, povTypeId, sceneCount, todoId));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddStory, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddStory, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddStory, 0, $"Failed to add story {name} {ex.Message}");
      }
    }

    public async Task<string> AddScene(int id, string name, string description, string entryState, string exitState, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();        
        var addedItem = await mediator.Send(new AddSceneCommand(id, name, description, entryState, exitState, todoId));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddScene, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddScene, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddScene, 0, $"Failed to add scene {name} {ex.Message}");
      }
    }

    public async Task<string> ScheduleBeatWriters(int storyId, int handlerDeskId, int? fromTodoId) {
      try {
        using var scope = _scopeFactory.CreateScope();        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();        
        var result = await mediator.Send(new ScheduleBeatWritersCommand(storyId, handlerDeskId, fromTodoId));        
        if (result == null) return _logger.DefaultAddEmptyMessage(Cx.CmdScheduleBeatWriters, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdScheduleBeatWriters, result);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdScheduleBeatWriters, 0, $"Exception Scheduling Beats for StoryId: {storyId} on desk {handlerDeskId} {ex.Message}");
      }
    }

    public async Task<string> AddBeat(int id, string name, string description, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var parentItem = await context.GetItemDtoById(id);
        if (parentItem == null) { throw new Exception($"Parent scene with id {id} not found"); }
        if (parentItem.ItemTypeId != (int)WeItemType.SceneModel) {
          throw new Exception($"Invalid parent item type {(WeItemType)parentItem.ItemTypeId}; requires a {WeItemType.SceneModel} type {(int)WeItemType.SceneModel} parent.");
        }
        var addedItem = await mediator.Send(new AddBeatCommand(id, name, description, todoId));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddBeat, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddBeat, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddBeat, 0, $"Failed to add beat {name} {ex.Message}");
      }
    }
        
    public async Task<string> AddCharacter(int id, string name, string description) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var addedItem = await mediator.Send(new AddCharacterCommand(id, name, description));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddCharacter, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddCharacter, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddCharacter, 0, $"Failed to add character {name}  {ex.Message}");
      }
    }

    public async Task<string> ScheduleBeatDirectors(int sceneId, int handlerDeskId, int? fromTodoId) {

      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();       
        var result = await mediator.Send(new ScheduleBeatDirectorsCommand(sceneId, handlerDeskId, fromTodoId));
        if (result == null) return _logger.DefaultAddEmptyMessage(Cx.CmdScheduleBeatDirectors, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdScheduleBeatDirectors, result);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdScheduleBeatDirectors, 0, $"Exception Scheduling Directors for SceneId: {sceneId} on desk {handlerDeskId} {ex.Message}");
      }
    }

    public async Task<string> AddCallSheet(int beatId, string name, string description, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();       
        var addedItem = await mediator.Send(new AddCallSheetCommand(beatId, name, description, todoId));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddCallSheet, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddCallSheet, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddCallSheet, 0, $"Failed to add call sheet {name} {ex.Message}");
      }
    }

    public async Task<string> AddCallSheetRole(int callSheetId, string name, string instruction, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();        
        var addedItem = await mediator.Send(new AddCallSheetRoleCommand(callSheetId, name, instruction, todoId));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddCallSheetRole, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddCallSheetRole, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddCallSheetRole, 0, $"Failed to add role {name} {ex.Message}");
      }
    }

    public async Task<string> AddCallSheetNarration(int callSheetId, string name, string narration, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();        
        var addedItem = await mediator.Send(new AddCallSheetNarrationCommand(callSheetId, name, narration, todoId));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddCallSheetNarration, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddCallSheetNarration, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddCallSheetNarration, 0, $"Failed to add narration {name} {ex.Message}");
      }
    }
        
    public async Task<string> ScheduleActorPerformances(int performanceId, int handlerDeskId, int? fromTodoId){
     try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var result = await mediator.Send(new ScheduleActorPerformancesCommand(performanceId, handlerDeskId, fromTodoId));
        if (result == null) return _logger.DefaultAddEmptyMessage(Cx.CmdScheduleActorPerformances, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdScheduleActorPerformances, result);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdScheduleActorPerformances, 0, $"Exception Scheduling Actors for PerformanceId: {performanceId} on desk {handlerDeskId} {ex.Message}");
      }
    }
    
    public async Task<string> AddPerformance(int sceneId, string name, string description) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var addedItem = await mediator.Send(new AddPerformanceCommand(sceneId, name));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddPerformance, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddPerformance, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddPerformance, 0, $"Failed to add performance {name} {ex.Message}");
      }
    }

    public async Task<string> AddPerformanceAction(int actorPerformanceId, string action, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var updatedItem = await mediator.Send(new AddPerformanceActionCommand(actorPerformanceId, action, string.Empty, todoId));
        if (updatedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddPerformanceAction, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddPerformanceAction, await context.ToSummary(updatedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddPerformanceAction, 0, $"Failed to add performance action {ex.Message}");
      }
    }

    public async Task<string> AddPerformanceLine(int actorPerformanceId, string line, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();                
        var addedItem = await mediator.Send(new AddPerformanceActionCommand(actorPerformanceId, string.Empty, line, todoId));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddPerformanceLine, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddPerformanceLine, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddPerformanceLine, 0, $"Failed to add performance line {line} {ex.Message}");
      }
    }
    
    public async Task<string> GetPerformanceRollup(int performanceItemId){
    try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();        
        var addedItem = await mediator.Send(new GetPerformanceRollupQuery(performanceItemId));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdGetPerformanceRollup, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdGetPerformanceRollup, addedItem);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdGetPerformanceRollup, 0, $"Failed to get performance rollup {ex.Message}");
      }
    }

    public async Task<string> AddObservation(int performanceId, string name, string description, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();        
        var addedItem = await mediator.Send(new AddObservationCommand(performanceId, name, description, todoId));
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddObservation, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddObservation, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddObservation, 0, $"Failed to add observed {name} {ex.Message}");
      }
    }

    public async Task<string> AddStoryRollup(int storyId, string realm, int todoId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var rollup = await mediator.Send(new AddStoryRollupCommand(storyId, realm, todoId));
        if (rollup == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddStoryRollup, 0);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddStoryRollup, await context.ToSummary(rollup, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddStoryRollup, 0, $"Failed to add story rollup for story {storyId} {ex.Message}");
      }
    }

  }
}
