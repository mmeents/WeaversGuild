# Storytime, Described

*A guide to the WeaversGuild Storytime Merger — a story-generation pipeline: what the nodes are, how work is scheduled across the desks, and how a finished scene is assembled. Written for a new operator loading the desk set for the first time — and for future-me, who will look at a line of this code and think "what was I thinking."*

*This document is the **map**. Each desk has its own child doc in this folder (see the index in §5) that goes deep on its prompt, tools, and gotchas. Read this first for the shape; go to a child doc when you need a specific desk.*

---

## 0. The one-sentence model

The pipeline turns a story premise into finished prose by sending **todos** around a **graph of nodes**, through a chain of **desks**. Each desk is a station: it takes a todo, does one model-inference job (all via MCP calls), and routes the result onward. Nothing runs on a script — everything runs because a todo landed on an enabled desk and the desk picked it up. Coordination happens entirely through the shared graph + SQL store; **there is no separate scheduler.**

> **Why it's shaped this way.** An earlier attempt (the "Prompter" app) tried to template advanced prompts as trees, with the model namespace as nested dictionaries. It didn't hold: the structure lived *inside the prompt*, so there was nothing to schedule, resume, inspect, or parallelize. The lesson that justifies this entire architecture — **put the structure in a graph you can schedule against, not in the prompt.** Once structure is graph nodes, everything else (fan-out, fan-in, retries, multi-machine workers) becomes ordinary graph + queue mechanics instead of prompt gymnastics.

> **How todos actually run (app machinery — summary).** theLoomApp's **Schedule** tab is the run loop: an ordered list of ready todos, top one runs next, in sequence on a **harness**. A todo reaches the schedule only if its desk is `Enabled`, its status is **Not Started**, and its `isReady` flag agrees (and its operator is reachable on a live harness). This is a *different layer* from the pipeline architecture below — see the **Scheduler Notes** child doc for detail. It's deliberately kept out of the architecture docs, and parts of it are still verify-against-code.

---

## 1. The one pattern to notice: requester → worker

Almost every stage is the same two-beat rhythm, repeated:

> **A requester desk fans out one worker todo per child item. The workers each do one bounded job and dead-end (no continuation). The requester's own continuation carries the chain forward to the next stage.**

You will see this three times in the chain below — it is not three mechanisms, it is one mechanism three times:

| Requester desk | fans out → | Worker desk (dead-ends) |
|---|---|---|
| Request Beat Writers (15) | per scene | Beat Writers (71) |
| Request Directors (46) | per beat | Beat Director (422) |
| Request Performance (55) | per Action entry | Actor Performance (47) |

Learn this pattern once and you understand the spine of the whole system. The rest is which node each stage reads and produces.

> **Why workers dead-end instead of chaining.** Fan-out work is independent by construction — no worker depends on another's output, so completion order doesn't matter. The requester's continuation (or a fan-in barrier) re-assembles the results afterward. Keeping workers as leaves is what makes the tier safe to run concurrently. (See design notes, §4.)

### Routing: the three pointers are the three ways a todo can end

A todo has exactly three terminal states, and each maps to one routing pointer on the desk:

| Terminal state | Pointer | Meaning |
|---|---|---|
| Complete Forward | `OnSuccessTo` | the desk did its job; advance the chain |
| Fail Forward | `OnFailTo` | **infra failure** (code exception). Routes to the fails desk (37, "DeskOfFails"). **Agents are unaware of this path** — it's not a choice they make. |
| Aborted Push Back | `OnPushbackTo` | the **agent's own reject/pushback** exit (to 9, "TheLoomAppSyncDesk"). Also the standard exit when a desk is disabled. |

That's the whole routing model: three outcomes, three pointers, plus `MaxAttempts` bounding retries. Keep the infra-vs-agent distinction in mind when reading any desk — `OnFailTo` is the machine tripping, `OnPushbackTo` is the operator declining.

---

## 2. The full chain (Story 430, [Burner Audit](BurnerAudit.md) Realm: Threadspace)

Top to bottom, authored order — universe down to finished prose:

