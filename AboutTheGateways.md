# About the Gateways

A **gateway** is a way work gets into or out of TheLoomApp. There are four:

| Gateway | Kind | Talks via | Schedule | Who does the work |
|---|---|---|---|---|
| **LM Studio** | LLM gateway | MCP tools + chat completion | Main Schedule | Local models (GPU PC, DGX Spark) |
| **Claude Code** | LLM gateway | MCP tools (`claude -p` subprocess) | Main Schedule | Claude models on subscription |
| **Comfy** | Generation gateway | ComfyUI HTTP API (`/prompt`, `/history`, `/view`) | Comfy Schedule | ComfyUI workflows (image, audio, …) |
| **Human / App** | Human gateway | TheLoomApp UI | — (reviews and approves both schedules) | You |

The first three are *machine* gateways and are enabled per harness. The fourth is implicit: the app itself is how a person creates todos, marks them Ready, reviews results, and edits the graph.

## Two kinds of machine gateway

**LLM gateways (LM Studio, Claude Code)** run an *operator* against a *desk*. The desk supplies the system prompt, the todo supplies the user prompt, and the model acts on the graph through MCP tools. These two gateways are interchangeable from the schedule's point of view — which one runs a todo is a provider choice (FactorySwitch), not a structural one.

**The Comfy gateway** is different in kind. ComfyUI models don't use MCP and don't take a system/user prompt pair. A Comfy job is a *workflow*: a JSON graph of nodes with inputs (prompt text, seed, lyrics, duration…). The gateway's job is to clone a workflow template, overwrite specific node inputs with values from the todo, submit it, wait, and collect the output file. Because of that difference Comfy has its own todo type and its own schedule. See [AboutComfy](AboutComfy.md).

## Where gateways live in the tree

```
Org
└── HarnessAppModel
    ├── Sessions
    └── Gateways            ← HasLmStudio / HasClaudeCode / HasComfy checkboxes
        ├── LM Studio        (Presence Gateway)
        ├── Claude Code      (Presence Gateway)
        └── Comfy            (Presence Gateway)
            ├── Configured Workflows
            └── Operations
```

### Enabling a gateway

1. Select the harness's **Gateways** folder.
2. Tick **HasLmStudio**, **HasClaudeCode**, and/or **HasComfy**.
3. Click **Save**.

Saving creates the matching child **Presence Gateway** items. A presence gateway is the harness saying "this machine can serve this kind of work."

## Two schedules

| | Main Schedule | Comfy Schedule |
|---|---|---|
| Serves | LM Studio, Claude Code | Comfy |
| Todo type | Desk todo | Comfy (operation) todo |
| Work grouping | WorkGroups under the **Org** | **Operations** folder under the **Comfy gateway** |
| Where in the app | Three root tabs: **Ready Review**, **Schedule**, **Results** | One root tab containing three inner tabs: **Comfy Todo Ready Review**, **Todo Schedule**, **Operation Results** |

The shape is the same on both lines — *review what's ready → run it → inspect results* — it's just that the main line is backed by three tables and gets root-level tabs, while the Comfy line is one table presented through three filtered views in a nested tab.

Both schedules use the same todo lifecycle: a todo is created, marked **Ready**, picked up by the schedule, and carries a **Status** through to its result. Todos on either line can be chained and run in batches.

## Open questions / to document later

- Can one harness serve both LLM gateways at once, and how is the provider picked per todo?
  - A: only one schedule should run at a time. only todo's local to the app harness can run. 
- Multi-machine: what happens when two harnesses both have `HasComfy` (or LM Studio) against the same GPU? (See the "lock per foothold" idea.)
  - A: user would need to go to the machine and play the schedule to run todo's.  We do not yet support remote schecule starts.
- Should the Human gateway become an explicit presence item so its actions are attributed like the others?
  - A: Yes, at the momement per machine, picks up user and creates a reference for them like an account. Assigns the sync stopping desk to the human and saves to settings to use for attribution for the human.
