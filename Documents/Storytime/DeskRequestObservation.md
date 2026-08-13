# Desk: Request Observation (41)

*Child doc of StorytimeDescribed. The **final desk** — renders a scene's performance into finished prose. This is where the pipeline produces the thing a reader actually reads. With this desk documented, the chain is complete end to end: story idea → readable scene.*

**Chain position:** Request Performance (55) → **Request Observation (41)** → *dead-ends (9)* (later: → a Story-level rollup)

---

## Role in one line

Given one scene's Performance, read the whole assembled script via `getPerformanceRollup` and write it out as continuous prose — holding POV and the realm's Tone, preferring the actor's performed version of each entry — then file it as an Observation under the performance.

Named "Request Observation" for chain symmetry with the other stage-openers, but it is **not a requester** — it fans out nothing. It's a **producer/leaf**: it reads the rollup and produces one ObservationModel, then dead-ends. (The name predates its final single-desk shape; think of it as "the Observer.")

---

## What it consumes: the rollup is the whole job

This desk is the payoff for everything `getPerformanceRollup` assembles. One call hands it the complete scene — realm Tone, story card, scene entry/exit/POV, character roster, and the Rank-ordered entry stream with each entry marked `Source: "Director"` or `"Actor"`. The observer's craft is to turn that ordered script into prose:

