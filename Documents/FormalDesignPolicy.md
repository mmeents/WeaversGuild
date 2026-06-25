# Formal Design Policy

**Status:** Authoritative. Every design, schema, API, and implementation task produced in this org MUST conform to this policy. If a requirement appears to conflict with this policy, raise it as a pushback rather than silently deviating. Do not introduce technologies, services, or patterns not sanctioned here.

---

## 1. Platform Constraints (non-negotiable)

These are hard constraints. A design that violates any of them is wrong by definition, regardless of how well-written it is.

### Persistence
- **Database:** Microsoft SQL Server ONLY. Do not propose PostgreSQL, MySQL, SQLite, Mongo, or "SQL Server or X." SQL Server is the single target.
- **Data access:** Entity Framework Core, code-first. Schema is expressed as EF Core entity classes + a `DbContext`; migrations are EF Core migrations. Do not hand-author DDL as the primary artifact — DDL is generated from the model.
- **No cloud-managed database services.** No Azure SQL-as-a-requirement, no RDS. It must run against a local SQL Server instance (Developer/Express edition is the assumed deployment).

### Storage
- **Image/binary files live on the local filesystem.** No Azure Blob Storage, no AWS S3, no GCS, no object store of any kind. File paths/references are stored in the database; bytes live on disk in a configured root folder.
- Original and derived (compressed/thumbnail) artifacts are both local files tracked by rows in the DB.

### Runtime & Cost
- **Free, local operation is a first-class requirement.** No design may depend on a paid per-call service, a paid API tier, or a subscription to function. If an external service is genuinely needed, it must be optional and degrade gracefully when absent.
- Any AI/ML capability (face detection, scene detection, dup-similarity) must have a local/free implementation path. A paid cloud vision API may be named as an *optional enhancement* only, never as the baseline.
- Assume single-machine, single-household deployment. This is not a multi-tenant SaaS.

### Identity & Security
- **Single-user / single-household local app.** Do NOT design JWT/OAuth2, per-user authorization, tenant isolation, or "users can only see images they own." There is effectively one trusted user on their own machine.
- Security effort goes into: not corrupting data, not losing originals, safe handling of the iCloud intake folder. Not into multi-user access control that does not exist here.
- Sensitive metadata (e.g. GPS) stays local; "encryption at rest" is out of scope unless the user explicitly asks.

### Delivery / UI
- **API surface:** C#-oriented API libraries. A class-library boundary the UI consumes; a thin ASP.NET Core API only if a browser UI requires it.
- **UI is one of:** Angular running in a browser (served by a local ASP.NET Core host), OR a Windows Forms desktop app. Pick one per project and state which. Default to WinForms unless browser reach is a stated requirement.
- No React/Vue/Blazor/MAUI unless explicitly requested.

---

