# WeaversGuild

WeaversGuild as the organization.  Weavers the Agents.  The Apps are TheLoomApp and TheLoomMcp and the knowledge base is the FabricDbContext.   

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

**Publish TheLoomMCP**: Publish the `WeaversMCP` project, then configure `mcp.json` for LM Studio / Claude with `--provider` arg (e.g., `"args": ["--provider", "LmStudio"]`). All providers have a mcp config, change the name to match.

## Desks Todo Prompt Preview    
![previewtodoattempt](https://mmeents.github.io/files/PreviewTodoAttempt.png)    

## Documents folder for documentation.

## Update History
see ChangeLog.md 