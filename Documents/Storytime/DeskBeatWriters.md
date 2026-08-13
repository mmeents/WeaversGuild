# Desk: Beat Writers (71)

*Child doc of StorytimeDescribed. The **worker** on the receiving end of Request Beat Writers' fan-out — one of the three worker desks. Unlike the actor leaves, this worker **continues** the chain. See the map for the requester→worker pattern and routing.*

**Chain position:** Request Beat Writers (15) ⟹ *fans out per scene* → **Beat Writers (71)** → Request Directors (46)

---

## Role in one line

Given one Scene, break it into 3–6 ordered Beats that start at the scene's `EntryState`, land exactly on its `ExitState`, and hold the story/realm tone and POV — then file them as BeatModels under the scene and continue to Request Directors.

A **worker desk**, and a content-producing writer: it does one scene's worth of work per todo (there is one todo per scene, planted by desk 15).

---

## A worker that continues (the asymmetry worth noticing)

The map calls workers "leaves that dead-end," and that's true of the **actor** worker (47) — but **not** this one. Beat Writers has `OnSuccessTo: 46` (Request Directors). Each beat-writing todo *continues* the chain.

Why the difference? It comes down to **carrier-vs-requester asymmetry**: when the next stage after a worker is *also* per-scene, the worker can carry the chain forward itself — it finishes its scene's beats and hands *that same scene* to the next requester (Request Directors, which fans out per beat within the scene). The actor worker dead-ends instead because *its* next stage (observation) is per-**scene** while the actor is per-**Action-entry** — a per-entry leaf can't carry a per-scene continuation, so the fan-in barrier handles it. 

> **Rule of thumb:** a worker **carries** when the next stage's granularity matches its own todo's subject (here: scene → scene). A worker **dead-ends** when the next stage is coarser than the worker's subject (actor entry → scene observation), leaving re-assembly to a barrier. Beat Writers is the carry case; Actor Performance is the dead-end case.

So three beat-writing todos (one per scene) each independently continue to Request Directors — the fan-out from 15 becomes three parallel sub-chains, each carrying its own scene onward.

---

## Desk configuration (live values)

| Field | Value | Note |
|---|---|---|
| `DeskRole` | 64 | supplies the tool list |
| `Operator` | 66 | the creative writer operator (same as scene/idea desks) |
| `Enabled` | True | scheduling filter (see map / Scheduler Notes) |
| `OnSuccessTo` | 46 | Request Directors — **this worker carries** |
| `OnFailTo` | 37 | DeskOfFails — infra/exception only, agent-invisible |
| `OnPushbackTo` | 9 | TheLoomAppSyncDesk — agent reject / disabled-desk exit |
| `MaxAttempts` | 5 | higher — beats must satisfy the entry/exit boundary exactly (see gotchas) |
| `FilePath` | `WorkGroups\BeatWriter.json` | desk export target |

---

## What it reads

The SysPrompt gives it **both `storyId` and `sceneId`** from its todo (the todo names them directly — e.g. "Write the Beats for storyId 430, sceneId 435"), then `getSummaryById(includeProps: true)` on four things:

- **Id 24 — `WriteBeats` skill card.** "Read this first." The craft spec and the safety rules that make parallel scene-writing work:
  - **Start at the entry state, end at the exit state** — the first beat opens on the entry situation (not before), the last beat lands on the exit fact (not past it).
  - **Do not resolve anything the exit state leaves open** — *this is the load-bearing rule.* Another writer owns the next scene and is working from the same exit state. If this desk decides something the exit state doesn't assert, the two scenes disagree and one is wrong. (See the safety note below.)
  - **3–6 beats**, each one continuous unit of action/attention. "If a beat needs the word 'then,' it is two beats."
  - **Hold the POV** (third-person limited = only what the POV character perceives; no other-character interiority, no narrator omniscience).
  - **Tone is binding**; **don't contradict the realm's** established facts.
  - **No dialogue, no invented named characters** — spoken lines are written downstream by actors; a line written here becomes a second source of truth that conflicts with theirs. Functionaries stay unnamed, defined by role.
- **Id 103 — the realm**, `nodesUp: false` (tone/facts; no sibling stories needed).
- **The storyId**, `includeProps: true` — the story card (premise, engine, ending question, `PovDefault`).
- **The sceneId**, `includeProps: true` — the scene being written, carrying `EntryState`, `ExitState`, and `POV`. **This is the boundary contract.**

Each beat should carry a clear purpose (Setup / Choice / Escalation / Climax / Resolution) per the prompt.

## What it produces

