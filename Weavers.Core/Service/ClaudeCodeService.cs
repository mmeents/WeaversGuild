using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace Weavers.Core.Service {

  public interface IClaudeCodeService {
    Task<List<LmModel>> GetLlmModelsAsync(int GatewayPresenceId, CancellationToken ct = default);
    Task<ChatResponse> ChatAsync(int GatewayPresenceId, ChatRequest request, bool skipPermissions, CancellationToken ct = default);
  }

  public class ClaudeCodeService : IClaudeCodeService {
    private readonly FabricDbContext _fabricDbContext;
    private readonly ILogger<ClaudeCodeService> _logger;

    // Hardcoded model list. Add Fable here as a one-liner when it's back.
    // NOTE: confirm whether installed CLI wants short alias ("opus"/"sonnet") or full API string.
    private static readonly List<LmModel> _models = new() {
      new LmModel { Key = "opus", DisplayName = "Opus" },
      new LmModel { Key = "sonnet", DisplayName = "Sonnet" },
    };

    public ClaudeCodeService(FabricDbContext fabricDbContext, ILogger<ClaudeCodeService> logger) {
      _fabricDbContext = fabricDbContext;
      _logger = logger;
    }

    public Task<List<LmModel>> GetLlmModelsAsync(int GatewayPresenceId, CancellationToken ct = default)
      => Task.FromResult(_models);

    public async Task<ChatResponse> ChatAsync(int GatewayPresenceId, ChatRequest request, bool skipPermissions, CancellationToken ct = default) {

      // ---- pull what we need off the request (ADJUST to your real ChatRequest members) ----
      string model = request.Model; // e.g., "opus" or "sonnet"
      string systemPrompt =  request?.SystemPrompt ?? "";
      string userPrompt = request?.Input as string ?? "";      

      if (string.IsNullOrWhiteSpace(userPrompt))
        throw new InvalidOperationException("UserPrompt cannot be null or empty.");

      var psi = new ProcessStartInfo {
        FileName = "claude",                          // resolved via system PATH
        WorkingDirectory = WeaverExt.ClaudeExecutablePath, // empty working folder w/ .mcp.json
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,                         // harness runs headless
        StandardOutputEncoding = System.Text.Encoding.UTF8,
        StandardErrorEncoding = System.Text.Encoding.UTF8,
      };

      // ArgumentList => .NET handles all platform quoting. No hand-rolled escaping.
      psi.ArgumentList.Add("-p");
      psi.ArgumentList.Add(userPrompt);
      if (!string.IsNullOrWhiteSpace(systemPrompt)) {
        psi.ArgumentList.Add("--append-system-prompt"); // verify flag name vs --system-prompt
        psi.ArgumentList.Add(systemPrompt);
      }
      psi.ArgumentList.Add("--model");
      psi.ArgumentList.Add(model);
      psi.ArgumentList.Add("--output-format");
      psi.ArgumentList.Add("text");                    // verify still valid on installed CLI
      if (skipPermissions)
        psi.ArgumentList.Add("--dangerously-skip-permissions");

      using var process = new Process { StartInfo = psi };

      try {
        process.Start();

        // Read both pipes concurrently to avoid buffer-fill deadlock.
        var stdoutTask = process.StandardOutput.ReadToEndAsync(ct);
        var stderrTask = process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);
        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
          throw new Exception($"claude -p exited with code {process.ExitCode}: {stderr}");

        // ---- map stdout into your WeaversGuild ChatResponse (ADJUST to real shape) ----
        var response = new ChatResponse {
          Output = new List<OutputItem> {
             new MessageOutputItem {
                Content = stdout.Trim()
             }
           }
        };

        return response;
      } catch (OperationCanceledException) {
        KillTree(process);
        throw;
      } catch (Exception ex) {
        KillTree(process);
        _logger.LogError(ex, "Error in ClaudeCodeService.ChatAsync: {Message}", ex.Message);
        throw;
      }
    }

    private static void KillTree(Process process) {
      try { if (!process.HasExited) process.Kill(entireProcessTree: true); } catch { /* already gone */ }
    }
  }
}
