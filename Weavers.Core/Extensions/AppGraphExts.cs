using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Weavers.Core.Models;
using Weavers.Core.Tools;

namespace Weavers.Core.Extensions {
  public static class AppGraphExts {
    public static string DefaultAddEmptyMessage(this ILogger _logger, string commandName, int id) {
      string msg = $"yea sorry, {commandName} failed to return new item for itemid:{id}";
      _logger.LogError(msg);
      var opResult = McpOpResult.CreateFailure(commandName, msg);
      return JsonSerializer.Serialize(opResult);
    }

    public static string DefaultFailToFindMessage(this ILogger _logger, string commandName, int id) {
      string msg = $"yea sorry, {commandName} failed to find itemid:{id}";
      _logger.LogError(msg);
      var opResult = McpOpResult.CreateFailure(commandName, msg);
      return JsonSerializer.Serialize(opResult);
    }

    public static string DefaultInvalidParentMessage(this ILogger _logger, string commandName, int id) {
      string msg = $"yea sorry, {commandName} validation failed itemid:{id} is not a valid parent.";
      _logger.LogError(msg);
      var opResult = McpOpResult.CreateFailure(commandName, msg);
      return JsonSerializer.Serialize(opResult);
    }

    public static string ToOpResult(this Exception ex, ILogger _logger, string commandName, int id, string message) {
      _logger.LogError(ex, $"{commandName} Exception on itemId: {id} with message: {message}");
      var opResult = McpOpResult.CreateFailure(commandName, $"yea sorry, {commandName} excepted on itemid:{id} with an internal error.", ex);
      return JsonSerializer.Serialize(opResult);
    }
  }
}
