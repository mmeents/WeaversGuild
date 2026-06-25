using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Weavers.Core.Models {
  #region SHARED / COMMON 
  /// <summary>Base class for polymorphic chat input items.</summary>
  [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
  [JsonDerivedType(typeof(TextInputItem), "message")]
  [JsonDerivedType(typeof(ImageInputItem), "image")]
  public abstract class InputItem {
    [JsonPropertyName("type")]
    public abstract string Type { get; }
  }

  public class TextInputItem : InputItem {
    [JsonPropertyName("type")]
    public override string Type => "message";

    [JsonPropertyName("content")]
    public required string Content { get; set; }
  }

  public class ImageInputItem : InputItem {
    [JsonPropertyName("type")]
    public override string Type => "image";

    [JsonPropertyName("data_url")]
    public required string DataUrl { get; set; }
  }
  #endregion

  #region INTEGRATIONS
  [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
  [JsonDerivedType(typeof(PluginIntegration), "plugin")]
  [JsonDerivedType(typeof(EphemeralMcpIntegration), "ephemeral_mcp")]
  public abstract class Integration {
    [JsonPropertyName("type")]
    public abstract string Type { get; }
  }

  public class PluginIntegration : Integration {
    [JsonPropertyName("type")]
    public override string Type => "plugin";

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("allowed_tools")]
    public List<string>? AllowedTools { get; set; }
  }

  public class EphemeralMcpIntegration : Integration {
    [JsonPropertyName("type")]
    public override string Type => "ephemeral_mcp";

    [JsonPropertyName("server_label")]
    public required string ServerLabel { get; set; }

    [JsonPropertyName("server_url")]
    public required string ServerUrl { get; set; }

    [JsonPropertyName("allowed_tools")]
    public List<string>? AllowedTools { get; set; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }
  }
  #endregion

  #region CHAT REQUEST

  public class ChatRequest {
    [JsonPropertyName("model")]
    public required string Model { get; set; }

    /// <summary>
    /// Either a plain string or a list of InputItems (TextInputItem / ImageInputItem).
    /// Use the factory helpers below for convenience.
    /// </summary>
    [JsonPropertyName("input")]
    public required object Input { get; set; }

    [JsonPropertyName("system_prompt")]
    public string? SystemPrompt { get; set; }

    [JsonPropertyName("integrations")]
    public List<Integration>? Integrations { get; set; }

    [JsonPropertyName("stream")]
    public bool? Stream { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    [JsonPropertyName("top_k")]
    public int? TopK { get; set; }

    [JsonPropertyName("min_p")]
    public double? MinP { get; set; }

    [JsonPropertyName("repeat_penalty")]
    public double? RepeatPenalty { get; set; }

    [JsonPropertyName("max_output_tokens")]
    public int? MaxOutputTokens { get; set; }

    /// <summary>"off" | "low" | "medium" | "high" | "on"</summary>
    [JsonPropertyName("reasoning")]
    public string? Reasoning { get; set; }

    [JsonPropertyName("context_length")]
    public int? ContextLength { get; set; }

    [JsonPropertyName("store")]
    public bool? Store { get; set; }

    [JsonPropertyName("previous_response_id")]
    public string? PreviousResponseId { get; set; }

    // ── Convenience factories ─────────────────────────────────

    public static ChatRequest Simple(string model, string prompt, string? systemPrompt = null) =>
        new() {
          Model = model,
          Input = prompt,
          SystemPrompt = systemPrompt
        };

    public static ChatRequest WithItems(string model, List<InputItem> items, string? systemPrompt = null) =>
        new() {
          Model = model,
          Input = items,
          SystemPrompt = systemPrompt
        };
  }
  #endregion


  #region CHAT RESPONSE
  public class ChatResponse {
    [JsonPropertyName("model_instance_id")]
    public string? ModelInstanceId { get; set; }

    [JsonPropertyName("output")]
    public List<OutputItem> Output { get; set; } = [];

    [JsonPropertyName("stats")]
    public ChatStats? Stats { get; set; }

    [JsonPropertyName("response_id")]
    public string? ResponseId { get; set; }

    // ── Helpers ───────────────────────────────────────────────

    /// <summary>Returns the concatenated text of all message output items.</summary>
    public string GetText() =>
        string.Concat(Output.OfType<MessageOutputItem>().Select(m => m.Content));
  }

  [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
  [JsonDerivedType(typeof(MessageOutputItem), "message")]
  [JsonDerivedType(typeof(ToolCallOutputItem), "tool_call")]
  [JsonDerivedType(typeof(ReasoningOutputItem), "reasoning")]
  [JsonDerivedType(typeof(InvalidToolCallOutputItem), "invalid_tool_call")]
  public abstract class OutputItem {
    [JsonPropertyName("type")]
    public abstract string Type { get; }
  }

  public class MessageOutputItem : OutputItem {
    [JsonPropertyName("type")]
    public override string Type => "message";

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
  }

  public class ToolCallOutputItem : OutputItem {
    [JsonPropertyName("type")]
    public override string Type => "tool_call";

    [JsonPropertyName("tool")]
    public string Tool { get; set; } = string.Empty;

    [JsonPropertyName("arguments")]
    public JsonElement Arguments { get; set; }

    [JsonPropertyName("output")]
    public string? Output { get; set; }

    [JsonPropertyName("provider_info")]
    public ProviderInfo? ProviderInfo { get; set; }
  }

  public class ReasoningOutputItem : OutputItem {
    [JsonPropertyName("type")]
    public override string Type => "reasoning";

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
  }

  public class InvalidToolCallOutputItem : OutputItem {
    [JsonPropertyName("type")]
    public override string Type => "invalid_tool_call";

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public InvalidToolCallMetadata? Metadata { get; set; }
  }

  public class InvalidToolCallMetadata {
    /// <summary>"invalid_name" | "invalid_arguments"</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("tool_name")]
    public string ToolName { get; set; } = string.Empty;

    [JsonPropertyName("arguments")]
    public JsonElement? Arguments { get; set; }

    [JsonPropertyName("provider_info")]
    public ProviderInfo? ProviderInfo { get; set; }
  }

  public class ProviderInfo {
    /// <summary>"plugin" | "ephemeral_mcp"</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("plugin_id")]
    public string? PluginId { get; set; }

    [JsonPropertyName("server_label")]
    public string? ServerLabel { get; set; }
  }

  public class ChatStats {
    [JsonPropertyName("input_tokens")]
    public double InputTokens { get; set; }

    [JsonPropertyName("total_output_tokens")]
    public double TotalOutputTokens { get; set; }

    [JsonPropertyName("reasoning_output_tokens")]
    public double ReasoningOutputTokens { get; set; }

    [JsonPropertyName("tokens_per_second")]
    public double TokensPerSecond { get; set; }

    [JsonPropertyName("time_to_first_token_seconds")]
    public double TimeToFirstTokenSeconds { get; set; }

    [JsonPropertyName("model_load_time_seconds")]
    public double? ModelLoadTimeSeconds { get; set; }
  }

  // ─────────────────────────────────────────────────────────────
  // MODELS RESPONSE
  // ─────────────────────────────────────────────────────────────

  public class ModelsResponse {
    [JsonPropertyName("models")]
    public List<LmModel> Models { get; set; } = [];
  }

  public class LmModel {
    /// <summary>"llm" | "embedding"</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }

    [JsonPropertyName("key")]  // is used saved in app as model name key.
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("display_name")] // is used for display in UI as human readable name.
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("architecture")]
    public string? Architecture { get; set; }

    [JsonPropertyName("quantization")]
    public ModelQuantization? Quantization { get; set; }

    [JsonPropertyName("size_bytes")]
    public long SizeBytes { get; set; }

    [JsonPropertyName("params_string")]
    public string? ParamsString { get; set; }

    [JsonPropertyName("loaded_instances")]
    public List<LoadedInstance> LoadedInstances { get; set; } = [];

    [JsonPropertyName("max_context_length")]
    public int MaxContextLength { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("capabilities")]
    public ModelCapabilities? Capabilities { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("variants")]
    public List<string>? Variants { get; set; }

    [JsonPropertyName("selected_variant")]
    public string? SelectedVariant { get; set; }

    // ── Helpers ───────────────────────────────────────────────

    public bool IsLoaded => LoadedInstances.Count > 0;

    public double SizeGb => Math.Round(SizeBytes / 1_073_741_824.0, 2);
  }

  public class ModelQuantization {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("bits_per_weight")]
    public int BitsPerWeight { get; set; }
  }

  public class LoadedInstance {
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("config")]
    public LoadedInstanceConfig? Config { get; set; }
  }

  public class LoadedInstanceConfig {
    [JsonPropertyName("context_length")]
    public int ContextLength { get; set; }

    [JsonPropertyName("eval_batch_size")]
    public int EvalBatchSize { get; set; }

    [JsonPropertyName("flash_attention")]
    public bool FlashAttention { get; set; }

    [JsonPropertyName("offload_kv_cache_to_gpu")]
    public bool OffloadKvCacheToGpu { get; set; }
  }

  public class ModelCapabilities {
    [JsonPropertyName("vision")]
    public bool Vision { get; set; }

    [JsonPropertyName("trained_for_tool_use")]
    public bool TrainedForToolUse { get; set; }
  }
  #endregion
}


