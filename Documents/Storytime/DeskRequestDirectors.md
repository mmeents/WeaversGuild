# Desk: Request Directors (46)

*Child doc of StorytimeDescribed. The **second requester** — and the one that both **fans out and continues**, unlike Request Beat Writers (15) which dead-ends. Read the 15 doc first for the shared fan-out mechanism; this doc covers what's different: the continuation, and the runtime ordering that makes it work.*

**Chain position:** Beat Writers (71) → **Request Directors (46)** ⟹ *fans out per beat* → Beat Director (422), **and continues** → Request Performance (55)

---

## Role in one line

Given one Scene (with beats), schedule **one Beat-Director todo per beat**, then **continue to Request Performance**. Like all requesters it's a dispatcher — the `scheduleBeatDirectors` call does the fan-out — but unlike 15 it does *not* dead-end. Its own continuation carries the scene forward to the performance stage.

---

## Fan out AND continue — and why the order is automatic

This desk does two things that look like they'd conflict: it plants a batch of Beat-Director todos (fan-out) *and* its own todo continues to Request Performance (55). How do all the beat directors run *before* the performance stage, without any explicit dependency tracking?

**The answer is purely runtime ordering, and it's the elegant part.** At execution time, `scheduleBeatDirectors` creates the fan-out todos **first**, and only *then* does this desk's `completeTodo` create the continuation todo. Because the scheduler runs todos in creation order (a FIFO over ready/not-started todos), the beat-director todos are already sitting ahead of the continuation in the queue. So:

> **"Schedule children, then complete" gives topological ordering for free.** The children exist before the parent's continuation does, so the scheduler naturally drains all the beat directors before it reaches the Request Performance todo. No dependency graph, no wait condition, no barrier — just the order the todos were born in.

This is why a requester can safely continue: its continuation is *guaranteed to be younger* than the work it spawned, so it runs after. (Contrast the **fan-in** case downstream — actor performances into observation — where the continuation must wait on *completion*, not just creation order, which is why that one needs a real barrier. Fan-out ordering is free; fan-in ordering is not.)

---

## Desk configuration (live values)

| Field | Value | Note |
|---|---|---|
| `DeskRole` | 110 | supplies the tool list |
| `Operator` | 40 | dispatch seat (same lighter operator as 15) |
| `Enabled` | True | scheduling filter (see map / Scheduler Notes) |
| `OnSuccessTo` | 55 | **Request Performance — this requester continues** |
| `OnFailTo` | 9 | ⚠️ **not 37.** Routes fail to the sync/stop desk, not DeskOfFails (see gotchas) |
| `OnPushbackTo` | 9 | TheLoomAppSyncDesk — agent reject / disabled-desk exit |
| `MaxAttempts` | 3 | |
| `FilePath` | `WorkGroups\RequestDirectors.json` | desk export target |

The key contrast with desk 15: 15 has `OnSuccessTo: 9` (dead-end), this has `OnSuccessTo: 55` (continue). Same fan-out call shape, opposite continuation behavior — because after directing, the scene needs to advance to performance, and this requester carries it there.

---

## What it reads

Like all requesters, almost nothing — the **sceneId from its todo** (threaded from Beat Writers' close reason; carrier pattern). No skill card, no realm. A dispatcher needs no craft.

## What it produces

- Calls **`scheduleBeatDirectors(sceneId, fromTodoId: <thisTodo>, handlerDeskId: 422)`**. Per the tool: *"Adds todo for each beat in scene to direct the beat on the handler desk. Skips beats that have been requested or if it has a call sheet. Details in results."*
- **Side effect worth noting:** the prompt states the call *"will also ensure a new CallSheetItem is added to each beat."* So this fan-out doesn't just create todos — it also creates the empty **CallSheetModel** node each Beat Director will fill. The call sheet exists before the director runs.
- On the happy path, **`completeTodo`** passing the sceneId → continues to Request Performance (55).
- On a problem, **`rejectTodo`** with the reason.

## Routing

- **Success → Request Performance (55).** The continuation carries the scene onward — see the ordering section for why this is safe alongside the fan-out.
- **Fail → 9** (not 37 — see gotchas).
- **Pushback → 9.** Agent reject / disabled-desk exit.

---

## Worked instance (real runs — three scenes, per-beat fan-out each)

All three scene todos completed here, each fanning out one director todo per beat (4 beats each → 4 todos each) onto desk 422:

- **Todo 449 → Scene 435 "Red LIVE"** (`FromTodo: 440`): *"Scheduled Beat Directors for all 4 beats in scene Red LIVE. Added todos: 496, 498, 500, 502."*
- **Todo 486 → Scene 436 "The Waiver"** (`FromTodo: 441`): *"Added todo ids: 506, 508, 510, 512."*
- **Todo 493 → Scene 437 "Unweave Quiet"** (`FromTodo: 442`): *"Added todos 516, 518, 520, 522 to desk 422."*

Note the enumerated todo ids in every close reason — same auditable fan-out report as desk 15. Three scenes × 4 beats = 12 Beat-Director todos spawned, and each of the three scene todos then continued to Request Performance. This is the fan-out-and-continue pattern fully realized: **12 spawned + 3 continuations**, all correctly ordered because the 12 were created before their 3 continuations.

---

## Gotchas & notes

- **`OnFailTo` is 9, not 37 — an inconsistency worth flagging.** Every other desk documented so far routes infra-failure to 37 (DeskOfFails); this one routes fail to 9 (the sync/stop desk), same as pushback. That means an infra exception here is treated like an agent reject rather than landing in the fails desk. **Verify whether this is intentional** — if DeskOfFails is where you look for crashes, this desk's crashes won't be there. Could be deliberate (requesters are low-risk dispatch) or could be a copy-paste miss when the desk was built.
- **`handlerDeskId: 422` is baked into the prompt** — the coupling to Beat Director. (Requester→worker hard-codes: 15→71, **46→422**, 55→47.)
- **The call creates CallSheet nodes as a side effect**, not just todos. So a partial-failure re-run has two things to be idempotent about: skip beats already requested *and* beats that already have a call sheet — which is exactly what the tool's skip logic checks (*"skips beats that have been requested or if it has a call sheet"*). The call sheet's existence is the idempotency marker.
- **Don't be misled by "continue" into thinking it waits.** The continuation to 55 does *not* wait for the beat directors to finish — it just gets created after them and therefore runs after them in queue order. If beat directors were slow or failing, Request Performance would still fire once the queue reaches it. Correct topological order here rests on FIFO creation order, not on completion-gating. (That distinction is the whole reason fan-**in** needs a barrier and fan-**out** does not.)

---

## Desk Prompt

The desk's live SysPrompt (copy out, renumber ids as needed):
```
This is the {{model.desk}} desk. 

Your role is to schedule the Beat directors, one per beat.
The sceneId given in your todo. 

use scheduleBeatDirectors with sceneID, todoId, and handlerDeskId: 422 - Beat Director Desk;

the mcp call should do all the work of adding the todo to the handler desk for each beat in the scene.  It will also ensure a new CallSheetItem is added to each beat.
 
Your role is reading the results making calls.   

if you see errors or a reason to stop use rejectTodo instead of completeTodo and pass the reason. 

Finally, Happy path, completeTodo to mark this todo complete, pass the sceneId as reference item.

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}
```

---

*Status: first draft, written against live desk 46 + the `scheduleBeatDirectors` tool contract. Flag: confirm whether `OnFailTo: 9` (vs 37 elsewhere) is intentional.*
