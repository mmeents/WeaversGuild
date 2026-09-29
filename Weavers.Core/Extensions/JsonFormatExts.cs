using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Weavers.Core.Extensions {
  public static class JsonFormatUtil {

    private static readonly JsonSerializerOptions PrettyOptions = new() {
      WriteIndented = true,
      // keeps escaped-looking node titles/prompts readable rather than \u-escaped
      Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private static readonly JsonSerializerOptions CondensedOptions = new() {
      WriteIndented = false
    };

    /// Reformats an arbitrary JSON string into indented, human-readable form.
    /// Throws JsonException if the input isn't valid JSON — caller decides whether that's worth catching.
    public static string ToPretty(string json) {
      using var doc = JsonDocument.Parse(json);
      return JsonSerializer.Serialize(doc.RootElement, PrettyOptions);
    }

    /// Round-trips back to condensed form, e.g. before storing in a Data field where you don't want
    /// indentation whitespace bloating every row.
    public static string ToCondensed(string json) {
      using var doc = JsonDocument.Parse(json);
      return JsonSerializer.Serialize(doc.RootElement, CondensedOptions);
    }

    /// Non-throwing variant for editor display — falls back to the original string if it isn't valid JSON,
    /// so a partially-typed or malformed field doesn't blow up the UI while someone's still editing it.
    public static string TryToPretty(this string json) {
      try { return ToPretty(json); } catch (JsonException) { return json; }
    }
  }
}
