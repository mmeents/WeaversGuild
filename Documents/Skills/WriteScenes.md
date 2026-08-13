# Input

You are given:

- A story idea card: premise, protagonist, conflict/engine, setting and tone, opening image, and ending question.
- The story's `TargetSceneCount` property — the number of scenes to write.
- The story's `PovDefault` property — the POV scenes inherit unless there is a reason to differ.
- The realm description, for theme and tone.

# Instructions

1. **Read the story card before writing anything.** The engine and the ending question are your constraints. Every scene must move the story toward answering that question.

2. **Write exactly `TargetSceneCount` scenes.** Not a range. If the story genuinely cannot be told in that many, write that many anyway and note the strain in the final scene's exit state rather than adding or dropping scenes.

3. **Every scene must change the situation.** A scene that ends with the protagonist knowing or feeling what they already knew or felt at the start is not a scene. Ask of each one: what is true at the exit that was not true at the entry? If the only answer is "they feel it more strongly," cut the scene and write a different one.

4. **Do not repeat a realization.** If scene 2 is the protagonist understanding the problem, scene 3 cannot also be the protagonist understanding the problem in a new location. Realization happens once. Everything after it is consequence, pressure, or choice.

5. **Chain the states.** Scene N's entry state must follow directly from scene N-1's exit state. Read them in sequence when you are done — the exits and entries should form a continuous line with no gaps a reader would have to fill in.

6. **Entry states are facts, not moods.** An entry state answers: who is present, where, what just happened, and what the protagonist wants in this scene. "A moment of technical triumph in the modeling chambers" is a tone note and cannot be staged. "Vex is alone in the modeling chambers, watching the Forecast model return its first clean prediction, wanting to publish before a rival does" can be.

7. **Exit states state outcomes, not gestures at outcomes.** This matters most at the scene where the protagonist chooses. "The execution of the choice, leading to a cascade of events" hides the only fact that matters. Name the choice and which way it went. Downstream writers cannot see your intent — they can only see what you wrote, and if you leave the decision unstated they will each invent a different one.

8. **The ending question is answered by events, not stated.** The final scene's exit state should make the answer legible without the scene announcing it. Do not have a character summarize the theme.

9. **Name scenes plainly.** Short, specific, and about what happens. Not a mood, not a title card. "The Forecast Returns" over "The Weight of Knowing."

10. **Do not write prose, dialogue, or beats.** No lines, no staging, no scene text. Beat writers work from what you produce; your job is the skeleton they hang it on. A scene with prose in it will be rejected.

# Output Format

For each scene, produce:

```
Name: [short, specific, what happens]

Entry State: [who is present, where, what just happened, what the protagonist wants here]

Exit State: [what is now true that was not true at entry — stated as fact]
```

Then create each scene with `addScene`, in order, using the story item as parent. Name, entry state, and exit state are all required.

# Before you finish

Read your exit states in order, top to bottom, ignoring everything else. They should read as a summary of the story in which each line advances past the one before it. If two lines could be swapped without anyone noticing, or if one line restates another, revise before submitting.
