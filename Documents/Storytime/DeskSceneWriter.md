# Desk: Scene Writer (68)

*Child doc of StorytimeDescribed. Second desk in the chain — turns a Story into its ordered Scenes. See the map for the requester→worker pattern, the routing model, and the Schedule loop; this doc goes deep on desk 68 alone.*

**Chain position:** Story Idea Maker (39) → **Scene Writer (68)** → Request Beat Writers (15)

---

## Role in one line

Given a Story, break it into exactly `TargetSceneCount` scenes whose **entry/exit states chain into a continuous line** — each scene changing the situation — then file them as SceneModels under the story and hand off to Request Beat Writers.

A **writer desk**, not a requester: it produces several nodes (the scenes) in one run and continues once; it does not fan out todos.

---

## Desk configuration (live values)

| Field | Value | Note |
|---|---|---|
| `DeskRole` | 59 | supplies the tool list |
| `Operator` | 66 | same operator as Story Idea Maker |
| `Enabled` | True | scheduling filter (see map / Scheduler Notes) |
| `OnSuccessTo` | 15 | Request Beat Writers |
| `OnFailTo` | 37 | DeskOfFails — infra/exception only, agent-invisible |
| `OnPushbackTo` | 9 | TheLoomAppSyncDesk — agent reject / disabled-desk exit |
| `MaxAttempts` | 3 | |
| `FilePath` | `WorkGroups\SceneWriter.json` | desk export target |

---

## What it reads

The SysPrompt first tells the operator to find the **storyId in the todo's Notes** (the carried context from the previous desk — see "todo chaining" below), then `getSummaryById(includeProps: true)` on three things:

- **Id 65 — `WriteScenes` skill card.** "These are the directions you must follow. Read this first." The craft spec, and the discipline that defines this desk:
  - **Write exactly `TargetSceneCount` scenes** — not a range. If the story can't fit, write that many anyway and note the strain in the last scene's exit state rather than adding/dropping.
  - **Every scene must change the situation** — what is true at exit that wasn't true at entry? "They feel it more strongly" is not a change; cut and rewrite.
  - **A realization happens once.** Scene 3 can't re-realize scene 2's insight in a new location. After realization comes consequence, pressure, or choice.
  - **Chain the states** — scene N's entry must follow directly from scene N−1's exit; read in sequence they form a continuous line with no reader-filled gaps.
  - **Entry states are facts, not moods** (who/where/what-just-happened/what-the-protagonist-wants — stageable, not a tone note).
  - **Exit states name outcomes, not gestures at them** — especially at the choice scene: name the choice *and which way it went*. Downstream writers see only what's written; an unstated decision gets re-invented differently by each of them.
  - **No prose, dialogue, or beats** — this is the skeleton beat writers hang scenes on. Prose gets rejected.
- **Id 103 — the realm**, `nodesUp: false` (theme/tone only; unlike Story Idea Maker it does *not* need the sibling-stories list, because it isn't checking distinctness).
- **The storyId from the task**, `nodesUp: false, includeProps: true` — the story idea card, which **carries `TargetSceneCount` and `PovDefault` in its props**. These two props are the whole reason Story Idea Maker set them.

## What it produces

- Calls **`addScene`** once per scene, in order, with the **storyId as parent**. Name, entry state, and exit state are all required per scene.
- Calls **`completeTodo`** passing the storyId as the produced item.

Scenes inherit `PovDefault` unless there's reason to differ — that's how `Pov` ends up on each SceneModel (later resolved to a string in the performance rollup).

## Routing

- **Success → Request Beat Writers (15).** The continuation carries the story forward; 15 fans out a beat-writing todo per scene.
- **Fail → 37 (infra only).** Code exceptions; the agent never chooses this.
- **Pushback → 9.** The agent's reject exit; also where the todo goes if this desk is disabled.

---

## Todo chaining (worth seeing here — it's the whole hand-off mechanism)

This desk's todos show how the pipeline carries context forward without a shared blackboard. The Story Idea Maker's `completeTodo` close reason becomes the **next todo's UserPrompt notes**. Real example, from todo 431:

> *"Working on StoryModel item Id:430 Burner Audit from previous todo id 116. Previous todo Notes given: Added new story idea 'Burner Audit'… Story ID 430, targetSceneCount 3, POV third-person limited, distinct from existing Static Debt."*

So the storyId (430), scene count, and POV all arrive **in the prompt text**, threaded from the upstream desk's close reason via `FromTodo: 116`. The desk doesn't query "what should I work on" — the requester that scheduled it already wrote the answer into the todo. This is the carrier pattern: the completing desk states its result, and that statement *is* the next desk's input.

## Worked instance (real runs)

Desk 68 has two completed runs in its history — one per story the pipeline has made:

- **Todo 431 → Story 430 "Burner Audit"** (Status *Complete Forward*): close reason — *"Wrote 3 scenes for Burner Audit… chain from discovery of fatal flaw through waiver choice to painless unweave, answering ending question about being right without pain."* Those are Scenes 435–437, the ones every other doc traces. Note the close reason explicitly describes the **chained arc** (discovery → choice → consequence) and ties the final scene to the ending question — the skill card's discipline, visible in the output.
- **Todo 427 → Story 426 "The Static Debt"**: still `Ready: 0`, not yet run — the other story seeded in Threadspace.

---

## Gotchas & notes

- **`TargetSceneCount` is exact, and it's a story prop — not a desk constant.** Unlike Story Idea Maker's hard-coded `sceneCount: 3`, this desk *reads* the count from the story card's props. So variable-length stories already work here; the only place 3 is hard-coded is upstream (desk 39). If scene counts come out wrong, check the story's `TargetSceneCount` prop, not this desk.
- **The exit-state discipline is load-bearing for everything downstream.** The skill card's rule 7 (name the choice and which way it went) exists because beat writers, directors, and actors all read the exit state and can't see intent. A vague exit state here silently corrupts every stage below it — and it'll look like a *director* or *actor* problem three desks later. When a scene's downstream output drifts, suspect a mushy exit state first.
- **Reads the realm with `nodesUp: false` on purpose** — it doesn't need sibling stories (no distinctness check at this stage), so it keeps the read lean. Don't "fix" this to `nodesUp: true`; it would just add noise.
- **State chaining is checked by re-reading exits in order.** The skill card's "Before you finish" step (read all exit states top-to-bottom; if two could swap or one restates another, revise) is the validator you'd want to enforce mechanically later — a grep/consistency pass comparing scene N entry against scene N−1 exit would catch the contradiction class automatically.

---

## Desk Prompt

The desk's live SysPrompt (copy out, renumber ids as needed):
```
This is the {{model.desk}} desk. 

Your role is to break a story into scenes that align with the realm's theme and tone.

Look to the todos Notes Taken section for a story Id.

Use getSummaryById with includeProps => true, on the following:
Id: 65 — WriteScenes. These are the directions you must follow. Read this first.
Id: 103 — the realm description. nodesUp=>false; you do not need the other stories.
The storyId given in your task — nodesUp false, includeProps true. This is the story idea card you are writing from, and carries TargetSceneCount and PovDefault in the props.

Write the scenes and add with addScene, using the storyId from your task as the parent.

Finally pass the storyId to completeTodo to mark this todo complete.

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}
```

---

*Status: first draft, written against live desk 68 + skill card 65. Verify `addScene`'s exact param names (name / entryState / exitState / parent) against the tool signature on the testing pass.*