1. **Story Idea Maker (39)** — turns a story idea (430) against its Realm into a story. Continues to Scene Writer.
2. **Scene Writer (68)** — writes the story's Scenes (435–437). Continues to Request Beat Writers.
3. **Request Beat Writers (15)** — *requester*: fans out a todo per Scene onto Beat Writers; those workers dead-end.
4. **Beat Writers (71)** — writes the Beats (445–448) for a scene, then continues to Request Directors.
5. **Request Directors (46)** — *requester*: calls `ScheduleBeatDirectors`, fanning out todos onto Beat Director; continues to Request Performance on completion.
6. **Beat Director (422)** — creates a CallSheet for its Beat and directs it via MCP calls; dead-ends.
7. **Request Performance (55)** — *requester*: adds a Performance and calls `ScheduleActorPerformances`, fanning out todos onto Actor Performance; continues to Request Observation.
8. **Actor Performance (47)** — actors act their parts, enriching the performance; dead-ends.
9. **Request Observation (41)** — produces prose from the performance interpretation.

---

## 3. Worked example: one moment through the performance tier

*The fastest way to feel the fan-out/re-assemble spine is to watch a single moment travel it. Real data from Scene 435, "Red LIVE."*

1. **Request Performance (55)** picks up its todo for Scene 435. Calls `addPerformance(sceneId)` → creates **Performance 678**, whose `Data` column holds the combined script: the scene's call sheets flattened into one ordered stream of entries (Narration + Action), each with a `Rank`.
2. It calls `scheduleActorPerformances(performanceId: 678, handlerDeskId: 47, fromTodoId: …)`. This fans out **one `ActorPerformanceModel` per Action entry** — nodes 679–688 — and **one todo per node** — 689–698 — onto the Actor desk. Narration entries get no actor (nobody performs the room).
3. **Actor desk (47)**, running Gemma, picks up todo 689 → ActorPerformance **679** (Rank 1, character Kade-7). Reads the whole script for context via `getPerformanceRollup(678)`, reads its own instruction via `getSummaryById(679)`, and **enriches** the director's instruction into performed text — writing entries into 679's own `Data` via `addPerformanceAction` / `addPerformanceLine`.
4. Each actor todo is a **leaf** — completes with no continuation; the chain parks. (Currently `OnSuccessTo` → dead-end stop desk 9 for isolated testing; eventually → a fan-in barrier.)
5. **`getPerformanceRollup(678)`** called again (by the Observer, later): Rank 1 now returns `Source: "Actor"` — the rollup spliced 679's enriched entries in place of the director's original, keyed by rank. Un-acted Action ranks still read `Source: "Director"`. Once all actors run, the whole stream is actor-performed and ready for observation.

**Fan out to independent leaves, then re-assemble by rank** — that's the spine.

---

## 4. Node types & cross-cutting design decisions

### Structure nodes (authored top-down)
- **RealmModel** — the universe. Carries a `Tone` property (binding style register) and a Content bible (used at writing time, not observation time).
- **StoryModel** — one story in a realm. Card in Description (premise / protagonist / engine / ending question). Props: `TargetSceneCount`, `PovDefault`.
- **SceneModel** — one scene. Props: `EntryState`, `ExitState`, `Pov`. Character relations hang here.
- **BeatModel** — a unit within a scene the director interprets.

