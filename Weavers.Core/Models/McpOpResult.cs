using System.Text.Json;

namespace Weavers.Core.Models {
  public class McpOpResult {
    public bool Success { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static McpOpResult CreateSuccess(string operation, string message, object? data = null) {
      if (string.IsNullOrEmpty(operation)) {
        throw new ArgumentException("Error operation must be provided", nameof(operation));
      }
      if (string.IsNullOrEmpty(message)) {
        throw new ArgumentException("Error message must be provided", nameof(message));
      }
      return new() { Success = true, Operation = operation, Message = message, Data = data };
    }

    public static McpOpResult CreateFailure(string operation, string errorMessage, Exception? exception = null) {
      if (string.IsNullOrEmpty(operation)) {
        throw new ArgumentException("Error operation must be provided", nameof(operation));
      }
      if (string.IsNullOrEmpty(errorMessage)) {
        throw new ArgumentException("Error errorMessage must be provided", nameof(errorMessage));
      }
      return new() { Success = false, Operation = operation, ErrorMessage = errorMessage };
    }

    public override string ToString() {
      return JsonSerializer.Serialize<McpOpResult>(this);
    }
  }
}