- **Render in Rank order, start to finish** — the flattened stream *is* the playback order (assigned back at Request Performance's flatten step, preserved through the actor merge).
- **Prefer the Actor's version** when `Source` is `"Actor"` — that performed text is the enriched version; the Director text is the fallback for un-acted entries. This is where the "run with or without actors" design finally cashes out: the observer renders whatever the rollup holds, actor-performed or not, without branching.
- **Narration → narration, Action → described action, Line → spoken dialogue.** This is the *only* desk that renders quoted dialogue — every upstream desk deliberately avoided it (beat writers and directors were forbidden dialogue precisely so the line would be written once, here, from the actor's performed text).
- **Open on the entry state, land on the exit state, resolve nothing past it** — the same boundary discipline the writer desks established, now applied to the finished prose.

---

## Desk configuration (live values)

| Field | Value | Note |
|---|---|---|
| `DeskRole` | 802 | carries the observer tools (getPerformanceRollup, addObservation, completeTodo, rejectTodo) |
| `Operator` | 66 | the **writer** model — judgment/craft seat, not the fast local one (correct per the staffing principle: one call per scene, real prose) |
| `Enabled` | True | |
| `OnSuccessTo` | 9 | dead-end (leaf). Later this may carry toward a Story-level rollup (see notes) |
| `OnFailTo` | 37 | DeskOfFails — infra/exception only |
| `OnPushbackTo` | 9 | TheLoomAppSyncDesk — agent reject / disabled-desk exit / empty-rollup reject |
| `MaxAttempts` | 3 | |
| `FilePath` | `WorkGroups\RequestObservation.json` | desk export target |

**Why a leaf.** Observation is per-scene and currently nothing consumes it downstream, so it dead-ends. When the Story-level rollup lands (StoryObservation — see map §2 planned nodes), this desk's output becomes that rollup's input, and the fan-in that gathers all a story's scene observations will be the same barrier primitive as the actor→observation fan-in.

---

## What it reads

- The **performanceId and todoId** from its UserPrompt (threaded from Request Performance's close reason — carrier pattern; e.g. "Working on PerformanceModel item Id:678 Red LIVE Performance").
- **`getPerformanceRollup(performanceId)`** — the entire scene in one call. No skill card; the craft is inline in the prompt.

## What it produces

- **`addObservation(performanceId, name, contents)`** — creates an **ObservationModel** child of the performance, named "<scene> Observation", with the finished prose in `contents`. (This is the first appearance of `ObservationModel`, TypeId 1179.)
- **`completeTodo`** with the performanceId and a one-line note.
- On an empty/unrenderable rollup, **`rejectTodo`** with the reason.

## Routing

- **Success → 9 (dead-end / leaf).**
- **Fail → 37 (infra only).**
- **Pushback → 9** (also the empty-rollup reject path).

---

## Worked instance (the pipeline's first readable scene)

**Todo 699 → Performance 678 "Red LIVE"** (`FromTodo: 503`, Operator 66 / Muse): close reason *"Observation rendered for Red LIVE Performance 678."* Produced **ObservationModel 805, "Red LIVE Observation"** — the first end-to-end prose the pipeline has ever made, story-idea to finished scene.

It works, and it's on-tone. A few markers from 805 that show the whole chain landing:

- **Realm Tone honored literally** — the observer wrote in Threadspace's register: "meat-thinking running thin," "a city that doesn't know it's already dead," "bookkeeping with a throat," "still as a dead fork." That diction traces straight back to the realm's `Tone` property, carried through the rollup.
- **Entry→exit boundary held** — opens on the red LIVE lane with Kade-7 frozen (the scene's entry state) and lands exactly on the signature tracing to Kade-prime / the closed-loop realization (the exit state). No drift past the boundary — the discipline the Scene and Beat writers enforced survived all the way to prose.
- **The actor enrichment is visible in the output** — the "frozen silhouette, crimson light ticking in his pupils, holding the secret like a live wire in his gut" is Gemma's Rank-1 performance (node 679) from the actor-tier test, rendered into the final scene. The `Source: "Actor"` preference worked: the performed version reached the page.

> **Note for the write-up: this run was rendered from a *partially* acted performance.** Only some of Performance 678's actor entries had run during the isolated actor test; the rest were still `Source: "Director"`. The observer rendered both seamlessly — which is exactly the "with or without actors" property working as designed. A full run (all actors performed first) would render entirely from actor text.

---

## Gotchas & notes

- **This is the only desk that writes dialogue as dialogue.** If quoted speech ever appears upstream (in beats or call sheets), it's a bug there — the whole pipeline defers spoken lines to this render step so they're written once, from the actor's performed line. (In Red LIVE, Kade never speaks, so 805 is all narration/action — the dialogue path is untested until a scene with spoken lines runs through.)
- **Renders whatever the rollup holds — it does not wait for actors.** Because this desk currently fires based on FIFO ordering after Request Performance (not on actor completion), it *will* render a partially-acted or fully-director performance if run before the actors finish. That's fine by design (the rollup is always coherent), but for the *best* prose you want all actors done first — which is the fan-in barrier the map flags as KNOWN-OPEN. Todo 699 was rendered partial and still read well; a barrier would just guarantee full enrichment.
- **`Operator 66` (writer model) is the right seat, and it's slow.** This is a full-scene prose generation on the heavy model — the run "took some time" on Muse. That's expected: it's one high-value call per scene, the opposite of the actor tier's many-fast-calls profile. Don't move it to the fast local model; prose quality is the whole point here.
- **Observation is the natural StoryObservation input.** When the story-level rollup is built, it gathers these ObservationModels (one per scene) in scene-Rank order and assembles the finished article. This desk's `OnSuccessTo: 9` becomes the point where that future fan-in attaches. Design it as the same barrier primitive as the actor fan-in.

---

## Desk Prompt

The desk's live SysPrompt (copy out, renumber ids as needed):
```
This is the {{model.desk}} desk.

Your role is to render one scene's performance into finished prose — the observation. This is the scene as a reader will read it.

The todo user prompt references a PerformanceModel item Id as performanceId and the todo id as todoId.

1. Use getPerformanceRollup with performanceId. This returns the whole scene assembled in one call: the Realm tone, the Story card, the Scene (entry state, exit state, POV), the Characters, and the Performance — a stream of entries in Rank order. Each entry is Narration or a character's Line/Action, marked Source "Director" or "Actor". Read it as the ordered script of the scene.

2. Write the scene as continuous prose, in Rank order, start to finish:
   - Hold the Scene's POV throughout, and write in the Realm's Tone (read the Tone as literally as you can — it governs diction in every sentence).
   - Render each entry in order: narration becomes narration, a character's action becomes described action, a spoken line becomes spoken dialogue. Prefer the Actor's version of an entry when Source is "Actor"; it is the performed text.
   - Open on the Scene's entry state and land on its exit state — no earlier, no later. Do not resolve anything past the exit state; the next scene owns what comes after.
   - Enrich into flowing prose, but do not invent events the script does not contain, do not add or rename characters, and do not contradict the Realm.

3. Use addObservation with performanceId, a name derived from the scene (e.g. "<scene name> Observation"), and contents set to the finished prose.

4. If the rollup is empty, or you cannot render the scene, use rejectTodo with the reason instead of completeTodo.

5. Finally, pass the performanceId to completeTodo to mark this todo complete, with a one-line note.

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}
```

---

*Status: first draft, written against live desk 41 + real run (Observation 805 from Performance 678). This completes the desk-chain documentation set. Remaining pipeline work: the fan-in barrier (map KNOWN-OPEN) and the planned StoryObservation rollup that consumes these observations.*