### Output nodes (produced by desks)
- **CallSheetModel** — a director's interpretation of one **beat**.
- **PerformanceModel** — the scene's call sheets flattened into one ranked script in its `Data` column (`PerformanceScript`). The single source of truth for a scene's content.
- **ActorPerformanceModel** — one per Action entry. Props: `Character`, `Rank` (join key back to the performance entry), `Instructions` (director's text). The actor writes enriched output into this node's own `Data`.
- **(planned) SceneObservationModel** — the observer's final rendering of a performance.
- **(planned) StoryObservationModel** — all scene observations assembled into the finished article.

> **`PerformanceEntry.Type` is `Narration | Action | Line`.** The director only ever emits Narration and Action. **Line appears only after acting**, set by which tool the actor called (`addPerformanceLine` → Line). There is intentionally no separate "mode" field; the tool identity *is* the type.

> **DESIGN DECISIONS & KNOWN-OPEN** *(the stuff that reads as mysterious later — captured here in the map because it cuts across desks)*
>
> - **Match on `Rank`, not on reconstructed node name.** A name-based match (`"Perf:{id} Line:{rank} Character {name}"`) was tried and silently failed — real node names didn't match that format, so *every* entry fell through to Director and the actor pass looked like it did nothing. Any name with punctuation or a stray space breaks it again. Rank is the stable key. **Do not reintroduce name matching.**
> - **`ActorPerformanceModel` has no Mode field — on purpose.** Line-vs-action is the actor's discretion; that choice *is* the tier's reason to exist. A Mode field would pre-make the decision and hollow out the actor. Deliberate, not an oversight.
> - **The append tools reload `Data` by id** before adding an entry, instead of trusting the passed-in DTO. Looks redundant; isn't. Once one todo can produce multiple entries (a multi-action-then-line moment), a stale DTO causes a lost update — the second call overwrites the first, silently swallowing (e.g.) a punchline. Reload-by-id closes it.
> - **KNOWN-OPEN: the director over-writes.** Director instructions currently arrive as finished prose with interior state, not terse intent, leaving the actor "enriching" text that's already written — inviting parroting or drift. Intended fix: a **validator-grep upstream** (cap length, strip interior-state prose) so the call sheet holds instruction, not performance. **Not yet done.**
> - **KNOWN-OPEN: concurrency & fan-in atomicity.** Serial single-worker execution hides two races that surface the moment two harnesses run: (1) todo **claiming** must be a conditional update, not read-then-write, or two boxes perform the same todo; (2) the fan-in barrier tally must be atomic, or observation fires twice / never. The self-pointing wait desk is a spin-lock and needs backoff + a lease/timeout so a dead worker doesn't wedge it. Prove both by running two harnesses against one SQL instance *on one box* before distributing.

---

## 5. Desk reference (child docs in this folder)

Each desk gets its own doc: SysPrompt, DeskRole/tools, what it reads, what it produces, routing, gotchas. Filled one at a time.

| Desk | Id | Child doc |
|---|---|---|
| Story Idea Maker | 39 | [Story Maker](DeskStoryIdeaMaker.md)  |
| Scene Writer | 68 | [Scene Writer](DeskSceneWriter.md) |
| Request Beat Writers | 15 | [Request Beats](DeskRequestBeatWriters.md) |
| Beat Writers | 71 | [Beat Writer](DeskBeatWriters.md) |
| Request Directors | 46 | [Request Directors](DeskRequestDirectors.md) |
| Beat Director | 422 | [Beat Directors](DeskBeatDirector.md) |
| Request Performance | 55 | [Request Performance](DeskRequestPerformance.md) |
| Actor Performance | 47 | [Actor Performance](DeskActorPerformance.md) |
| Request Observation | 41 | [Request Observation](DeskRequestObservation.md) |
| *(app machinery)* | — | [TheLoomApp Scheduling Notes](DeskSchedulerNotes.md) — stub, verify-against-code |

**Common desk anatomy** (so each child doc can assume it): a desk has a `SysPrompt` (Scriban template), a `DeskRole` (supplies the tool list via `{{model.role_commands}}`), an `Operator` (the model that runs it), `Enabled`, and routing pointers `OnSuccessTo` / `OnFailTo` / `OnPushbackTo` / `MaxAttempts`. A todo lands, the operator runs the prompt with its tools, the result routes by pointer (see §1 routing).

> **Staffing principle.** Judgment-heavy, low-volume seats (directors, coordination) get the strong model; high-volume, bounded seats (actors) get the fast local model (Gemma). The same split maps onto machines: fast bounded execution → Spark harness, coordination → primary box.

---

## 6. Operator quick-start

1. Import the desk set; confirm each desk's `DeskRole` resolves and its tool list renders under `{{model.role_commands}}`.
2. Assign an `Operator` (model) to each desk. **Known bug:** a model present on two hosts (PC + Spark) returns twice from the API under the same name and assignment breaks; until the identity fix lands, keep a given model on one host. The real fix binds team members to a composite `{device, model}` identity. *(Note: a bad operator assignment stalls a todo the same as a disabled desk — it never reaches a harness. See Scheduler Notes.)*
3. Enable desks in chain order; seed a story; watch todos flow.
4. To test one tier in isolation, point its `OnSuccessTo` at the stop desk (9) so results park instead of cascading.

---

*Last updated: through the git-integration additions + the Storytime port (rollup + actor-enrichment tier proven end-to-end on Scene 435). Next commit targets: model identity resolution across hosts, and the concurrency/fan-in hardening in §4.*
