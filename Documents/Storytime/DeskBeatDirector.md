# Desk: Beat Director (422)

*Child doc of StorytimeDescribed. The **bridge desk** — the worker that turns beats into directed CallSheets. Its output is the raw material the Performance rollup flattens, so this is the seam between the writing half of the pipeline and the performance half. See the map for the requester→worker pattern; see the getPerformanceRollup section of the map for where its output goes next.*

**Chain position:** Request Directors (46) ⟹ *fans out per beat* → **Beat Director (422)** → *dead-ends (9)*

---

## Role in one line

Given one Beat, fill its (already-created) CallSheet with an ordered, interleaved sequence of **narration** and **character roles** — the beat played out moment-by-moment in dramatic order — then dead-end. The call sheet it produces is the director's interpretation of the beat, and the unit the performance stage later flattens and fans out from.

A **worker desk**, and a **leaf** (dead-ends to 9, like the actor worker — it does not carry). One todo per beat, planted by Request Directors.

---

## Why this desk is the seam

Everything before this desk built *structure* (story → scenes → beats). Everything after it is *performance* (call sheets → performance → actors → observation). Beat Director is where structure becomes performable material.

Its output — a filled CallSheetModel — is exactly what the earlier performance-side docs consume:
- **Request Performance (55)** flattens all of a scene's call sheets into one ranked `PerformanceScript` (the `Data` column).
- **`getPerformanceRollup`** reads that flattened stream.
- The call sheet's two entry kinds (**Narration** and **Role/Action**) become the `PerformanceEntry.Type` values Narration and Action — and only the Action entries spawn actor performances.

So the interleaving decision this desk makes *is* the playback order of the finished scene. "The order of your calls is the order of playback," as the prompt says — and that order survives all the way to the rollup, because Rank is assigned from it.

---

## Desk configuration (live values)

| Field | Value | Note |
|---|---|---|
| `DeskRole` | 36 | supplies the tool list |
| `Operator` | 40 | |
| `Enabled` | **0 (disabled)** | ⚠️ currently off — see gotchas |
| `OnSuccessTo` | 9 | **dead-end (leaf)** — does not carry; the scene already advanced via Request Directors' continuation |
| `OnFailTo` | 37 | DeskOfFails — infra/exception only |
| `OnPushbackTo` | 9 | TheLoomAppSyncDesk — agent reject / disabled-desk exit |
| `MaxAttempts` | 3 | |
| `FilePath` | `WorkGroups\BeatDirector.json` | desk export target |

**Why a leaf here?** Beat Director is per-**beat**, but the next stage (Request Performance) is per-**scene**. A per-beat worker can't carry a per-scene continuation — so it dead-ends, and the per-scene continuation was already carried forward by Request Directors (46) when *it* completed. This is the same carrier-vs-requester asymmetry as the actor worker: worker dead-ends when the next stage is coarser than its own subject. (See the Beat Writers doc for the full rule.)

---

## What it reads

The todo gives it the **beatId** and, spelled out in the UserPrompt, the **already-created callSheetId** (e.g. "The Call Sheet has already been created. Use Call Sheet Id: 495"). Then `getSummaryById(includeProps: true)`:

- **the sceneId**, `nodesUp: true` — to see all the scene's beats and characters (context: where this beat sits, who's available).
- **the beatId**, `nodesUp: true` — to see the beat's details and confirm the callSheetId.

No skill card is referenced — the craft is inline in the SysPrompt (unlike the writer desks, which read a card). The direction is brief: build the call sheet in exact dramatic order.

## What it produces

