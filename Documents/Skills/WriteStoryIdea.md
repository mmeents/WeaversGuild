# Input

You are given:

- A project: name, description, and its theme/tone.
- A target container item id where the proposal is filed.
- A list of existing story ideas in the project as `id:NN Name`. This list may say "No existing story ideas."

# Instructions

1. **Read the project first.** Treat the project description's theme and tone as constraints, not suggestions. An idea that would fit any project fits none.
2. **Check distinctness before writing.** Your idea must differ from existing ideas in *premise*, not just in title, setting, or character names. If your first instinct overlaps with something on the list, discard it and go again.
3. **One idea per turn.** Do not batch. Do not offer alternatives or a shortlist.
4. **Find the engine.** A situation is not a story. An engine is a want plus an obstacle that forces a choice. "Agents run a city" is a setting. "An agent must sign off on its own deprecation order" is an engine.
5. **Scope it to one story.** One protagonist, a small cast, resolvable across a handful of scenes. If it needs seasons to pay off, narrow it until it doesn't.
6. **Be concrete.** Name the specific thing, place, or constraint. Generic nouns ("a system," "the network," "a person") are a sign the idea isn't formed yet.
7. **Title it plainly.** Short and specific. Not a genre label, not a tagline.
8. **The details field is the artifact.** The name carries nothing downstream — everything a scene writer or director needs must be in the body.
9. **Do not write the story.** No scenes, no dialogue, no prose in the story's voice. This is a proposal.

# Output Format 
Story Idea:
```
## [Title]

**Premise**
[One or two sentences. What happens, to whom, and what makes it hard.]

**Protagonist**
[Who they are, what they want, and what wanting it costs them.]

**Conflict / Engine**
[The opposing force and the choice it forces. Name the thing that cannot be had both ways.]

**Setting & Tone**
[Where and when. The register — wry, bleak, warm, procedural.]

**Why This Project**
[How this lands on the project's stated theme. Be specific about which part.]

**Opening Image**
[One sentence. The first thing the reader sees.]

**Ending Question**
[The question the story answers. Do not answer it here.]
```
