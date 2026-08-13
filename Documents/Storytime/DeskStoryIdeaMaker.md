# Desk: Story Idea Maker (39)

*Child doc of StorytimeDescribed. First desk in the chain — turns a Realm into a new Story. See the map for the requester→worker pattern and the full chain; this doc goes deep on desk 39 alone.*

**Chain position:** *(chain start)* → **Story Idea Maker (39)** → Scene Writer (68)

---

## Role in one line

Given a Realm, invent **one** new story idea that fits that realm's theme and tone, is distinct from the stories already in it, and has a real *engine* — then file it as a StoryModel and pass it downstream to the Scene Writer.

This is a **writer desk**, not a requester. It produces a single node (a Story) and continues; it does not fan out.

---

## Desk configuration (live values)

| Field | Value | Note |
|---|---|---|
| `DeskRole` | 86 | supplies the tool list |
| `Operator` | 66 | the model that runs it |
| `Enabled` | True | is desk ready to work filter for theLoomApp Schedule.|
| `OnSuccessTo` | 68 | Scene Writer |
| `OnFailTo` | 37 | fail desk |
| `OnPushbackTo` | 9 | stop / dead-end |
| `MaxAttempts` | 5 | generous — distinctness retries can burn attempts (see gotchas) |
| `FilePath` | `WorkGroups\StoryIdeaMaker.json` | desk export target |

---

## What it reads

The SysPrompt directs the operator to `getSummaryById(includeProps: true)` on two items:

- **Id 69 — `WriteStoryIdea` skill card.** The craft spec: how to structure the idea and, more importantly, the standard it's held to. Key demands from that card:
  - **Find the engine.** "A situation is not a story. An engine is a want plus an obstacle that forces a choice." (*Agents run a city* is a setting; *an agent must sign off on its own deprecation order* is an engine.)
  - **Distinct in premise, not just title/setting/character names.**
  - **One idea per turn** — no batching, no shortlist.
  - **The details/body is the artifact** — the name carries nothing downstream, so everything a scene writer needs must be in the body.
  - **Do not write the story** — this is a proposal: no scenes, no prose in the story's voice.
  - Output is a fixed card: Premise / Protagonist / Conflict-Engine / Setting & Tone / Why This Project / Opening Image / Ending Question.
- **Id 103 — the Realm** (Threadspace), with `nodesUp: true` so the operator sees the **existing stories** in that realm — the distinctness check depends on this list being present.

> The realm's theme/tone are treated as **constraints, not suggestions** — per the skill card, "an idea that would fit any project fits none."

## What it produces

- Calls **`addStory`** with `storyId: 103` (the realm) as parent, and **`sceneCount: 3`** to set `TargetSceneCount`.
- The story card lands in the new StoryModel's Description (that's the artifact the whole downstream chain reads).
- Calls **`completeTodo`** passing the new story id as the produced item.

## Routing

- **Success → Scene Writer (68).** The continuation carries the new story forward; 68 writes its scenes.
- **Fail → DeskOfFails 37** Used if infra fails(exceptions in the code). Agents are un aware of it. 
- **Pushback → TheLoomAppSyncDesk 9.** Standard fail/reject the todo option agents have. desk is disabled the standard exit.

---

## Worked instance (real run)

The desk's own todo history shows a completed run: from Realm 103 it produced **Story 430, "Burner Audit"** — `TargetSceneCount: 3`, POV third-person limited — with a close reason noting it was made **distinct from an existing story ("Static Debt")**. That close reason is the distinctness rule doing its job: the operator saw the existing-stories list via `nodesUp` and deliberately avoided overlap. Burner Audit is the story every other doc in this set traces.

---

## Gotchas & notes

- **Distinctness burns attempts.** `MaxAttempts` is 5, higher than most desks, because the skill card tells the operator to *discard and retry* when its first premise overlaps an existing story. On a realm that already has several stories, early attempts can legitimately fail the distinctness bar. Don't lower MaxAttempts here without knowing that.
- **The name is deliberately worthless downstream.** Per the skill card, the title carries nothing — everything must be in the body. If a downstream desk seems to be missing context, the fix is almost always "the idea body was thin," not "pass the title along."
- **`nodesUp: true` on the realm is load-bearing.** It's what surfaces the existing-stories list for the distinctness check. If that read regressed to one level or dropped props, the operator would write blind and start duplicating premises — and it would look like a *creativity* problem, not a *read* problem. Worth remembering when debugging repetitive ideas.
- **`sceneCount: 3` is currently hard-coded in the prompt.** Fine for now; if you want variable-length stories later, that's the line to lift into a realm or request property (same id-direct-routing philosophy as elsewhere).  change as you see fit, it's hard to find.

---

## Desk Prompt

The desk's prompt
```

This is the {{model.desk}} desk. 
Your role is to generate compelling, a unique story idea card that aligns with a project's theme and tone.

Use getSummaryById with includeProps => true, the following items for details.
Id: 69 for notes on how to structure the new story.
Id: 103 is the realm we are writing for, nodesUp => true param adds the list of stories to see what has been done already.

Write one unique story idea and use addStory with Id: 103 as storyId. 
Please use sceneCount 3 to set targetSceneCount to 3 for the story. 

Finally pass the new story Id to completeTodo to mark this todo complete.  

Tools available to you are:
{{for cmd in model.role_commands}}  {{cmd.command}}
{{end}}

```

*Status: first draft, written against live desk 39 + skill card 69. Verify `addStory`'s exact param names and whether `sceneCount` vs `targetSceneCount` matches the tool signature when you do the testing pass.*


