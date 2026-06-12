# WeaversGuild

WeaversGuild as the organization.  Weavers the LM Studio Agents, Claude Consultants.  The Apps are TheLoomApp and TheLoomMcp and the knowledge base is the FabricDbContext.   

## Company Charter

WeaversGuild is dedicated to creating tools that seamlessly integrate code, documentation, and data to enhance software development. Our mission is to empower developers with innovative solutions that streamline workflows, foster collaboration, and drive productivity. We are committed to building a vibrant community where knowledge sharing and continuous learning are at the core of everything we do.

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

## Desks Todo Prompt Preview    
![previewtodoattempt](https://mmeents.github.io/files/PreviewTodoAttempt.png)    



## Update History
- Db Verion 134 x2 (6/12/2026)
  - Adds ScheduledItem entity, ScheduledItems table. 
    - Handlers to get, add and update status of ScheduledItem 
      - then reverted it all.
  - Adds blue fire icon.
  - Adds Ready Review and Schedule Tabs
    - listbox to show todos that are ready and a tab to host a play button and a queue of the ones review marked ready. 
    - Confirmd playable and automation en mass begins with documentation desk and build out of the CommandHelps org folder and documentation. (6/12/2026)
    
- Db Version 134 
  - Adds Write button for Org Doc, mdFiles, html and json files. dialog open location.
  - Adds Foreach Todo dialog to add sets of todo's, found issue where desk was not being marked as completed when agent marks complete.
  - Initial documentation desk is working, need to test and verify review desk. need to get a automaion play button going soon.
  - Adds Default ticket header to all desk to desk transfers.
  - Adds TodoDepth property to todo to track todo chain depth.
  - Tunes RunTodoAttemptCommandHandler to integrate well with 5 todo handlers. (CompleteTodo, RejectTodo, ReviewFail, ReviewPass, and FailTodo) 
  
- Db Version 133 June 7
  - chops out DeskRoles as the ones chosen are too broad.  focues the existing for use with initial testing.
    - RoleOrgDocWriter
    - RoleReviewOrgDocWriter
    - RoleOrgResearcher
    - RoleReviewOrgResearcher
  - Adds 2 more mcp commands for org setup.
    - CmdAddOrgDesk 
    - CmdAddDeskTodo

- Db Version 132 
  - Adds CurrentTodo to desk. 
  - 4 MCP Commands CmdCompleteTodo, CmdRejectTodo,( CmdReviewPass, CmdReviewFail still need to do impls)
  - Adds internal CmdFailTodo and wired into the Run and CmdCompleteTodo and CmdRejectTodo.
  - Adds WeItemType.RanWithoutClose to run statuses for the desk network
  - Worked out the import of Operators and Desks along with OrgDocs when importing orgainzation.

- Db Version 131 
  - Adds props in todo for FromTodo, CloseReason.  
  - Export now includes Digital Operators and Desks. Went json for the desk and operators.  
    - Still need to update the import side to import them.  

- db Version 130 - cleans up lenth of many long property names. 
  - Adds Dialog to Preview Todo. 

- db Version 129 
  - Mainly Desks Todos items new editors. 
  - Adds Preview Editors for prompts, 
  - Adds Reference Editor that allows for picking the type then pick the items from the existing by previous type pick. 
  - Removes sysPrompt and userPrompt as is duplicate now with preview editors. 
  
- Db Version 121 May 19 
  - Dialog and rework tools to pass all needed info in the add call so can configure first attempt. 
  - Adds Organization to the left.

- Db Version 117 May 13
  - Hooked the Mediatr event chain to log to MediatorLogs table. Build writes everything to disk and calls dot net build. records result. 
  
- Db Version 115 around May 5th  
  - Types set up for the creates for most c# library items completed.  
 
- Db Version 1 around April 4th, 2026
  - Started with Item Relation model from storytime and merged in the Models properties from DaemonsMCP database.
  - worked out initial set of defaults for the types.  
  - Worked out initial TheLoomApp, TheLoomMcp and Weavers.Api and Weavers.Core libraries and apps.  
  