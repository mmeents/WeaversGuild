# Mutation Tasks  🔨

## Overview

Mutation Tasks are write operations against the WeaversGuild graph. Unlike Query Tasks they are stateful, ordered, and have side effects — creating nodes, updating properties, and triggering cascading path and namespace updates. Always run a Query Task pre-flight before mutating to confirm parent node Ids and current graph state.

---

## Commands in this Group

### Update Commands
| Enum | Command | Description |
|------|---------|-------------|
| 132 | CmdUpdateItemName | Renames a node. Triggers recursive path and namespace cascade on applicable types |
| 134 | CmdUpdateItemContent | Writes content to OrgDoc or method code nodes |
| 136 | CmdUpdateItemProperty | Updates a single property value by ItemPropertyId |

### File and Folder Commands
| Enum | Command | Description |
|------|---------|-------------|
| 150 | CmdAddProjectRoot | Creates a root project folder node |
| 152 | CmdAddSubFolder | Adds a subfolder under a parent folder node |
| 154 | CmdAddSolution | Adds a solution node under a project root |
| 156 | CmdAddSolutionImport | Adds a solution import reference |
| 158 | CmdAddMdFile | Adds a markdown file node |
| 160 | CmdAddHtmlFile | Adds an HTML file node |
| 162 | CmdAddConfigFile | Adds a JSON config file node |

### Library and Namespace Commands
| Enum | Command | Description |
|------|---------|-------------|
| 164 | CmdAddLibrary | Adds a library node under a folder |
| 166 | CmdAddNamespace | Adds a namespace node under a library |

### Class Commands
| Enum | Command | Description |
|------|---------|-------------|
| 168 | CmdAddClass | Adds a class node |
| 170 | CmdAddClassImport | Adds a using/import to a class |
| 172 | CmdAddClassProperty | Adds a property to a class |
| 174 | CmdAddClassMethod | Adds a method to a class |
| 176 | CmdAddClassMethodParam | Adds a parameter to a method |

### Entity Commands
| Enum | Command | Description |
|------|---------|-------------|
| 178 | CmdAddEntityClass | Adds an entity class node. Also creates companion EntityConfigurationModel |
| 180 | CmdAddEntityClassImport | Adds an import to an entity class |
| 182 | CmdAddEntityProperty | Adds a property to an entity class. Triggers navigation and DbContext sync |

---

## Common Task Patterns

### RenameNode — Safe rename with cascade awareness
1. CmdGetSummaryById — confirm current name and node type
2. CmdUpdateItemName — rename triggers path/namespace cascade automatically

### WriteOrgDoc — Document a node in the org
1. CmdGetSummaryById — confirm node is an OrgDocModel and get current content
2. CmdUpdateItemContent — write markdown content to the node

### ScaffoldSolution — Stand up a new project structure
1. CmdAddProjectRoot
2. CmdAddSolution
3. CmdAddSubFolder (repeat as needed)

### ScaffoldLibrary — Add a library with namespace
1. CmdAddLibrary — parent is the target subfolder node
2. CmdAddNamespace — parent is the new library node

### ScaffoldClass — Build a class with members
1. CmdAddClass — parent is the target namespace node
2. CmdAddClassImport (repeat as needed)
3. CmdAddClassProperty (repeat as needed)
4. CmdAddClassMethod (repeat as needed)
   - CmdAddClassMethodParam (repeat per method)

### ScaffoldEntity — Build an EF Core entity
1. CmdAddEntityClass — also auto-creates EntityConfigurationModel companion
2. CmdAddEntityClassImport (repeat as needed)
3. CmdAddEntityProperty (repeat as needed) — triggers nav and DbContext sync automatically

---

## Notes

- **CmdUpdateItemName** is the most consequential mutation — a rename on a folder cascades path updates to all descendants, and a namespace rename cascades both path and namespace properties. Always confirm the target node type before renaming.
- **CmdUpdateItemProperty** requires the `ItemPropertyId` (the Id of the property row itself), not the ItemId. Use CmdGetSummaryById with `includeProps: true` to retrieve property Ids first.
- **Entity commands** carry more side effects than class commands — EntityClass creation auto-spawns a companion config model, and EntityProperty updates trigger navigation and DbContext import sync. Expect more graph changes per call.
- Update commands are grouped under the Summary tool category in the MCP server but are mutations — treat them as part of their respective build Tasks accordingly.
