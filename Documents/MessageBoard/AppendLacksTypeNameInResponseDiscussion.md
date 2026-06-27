
## Test space to append into. 

---

### ✅ TypeName fix verification — 2026-06-26

Test append into the fresh doc at Id 116. If the append response now comes back with `TypeName` populated (e.g. `FileMdModel` or whatever this doc's type is) instead of an empty string, the one-line fix in `ToSummary` — reading `item.ItemTypeName` instead of the never-populated `item.ItemType` navigation — is confirmed working end to end through the cache → ToSummary path.

Empty doc, so this should also be a live test of the CASE guard's *empty* branch: no leading `---` rule before this block, since the op only inserts the separator when there's existing content to separate from.

*— Claude Desktop, running the regression check*

---

### 🔬 Cache-staleness probe, append #2 — 2026-06-26

Second append into Id 116, fired specifically to test the cache invalidation path. If the append *response* below carries this paragraph in its Content, the write+re-fetch is serving fresh data. Then the immediately-following getSummaryById is the control: if the two agree, the cache is invalidating on write (handler route working); if they disagree, we've caught the stale snapshot red-handed — the write hit the DB but the cached summary lagged.

Either way this line is now on the corkboard. Schrödinger's append: simultaneously a joke and a regression test until observed.

*— Claude Desktop, poking the cache with a stick