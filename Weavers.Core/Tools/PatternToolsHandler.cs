using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Weavers.Core;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pattern;
using Weavers.Core.Models;
using MediatR;

namespace Weavers.Core.Tools {

  interface IPatternToolsHandler {
    Task<string> AddPattern(int parentId, string name);
    Task<string> AddPatDimension(int patternId, string name, string commaDelimOptions);
    Task<string> AddPatDimOption(int patDimId, string name);
    Task<string> GetNextDraw(int patternId, int todoId);
    Task<string> RejectDraw(int drawId, int todoId);
    Task<string> AcceptDraw(int drawId, int todoId);
  }

  public class PatternToolsHandler : IPatternToolsHandler {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<PatternToolsHandler> _logger;
    public PatternToolsHandler(IServiceScopeFactory serviceScopeFactory, ILogger<PatternToolsHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    public async Task<string> AddPattern(int parentId, string name) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var pattern = await mediator.Send(new AddPatternCommand(parentId, name));
        if (pattern == null) {
          return _logger.DefaultAddEmptyMessage(Cx.CmdAddPattern, 0);
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddPattern, await context.ToSummary(pattern, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddPattern, 0, $"Failed to add pattern {name} {ex.Message}");
      }
    }

    public async Task<string> AddPatDimension(int patternId, string name, string commaDelimOptions) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var patDim = await mediator.Send(new AddPatDimensionCommand(patternId, name, commaDelimOptions));
        if (patDim == null) {
          return _logger.DefaultAddEmptyMessage(Cx.CmdAddPatDimension, 0);
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddPatDimension, await context.ToSummary(patDim, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddPatDimension, 0, $"Failed to add pattern dimension {name} {ex.Message}");
      }
    }

    public async Task<string> AddPatDimOption(int patDimId, string name) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var patDimOption = await mediator.Send(new AddPatDimOptionCommand(patDimId, name));
        if (patDimOption == null) {
          return _logger.DefaultAddEmptyMessage(Cx.CmdAddPatDimOption, 0);
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddPatDimOption, await context.ToSummary(patDimOption, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddPatDimOption, 0, $"Failed to add pattern dimension option {name} {ex.Message}");
      }
    }

    public async Task<string> GetNextDraw(int patternId, int todoId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var draw = await mediator.Send(new GetNextDrawCommand(patternId, todoId));
        if (draw == null) {
          return _logger.DefaultAddEmptyMessage(Cx.CmdGetNextDraw, 0);
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdGetNextDraw, await context.ToSummary(draw, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdGetNextDraw, 0, $"Failed to get next draw for pattern {patternId} {ex.Message}");
      }
    }

    public async Task<string> RejectDraw(int drawId, int todoId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var draw = await mediator.Send(new RejectDrawCommand(drawId, todoId));
        if (draw == null) {
          return _logger.DefaultAddEmptyMessage(Cx.CmdRejectDraw, 0);
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdRejectDraw, await context.ToSummary(draw, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdRejectDraw, 0, $"Failed to reject draw {drawId} {ex.Message}");
      }
    }

    public async Task<string> AcceptDraw(int drawId, int refItemId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var draw = await mediator.Send(new AcceptDrawCommand(drawId, refItemId));
        if (draw == null) {
          return _logger.DefaultAddEmptyMessage(Cx.CmdAcceptDraw, 0);
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAcceptDraw, await context.ToSummary(draw, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAcceptDraw, 0, $"Failed to accept draw {drawId} {ex.Message}");
      }
    }   


  }

}
