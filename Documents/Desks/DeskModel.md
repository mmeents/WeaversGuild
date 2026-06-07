# DeskModel Item

## Overview

Thinking a desk is the primary part of the organization. They consist of the following:
- Desk has a Role.  The Guild has set up a set of Roles desks can have. It's a primary component of the setup of a new desk. Was thinking the roles would add a new DeskRole lookup type with items being like the following: 
  -  WeItemType.Researching
  -  WeItemType.ReviewResearch  
  -  WeItemType.Designing
  -  WeItemType.ReviewDesign  
  -  WeItemType.Planning
  -  WeItemType.ReviewPlan  
  -  WeItemType.BuildingOut
  -  WeItemType.ReviewBuildOut  
  -  WeItemType.Testing
  -  WeItemType.ReviewTests  
  -  WeItemType.Documenting
  -  WeItemType.ReviewDocument  
  -  WeItemType.Packaging
  -  WeItemType.ReviewPackage
- Has a on call Digital Operator(Weaver) as a property of the desk.  this is the LLM used to service infrence for the desk. 
- Maintain a todo list. Maintain a catagory of documentation centered around the desks purpose. 
- OrgChartModel the Desks Parent Drives the infrence progress. 
- Maintain Success and Failed continuation operations.  (Create a new todo on the destinations todo list. Catch log Errors)
- Maintains SystemPromptTemplate and SystemPrompt fields. 

Thinking to bring the desk into being is a right click property on a OrgChart node, AddOrgDesk context menu, dialog pops up.
The dialog or mcp interface would asking for Name, Role, SuccessTo and FailTo desks pickers and it create the desk in the graph or tree.(not enabled)  

## Desk is Responsible for System Prompt.

Invoking a weaver requires a System Prompt and a User Prompt.  
- The System Prompt is where we name what tools are available, how to use them. This comes from SystemPromptTemplate or SystemPrompt, if template is null or not. 
- The User Prompt is where direct them on what todo from the next todo item on the pile.

Propose a next prompt preview method that would run the templates and report back what the two prompts would be given the next todo and desk, to verify.

## Desk carries a nullable string RequestTemplate

When not null it uses scriban template to generate the SystemPrompt from it. 
- The model it runs with is set from the Role.
if The RequestTemplate is null then will look to static RequestText to fill in the System Prompt. 

## The TodoItem Pile

Desk nodes have child nodes(TodoItemModel Type), which together are a rank orderd list of todo items.

## Weavers do their work via MCP Calls.

MCP calls to read, write or update details within the graph database.

## Desks take next todo and uses it to create the userPrompt.

When weaver is invoked, their todo item has been marked InProgress and they are requested to end their session to mark either complete or push it back.  either will clear the todo and allow for the next to happen else it should keep getting the current inprogess until it completes.  (maybe have a max attemp counter.) 

## Resource Sharing 

When Active and Todo count is greater than 0 then it's ready, OrgChartModel will invode the desk.   