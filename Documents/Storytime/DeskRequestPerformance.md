# Desk: Request Performance (55)

*Child doc of StorytimeDescribed. The **third and final requester** — and the entry point to the performance half of the pipeline. Like Request Directors (46) it fans out AND continues; unlike the other requesters it also **produces a node** (the Performance) before fanning out. Read the 15 doc for the shared fan-out mechanism and the 46 doc for fan-out-and-continue ordering; this doc covers what's unique.*

**Chain position:** Request Directors (46) → Beat Director (422) fills call sheets → **Request Performance (55)** ⟹ *fans out per Action entry* → Actor Performance (47), **and continues** → Request Observation (41)

---

## Role in one line

Given one Scene (whose beats now have filled call sheets), **create the scene's Performance** — flattening all its call sheets into one ranked `PerformanceScript` — then schedule **one Actor Performance todo per Action entry**, then continue to Request Observation.

It's a requester (fans out via `scheduleActorPerformances`) **and** a producer (creates the PerformanceModel first). That producer step is what makes it different from the other two requesters, which only dispatch.

---

## Two jobs in one desk: produce, then fan out

Unlike desks 15 and 46 (pure dispatchers), this desk does real graph work before it fans out:

1. **`addPerformance(sceneId, name)`** creates a new **PerformanceModel** whose `Data` column is the scene's call sheets **flattened into one ranked stream** — every narration and role from every beat's call sheet, in playback order, each assigned a `Rank`. This is the single source of truth for the scene's content, and the thing `getPerformanceRollup` reads. (The flattening itself happens inside `addPerformance` / the performance build — the call sheets from all the scene's beats become one ordered `PerformanceScript`.)
2. **`scheduleActorPerformances(performanceId, handlerDeskId: 47, fromTodoId)`** then fans out — one ActorPerformance node + todo **per Action entry** in that flattened stream. Narration entries get no actor.

So the fan-out here is per-**Action-entry**, finer-grained than the per-scene (15) and per-beat (46) fan-outs before it. One scene can yield many actor todos (Scene 435 → 10).

> This is the desk documented in depth from the performance side. See the map's **§3 worked example** and the **getPerformanceRollup** section — Performance 678 and actor nodes 679–688 are this desk's output for Scene 435.

---

## Desk configuration (live values)

| Field | Value | Note |
|---|---|---|
| `DeskRole` | 111 | supplies the tool list |
| `Operator` | 40 | dispatch/producer seat |
| `Enabled` | True | scheduling filter (see map / Scheduler Notes) |
| `OnSuccessTo` | 41 | **Request Observation — continues** (fan-out-and-continue, like 46) |
| `OnFailTo` | 37 | DeskOfFails — infra/exception only |
| `OnPushbackTo` | 9 | TheLoomAppSyncDesk — agent reject / disabled-desk exit |
| `MaxAttempts` | 3 | |
| `FilePath` | `WorkGroups\RequestPerformance.json` | desk export target |

Same fan-out-and-continue shape as desk 46: the actor todos are created before this desk's `completeTodo` creates the continuation to 41, so **FIFO creation order** drains all the actors before Request Observation runs. (See the 46 doc's ordering section — this is the same free topological ordering. Note: this only guarantees the observation todo is *created* after the actors; whether observation should *wait for actor completion* is the fan-in question — see gotchas.)

---

## What it reads

