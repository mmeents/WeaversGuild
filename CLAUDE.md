# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

**Solution file:** `Weavers.Core/Weavers.Core.sln` (it lives inside the `Weavers.Core` folder and references the sibling projects by `..\` paths).

```powershell
# Build entire solution
dotnet build Weavers.Core/Weavers.Core.sln

# Run tests
dotnet test Weavers.Core/Weavers.Core.sln

# Run a single test by name
dotnet test Weavers.Core/Weavers.Core.sln --filter "FullyQualifiedName~TestTemplates.TestMethod1"
```

`ResearchSpaceTests` is a scratch/research project, not a regression suite: most tests are named `TestMethod1`/`TestMethod2` (so filter by class name too), several have `[TestMethod]` commented out, and some hit live services (RSS feeds, LM Studio, git). A green `dotnet build` is the practical verification for most changes.

**Run TheLoomApp**: WinForms (`net9.0-windows`); set as startup project in Visual Studio and press F5.

**Publish TheLoomMCP**: Publish the `WeaversMCP` project (csproj is `TheLoomMCP.csproj`), then configure the host's `mcp.json` with a `--provider` arg (e.g. `"args": ["--provider", "LmStudio"]`). The provider name becomes the session name used for attribution. The published exe is locked while its host (Claude Desktop, Claude Code, LM Studio) is running — shut the host down before republishing.

**First run**: after the database is created, set the default root to this repo in TheLoomApp's settings tab and use **Import Org Docs** to load `OrgChart/`, `DigitalOperators/`, `OrgDeskRoles/`.

### Database & migrations

SQL Server on `localhost`, integrated security. Migrations run from the Visual Studio Package Manager Console with Default Project set to `Weavers.Core`:

```
Add-Migration <Name> -context FabricDbContext
Update-Database -context FabricDbContext
```

The database name carries the schema version: `FabricCoreV{version}` (currently `FabricCoreV160`). When bumping the version, update all of:
- the connection string in the three `appsettings.json` files (`TheLoomApp/`, `WeaversMCP/`, `Weavers.Api/`)
- `Cx.AppVersion` (`1.2.{dbVersion}`)
- `README.md` and a `ChangeLog.md` entry (note any manual SQL users must run before migrating)

**Lookup tables are seeded from enums via `HasData`**, so changing an enum or its mapping extension changes the EF model and needs a migration. This applies to `ItemType` (from `WeItemType`), `CommandDef` (from `WeCmdType`), `ItemPropertyDefault`, `DataType`, `EditorType`, and `RelationType`.

## Architecture Overview

WeaversGuild models a "virtual decentralized organization": humans and AI agents work todos at desks, and everything they produce lives in one SQL Server graph (`FabricDbContext`). TheLoomApp is the human harness; TheLoomMCP is the agent harness.

### Projects

| Project | Type | Purpose |
|---|---|---|
| `Weavers.Core` | Class library (net9.0) | All data access, MediatR handlers, MCP tools, EF Core entities/migrations |
| `TheLoomApp` | WinForms (net9.0-windows) | Desktop UI — tree view of the graph, property editors, schedules |
| `WeaversMCP` (`TheLoomMCP.csproj`) | WinExe (net9.0-windows) | MCP server hosted service |
| `Weavers.Api` | ASP.NET Core (net9.0) | Minimal-API REST layer (endpoints in `Extensions/`) |
| `ResearchSpaceTests` | MSTest.Sdk (net9.0) | Research/scratch tests |

All runtime projects reference `Weavers.Core`. DI is wired in `Weavers.Core/DependencyInjection.cs`: `AddWeaversCore<TContext>()` registers everything shared; `AddWeaversMCPCore()` adds `WeaversMcpHostedService` on top.

### The Graph Model

Everything is an `Item` (table `dbo.Items`):
- `ItemType` — keyed by the `WeItemType` enum (`Weavers.Core/Enums/WeItemType.cs`). Indentation in that file documents the intended parent/child tree.
- `ItemProperty` — key/value pairs on items. Property names are `Cx.It*` constants (`Weavers.Core/Constants/Cx.cs`); which properties a type gets, with editor and data type, is declared in `ItemPropertyDefaultsExt.cs`.
- `Relation` — directed edge between two items, typed by `RelationType` (`WeRelationTypes`).

`WeItemType` ranges: below 1000 are lookup/status enumerations (todo statuses, SQL/C# types, story status, Comfy override types…); 1000+ are tree nodes.

```
OrganizationModel (1000)
├── HarnessAppModel (1010)            ← one per machine running the loom
│   ├── HarnessSessionsModel (1011)   ← a session item per app/MCP run
│   └── HarnessGatewaysModel (1013)   ← Presence gateways: TheLoomApp, LM Studio, Claude, Comfy
├── CredentialStoreModel (1025)
├── DigitalOperatorPoolModel (1030)   ← DigitalOperatorModel (1035) = AI agents
├── OrgDeskRolesModel (1036)          ← DeskRoleModel (1038): system prompt template + allowed commands
├── WorkGroupModel (1040)
│   └── DeskModel (1045) → TodoModel (1050) → TodoAttemptModel (1055)
├── OrgFolderModel (1060) / OrgFileModel (1065)   ← skills and docs
└── Rss (1070s), GameRoom/Chess (1080s), Pattern (1090s)

ProjectFolderModel (1100)             ← files, git repos, md/html/config
ComfyServiceModel (1150)              ← workflow templates, operations, comfy todos, media files
RealmModel (1160)                     ← Storytime: Story → Scene → Beat/Performance/Observation; MusicAlbum → AlbumTrack; LinkedOn
SolutionModel (1190) / LibraryModel (1200) → Namespace, Class, EntityClass, Handler…  ← code-gen graph
```

#### Adding an item type

A new `WeItemType` value must be handled in each of these switch-based extensions in `Weavers.Core/Extensions/`, or seeding/UI will be wrong:

- `ItemTypeSeedParentTypeExt` (parent type), `ItemTypeSeedDescriptionExt`, `ItemTypeSeedDefaultRankExt`, `ItemTypeSeedDefaultEditorTypeExt`
- `ItemTypeExt` — the `Is*` predicates (folder types, writeable types, valid parents, etc.)
- `ItemTypeDraggableExts` — drag/drop and duplicate rules
- `ItemPropertyDefaultsExt` — default properties, plus `Cx.It*` constants for any new keys

Then add an `Add…CommandHandler` under `Weavers.Core/Handlers/<Area>/`, a tree-view extension in `TheLoomApp/Extensions/Tv*Exts.cs`, menu wiring in `Form1.cs`, and a migration.

### Handlers

Business logic is MediatR request/handler pairs under `Weavers.Core/Handlers/<Area>/`, with the request record and handler in the same file. Requests that should be audited also implement the `IMcpRequest` marker (defined in `Handlers/Pipeline/McpLoggingBehavior.cs`); that pipeline behavior logs only marked requests to `MediatorLog`, with input/output JSON and the current `IAppSessionService` session. Services in `Weavers.Core/Service/` (`AppDataService`, `AppGraph*Service`) are thin facades over the mediator used by TheLoomApp.

### MCP Tools Pattern

Tools in `Weavers.Core/Tools/` come in pairs:

1. **Static tool class** (e.g. `ComfyTools`) — static methods with `[McpTool(Cx.CmdXxx, Cx.CmdXxxDesc)]` and `[Description]` on parameters. Each delegates to `DiBridgeService.GetService<IXxxToolsHandler>()`.
2. **Handler class** (e.g. `ComfyToolsHandler`) — a DI singleton that creates a scope per call, sends MediatR requests, and returns `McpOpResult…ToString()`; exceptions go through `ex.ToOpResult(...)`.

Adding a tool touches:
- `Cx.cs` — `CmdXxx` name and `CmdXxxDesc` constants
- `Weavers.Core/Entities/CommandDef.cs` — a `WeCmdType` value plus its `McpCode()`, `Group()` and `Described()` switch arms (seeded into `CommandDefs`, so a migration is needed). Desk roles pick their allowed commands from this table.
- `DependencyInjection.cs` (handler singleton) and `WeaversMcpHostedService.ExecuteAsync()` (`MCPServer.Register<T>()`) for a new tool class

Commands used to be `WeItemType` values in the 120–210 range; `WeItemType.LoomMcpCommands` remains only as a category, and `WeCmdType154`/`PreviousId()` exist for upgrading old role data.

The MCP library is MCPSharpMM (a patched MCPSharp fork).

### Desks, Todos, and Gateways

Desks hold a queue of todos and reference other desks via `OnSuccessSendTo`, `OnFailSendTo`, `OnPushbackSendTo` properties. Todo status: `TodoNotStarted → TodoInProgress → TodoCompleteForward / TodoAbortedPushBack / TodoFailedForward`. When an agent calls `completeTodo` / `rejectTodo` / `reviewPass` / `reviewFail` the todo is closed and forwarded to the next desk.

Desk system prompts are **Scriban** templates rendered by `RenderTemplateFieldCommandHandler` with a `model` variable (`RoleModel` or `TodoTemplateModel`); see `Documents/Skills/ScribanTemplateModels.md`.

A **gateway** is how work enters or leaves the app (`AboutTheGateways.md`):

| Gateway | Service | Runs |
|---|---|---|
| LM Studio | `LmStudioService` — OpenAI-compatible HTTP; URL/token from item properties, token decrypted by `CryptoService` | desk todos via MCP tools |
| Claude Code | `ClaudeCodeService` — spawns `claude -p` from PATH | desk todos via MCP tools |
| Comfy | `Handlers/Comfy/RunComfyTodoAttemptCommandHandler` — ComfyUI HTTP API | workflow todos, no MCP/system prompt |
| Human | TheLoomApp itself | review, approval, starting schedules |

There are two schedules, both started from TheLoomApp and both limited to todos local to the running harness: the Main Schedule (LLM gateways, `Handlers/Presence/RunTodoAttemptCommandHandler`) and the Comfy Schedule. `GatewayRunRegistry` gates runs per harness. A Comfy todo clones a `ComfyWorkflowTemplate` (API-format JSON in the item's Data field) and writes each param's value at `SectionKey/ObjectKey/PropKey`; output is saved as a `ComfyMediaFileModel` (`AboutComfy.md`).

### Code Generation & Publishing

`Weavers.Core/Handlers/Builds/` walks the graph and writes files; this is what the **Write** button in the app invokes:
- `WriteLibraryCommandHandler` / `WriteSolutionCommandHandler` — `.csproj`, class files, `.sln` from a `LibraryModel` / `SolutionModel` subtree (templates in `Handlers/Templates/`)
- `WriteOrganizationCommandHandler` — exports org JSON to `OrgChart/`, `DigitalOperators/`, `OrgDeskRoles/`; `ImportOrgDocCommandHandler` reads them back
- `WriteDocumentCommandHandler` — writes content-type items (md/html/config/org files) to disk; for a folder-type item it delegates to the realm export below
- `ExportRealmCommandHandler` (request is `ExportItemCommand`) — publishes a folder of Realms (stories, albums, media flagged `ExportOnWrite`) as a static site, using the HTML/CSS/JS emitted by `RealmPublishingExt.cs`

Results are tracked in `Build` / `BuildFile` entities.

### File System Conventions

App data root is `%ProgramData%\WeaversGuild\` (`WeaverExt.CommonAppPath`): `projects/` (generated output), `logs/` (Serilog rolling logs, one set per app), `exports/`, `keys/` (ASP.NET Data Protection keys used by `CryptoService`), `claude/` (`WeaversMCP.exe` and `.mcp.json` for Claude).

### Repo Docs

- `Documents/CommandHelps/*.md` — one file per MCP command, generated by the documentation desk workflow; don't hand-edit.
- `Documents/Desks/`, `Documents/Storytime/` — desk model and the Storytime production chain write-up.
- `OrgChart/*.json`, `DigitalOperators/*.json`, `OrgDeskRoles/*.json` — exported organization state, round-tripped by the Write/Import handlers above.

## Code Style

C# here uses 2-space indentation and K&R braces (opening brace on the same line), block-scoped namespaces, and primary constructors or `_camelCase` readonly fields for injected services.
