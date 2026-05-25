using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Weavers.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using Weavers.Core.Extensions;
using Weavers.Core.Constants;

namespace Weavers.Core.Service {

  public interface ILmStudioService {    
    Task<List<LmModel>> GetLlmModelsAsync(int GatewayPresenceId, CancellationToken ct = default); // app uses this one.
    Task<ChatResponse> ChatAsync(int GatewayPresenceId, ChatRequest request, CancellationToken ct = default);
  }

  public class LmStudioService : ILmStudioService {
    private readonly FabricDbContext _fabricDbContext;
    private readonly ILogger<LmStudioService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() {
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };
    private readonly ICryptoService _cryptoService;

    public LmStudioService(FabricDbContext fabricDbContext, ILogger<LmStudioService> logger, ICryptoService cryptoService) {
      _fabricDbContext = fabricDbContext;
      _logger = logger;
      _cryptoService = cryptoService;
    }

    public async Task<List<LmModel>> GetLlmModelsAsync(int GatewayPresenceId, CancellationToken ct = default) {
      var gateway = await _fabricDbContext.GetItemDtoById(GatewayPresenceId, ct);
      if (gateway == null) { 
        throw new InvalidOperationException($"Gateway with ID {GatewayPresenceId} not found."); 
      }

      var lmStudioUrl = gateway.Properties.FirstOrDefault(p => p.Name == Cx.ItUrlBase)?.Value;
      var ApiToken = gateway.Properties.FirstOrDefault(p => p.Name == Cx.ItApiToken)?.Value;
      if (ApiToken == null || lmStudioUrl == null) {
        throw new InvalidOperationException($"bad API token or LM Studio URL for Gateway ID {GatewayPresenceId}.");
      }
      if (_cryptoService != null) {
        ApiToken = _cryptoService.Decrypt(ApiToken);
      }
      var _http = new HttpClient {
        BaseAddress = new Uri(lmStudioUrl.TrimEnd('/') + "/"),
        Timeout = Timeout.InfiniteTimeSpan
      };
      _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiToken);

      var response = await _http.GetAsync("api/v1/models", ct);
      response.EnsureSuccessStatusCode();
      var result = await response.Content.ReadFromJsonAsync<ModelsResponse>(JsonOptions, ct)
             ?? throw new InvalidOperationException("Null response from /api/v1/models");

      return result.Models.Where(m => m.Type == "llm").ToList();
    }

    public async Task<ChatResponse> ChatAsync(int GatewayPresenceId, ChatRequest request, CancellationToken ct = default) {
      try {

        var gateway = await _fabricDbContext.GetItemDtoById(GatewayPresenceId, ct);
        if (gateway == null) {
          throw new InvalidOperationException($"Gateway with ID {GatewayPresenceId} not found.");
        }

        var lmStudioUrl = gateway.Properties.FirstOrDefault(p => p.Name == Cx.ItUrlBase)?.Value;
        var ApiToken = gateway.Properties.FirstOrDefault(p => p.Name == Cx.ItApiToken)?.Value;
        if (ApiToken == null || lmStudioUrl == null) {
          throw new InvalidOperationException($"bad API token or LM Studio URL for Gateway ID {GatewayPresenceId}.");
        }
        if (_cryptoService != null) {
          ApiToken = _cryptoService.Decrypt(ApiToken);
        }
        var _http = new HttpClient {
          BaseAddress = new Uri(lmStudioUrl.TrimEnd('/') + "/"),
          Timeout = Timeout.InfiniteTimeSpan
        };
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiToken);

        var httpResponse = await _http.PostAsJsonAsync("api/v1/chat", request, JsonOptions, ct);

        httpResponse.EnsureSuccessStatusCode();

        return await httpResponse.Content.ReadFromJsonAsync<ChatResponse>(JsonOptions, ct)
          ?? throw new InvalidOperationException("Null response from /api/v1/chat");


      } catch (Exception ex) {
        _logger.LogError(ex, "Error in ChatAsync: {Message}", ex.Message);
        throw;
      }
    }
  }
}
