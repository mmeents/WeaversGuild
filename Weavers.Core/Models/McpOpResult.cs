using System.Text.Json;
using System.Text.Json.Serialization;

namespace Weavers.Core.Models {
  public class McpOpResult {
    public bool Success { get; set; }
    public string Op { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Result { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMsg { get; set; }

    public static McpOpResult CreateSuccess(string operation, object? data = null) {
      if (string.IsNullOrEmpty(operation)) {
        throw new ArgumentException("Error operation must be provided", nameof(operation));
      }
      McpOpResult result = new() { Success = true, Op = operation, Result = data };      
      return result;
    }

    public static McpOpResult CreateFailure(string operation, string errorMessage, Exception? exception = null) {
      if (string.IsNullOrEmpty(operation)) {
        throw new ArgumentException("Error operation must be provided", nameof(operation));
      }
      if (string.IsNullOrEmpty(errorMessage)) {
        throw new ArgumentException("Error errorMessage must be provided", nameof(errorMessage));
      }
      return new() { Success = false, Op = operation, ErrorMsg = errorMessage };
    }

    public override string ToString() {
      return JsonSerializer.Serialize<McpOpResult>(this);
    }
  }
}
