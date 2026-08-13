# Scriban Template Models

Both Desk and Todo prompts are rendered via Scriban templating. In both cases a model is added and can be used to reference a namespace of data.

The model passed into Scriban has its properties renamed to Scriban naming rules (snake_case). Below are the names available model by type, and can be used to script prompts.

Use `{{` and `}}` to open and close Scriban code. Use `{%{` and `}%}` to escape a block so it renders literally.

## Desk prompt transformation model

When rendering the desk prompt the following model tree is available.

- model - root object
  - desk - string, desk name
  - operator - string, name of the agent wired to the desk
  - role - string, name of the desk role
  - role_commands - list of RoleCommand items

RoleCommand is:
- command - string the mcp command

Desk example:
```
This is the {{model.desk}} desk.

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}
```

## Todo prompt transformation model

Todo nodes have the following objects available.

- model - root object
  - todo - an ItemSummary object
  - target - an ItemSummary object (may be null; see Gotchas)

Where an ItemSummary has the following properties:

- ItemSummary
  - id - integer
  - parent_id - integer
  - rank - integer
  - name - string
  - type_id - integer
  - type_name - string
  - nodes_up - bool, a representation of has children been loaded, is nodes populated.
  - content - string, used when a node has content.  hides when it does not.
  - data - string hidden when null, used only for types CallSheetModel and PerformanceModels.  is a JSON field.
  - nodes - list of child ItemSummary objects, present only when nodes_up is true
  - props - list of PropSummary items

- PropSummary
  - id - property id
  - name - name of the property
  - value - the property value
  - data_type - string
  - reference_type - string
  - editor_type - string

Todo example (print the Tone property of a realm target):
```
TodoId: {{model.todo.id}};
You are to go to {{model.todo.name}} to {{model.target.name}} realmId {{model.target.id}}.
Please use the tone: {{for prop in model.target.props; if prop.name == 'Tone'}}{{ prop.value }}{{ end }}{{ end }}
```

Note the two closing `{{ end }}` tags on the last line. The `for` and the `if` are opened as two statements in a single block, separated by a semicolon, so both must be closed — the first `end` closes the `if`, the second closes the `for`. Delete one and the template will not parse.

## Gotchas

**Whitespace control uses a tilde, not a hyphen.** Scriban writes this as `{{~` and `~}}`, unlike Liquid and Jinja which use `{{-` and `-}}`. Reaching for the hyphen by reflex will produce a parse error.

**Null target.** Not every todo has a target. Before writing `{{model.target.name}}`, guard it:
```
{{if model.target}}targeting {{model.target.name}}{{end}}
```

**Missing content and props.** A node with no content may return an empty string rather than a populated one, and `props` may be an empty list. Loops over `props` handle this safely; direct property access does not.

**nodes.** The `nodes` list is only populated when `nodes_up` is true, and only one level deep — children, not grandchildren. Do not write templates that walk further down the tree.