Fills the pre-existing CallSheet (created by Request Directors' schedule call) using two tools, interleaved in playback order:

- **`addCallSheetNarration(callSheetId, name, narration)`** — a narration beat between character moments (the "room," atmosphere, non-character action). → becomes a Narration entry.
- **`addCallSheetRole(callSheetId, character, directions)`** — casts a character with an acting **instruction** (not finished prose — intent for the actor to perform). → becomes an Action entry, and later spawns an ActorPerformance.

> **Key side effect — this is where Characters are born.** `addCallSheetRole` *"Adds Character to scene if not already present by character"* (by name). So naming a character in a role call **auto-creates the CharacterModel** on the scene if it's new. This answers a question that surfaces in the performance-side docs: the scene's character roster (which `getPerformanceRollup` reads from scene relations) is populated *here*, as a side effect of casting. It's also why each scene's "Kade-7" is a distinct character id — each scene's directors cast independently, minting a fresh CharacterModel per scene by name.

- Calls **`completeTodo`** passing the **callSheetId** as the produced item, then dead-ends.

## Routing

- **Success → 9 (dead-end / leaf).** Its work is the filled call sheet; nothing continues from here. The scene's forward motion already happened at Request Directors.
- **Fail → 37 (infra only).**
- **Pushback → 9.**

---

## Worked instance (real runs — 12 beats directed across 3 scenes)

All 12 Beat-Director todos (from Request Directors' three fan-outs) completed here. A representative sample:

- **Todo 496 → Beat 445 "Red LIVE on the hot lane"**, CallSheet 495 (`FromTodo: 449`): *"Call sheet for beat 445 constructed. Dramatized the internal conflict of Kade-7 against the ticking clock of the grid-sync manifest."*
- **Todo 502 → Beat 448 "Signature traces to prime"**, CallSheet 501: *"Call sheet 501 for beat 448 has been built with interleaved narration and roles for Kade-7…"*
- **Todo 512 → Beat 485 "Waiver Signed"**, CallSheet 511: *"…interleaved narration and character roles for Kade-7 and the Spindle Release Daemon."* ← the two-character beat that shows up as the multi-character roster in Scene 436's rollup.
- **Todo 522 → Beat 492 "Painless Unweave"**, CallSheet 521: *"…a sequence of narrations and a final acting role for Kade-7, capturing the clinical and indifferent nature of the unweave protocol."*

Every close reason describes the **interleaving of narration and roles** — the desk reporting the dramatic order it built. Those 12 call sheets (495, 497, 499, 501 for scene 435; 505–511 for 436; 515–521 for 437) are precisely what Request Performance flattens into Performances 678 / 703 / 723.

> **This is where the "director over-writes" known-issue originates.** Look at CallSheet content in the performance docs: the role `directions` came through as finished prose with interior state ("the weight of a thousand simulated failures pressing down on him") rather than terse instruction. That happens *here* — the director (operator 40) writing performance instead of direction. The intended validator-grep (cap length, strip interior-state) would sit on this desk's output. See the map's KNOWN-OPEN block.

---

## Gotchas & notes

- **`Enabled` is 0 — this desk is currently disabled.** The 12 todos already ran (the call sheets exist), but if you re-seed a story, Beat Director won't pick up new todos until you re-enable it. Likely left off during the actor-tier isolation testing. Flip it on for a full run.
- **The call sheet is created upstream, filled here.** Request Directors' `scheduleBeatDirectors` creates the empty CallSheet; this desk fills it. So the callSheetId always pre-exists and arrives in the UserPrompt — this desk never creates a call sheet, only populates one. (If a callSheetId is missing from the prompt, the upstream schedule call didn't do its side-effect — look at desk 46, not here.)
- **Narration vs Role is the Narration/Action split downstream.** The director's choice of which tool to call per entry determines whether that entry ever gets an actor. Narration entries are performed by no one (they pass straight through the rollup as Director); role entries become the actor tier's work. Mis-casting a line as narration means no actor ever touches it.
- **Roles carry instruction, not prose (in principle).** `addCallSheetRole`'s `directions` is meant to be an acting instruction the actor enriches — the intent, behavior, and constraint, not the finished line. The current operator over-writes this (see worked-instance note); that's the upstream half of the director-vs-actor tension documented on the performance side.
- **`beatId` and `sceneId` look duplicated in the todo names** (e.g. "beatId: 445, sceneId 445") — that's a quirk of the todo-naming template, not a real id collision; the beat and scene are different nodes. The UserPrompt uses beatId correctly.

---

## Desk Prompt

The desk's live SysPrompt (copy out, renumber ids as needed):
```
This is the {{model.desk}} desk. 

Your job is to direct the 1 beat in the scene.

Use getSummaryById with includeProps => true, on the following:
Id: sceneId - nodesUp => true to see all the beats in the scene and characters.
Id: beatId - nodesUp => true to see the callSheetId.

Build the call sheet in the exact dramatic order the beat should play out.
Use these two tools to do it: 
  addCallSheetRole - cast a character with their acting instruction.
  addCallSheetNarration - add a narration beat between character moments. 
Interleave narration and roles naturally, the order of your calls is the order of playback.

Finally pass the callSheetId to completeTodo to mark this todo complete.

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}
```

---

*Status: first draft, written against live desk 422 + the addCallSheetRole/addCallSheetNarration tool contracts. Note: desk is currently Enabled=0. The role-directions-as-prose issue is the upstream origin of the director-vs-actor tension; validator-grep would live on this desk's output.*
