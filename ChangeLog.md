
## Update History
- Db 154 9/2/2026  - Entertainment divisions forming in the guild!!!
  - rework ItemProperty defaults to use hash instead of autoincrement to smothen out future versions and being able to continue a db without starting over.
  - Adds Game Room as a folder and ChessGame objects so we can have them play chess.
  - Adds Chess Tab with the game board and manual play override to the Loom App.
  - Adds addGameRoom addChessGame chessStartGame chessMakeMove and getChessGame which is enough to create and start and play a game.
    - note: one still needs to set up a chess desks with valid team members to play. 
  - Rework Drag Drop and Duplicate methods to make node build out faster.
  - Session creation bug on 2nd machine failed to find human user.  
  - Chess is for fun and exploration purposes. 
    
- Db 153 8/25/2026
  - rework storytime and completeTodo handlers to reduce trips to sql server with new getItemsByIds to grab multiple items at one time. 
  - Adds to Write button for on Realm to write a Index.json and all the children StoryRollup type items.  if on a StoryRollup it makes the Story.json file at location of last folder before the realm.
  - Adds a cleanup desk for use on the sync desk. human is attributed to first todo on that desk. so if on another machine and attempt to empty the sync desk it fails because the setting isn't in settings table for it.  
    - there is only one sync desk per org so they are machine independent. Attribution is gathered from the todoId passing it in on the creates.  When humans do it with app they pass in value of todo created and attributed to them on the sync desk.
  - Adds the new ItemProject static class with static expressions to standardize the transformation.
  
- Db Version 153 8/18/2026 
  - Adds Empty Desk method to clean up the sync desk. added Deleted Todo's for Desks to clean up done work all via delete.
  
- Db Version 153 8/13/2026
  - Adds StoryRollup type off the story items. 
  - addStoryRollup mcp call where agent is to customize the realm on the call.
  - Adds Credits and Observation rollups to serve as base for deployments to built from.
  - Adds Attribution to the default types so credits auto rolled up.
  - On startup, added the app as a gateway and the user as a presence.  
    - then adds theLoomApp user to the default sync desk and adds a todo. 
      - this enables additions done by the app to register in the attribution.  
      - The credits are all new the AddedBy properites.
  - Adds new ResolveAttributionQueryHandler. 
  
- Db Version 149 - 152, 8/13/2026
  - Adds Storytime types and tools to the guild.
  - Adds MoveUp menu command to sort the tree.
  - Adds Storytime Desks to Org library for import
  - Adds Documentation of a working chain to produce stories.
  - [Storytime Described](Documents/Storytime/StorytimeDescribed.md)
  
- Db Version 145, 146, 147, 148 (7/(22,27)/2026)  The Loom can Clone and Checkout Github Repositories.  
  - Adds LibGit2Sharp component to add Repositories to the mix.
  - Adds CredentialStore folder with GithubToken items for username Pat style access models. (utilizing site crypto to save away in cipher, redacted for mcp layer.)
  - Adds GithubRepoModel, GithubRepoBranchModel typed nodes.  Support Clone, RefreshStatus, Checkout to start. 
  - Adds db ver 148 brings TheLoomMCP 4 new commands, CmdAddGithubRepo CmdDoGitClone CmdDoGitRefreshStatus CmdDoGitCheckout
  - Adds GitFolderModel and GitFileModel, Adds Sync to bring models into graph from within RefreshStatus call which is in both clone and checkout.
  
- Db Version 143, 144 (7/19/2026)
  - Adds SetTodoReady and updates CompleteTodo, PushbackTodo, ReviewPass, ReviewFail to return forward todo id in result.
  - Adds GuildNote property to Rss items for agents to keep notes as they process items.
  - Adds Methods AppendGuildNote and UpdateGuildNote to allow for easy writing to the notes. (seperator bug but it's working)
  - Adds Archive and Unarchive Mcp calls. (mcp side limited to RssItem, RssLinkedHtml, DeskTodo.)  
  
- Db Version 142 (7/13/2026)
  - Adds ValueHash property to the ItemProperty table to support an indexed search on values.
  - Adds RssLinkHtml item type to support link discovery and content extraction.  
  - Adds RssResolveLink, RssExtractLinks commands to the MCP tool for Rss operations on items having RssLinkedHtml and RssItems types.
  - RssResyncChannel only emits one layer of items now.
  - Drops the ItKey property. reset to use the ItHasUrl property for either the RssLinkedHtml or RssItems types.
  - Adds export/import of the new types, with folder and file names to support in future.

- Db Version 140 - 141 (7/12/2026)
  - Adds RssFolders, RssChannels, RssItems RssLinkHtml item types.
  - Adds CodeHollow.FeedReader to grab and parse the feeds.  
  - Adds SmartReader for general link content extraction and link discovery.  
  - Adds Commands in App and MCP tools for Rss operations 
    - AddRssFolder, nestable.
    - AddRssChannel, RssResyncChannel on channels 
    - RssResolveLink, RssExtractLinks on linkedHtml items.
  - Lacks export/import of the new types, but added folder and file names to support in future.  
  
- Db Version 139 (6/27/2026)
  - ✅ v139 remodel (Capabilities/Sessions split, single harness, WorkGroup nesting, Team rename, per-model SkipPerms)
  - ✅ Chain linkage reworked for the new harness structure 
  - ✅ Import/export folder-string fix (proven by importing the Documenter desks)
  - ✅ Claude/Code gateway runs a todo (Sonnet, item 137)
  - ✅ LM Studio gateway runs a review todo (Qwen, 138→140 pass)
  - ✅ Multi-desk handoff across two different gateways, correct TodoId lineage
  - ✅ appendItemContent: atomic concat, op-owned separator, CASE guard, TypeName fix, help command
  - ✅ Both gateways verified — the remodel is genuinely done
  - Verified fail celing is intact an stopped on schedule.
  
- Db Version 138 
  - Minor cleanup after 137 (6/25/2026)

- Db Version 137 (6/24/2026)
  - Adds Claude Code Gateway
    - Adds 2 new types a ClaudeGateway and PreClaudeModel types.
    - Adds new HasClaudeCode property to the app harness and updated the on update to pass it's value and create if checked.
    - Model discovery is just hard coded 2 models opus and sonnet atm.
    - Updated the queue query to include the claude types of gateway and presence in the todo queue results.
    - Update the call to switch depending on the gateway.
  - Adds ContextLength default for LmStudio calls to PresenceLmStudoModel set length per model vs Cx constant.
  - Tunes the Help command and method descriptions.
  - Adds AppendItemContent for Md files MCP tool. 
  - Testing found Claude code .mcp.json file first time in startup folder gets written, is pointed at nothing by default.  One needs to add the path to theLoomMcp.exe publish location.
  
- Db Version 136 (6/15/2026)
  - Adds Missing Mcp Cmd for AddOrgDeskRole
  - Removes Role from DeskRole.
  - Adds Import for DeskRoles.  They Read and Write with the Organization like the desk and operators.
  - Multi machine testing. 
  
- Db Version 135 (6/14/2024
  - Makes DeskRole and OrgDeskRole as first class graph items
  - Adds CmdPickerEditor to show a list and to select a set of them.
  ![OrgDeskRole](https://mmeents.github.io/files/OrgDeskRoleEditor.png)
  
- Db Version 134 x3 (6/13/2026)
  - Adds Results Tab like Ready and Schedule 
    - shows completed todo's with archive and delete operations. 
    - adds delete operation to Review Ready tab. 
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
  