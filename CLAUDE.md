# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

**Solution file:** `Weavers.Core/Weavers.Core.sln` — open this in Visual Studio or use from CLI.

```powershell
# Build entire solution
dotnet build Weavers.Core/Weavers.Core.sln

# Run tests
dotnet test Weavers.Core/Weavers.Core.sln

# Run a single test by name
dotnet test Weavers.Core/Weavers.Core.sln --filter "FullyQualifiedName~TestMethodName"
```

**Database migrations** (run in Visual Studio Package Manager Console, with Default Project set to `Weavers.Core`):
```
Update-Database -context FabricDbContext
```
The database is named `FabricCoreV{version}` (currently `FabricCoreV136`). When adding a migration, increment the DB version in the README and connection strings.

**Run TheLoomApp**: Set as startup project in Visual Studio, press F5. It is a WinForms app (`net9.0-windows`).

**Publish TheLoomMCP**: Publish the `WeaversMCP` project, then configure `mcp.json` for LM Studio / Claude with `--provider` arg (e.g., `"args": ["--provider", "LmStudio"]`).

## Architecture Overview

### Projects

| Project | Type | Purpose |
|---|---|---|
| `Weavers.Core` | Class library (net9.0) | All data access, business logic, MCP tools, EF Core models |
| `TheLoomApp` | WinForms (net9.0-windows) | Desktop UI — tree view of the graph, editors, org/desk management |
| `WeaversMCP` | Console (net9.0) | MCP server hosted service; registers MCP tools for LM Studio / Claude |
| `Weavers.Api` | ASP.NET Core (net9.0) | REST API layer (port 44344) |
| `ResearchSpaceTests` | MSTest (net9.0) | Test project using MSTest.Sdk |

All runtime projects reference `Weavers.Core`. DI is wired via `DependencyInjection.cs` extension methods: `AddWeaversCore<TContext>()` for shared services and `AddWeaversMCPCore()` adds the MCP hosted service on top.

### The Graph Model

Everything in the system is an `Item` (table `dbo.Items`) with typed relations:
- `ItemType` — the type of an item, keyed by the `WeItemType` enum (values in `Weavers.Core/Enums/WeItemType.cs`)
- `ItemProperty` — key/value pairs on items; property name constants live in `Cx.It*` (`Weavers.Core/Constants/Cx.cs`)
- `Relation` — directed edge between two items, typed by `RelationType`

The `WeItemType` enum encodes the full ontology (1000+ range = organizational/project tree nodes). The main tree starts at `OrganizationModel (1000)` and fans out to harness, desks, todos, project code graph items, etc.

### Organization & Desk Workflow

```
OrganizationModel (1000)
├── HarnessAppModel (1010)      ← represents the running TheLoomApp machine
├── OrgDeskRolesModel (1026)    ← DeskRoleModel entries with system prompt templates
├── DigitalOperatorPoolModel (1030) ← DigitalOperatorModel = AI agents (e.g., Marvin.json)
└── OrgChartModel (1040)
    └── DeskModel (1045)        ← work desk
        └── TodoModel (1050)    ← items in the desk queue
            └── TodoAttemptModel (1055)
```

Desks have `OnSuccessSendTo`, `OnFailSendTo`, and `OnPushbackSendTo` item-property references to other desks. When an agent completes a todo via MCP, the todo is closed and forwarded. Todo status flows: `TodoNotStarted → TodoInProgress → TodoCompleteForward / TodoAbortedPushBack / TodoFailedForward`.

Desk system prompts use **Scriban** templates rendered by `RenderTemplateFieldCommandHandler` — templates receive a `model` variable (`RoleModel` or `TodoTemplateModel`).

### MCP Tools Pattern

Tools in `Weavers.Core/Tools/` follow a strict two-class pattern:

1. **Static tool class** (e.g., `SummaryTools`, `AppGraphOrgTools`) — static methods decorated with `[McpTool(Cx.CmdXxx, "description")]`. Each method calls `DiBridgeService.GetService<IXxxHandler>()` and delegates.
2. **Handler class** (e.g., `SummaryToolsHandler`) — implements `IXxxHandler`, injected via DI, contains the actual logic using `IMediator` / `FabricDbContext`.

Registered in `WeaversMcpHostedService.ExecuteAsync()` via `MCPServer.Register<T>()` (MCPSharpMM library). Tool command name strings are constants in `Cx` and mirrored as `WeItemType` enum values in the `LoomMcpCommands` range (120+).

### Code Generation

`Weavers.Core/Handlers/Builds/` contains handlers that walk the item graph and write C# source to disk:
- `WriteLibraryCommandHandler` — generates `.csproj` + class files for a `LibraryModel` subtree
- `WriteSolutionCommandHandler` — generates `.sln` for a `SolutionModel`
- `WriteOrganizationCommandHandler` — exports organization JSON files to `OrgChart/`, `DigitalOperators/`, `OrgDeskRoles/`

Results are tracked in `Build` / `BuildFile` entities.

### Services

Key services in `Weavers.Core/Service/`:
- `AppDataService` — primary facade over MediatR; TheLoomApp's main service dependency
- `AppGraphOrgService`, `AppGraphFileService`, `AppGraphClassService` — domain-specific graph operations
- `LmStudioService` — HTTP client for LM Studio's OpenAI-compatible API; reads URL/token from item properties, decrypts token via `CryptoService`
- `SessionItemCacheService` — in-memory item cache (singleton)

### File System Conventions

App data root is `%ProgramData%\WeaversGuild\` (via `WeaverExt.CommonAppPath`):
- `projects/` — generated project output
- `logs/` — Serilog rolling logs
- `exports/` — org export files
- `keys/` — ASP.NET Data Protection keys
- `claude/` — contains `WeaversMCP.exe` and `.mcp.json` for Claude MCP config

### Documentation Files

`Documents/CommandHelps/*.md` — one Markdown file per MCP command, auto-generated by the documentation desk workflow.

`OrgChart/*.json`, `DigitalOperators/*.json`, `OrgDeskRoles/*.json` — exported organization state (desks, operators, roles). These are written by `WriteOrganizationCommandHandler` and imported back via `ImportOrgDoc`.
