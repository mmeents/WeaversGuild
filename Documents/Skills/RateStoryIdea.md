# Input

You are given:

- A todo Id and the Id of a story idea produced against the WriteThreadspaceStoryIdea card (Id 30).
- The review round, if stated in the todo. If it is not stated, this is round 1.

Gather the rest yourself, with getSummaryById and includeProps true:

1. **The story idea.** Read its full content, including the **Draw** section.
2. **The realm.** The story's parent. Its description is the world; its Tone property is the voice.
3. **The draws.** The Draw section lists the draw Ids. Read each draw item and use its properties as the source of truth for what was drawn. The writer's Draw section is a claim; the draw items are the record. If the draw Ids are missing, use the Draw section and note the gap under Hygiene.

# Instructions

You are the reviewer. You did not write this and you will not rewrite it. Your job is to find what is wrong. An idea that reaches you has already passed the writer's own judgment; assume there is something to find.

1. **Write Findings before Scores.** Complete the Findings block first, in full. Then score. Every score must be consistent with the Findings; if a finding shows a problem, the matching score cannot be high.
2. **Judge the idea, not the formatting.** A well-formed proposal with nothing behind it fails. A rough proposal with a real engine passes or gets a change request.
3. **Check every drawn option against its drawn meaning.** A drawn regulator is not a compliance officer. A drawn job interview is not a client call. Near enough is a miss.
4. **Run the delete test for every option.** For each drawn option, ask: if this were removed, would the premise still stand? Record PASS or FAIL per option with a reason.
   - **Membership is not use.** An option used only to say who someone is, where they work, or where the story happens fails the delete test. Use means the option changes what happens.
5. **Run the opposition test for Engine.** Write down what the protagonist wants, what the opposing force is, and what the protagonist actually chooses.
   - If both outcomes of the choice give the protagonist what they want, there is no engine.
   - **Forced choices fail.** If the pressure removes the protagonist's choice (they are ordered to do it, they cannot refuse, there is only one option), Engine scores at most 2.
6. **Obstacles need a cause.** A risk with no stated mechanism ("might trigger an error", "could go wrong") is contrived. Record it in Findings and score Specificity accordingly.
7. **Check terms and focus against the realm.** Each era realm defines its own vocabulary, what does not exist yet, and who the era is about. A term used outside its definition (a foothold is a machine, not market share), a concept imported from a later era, or an idea that leaves out what the era is centered on is a fidelity miss.
8. **One change request, not a list.** On failure, name the single highest-leverage fix. If you find five problems, the writer will fix none of them well. Pick the one that unblocks the rest.
9. **Do not supply the fix.** Say what is wrong and what would resolve it. Do not write a replacement premise, title, protagonist, or ending — that is the fix desk's work, and doing it here launders your opinion into the artifact.
10. **Distinguish fixable from dead.** A weak idea gets revised. An idea that is not about its draw is not fixable by revision and should be rejected outright.

## Calibration

Most first drafts score 3 on most dimensions. A 4 means the dimension is solid with one small weakness you can name. A 5 means you cannot name any way to improve it.

If your total is above 30, re-check every 5 and write down why no improvement is possible. If you cannot, lower it.

## Dimensions

| Dimension | 1 | 5 |
|---|---|---|
| **Draw Adherence** | Ignores the draw, or most options appear only by name | Every option present in its drawn meaning |
| **Integration** | Options sit side by side; any could be deleted | Every option passes the delete test; each causes or complicates another |
| **Engine** | Setting or vibe only, both outcomes satisfy the protagonist, or the choice is forced | Want and opposing force truly opposed; the protagonist makes a real choice that costs something either way |
| **Specificity** | Placeholder nouns throughout; risks with no mechanism | The object, the request, who made it, and how the danger works are all named |
| **Realm Fidelity** | Wrong voice, misused terms, later-era concepts, or the era's focus absent | Tone matches the realm's Tone property; terms used as defined; centered on what the era is about |
| **Scope** | A whole series, or a single scene | Fits one story with a small cast and the target scene count |
| **Hygiene** | Sections missing, placeholders or brackets left in, Draw section absent | All sections present and load-bearing; ending question answerable by a scene, not a rhetorical yes/no |

Integration is capped by the delete test: one option FAIL → at most 3; two or more → at most 2.

## Decision Rule

Apply in order. First match wins.

1. Draw Adherence scores 1, the premise retells an event the realm describes, or the idea belongs to a different era → **REJECT**
2. Any dimension scores 1 → **FAIL**
3. Engine scores 2 or lower → **FAIL**
4. Integration scores 2 or lower → **FAIL**
5. Total below 24 of 35 → **FAIL**
6. Otherwise → **PASS**

**Round limit.** If this is round 3 or later, FAIL is not available. Apply the rule; any result that would be FAIL becomes **PASS** if no dimension scores 1 and Engine scores 3 or higher, and **REJECT** otherwise. Say in the Notes that the round limit applied.

Do not adjust scores to reach a verdict you have already decided on. Findings first, then scores, then read the rule.

## Recording the Verdict

- **PASS** → reviewPass with the full rating as the review notes.
- **FAIL** → reviewFail with the full rating as reviewNotes and the Change Request alone as changeRequest.
- **REJECT** → rejectTodo with the full rating as the reason.

# Output Format

```
## Rating: <idea title>

Review round: N

**Findings**
- Delete test:
  - <dimension>: <option> — PASS | FAIL — <what the option changes in the plot, or why removing it changes nothing>
  - (one line per drawn option)
- Want: <what the protagonist wants>
- Opposing force: <what stands against it>
- Choice: <what the protagonist actually chooses, or "forced" if they have no real choice>
- Unexplained risks: <any danger with no stated mechanism, or "none">
- Era focus: <what this era is about, and whether the idea is about it>

**Scores**
- Draw Adherence: N/5 — <justification>
- Integration: N/5 — <justification, consistent with the delete test>
- Engine: N/5 — <justification, consistent with Want, Opposing force, Choice>
- Specificity: N/5 — <justification>
- Realm Fidelity: N/5 — <justification>
- Scope: N/5 — <justification>
- Hygiene: N/5 — <justification>

**Total** N/35

**Verdict** PASS | FAIL | REJECT

**Notes**
<Two or three sentences. What this idea is doing well, and where it is thin.>

**Change Request**
<FAIL only. One actionable instruction. Omit this section entirely on PASS.
 On REJECT, state which rule applied and why revision will not resolve it.>
```
