using MediatR;
using System.Diagnostics;
using System.Text.Json;
using Weavers.Core.Extensions;
using Weavers.Core.Entities;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Pipeline {

  public class McpLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull {
    private readonly FabricDbContext _context;
    private readonly IAppSessionService _session;
    public McpLoggingBehavior(FabricDbContext context, IAppSessionService session) {
      _context = context;
      _session = session;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct) {
      if (request is not IMcpRequest) return await next();
      var sw = Stopwatch.StartNew();
      try {
        var opName = typeof(TRequest).Name;
        var inputJson = JsonSerializer.Serialize(request);
        var response = await next();
        var outputJson = JsonSerializer.Serialize(response);
        sw.Stop();

        // write to log 
        var logEntry = new MediatorLog {
          CalledAt = DateTime.UtcNow,
          OpName = opName,
          InputJson = inputJson,
          OutputJson = outputJson,
          DurationMs = (int)sw.ElapsedMilliseconds,
          SessionId = _session.SessionId,
          Success = true
        };
        await _context.WriteMcpLogEntry(logEntry);
        return response;
      } catch (Exception) {
        sw.Stop();
        throw;
      }
    }
  }

  public interface IMcpRequest { }


}
