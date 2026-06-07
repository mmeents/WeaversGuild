# WeaversGuild

WeaversGuild as the organization.  Weavers the LM Studio Agents, Claude Consultants.  The Apps are TheLoomApp and TheLoomMcp and the knowledge base is the FabricDbContext.   

## Getting Started
- Sql Server developer Edition
- Visual Studio Community Edition
- LM Studio(and the models you want.)
- Clone Repo
- Open in Visual Studio by .sln and build all.
- Open Package Manager Console window in visual studio
  - Add the database, point Default Project at the Weavers.Core 
    - add call to "Update-Database -context FabricDbContext"
    - should have a database called FabricCoreVxxx after.
- Run TheLoomApp by right click on it from Solution Explorer and Set As Startup setting. then press play.
- Publish TheLoomMcp app for the agents.
- Configure the mcp.json for LM Studio and claude's defaults json add entry for theloommcp to point to the published location.
  - use args to name the source with --provider option like the following for LmStudio  "args": ["--provider", "LmStudio"]
  - LmStudio requires Api keys for Api usage, so make sure to enable and add one for the TheLoomApp's Harness settings.
  - LMStudio Settings changes
    - Require Authentication is required.
    - Allow per-request MCPs - can disable 
    - Allow calling servers from mcp.json to enable TheLoomMcp


## Update History
- db Version 132 - not up yet
  - Adds CurrentTodo to desk. 
  - 4 MCP Commands CmdCompleteTodo, CmdRejectTodo,( CmdReviewPass, CmdReviewFail still need to do impls)
  - Adds internal CmdFailTodo and wired into the Run and CmdCompleteTodo and CmdRejectTodo.
  - Adds WeItemType.RanWithoutClose to run statuses for the desk network

- db Version 131 
  - Adds props in todo for FromTodo, CloseReason.  
  - Export now includes Digital Operators and Desks. Went json for the desk and operators.  
    - Still need to update the import side to import them.  

- db Version 130 - cleans up lenth of many long property names. 
  - Adds Dialog to Preview Todo. 

- db Version 129 Mainly Desks Todos items new editors. 
  - Adds Preview Editors for prompts, 
  - Adds Reference Editor that allows for picking the type then pick the items from the existing by previous type pick. 
  - Removes sysPrompt and userPrompt as is duplicate now with preview editors. 