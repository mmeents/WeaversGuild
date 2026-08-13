# Desk: Actor Performance (47)

*Child doc of StorytimeDescribed. The **final worker** — and the canonical leaf. Turns a director's instruction into performed text (a line or actions). This is the desk whose prompt, tools, and rollup-merge were designed and proven end-to-end during the Storytime port; it's the most-worked corner of the pipeline. See the map's getPerformanceRollup section for how its output is read back.*

**Chain position:** Request Performance (55) ⟹ *fans out per Action entry* → **Actor Performance (47)** → *dead-ends (9)*; re-assembly happens in `getPerformanceRollup`, not through routing.

---

## Role in one line

Given one **moment** (one Action entry from a scene's performance), enrich the director's terse instruction into performed text — deciding whether it reaches the page as a spoken **line**, an observable **action**, or a short ordered run of them — and write it back to the ActorPerformance node. Then dead-end.

A **worker desk**, and the archetypal **leaf**: it completes with no continuation. The chain does not advance from here.

---

## Why it's a leaf, and how the chain continues anyway

Actor Performance is per-**Action-entry** — the finest granularity in the pipeline. Its downstream stage (Request Observation) is per-**scene**. A per-moment leaf cannot carry a per-scene continuation, so it dead-ends (`OnSuccessTo: 9`), exactly like Beat Director. (See the Beat Writers doc for the carrier-vs-worker rule.)

The scene's forward motion was already carried by **Request Performance (55)**, whose continuation to Request Observation was created *after* these actor todos (FIFO ordering — see the 46 doc). But there's a crucial difference from the fan-out cases: **observation actually needs these actors to be *complete*, not merely scheduled** — it renders their enriched output. That completion-gating is the fan-in barrier the map flags as KNOWN-OPEN. Right now the leaf dead-ends at the stop desk (9) so the tier can be tested in isolation without observation firing prematurely.

**Re-assembly is by data, not routing.** Each actor writes its output into its own ActorPerformance node's `Data`. `getPerformanceRollup` later splices those back into the flattened performance stream, matched by `Rank`. So the leaves never hand off to each other or to a next desk — they deposit output keyed by rank, and the rollup re-sequences the whole scene afterward. This is why completion order doesn't matter and why the leaves are safe to run concurrently.

---

## Desk configuration (live values)

| Field | Value | Note |
|---|---|---|
| `DeskRole` | 556 | supplies the tool list (all six tools + help render — verified) |
| `Operator` | 40 (target: Gemma) | the fast local model — high-volume bounded work; ideal concurrency target |
| `Enabled` | **0 (disabled)** | ⚠️ off — enabled per-run for isolated testing (see gotchas) |
| `OnSuccessTo` | 9 | **dead-end (leaf)** — re-assembly is via rollup, not routing |
| `OnFailTo` | 37 | DeskOfFails — infra/exception only |
| `OnPushbackTo` | 9 | TheLoomAppSyncDesk — agent reject / disabled-desk exit |
| `MaxAttempts` | 3 | |
| `FilePath` | `WorkGroups\ActorsPerformance.json` | desk export target |

---

## What it reads

The todo names three ids: **actorPerformanceId** (the one moment), **performanceId** (its parent), and **todoId**. Then:

1. **`getPerformanceRollup(performanceId)`** — reads the *whole* scene script for context. Entries show the director's version until an actor performs a moment, then switch to the actor's. The actor reads what comes before and after its moment so its performance fits. (This is the "let the actor see the whole scene" design — an actor is not blind to its surroundings, only to the other actors' in-flight choices.)
2. **`getSummaryById(actorPerformanceId, nodesUp: true, includeProps: true)`** — its own node: the **Character** and the director's **Instructions** for this one moment.

## What it produces

Writes into its own ActorPerformance node's `Data` (a mini `PerformanceScript`), using two tools — the actor's discretion decides which, and how many:

- **`addPerformanceLine(actorPerformanceId, line)`** — the character's spoken words. → entry Type `Line`.
- **`addPerformanceAction(actorPerformanceId, action)`** — an observable action. → entry Type `Action`.

**One moment can be a short ordered run** — e.g. action, then action, then the line it lands on ("last key entered → wallet unlocks → 'cheesus, I'm rich'"). The tools **append** (each call adds an entry at the next rank), so multiple calls build an ordered sequence rather than overwriting. The prompt caps drift: "if you are on a third or fourth entry, you are probably drifting into the next moment."

- **`completeTodo`** with actorPerformanceId as the produced item and a one-line note (line vs action / how many). Then dead-ends.

> **Line-vs-action is the actor's discretion — and that discretion IS this tier's reason to exist.** The director says *what happens*; the actor decides *how it reaches the page* (spoken or done) and writes the actual text. This is why the ActorPerformanceModel has **no Mode field** — encoding the choice upstream would hollow out the tier. (See map design-decisions block.)

---

## The rollup merge (how this desk's output is read back)

This desk deposits; `getPerformanceRollup` re-assembles. The mechanism, proven end-to-end this session:

- The rollup walks the performance's flattened entries. For each Action entry, it looks for an ActorPerformance node **matched by `Rank`** (not name — name matching was tried and silently failed; see map). If found with non-empty `Data`, it emits *that* node's entries with `Source: "Actor"`, re-numbering the flattened output so `Rank` stays unique even when one director instruction expanded into several actor entries.
- Un-acted Action entries and all Narration pass through as `Source: "Director"`.
- `ActorPerformed` flips true if any entry resolved to an actor.

So a partially-performed scene reads correctly: performed moments show `Actor`, the rest show `Director`, same call, no branching. This is what makes "run the observer with or without actors" work.

---

## Worked instance (the merge, proven)

Scene 435's ten actor todos (689–698) target ActorPerformance nodes 679–688 (ranks 1,3,5,7,9,12,14,17,19,21 of Performance 678). When node **679** (Rank 1, Kade-7) was performed by Gemma, `getPerformanceRollup(678)` returned Rank 1 as `Source: "Actor"` with `ActorPerformed: true` — every other Action rank still `Director`. Gemma's enrichment kept the director's blocking (frozen, staring into the red, holding the secret) and deepened the imagery on-tone ("a city that doesn't know it's already dead") rather than inventing new action — confirming the tier does real work, not restatement. That single-rank flip, against real data, is the proof the whole deposit-and-splice design holds.

---

## Gotchas & notes

- **`Enabled` is 0 — enabled per-run for isolated testing.** With `OnSuccessTo: 9` (dead-end), running this desk parks results without cascading to observation — the deliberate setup for testing the actor tier alone. Flip on to run, watch the rollup flip ranks to `Actor`, flip off.
- **The append tools reload `Data` by id before adding** — critical for multi-entry moments. A stale in-memory DTO would let a second call overwrite the first (silently swallowing, e.g., the "cheesus" line). Reload-by-id closes it. (See map design-decisions block — this is the fix from the multi-entry work.)
- **The director over-writes (upstream issue affecting this desk).** The `Instructions` this desk receives currently arrive as finished prose with interior state rather than terse intent, so the actor is "enriching" text that's already written — inviting parroting or drift. The fix is a validator-grep on **Beat Director's** output (desk 422), not here. This desk is the downstream victim, not the cause.
- **Per-Action fan-out means many sequential calls per scene.** Ten todos for Scene 435, each a separate local-model call. On the muse model that dragged; Gemma (an order of magnitude faster) makes it tolerable. The efficiency escape hatch is **per-character fan-out** (one todo carrying all of a character's ranks) — changes `scheduleActorPerformances` grouping, not this desk's logic. Prove the plumbing on per-Action first.
- **Concurrency-ready but concurrency-exposed.** These leaves are embarrassingly parallel (independent, no shared write target — each writes only its own node). That makes the actor tier the ideal first target for the Spark's concurrent workers. But the fan-**in** into observation (desk 55's continuation) is where concurrency bites — the barrier tally must be atomic. See map KNOWN-OPEN.

---

## Desk Prompt

The desk's live SysPrompt (copy out, renumber ids as needed):
```
This is the {{model.desk}} desk.

You are an actor, cast as one character in a scene's performance. Your todo gives
you a single moment to perform. It references one ActorPerformanceModel as
actorPerformanceId, its parent Performance as performanceId, and the todo id as
todoId. You perform only this one moment, for this one character.

1. Use getPerformanceRollup with performanceId to read the whole script for
   context. Entries show the director's instructions until an actor performs a
   moment, at which point that entry switches to the actor's version. Read what
   comes before and after your moment so it fits.

2. Use getSummaryById with actorPerformanceId, nodesUp=true, includeProps=true.
   This gives your character and the director's Instructions for your one moment.
   The director assembled these from the beats quickly — you are enriching the
   moment, not rewriting or deviating from it.

3. Decide how your moment reaches the page. Most moments are a single entry, but
   one moment can be a short run that belongs together — an action, then another
   action, then the line it lands on. Produce them in order, one tool call each:
   - for something the character says, use addPerformanceLine with
     actorPerformanceId and the spoken words.
   - for something the character does, use addPerformanceAction with
     actorPerformanceId and the observable action.
   Stay inside the director's instruction — enrich the moment, do not invent new
   moments, narrate the room, or perform other characters. Keep the run tight: if
   you are on a third or fourth entry, you are probably drifting into the next
   moment. Hold your character and the scene's Tone.

4. If you cannot tell which character you are, or the instruction is missing, use
   rejectTodo with the reason instead of completeTodo.

5. When your moment is performed, use completeTodo with actorPerformanceId as the
   produced item and a one-line note saying whether you added a line or an action.

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}
```

---

*Status: first draft, written against live desk 47. This desk + its rollup merge are the most-tested part of the pipeline (proven on Scene 435, node 679). Currently Enabled=0; enable per-run. The remaining open edge is the fan-in into Request Observation (41) — see map KNOWN-OPEN.*
