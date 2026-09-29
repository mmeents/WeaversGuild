# About the Comfy Gateway

The Comfy gateway lets TheLoomApp drive a ComfyUI install: pick a workflow, fill in its inputs from a todo, run it on the Comfy Schedule, and keep the output as an item in the graph. For how it fits alongside the LLM gateways, see **AboutTheGateways**.

## Concepts

| Term | Item type | What it is |
|---|---|---|
| **Comfy gateway** | Presence Gateway | Created when `HasComfy` is ticked and saved on the harness's Gateways folder. |
| **Workflow template** | `ComfyWorkflowTemplate` | An exported ComfyUI workflow (API format JSON) stored in the item's **Data** field. Lives under **Configured Workflows**. |
| **Parameter** | `ComfyWfParamModel` | A child of a template that points at one input inside the JSON and says what type it is. |
| **Operation** | Workgroup folder under **Operations** | Groups Comfy todos, like a WorkGroup does for desk todos. |
| **Comfy todo** | Comfy todo | One run of a workflow, with its own copy of the parameters holding the values for that run. |
| **Output** | `ComfyMediaFileModel` | The file ComfyUI produced (image, audio…), viewable inline in the app. |

```
Comfy (Presence Gateway)
├── Configured Workflows
│   └── TextToImage1           (ComfyWorkflowTemplate, Data = workflow JSON)
│       ├── Prompt             (ComfyWfParamModel → 67 / inputs / text)
│       └── seed               (ComfyWfParamModel → 70 / inputs / seed)
└── Operations
    └── <operation>
        └── <comfy todo>       (cloned params with PropValue filled in)
```

## 1. Enable Comfy Gateway
Find and install Comfy if not already. Pick a template you want to use.

From the Gateways item, when clicked find Details Tab, then Properties Tabs, and click the HasComfy checkbox and OK to save. That should create the Comfy Gateway, click it and inspect the properties.

You might need to right click and Reload Projects to refresh.

Comfy Gateway has 3 properties, UrlBase, Service Input and Service Output.
- UrlBase should be the local Comfy install `http://localhost:8188/` 
- ServiceInput - location for inbound files usually like: `c:\Users\YourUserName\AppData\Local\Comfy-Desktop\ComfyUI-Shared\input\`
- ServiceOutput - location for output files usually like: `c:\Users\YourUserName\AppData\Local\Comfy-Desktop\ComfyUI-Shared\output\`

important to set the folders so the assets link back. And UrlBase so Api gets called.

## 2. Export the workflow from ComfyUI

Build and test the workflow in ComfyUI first. Then export it using **Export (API)** — not the regular save. The API format is a flat object keyed by node id:

```json
"67": {
  "inputs": { "text": "a fox by a campfire...", "clip": ["62", 0] },
  "class_type": "CLIPTextEncode",
  "_meta": { "title": "CLIP Text Encode (Prompt)" }
}
```

The regular (UI) save format includes layout and links and won't work.

## 3. Add the template in TheLoomApp

1. Under the Comfy gateway, select **Configured Workflows** and add a workflow template.
2. Give it a name and choose the exported JSON file. The file's contents load into the template's **Data** field.
3. Template properties:
   - **Enabled** — whether it's offered when creating operations/todos.
   - **TimeoutSec** — how long to wait for ComfyUI to finish (default 300). Raise it for long jobs like music.
   - **Content / description** — worth filling in. It's what `listComfyWorkflows` returns, so a good description is how agents (and future-you) know what the workflow needs.

## 4. Add parameters

A parameter says "this input in the JSON is something a todo will supply." Each one addresses a value by three keys, following the shape of the JSON:

| Property | Meaning | Typical value |
|---|---|---|
| **SectionKey** | The node id (top-level key) | `67` |
| **ObjectKey** | The object inside the node | `inputs` |
| **PropKey** | The input name | `text`, `seed`, `caption`, `lyrics`… |
| **OverrideType** | Value type | **Seed**, **Int**, **String**, **Decimal** |
| **PropValue** | Default / example value | (optional on the template) |

So `67 / inputs / text` means `data["67"]["inputs"]["text"]`.

Name the parameter for what it means (**Prompt**, **Negative**, **Seed**, **Lyrics**), not for the node — the name is what a person or agent sees when filling in a todo.

**Finding the keys:** open the exported JSON, find the node by its `class_type` or `_meta.title`, and read the id off the top-level key. Common ones:

| You want | Look for `class_type` | PropKey |
|---|---|---|
| Positive / negative prompt | `CLIPTextEncode` (two of them — check which feeds `positive` on the sampler) | `text` |
| Seed | `KSampler` (or the sampler/generator node) | `seed` |
| Image size | `EmptyLatentImage` / `EmptySD3LatentImage` | `width`, `height` |
| Music caption / lyrics | the model's text-encode node | depends on the workflow |

> ⚠️ **Node ids can change when a workflow is re-exported.** If you edit a workflow in ComfyUI and re-import it, recheck every parameter's SectionKey against the new JSON.

## 5. Create an operation and todos

1. Right Click **Operations**, to create Todo.
2. Create a Comfy todo and choose the workflow template to clone.
3. The todo gets its own copy of the template's parameters. Fill in each **PropValue** with the value for this run — the prompt text, lyrics, and so on.
4. Mark the todo **Ready**.

Agents can do the same through MCP: `listComfyWorkflows` to see what's available, then `addComfyTodo`.

## 6. Run and review

The Comfy Schedule lives in its own root tab with three inner tabs:

- **Comfy Todo Ready Review** — todos waiting for approval.
- **Todo Schedule** — approved todos queued or running.
- **Operation Results** — finished runs and their output files.

When a todo runs, the gateway clones the template JSON, writes each parameter's PropValue into its SectionKey/ObjectKey/PropKey location, submits it to ComfyUI, waits (up to TimeoutSec), fetches the output, and saves it as a `ComfyMediaFileModel`. The todo's **Status** moves through to complete or failed.

Todos can be chained and run in batches.

## Parameter types

| Type | Use for | Notes |
|---|---|---|
| **Seed** | Random seeds | *TODO: document behavior when PropValue is blank (random per run?) vs set (fixed / reproducible).* |
| **Int** | Steps, width, height, duration in seconds | |
| **String** | Prompts, captions, lyrics | |
| **Decimal** | CFG, denoise, shift | |

## Open questions / to document later

- Seed behavior when blank (see above). A: Seeds have a Seed type that will auto populate a long, but type could be set to int and manually driven.
- What happens to existing todos when their source template is edited — they hold a copy, so presumably nothing? A: Correct, Source Templates get cloned when creating a todo.
- Image inputs (img2img / edit workflows) — how a reference image gets into a parameter. A: Not yet sure, been working on text to image, music so far.  need to work out a copy method that moves images during the run todo if a parameter is of type file.
- Workflows with more than one output node. A: Should make a output media file for each file found while walking the results. 