- Calls **`addBeat(sceneId, name, details)`** once per beat, in order, with the **sceneId as parent**. (Note: unlike scenes, beats carry `Details` prose — this is the first desk that writes actual scene-level prose content, in the realm's tone and POV.)
- Calls **`completeTodo`** passing the sceneId — which routes the chain onward to Request Directors.

## Routing

- **Success → Request Directors (46).** This worker carries; the completed scene (now with beats) goes to the next requester, which fans out per beat.
- **Fail → 37 (infra only).** Code exceptions.
- **Pushback → 9.** Agent reject exit; disabled-desk exit.

---

## Parallelism & safety (why the exit-state rule matters here specifically)

The WriteBeats card states it outright: *"You are one of several beat writers working on this story at the same time. You cannot see the other scenes' beats, and they cannot see yours."* The three todos from desk 15's fan-out are **independent and concurrent-safe** — but only because of rule 3 (don't resolve past the exit state).

Here's the mechanism: scene N's writer and scene N+1's writer share a boundary — N's `ExitState` is N+1's `EntryState`. As long as N stops exactly at its stated exit and N+1 starts exactly at its stated entry, the two scenes meet cleanly without ever seeing each other. The moment one writer improvises past its boundary, the seam tears — and because the writers are blind to each other, neither can detect it. **The exit-state discipline is what lets these run in parallel at all.** This is the same state-chaining contract the Scene Writer established, now enforced at the boundary between concurrent workers rather than between sequential scenes.

---

## Worked instance (real runs — the full fan-out landed)

All three todos from desk 15's fan-out completed here, each producing 4 beats:

- **Todo 440 → Scene 435 "Red LIVE"** (`FromTodo: 438`): *"Wrote 4 beats… starting at entry state and landing on exit state confirmation of manifest lock limitations and signature trace to Kade-prime."* → Beats 445–448.
- **Todo 441 → Scene 436 "The Waiver"**: *"Wrote 4 beats… starting at entry state and landing on exit state. Beats follow realm tone and POV constraints."*
- **Todo 442 → Scene 437 "Unweave Quiet"**: *"Created 4 beats… spanning entry to exit states with cynical cyber-noir tone and third-person limited POV."*

Every close reason names the entry→exit boundary explicitly — the desk reporting that it honored the contract. All three carried onward to Request Directors independently. This is the fan-out from 15 fully realized: one requester todo → three worker todos → three continuations.

---

## Gotchas & notes

- **`MaxAttempts` is 5 (high) because the boundary is exacting.** Landing *exactly* on the exit state — no earlier, no later — is a real constraint an operator can miss, and a miss should retry rather than ship a torn seam. Don't lower this without knowing the boundary discipline depends on it.
- **This is the first desk that writes prose (`Details`).** Scenes and stories were skeletons; beats carry actual tone-and-POV prose. But note the "no dialogue" rule — the prose describes what happens and what's registered, and leaves spoken lines to the actor tier. A beat that contains quoted dialogue is a bug: it pre-empts the actor and creates a conflicting source of truth.
- **The "don't resolve past exit" rule is a concurrency invariant, not a style note.** If you ever see two adjacent scenes' content disagree at their seam, suspect a beat writer that ran past its exit state — not the director or actor downstream. It's the same class of failure as the Scene Writer's mushy-exit-state gotcha, one level down.
- **The concurrency here is currently safe because reads are independent and there's no shared write target** — each writer only `addBeat`s under its own scene. This is the benign end of the concurrency spectrum; the sharp end is the fan-**in** (actor barrier), not this fan-**out**.

---

## Desk Prompt

The desk's live SysPrompt (copy out, renumber ids as needed):
```
This is the {{model.desk}} desk. 

Your role is to break a scene into beats that align with scenes entry and exit conditions and conforms to the story and realm.

Look to the todos for both storyId and sceneId.

Use getSummaryById with includeProps => true, on the following:
Id: 24 — WriteBeats. These are the directions you must follow. Read this first.
Id: 103 — the realm description. nodesUp=>false; you do not need the other stories.
The storyId given in your todo — nodesUp false, includeProps true. This is the story idea card you are writing from, and carries TargetSceneCount and PovDefault in the props.
The sceneId given in your todo - nodesUp false, includeProps true. This is the Scene we are writing beats to.  

Create 3-6 new Beats that tell this scene moment-by-moment. Each beat should have a clear purpose (Setup/Choice/Escalation/Climax/Resolution).

Write the beat with addBeat, using the sceneId from your task as the parent. 

Finally pass the sceneId to completeTodo to mark this todo complete.

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}
```

---

*Status: first draft, written against live desk 71 + skill card 24. Verify `addBeat`'s exact param names (sceneId / name / details) on the testing pass — confirmed present in the tool signature as of writing.*
