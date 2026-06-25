## Design Process Ordering (how a project is decomposed)

The first attempt failed because it fanned out into N independent component designs, each inventing its own database schema in isolation. That produces N locally-coherent, globally-incoherent designs. The correct order is **gather → converge → design**:

### Stage 1 — Field Requirements Gathering (fan-out)
For each functional component, produce ONLY its **data requirements**: what tables, columns, relationships, and indexes that component needs the database to provide. No architecture, no API, no UI yet — just "Dup Detection needs a `PerceptualHash` column and a unique index on it," "ImageProps needs an `ImageTag` table and an `ExifProperty` table," "Compression needs `StorageState`, `OriginalPath`, `CompressedPath`, `CompressionRatio`." Each component is a *contributor* to the schema, not an owner of it.

### Stage 2 — Canonical Data Model (CONVERGENCE — single owner)
ONE design task takes the union of all Stage-1 field requirements and produces the **single canonical EF Core data model** for the whole project: entity classes, relationships, keys, indexes, the `DbContext`. This is the spine. It is designed exactly once. Conflicts between components' field requests are reconciled here, not papered over. The database is NOT "component 1 of 7" — it is the substrate all components write into, so it cannot be designed before its contributors have declared their needs, nor independently by each of them.

### Stage 3 — Component Technical Designs (fan-out, anchored)
NOW each component gets its full technical design (architecture, API, processing flow), and every component **references the canonical data model by name** rather than inventing tables. A component design that defines its own tables instead of referencing the data model is a defect — push it back.

### Stage 4 — API / UI / Implementation Tasks
Derived from Stage 3, still bound by Section 1 (C# libraries, Angular-or-WinForms, EF Core).

The shape is two fan-outs around one convergence. The convergence (Stage 2) is mandatory — skipping it is what broke the first attempt.
