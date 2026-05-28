# Query Tasks

## Overview

Query Tasks are read-only operations against the WeaversGuild graph. They are safe, idempotent, and carry no side effects. They serve as the primary way an agent orients itself before executing any build or mutation task.

---

## Commands in this Group

| Enum | Command | Description |
|------|---------|-------------|
| 122 | CmdHelp | Returns documentation for all available MCP commands |
| 124 | CmdListProjects | Returns all root-level project nodes |
| 126 | CmdSearch | Searches nodes by name or type within a scope |
| 128 | CmdGetSummaryById | Returns a node summary including props and optional child tree |
| 130 | CmdGetTypeDetails | Returns metadata for a given item type enum |

---

## Common Task Patterns

### Orient — Discover the graph structure
1. CmdListProjects — establish the root nodes available
2. CmdSearch — locate a target node by name or type
3. CmdGetSummaryById — inspect the node and its properties

### Inspect a Type — Understand a node type before building
1. CmdGetTypeDetails — pass the TypeId to understand valid properties and relations

### Pre-flight Check — Validate before a mutation task
1. CmdSearch — confirm a named node does not already exist
2. CmdGetSummaryById — verify parent node Id and current state

---

## Notes

- Query Tasks should always precede [mutation](c:\pathtest\MutationTasks.md) Tasks when an agent is unfamiliar with the current graph state.
- CmdGetSummaryById supports `includeProps` and `nodesUp` flags — use both when a full picture is needed.
- Update commands (UpdateItemName, UpdateItemContent, UpdateItemProperty) are grouped here by tool category but are mutations — treat them as part of their respective build Tasks, not as Query Tasks.
