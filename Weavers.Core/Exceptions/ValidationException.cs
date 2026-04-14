using System.ComponentModel.DataAnnotations;

namespace Weavers.Core.Exceptions {

  [Serializable]
  public class ValidationException : Exception {
    public ValidationResult? ValidationResult { get; }

    public List<string> ValidationErrors { get; }

    public List<string> ValidationWarnings { get; }

    public ValidationException()
        : base("Validation failed") {
      ValidationErrors = new List<string>();
      ValidationWarnings = new List<string>();
    }

    public ValidationException(string message)
        : base(message) {
      ValidationErrors = new List<string>();
      ValidationWarnings = new List<string>();
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException) {
      ValidationErrors = new List<string>();
      ValidationWarnings = new List<string>();
    }

    public override string ToString() {
      string text = base.ToString();
      if (ValidationErrors.Count > 0) {
        text = text + "\nValidation Errors: " + string.Join(", ", ValidationErrors);
      }

      if (ValidationWarnings.Count > 0) {
        text = text + "\nValidation Warnings: " + string.Join(", ", ValidationWarnings);
      }

      return text;
    }
  }
}
