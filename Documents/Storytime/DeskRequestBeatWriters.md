# Desk: Request Beat Writers (15)

*Child doc of StorytimeDescribed. The **first requester desk** — the canonical instance of the fan-out pattern the map describes. The other two requesters (Request Directors 46, Request Performance 55) work the same way; read this one to understand all three.*

**Chain position:** Scene Writer (68) → **Request Beat Writers (15)** ⟹ *fans out to* → Beat Writers (71)

---

## Role in one line

Given a Story, schedule **one Beat-Writers todo per scene** in that story, then complete. It writes no content of its own — it is a **dispatcher**. The single MCP call `scheduleBeatWriters` does all the fan-out; the desk's job is to make that call and read its result.

---

## The thing to understand about this desk: it dead-ends, but the chain lives on

Look at the routing below and you'll notice something odd: **`OnSuccessTo` points at 9 (the dead-end), not at Beat Writers.** This desk's *own* todo ends and goes nowhere. That is correct and it is the essence of a requester.

The chain does not continue through *this todo's* routing. It continues through the **new todos the schedule call created.** `scheduleBeatWriters` reached sideways into the graph and planted one Not-Started todo per scene directly on the Beat Writers desk (71), each already carrying its scene context. Those todos are the live front of the pipeline now. This desk's continuation is spent — its work was to *spawn*, not to *carry*.

> **Requester vs writer, stated plainly.** A **writer** desk (39, 68) produces a node and its `OnSuccessTo` carries the chain to the next desk. A **requester** desk produces *todos on another desk* and then dead-ends; the chain rides the spawned todos, not the requester's own success pointer. When you see `OnSuccessTo: 9` on a desk, that desk is a requester and the fan-out call is where the real forward motion is.

---

## Desk configuration (live values)

| Field | Value | Note |
|---|---|---|
| `DeskRole` | 109 | supplies the tool list |
| `Operator` | 40 | a lighter operator — this is a dispatch seat, not a creative one |
| `Enabled` | True | scheduling filter (see map / Scheduler Notes) |
| `OnSuccessTo` | 9 | **dead-end** — the chain rides the spawned todos, not this pointer |
| `OnFailTo` | 37 | DeskOfFails — infra/exception only, agent-invisible |
| `OnPushbackTo` | 9 | TheLoomAppSyncDesk — agent reject / disabled-desk exit |
| `MaxAttempts` | 3 | |
| `FilePath` | `WorkGroups\RequestBeatWriters.json` | desk export target |

Note `OnSuccessTo` and `OnPushbackTo` are the *same* desk (9). For a requester the happy-path and the reject both simply stop — because either way, forward motion is carried by the spawned todos (happy path) or not at all (reject).

---

## What it reads

Almost nothing — that's the point. The SysPrompt gives it the **storyId from its todo** (threaded from Scene Writer's close reason; see the carrier pattern in the Scene Writer doc). It reads no skill card and no realm; a dispatcher needs no craft.

## What it produces

- Calls **`scheduleBeatWriters(storyId, fromTodoId: <thisTodo>, handlerDeskId: 71)`**. Per the tool: *"Adds todo for each scene in story to write the beats on the handler desk. Skips scenes that have been requested or if it has beats. Details in results."*
- On the happy path, calls **`completeTodo`** passing the storyId.
- On a problem, calls **`rejectTodo`** with the reason instead (the prompt is explicit about this branch).

The desk creates **no story nodes**. Its only output is the batch of todos on desk 71, plus its own close reason.

## The schedule call's result is the real artifact — read it

You asked me to look at the responses, and they do lay a lot out. The `scheduleBeatWriters` result is not a fire-and-forget ack; it's a **report of what fan-out actually happened**, and the operator is instructed to read it before completing. From the real run (todo 438) the desk summarized its result as:

> *"Successfully scheduled beat writing todos for all scenes in story 430 (Scenes: 435, 436, 437)."*

That enumeration — *which* scenes got todos — is what makes the fan-out auditable. It matters because of the tool's **skip logic**: it *"skips scenes that have been requested or if it has beats."* So the result distinguishes scenes newly scheduled from scenes skipped (already had beats or an in-flight request). That skip-and-report behavior is what makes the whole call **idempotent** — re-running it after a partial failure schedules only the missing scenes rather than duplicating, and the result tells you exactly which those were. The operator's job, per the prompt, is precisely "reading the results making calls": complete if the report is clean, reject if it shows a reason to stop.

## Routing

- **Success → 9 (dead-end).** The requester's work is done the moment the todos are planted. See the section above.
- **Fail → 37 (infra only).** Code exceptions in the schedule call.
- **Pushback → 9.** The agent's reject exit (used when the result shows a reason to stop), and the disabled-desk exit.

---

## Worked instance (real run)

**Todo 438 → Story 430 "Burner Audit"** (Status *Complete Forward*, `TodoDepth: 3`, `FromTodo: 431`):
- Received the carried notes from Scene Writer ("Wrote 3 scenes for Burner Audit… discovery → waiver choice → painless unweave").
- Called `scheduleBeatWriters(storyId: 430, handlerDeskId: 71)`.
- Close reason: *"Successfully scheduled beat writing todos for all scenes in story 430 (Scenes: 435, 436, 437)."*

Three scenes in, three beat-writing todos out onto desk 71 — one fan-out, cleanly reported. That's the pattern working end to end.

---

## Gotchas & notes

- **The `handlerDeskId: 71` is baked into the prompt.** It's the coupling between this requester and its worker desk (Beat Writers). If you renumber desks, this literal in the SysPrompt is a place that silently points at the wrong desk — same class of hazard as any hard-coded id. (The three requesters each hard-code their worker: 15→71, 46→422, 55→47.)
- **`fromTodoId` is what makes the spawned todos chainable.** The tool takes `fromTodoId` so each spawned worker todo records where it came from (`FromTodo`), preserving the carrier thread. Pass 0 only if there's no originating todo — here there always is.
- **Do not "fix" `OnSuccessTo: 9` to point at Beat Writers.** It looks like a bug (the success of a "request beat writers" desk not going to beat writers) and it is not. Pointing it at 71 would send *this* spent todo to the worker desk with no scene context, on top of the correctly-spawned per-scene todos. The dead-end is load-bearing.
- **This is a dispatch seat — operator 40, no skill card.** Don't assign a heavy creative model here; there's no creative work. Judgment is limited to "did the schedule call report cleanly," which is why the prompt's only branch is complete-vs-reject.

---

## Desk Prompt

The desk's live SysPrompt (copy out, renumber ids as needed):
```
This is the {{model.desk}} desk. 

Your role is create a todos on the Beat Writers desk for each scene in the story.

The storyId given in your todo. 

use scheduleBeatWriters with storyID, todoId, and handlerDeskId: 71  - Beat Writers Desk;

the mcp call should do all the work of adding the todo to the handler desk for each scene in the story. 
Your role is reading the results making calls.   

if you see errors or a reason to stop use rejectTodo instead of completeTodo and pass the reason. 

Finally, Happy path, pass the storyId to completeTodo to mark this todo complete.

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}
```

---

*Status: first draft, written against live desk 15 + the `scheduleBeatWriters` tool contract. This is the reference requester doc — 46 and 55 will point back here for the pattern.*
