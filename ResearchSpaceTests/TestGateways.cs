using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ResearchSpaceTests {

  // these are proof of concept tests for the ComfyUI gateway, which is a local-only service.
  // so these tests are not meant to be run in a CI/CD pipeline, but rather as a local test harness
  // for development and experimentation.  They show the minimum steps needed to submit a prompt and
  // retrieve an image output from the ComfyUI gateway.  
  //[TestClass]
  public class TestGateways {

    //[TestMethod]
    public async Task TestMethod1() {
      // Implement your test logic here
      const string gatewayUrl = "http://localhost:8188/"; // Replace with your actual gateway URL
      const string TextToImageExportedJsonPath = "C:\\Floor\\WeaversGuild\\ResearchSpaceTests\\FirstExport.json"; // Path to your exported JSON file

      // 1. Load the API-format JSON you exported via "Save (API Format)"
      var workflowJson = await File.ReadAllTextAsync(TextToImageExportedJsonPath);
      var workflow = JsonNode.Parse(workflowJson)!.AsObject();

      // 2. Mutate the specific node inputs — node IDs come from the exported file itself,
      // open it once and note which node holds your prompt text / seed.
      var PromptText = "a red panda wearing a tiny wizard hat, comic panel style";
      var SecondPrompt = "low quality, bad anatomy, extra digits, missing digits, extra limbs, missing limbs";

      workflow["67"]!["inputs"]!["text"] = PromptText;
      workflow["71"]!["inputs"]!["text"] = SecondPrompt;
      workflow["70"]!["inputs"]!["seed"] = Random.Shared.NextInt64(0, long.MaxValue);

      // 3. Wrap in the envelope /prompt expects
      var clientId = Guid.NewGuid().ToString();
      var payload = new JsonObject {
        ["prompt"] = workflow,
        ["client_id"] = clientId
      };

      using var http = new HttpClient { BaseAddress = new Uri(gatewayUrl) };

      // 4. Submit — this returns immediately, generation is async
      var submitResponse = await http.PostAsJsonAsync("/prompt", payload);
      submitResponse.EnsureSuccessStatusCode();
      var submitResult = await submitResponse.Content.ReadFromJsonAsync<JsonObject>();
      var promptId = submitResult!["prompt_id"]!.GetValue<string>();

      Assert.AreEqual(false, string.IsNullOrEmpty(promptId));

      // 5. Poll /history until it's populated (fine for a test; use the websocket in real code)
      JsonObject? historyEntry = null;
      for (var i = 0; i < 30; i++) // ~30s timeout
      {
        await Task.Delay(1000);
        var history = await http.GetFromJsonAsync<JsonObject>($"/history/{promptId}");
        if (history is not null && history.ContainsKey(promptId)) {
          historyEntry = history[promptId]!.AsObject();
          break;
        }
      }

      Assert.AreEqual(false, historyEntry is null);

      // 6. Pull the output filename and confirm we can fetch bytes back
      var outputs = historyEntry!["outputs"]!.AsObject();
      var firstNodeOutput = outputs.First().Value!.AsObject();
      var image = firstNodeOutput["images"]!.AsArray()[0]!.AsObject();
      var filename = image["filename"]!.GetValue<string>();
      var subfolder = image["subfolder"]!.GetValue<string>();

      var imageBytes = await http.GetByteArrayAsync(
          $"/view?filename={filename}&subfolder={subfolder}&type=output");

      Assert.AreEqual(true, imageBytes.Length > 0);



    }

    public enum WeItemT9 {

      ComfyServiceModel = 10,  // Properties like:
                               // BaseUrl = "http://localhost:8188/",  // the base url for the comfy service, used to build the full url for the workflow api calls.
                               // RunState = "Idle",  // the current run state of the service, can be Idle, Running, or Error.
                               // InputFolder  // Comfy services only run locally.  so path is relative to the service's local file system.  this is the folder where the service will look for input files to process.
                               // ExportFolder  // Comfy services only run locally.  so path is relative to the service's local file system.  this is the folder where the service will write output files to.
        ComfyTemplatesModel = 20,  // folder to hold workflow templates, created when service is created.  
          ComfyWorkflowTemplates = 30, // holds the default workflow json in data column, needs imported from a comfy workflow export api json file.
            ComfyWfParamModel = 40,  // holds a single node identifier parameter to locate json value with a reference item requirement.
        ComfyOperationsModel = 50,  // folder to hold workflow operations, created when service is created.
          ComfyOpWorkflowModel = 60,  // holds a single node identifier parameter to locate json value with a parameter var requirement.
            ComfyOpParamModel 


    }

    //[TestMethod]
    public async Task ExploreJsonLoadUp() {
        
      const string TextToImageExportedJsonPath = "C:\\Floor\\WeaversGuild\\ResearchSpaceTests\\FirstExport.json"; // Path to your exported JSON file

      // 1. Load the API-format JSON you exported via "Save (API Format)"
      var workflowJson = await File.ReadAllTextAsync(TextToImageExportedJsonPath);
      var workflow = JsonNode.Parse(workflowJson)!.AsObject();

      // 2. Mutate the specific node inputs — node IDs come from the exported file itself,
      // open it once and note which node holds your prompt text / seed.
      var PromptText = "a red panda wearing a tiny wizard hat, comic panel style";
      var SecondPrompt = "low quality, bad anatomy, extra digits, missing digits, extra limbs, missing limbs";

      workflow["67"]!["inputs"]!["text"] = PromptText;
      workflow["71"]!["inputs"]!["text"] = SecondPrompt;
      workflow["70"]!["inputs"]!["seed"] = Random.Shared.NextInt64(0, long.MaxValue);

      // 3. Wrap in the envelope /prompt expects
      var clientId = Guid.NewGuid().ToString();
      var payload = new JsonObject {
        ["prompt"] = workflow,
        ["client_id"] = clientId
      };

    }

  }
}