- The **sceneId and todoId** from its UserPrompt (threaded from Request Directors' close reason — carrier pattern).
- **`getSummaryById(sceneId, nodesUp: true, includeProps: true)`** — to see the scene, its beats, and their call sheets (the material to flatten).

No skill card — the craft is the flatten-and-schedule mechanic, inline.

## What it produces

- **A PerformanceModel** (via `addPerformance`) — the flattened ranked script. This is the desk's durable artifact.
- **N Actor Performance todos** (via `scheduleActorPerformances`) — one per Action entry, on desk 47. Per the tool: *"Adds todo for each role in performance… Skips roles that have been requested. Details in results."*
- **`completeTodo`** with a note naming the performanceId and the scheduled actor todos → continues to Request Observation (41).
- On a problem, **`rejectTodo`** with the reason.

## Routing

- **Success → Request Observation (41).** Continuation carries the scene (now with a performance) to the observation stage.
- **Fail → 37 (infra only).**
- **Pushback → 9.**

---

## Worked instance (real runs — three performances, the whole tier's input)

All three scene todos completed, each producing a Performance and fanning out its actor todos:

- **Todo 503 → Scene 435 "Red LIVE"** (`FromTodo: 449`): *"Created 'Red LIVE Performance' (performanceId: 678) and scheduled 10 actor performance todos (689–698) for the handler desk."*
- **Todo 513 → Scene 436 "The Waiver"** (`FromTodo: 486`): *"Created 'The Waiver Performance' (performanceId: 703) and scheduled actor performance todos 712–719 on handler desk 47."* (8 todos — the two-character scene.)
- **Todo 523 → Scene 437 "Unweave Quiet"** (`FromTodo: 493`): *"Created performance 'Unweave Quiet Performance' (ID: 723) and scheduled 6 actor performance todos (730–735) on desk 47."*

Note the fan-out counts differ by scene — 10 / 8 / 6 — because each has a different number of Action entries (narration-heavy scenes spawn fewer actors). These three Performances (678/703/723) are exactly the ones the getPerformanceRollup testing was run against.

---

## Gotchas & notes

- **This desk produces the node the whole performance half reads.** If `getPerformanceRollup` ever returns an empty or malformed stream, suspect the flattening in `addPerformance` here — the rollup only reads what this desk assembled. The Rank ordering that everything downstream depends on is assigned *at this flattening step*.
- **`handlerDeskId: 47` baked in the prompt** (requester→worker: 15→71, 46→422, **55→47**).
- **Continue-to-41 does NOT currently wait for actors — and that's the open fan-in question.** Same FIFO-ordering property as desk 46 makes the observation todo run *after* the actor todos are created. But observation genuinely needs the actors to be *complete* (it renders their enriched output), not merely scheduled. In serial single-worker execution this happens to be fine (actors drain before observation is reached). Under concurrency, or if an actor fails, observation could run against a half-performed scene. **This is the fan-in barrier the map flags as KNOWN-OPEN** — Request Performance's continuation is exactly the edge that needs completion-gating, not just creation-ordering. (For current isolated testing, the actor desk dead-ends at 9 so observation doesn't fire yet anyway.)
- **Per-Action fan-out, not per-character.** The finest granularity in the pipeline. Documented as a known design point on the performance side: one todo per Action entry means many sequential local-model calls per scene; the per-character alternative (one todo carrying all of a character's ranks) is the efficiency escape hatch if the actor tier drags. See the actor desk doc.

---

## Desk Prompt

The desk's live SysPrompt (copy out, renumber ids as needed):
```
This is the {{model.desk}} desk.

Your role is to create a performance for the Scene referenced in your todo, then schedule Actor Performances for that new Performance.

The todo user prompt section references a SceneModel item Id as sceneId and the todo id as todoId.

Use getSummaryById with sceneId, nodesUp=true, includeProps=true.
Use addPerformance with sceneId and a name derived from the scene, e.g. "<scene name> Performance". Note the new performance item's Id as performanceId.

Use scheduleActorPerformances with:
  performanceId = performanceId
  handlerDeskId = 47
  fromTodoId = todoId

The MCP call will add a todo to the handler desk for each role in the performance. Read the results.

If you see errors or a reason to stop, use rejectTodo instead of completeTodo and pass the reason.

Finally, on happy path, call completeTodo with a note summarizing which actor performance todos were scheduled and the performanceId used.

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}
```

---

*Status: first draft, written against live desk 55 + the scheduleActorPerformances contract. The continue-to-41 edge is the fan-in barrier point — see map KNOWN-OPEN. Flattening-in-addPerformance assigns the Rank ordering everything downstream relies on.*
