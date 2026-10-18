# DeskModel Item

## Overview

A desk is the primary working unit of the organization. Desks live under a **Work Group** (`WorkGroupModel`) and each one:
- Has a **Desk Role** (`DeskRoleModel`, kept under the Org Desk Roles node). The role holds the list of MCP commands the desk may use, plus optional pre-assert checks.
- Has a **Digital Operator** (a Weaver, `DigitalOperatorModel`, kept under the team / Digital Operator Pool node). The operator's `Presence` property points at a model on a gateway (LM Studio or Claude Code), and that model does the inference for the desk.
- Holds a queue of **todos** as child nodes (`TodoModel`). Each run of a todo is recorded as a child `TodoAttemptModel`.
- Routes finished work to other desks with `OnSuccessSendTo`, `OnFailSendTo` and `OnPushbackSendTo`. Closing a todo creates a new todo on the destination desk.
- Carries a `SystemPromptTemplate` (Scriban) that is rendered into the system prompt for each attempt.

To create one in TheLoomApp, right-click a Work Group and choose **Add Desk**, then fill in the properties in the properties tab. Use **Add Desk Todo** on a desk to queue work by hand.

## Desk properties

| Property | Purpose |
|---|---|
| `DeskRole` | The role (allowed commands, pre-asserts) |
| `Enabled` | Desk on/off flag |
| `Operator` | The Digital Operator that services this desk |
| `SystemPromptTemplate` | Scriban template for the system prompt |
| `OnSuccessSendTo` / `OnFailSendTo` / `OnPushbackSendTo` | Next desks in the chain |
| `MaxAttempts` | Attempts allowed per todo before it is failed forward (default 3) |
| `CurrentTodo` | The todo the desk is working on |
| `FilePath` | Optional file location for the desk's documentation |

The parent Work Group has a `FloorStatus` (Disabled / Operational), a `RelativeFolder` and `Notes`.

## The system prompt and the user prompt

Invoking a Weaver takes a system prompt and a user prompt.
- **System prompt.** This is the operator's own `SystemPrompt` followed by the desk's rendered `SystemPromptTemplate`. It names the tools that are available and explains how to use them.
- **User prompt.** This is rendered from the todo's `UserPromptTemplate`. It tells the Weaver what to do with the todo's reference item.

To check both prompts before running anything, select a todo in the schedule's Ready list. That shows a preview of the rendered prompts without calling the model (`RunTodoAttemptCommand` with `IsPreview`).

## Desk prompt is a Scriban transformation

The desk template is rendered with a `RoleModel`. Names are snake_case in Scriban (for example `{{model.desk}}`).
- model - root object
  - desk - string, the desk name
  - operator - string, the name of the agent wired to the desk
  - role - string, the name of the desk role
  - role_commands - list of RoleCommand items

A RoleCommand has:
- command_type - the `WeCmdType` enum value
- command - string, the MCP command name

The full reference, including the todo model, is [Scriban Template Models](../Skills/ScribanTemplateModels.md).

## Weavers do their work via MCP calls

Weavers use TheLoomMCP tools to read, write and update items in the graph database. The desk role decides which tools are listed in the prompt.

## Running a todo

The Main Schedule in TheLoomApp picks up todos that meet all of these conditions:
- The status is `TodoNotStarted`.
- `Ready` is set.
- The desk's operator has a presence on a gateway that belongs to the running harness (this machine).

For each attempt:
1. The todo is marked `TodoInProgress` and becomes the desk's `CurrentTodo`.
2. The Weaver is invoked and is expected to close the todo with one of the MCP calls: `completeTodo`, `rejectTodo`, `reviewPass` or `reviewFail`.
3. Closing the todo forwards it along the matching `On…SendTo` desk as a new todo.
4. If the Weaver does not close the todo, it stays `InProgress` and the next pass retries it. Once `MaxAttempts` is reached, it is failed forward.

## Resource sharing

A schedule runs one todo at a time per harness, and `GatewayRunRegistry` guards each gateway. When TheLoomApp instances join the **swarm**, `LineLeases` also lets each machine run only one line (Main, Comfy) at a time. That matters because most local GPU inference apps can't run two inferences at once. See the README for swarm setup.

## See also
- [Todo Item](TodoItemModel.md)
- [Todo Attempt](TodoAttemptModel.md)
- [Storytime Described](../Storytime/StorytimeDescribed.md) for a working nine-desk chain
