# Scheduler Notes (app machinery — STUB)

*Child doc of StorytimeDescribed. **Deliberately a stub.** The App Scheduler is how theLoomApp picks which todo to run next — app machinery, a different layer from the pipeline architecture the other docs describe. Captured here are only the facts known for certain; anything marked verify-against-code is to be written properly later, from the scheduler source, not from memory.*

---

## Scope boundary

The desk docs describe **how a story becomes prose** through the desk chain. This doc is about **how the app selects and runs todos** — the run loop, not the pipeline. It's separated on purpose so uncertain execution details don't leak into (and muddy) the architecture docs.

---

## Known for certain

### Todo status ladder
A todo moves through 5 states:

**Not Started** → **In Progress** → one of three terminals:
- **Complete Forward**
- **Fail Forward**
- **Aborted Push Back**

Not Started is the initial state; the three terminals are the only ways a todo ends.

> **This ladder is why a desk has exactly three routing pointers** — each terminal maps to one:
> - Complete Forward → `OnSuccessTo`
> - Fail Forward → `OnFailTo`
> - Aborted Push Back → `OnPushbackTo`
>
> (See the map's routing section — this mapping is architectural and lives there too.)

### The Scheduler UI
- Three tabs — **Schedule**, **Ready**, **Results** — all driven by the **same SQL select**.
- All three preview the **system + user prompt** for the todo in question.
- **Schedule** tab is the ordered run-list: the **top ready todo goes next**; the operator runs the list in sequence.
- The Schedule tab has a **harness filter** to switch between harnesses when more than one exists.

### Gating (what puts a todo on the schedule)
- The todo's desk must be **`Enabled`** (disabled desk → its todos never schedule).
- The todo must be **Not Started**.
- The todo's **`isReady` flag** must also agree.
- (And, per the multi-machine model, its operator must be reachable on a live harness.)

### Multi-machine
- Multi-harness support is **baked in but lightly used**.
- A harness relates to the **operator model's host/parent**.
- **No cross-harness desk testing yet.**

---

## KNOWN-OPEN / verify against code before writing this up properly

- **`isReady` vs `completeTodo`.** `isReady` may be a parameter `completeTodo` takes, OR it was refactored so completion sets ready **conditionally** (ready only if everything else is satisfied). Current shape uncertain — read the code.
- **Exact harness ↔ operator binding.** "Operator model's parent/host" is the gist; the precise field/relation that assigns a todo to a harness needs confirming.
- **The shared SQL select** behind the three tabs — worth documenting once, since it defines "ready" operationally.
- **Backoff / lease / claim mechanics** for concurrent harnesses — not built yet; see the concurrency KNOWN-OPEN in the map (§4).

---

*Status: stub. Fill from scheduler source in a later pass. Until then, treat everything outside "Known for certain" as provisional.*
