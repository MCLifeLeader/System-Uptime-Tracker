# Product Scope

## Purpose

System Uptime Tracker is intended to collect machine uptime and lightweight telemetry from Windows and Ubuntu computers, send that data to a central API, and store it in SQL Server for historical reporting and operational analysis.

The system also includes an owner-facing web management portal running on NodeJS. That portal is the primary human interface for managing device accounts, reviewing machine and power-meter records, and interacting with the data collected by the platform.

The system also needs an extensible path for optional power telemetry, starting with Shelly Plug US Gen4 devices, without making power-meter support a prerequisite for computer monitoring.

## Primary Goals

- Monitor Windows computers through a Windows Service.
- Monitor Ubuntu computers through a systemd-managed daemon.
- Receive and persist heartbeat and telemetry data through a .NET web API.
- Provide a NodeJS web management portal for owner-facing administration and operational data access.
- Preserve historical uptime through runtime-session reconstruction, not by treating each heartbeat as an isolated event.
- Support optional, independent registration of Shelly power meters.
- Allow later association between machines, monitored devices, locations, and power meters.

## Non-Goals For The Initial Release

- Remote administration of monitored machines.
- Power-based billing or highly precise device-level power attribution.
- Rich dashboarding, ad hoc analytics, and advanced reporting UI beyond the basic management portal.
- Software inventory, patch management, or process inspection.
- Automatic agent updates.
- Real-time alerting beyond basic health visibility.
- A multi-tenant SaaS platform (isolated organizations, billing, cross-tenant data partitioning). The owner/device-account model described under [Decisions](#decisions) is single-deployment ownership and access control, not a multi-tenant hosting model — the first release supports multiple owner accounts in a single trust domain with no per-owner data isolation (decided, TASK-0002; see [Decisions](#decisions)).

## Product Principles

### Outbound-Only Monitoring

Monitored computers and optional power integrations should send data to the API. The server should not require inbound access to monitored machines.

### Independent First-Class Entities

Machines, power meters, monitored devices, and locations must all be creatable independently. Associations are optional and added later when the real-world relationship exists.

### Accurate Historical Context

Time-aware associations and session modeling are required so the system can answer both current-state and historical questions.

### Normalized Telemetry Ownership

Measured power belongs to the power meter. Measured uptime belongs to the machine heartbeat and runtime-session model. Context is created through associations rather than by duplicating telemetry across related entities.

### Minimal Attack Surface

The ingestion API is the most exposed part of the system and should be hardened accordingly: every non-health endpoint requires authentication, device-facing credentials carry the least privilege needed to submit telemetry (never administrative access), credentials are stored hashed rather than in plaintext, and authentication endpoints are protected against brute-force attempts (lockout, rate limiting). Convenience for constrained devices (see the Basic Auth/API-key fallback under [Decisions](#decisions)) must not come at the cost of these baseline protections.

## Initial Scope Boundary

### In Scope

- ASP.NET Core ingestion API.
- SQL Server persistence model.
- Windows Service packaging with artifact-contained PowerShell install,
  repeat-install, upgrade, rollback, and uninstall paths.
- Ubuntu daemon packaging and systemd installation path.
- Agent identity, heartbeat scheduling, retry queue, and telemetry publishing.
- Machine telemetry including uptime context, OS metadata, CPU, memory, and storage.
- Optional Shelly polling support through the agent.
- Optional independent Shelly registration and future non-agent ingestion paths.
- NodeJS web management portal for owner login, device-account administration, and interaction with collected machine and power data.
- Owner-facing administrative and data-read API endpoints that the portal uses
  for the same workflows.
- Minimum location and monitored-device management needed to associate power meters to real-world equipment when Shelly support is introduced.
- Documentation for design, planning, and implementation sequence.

### Deferred But Supported By The Design

- Dedicated power-meter ingestion service.
- MQTT-based Shelly ingestion.
- Device-level estimated power allocation (see `PowerAllocationRule` in [domain-model.md](./domain-model.md)).
- Administrative workflows for approval and lifecycle management.
- Power-aware state inference across large fleets.
- Broad software inventory beyond the minimum physical device and location context needed for power-meter associations.
- Rich dashboarding and custom reporting beyond the basic management portal.

## Target Users And Operators

- A system owner who needs machine uptime history.
- A system owner or operator who needs a web portal to manage device accounts and inspect collected data.
- An administrator deploying Windows and Ubuntu background services.
- An operator who wants to add power telemetry later without redesigning the system.
- A future maintainer who needs a clean, explicit architecture boundary between agents, API, and data model.

## Success Criteria

- A machine can be monitored with no Shelly configuration present.
- A Shelly power meter can be registered with no machine agent present.
- An owner can sign into the management portal and use it to manage device accounts and review collected telemetry through the same API surface used by devices.
- A reporting machine can optionally be linked to a power meter as dedicated load, shared load, or collector only.
- Runtime sessions can be derived from heartbeat data with reliable gap handling.
- The design supports staged delivery, starting with computer telemetry and adding power telemetry later.
- A Windows operator can install or upgrade the packaged agent by running the
  included PowerShell installer, and a failed startup restores the previous
  working release without deleting durable agent state.

## Assumptions

- The implementation will use .NET and C# for API and agent workloads.
- The owner-facing management portal will run on NodeJS.
- SQL Server is the system of record for production data.
- HTTPS is required for all agent-to-API communication.
- Windows and Ubuntu are the first supported operating systems.
- Power telemetry is optional for the first deployment wave.
- Authentication uses ASP.NET Core Identity with local user accounts and JWT bearer tokens; no external identity provider is in scope for the first release.

## Constraints

- The first usable deployment should stay operationally simple.
- Cross-platform agent logic should be shared where possible.
- Platform-specific behavior should be isolated to hosting, installation, and OS-specific telemetry collection.
- Security controls should be strong enough for unattended service-to-API communication.
- Windows installation requires elevation, but the running service uses an
  explicit least-privilege identity. Credentials are provisioned separately
  and must not be supplied as installer command-line arguments or logged.
- Portal features must consume the shared API contract rather than introducing
  direct database access or a second backend rule set.

## Implementation Approach

The first release should be delivered in paired backend and frontend slices.

- Backend or platform work should lead when new schema, routes, authentication,
  authorization, or host-runtime behavior is required.
- Frontend work should follow closely behind each backend slice so owner-facing
  workflows are validated against the real API and not against temporary mocks.
- Phase intent lives in [implementation-plan.md](./implementation-plan.md), and
  detailed execution order lives in the
  [task dependency tree](./backlog/dependency-tree.md).

## Decisions

- **Windows installation model (decided):** The `win-x64` artifact includes
  `Install-SystemUptimeTrackerWindowsService.ps1` and
  `Uninstall-SystemUptimeTrackerWindowsService.ps1`. The install script uses
  its own directory as the package source and safely handles first install and
  upgrade. The default service name is `SystemUptimeTrackerAgent`, the
  application root is `C:\Program Files\SystemUptimeTracker\Agent`, and durable
  identity, retry, and diagnostic data is stored separately under
  `C:\ProgramData\SystemUptimeTracker\Agent`. Upgrades stage a versioned
  release, configure automatic startup and recovery, validate startup, and
  roll back on failure. Uninstall retains durable data unless an explicit purge
  is requested. This adapts the proven deployment shape in the local
  `C:\Code\Personal\FamilyTools` repository while replacing its positional
  arguments, fixed sleeps, destructive live-directory replacement, and
  implicit identity handling. See
  [windows-service-reference.md](./windows-service-reference.md).
- **Authentication model (decided):** The API authenticates callers through ASP.NET Core Identity local user accounts — no external identity provider. There are two kinds of account:
  - **Owner account**: a human user who administers the deployment. An owner creates and removes device accounts, and decides whether devices share one account or each get their own.
  - **Device account**: the credential a reporting agent (or other telemetry-producing device) uses to call the ingestion API. Every device account is owned by exactly one owner account. An owner may create one device account per machine, or a single shared device account used by many machines — both are supported, and the choice is the owner's, not a fixed system default.
  - Device accounts authenticate through **either** of two schemes, chosen per device account based on what the device can support:
    1. **JWT bearer tokens (primary).** The device is given its account's credentials out-of-band when the Windows Service or systemd daemon is first registered, uses them once to obtain an access token and refresh token, and rotates the access token periodically thereafter — it does not resend the original credential on every call.
    2. **HTTP Basic Auth with a long-lived API key (fallback).** For devices that cannot perform a login/refresh flow — for example, a Shelly Plug US Gen4 driven by a webhook or on-device script — the device account's "password" is a hashed, individually revocable API key rather than a real rotating credential.
  - Both schemes authorize into the same restricted, telemetry-only scope; neither can reach administrative endpoints (association management, location management, device-account management), which require an owner account.
  - The NodeJS management portal uses owner accounts against the same API rather than a separate backend. Device ingestion and human administration share one API surface, separated by route purpose and authorization scope rather than by separate servers.
  - This supersedes the `Authorization: AgentKey ...` scheme shown in [inital-spec.md](./inital-spec.md). See [architecture-overview.md](./architecture-overview.md#authentication-and-authorization) for the design and [domain-model.md](./domain-model.md) for the `DeviceAccount` entity and how a `Machine` links to it.
- **Machine registration model (decided, TASK-0001):** First-release registration
  is **self-service and auto-approved, gated by owner-provisioned credentials**.
  The owner creates a device account and delivers its bootstrap credential to the
  machine out-of-band; any agent that authenticates with a valid, active device
  account may register itself and immediately becomes `Active`. There is no
  approval queue in the first release, so the deferred approval workflow
  (TASK-1507) is **not selected** and the `Discovered` and `PendingApproval`
  lifecycle states remain reserved and unreachable until an approval workflow is
  ever adopted. An owner may optionally pre-create a machine record (with or
  without an assigned device account); when an agent registers with matching
  identity, the registration binds to that record instead of creating a new one.
  Concrete states, transitions, and the actors allowed to perform them are
  documented in [domain-model.md](./domain-model.md#registration-lifecycle).
- **Owner model and data visibility (decided, TASK-0002):** The first release
  supports **multiple owner accounts in a single trust domain with no per-owner
  data isolation**. Every user in the `Owner` role can view and administer every
  device account, machine, power meter, session, and telemetry record in the
  deployment. `DeviceAccount.OwnerUserId` records administrative responsibility
  and audit context (who created or currently stewards the account, reassignable
  by any owner); it is **not** a query filter or visibility boundary.
  Consequences for implementation: owner-facing endpoints authorize on the
  `Owner` role alone, queries do not filter by owner, and no entity needs an
  owner-scoping column beyond the existing `OwnerUserId` audit reference. This
  keeps the explicit non-goal of multi-tenancy intact; introducing per-owner
  isolation later would be a deliberate, breaking authorization redesign.
- **Default device-account policy (decided, TASK-0003):** The default is **one
  dedicated device account per machine**; a shared device account backing many
  machines remains a fully supported owner opt-in. Portal and documentation
  flows should lead the owner toward creating a dedicated account when onboarding
  a machine, because a dedicated account gives per-machine credential revocation
  and the smallest blast radius when a credential is compromised. The owner can
  still create a shared account and point any number of machines at it; nothing
  in the schema or API privileges one mode over the other beyond this first-run
  default.
- **Bootstrap credential lifecycle (decided, TASK-0004):** The device-account
  bootstrap password for JWT-capable devices is **single-use**. Lifecycle:
  - **Issue:** Creating (or rotating credentials on) a device account produces a
    cryptographically random bootstrap password, displayed to the owner exactly
    once and stored only as an Identity password hash.
  - **First use:** The device logs in once with the bootstrap password to obtain
    its first access token and refresh token. On the first successful login the
    bootstrap password is invalidated; it can never authenticate again.
  - **Rotation:** Thereafter the device lives on refresh-token rotation. The
    access token is refreshed proactively before expiry, and each refresh rotates
    the refresh token.
  - **Revocation:** An owner can revoke outstanding refresh tokens or disable the
    device account at any time; the device is locked out no later than the next
    access-token expiry or refresh attempt.
  - **Recovery:** If a device loses its refresh token, or the token expires or is
    revoked, the owner issues a new single-use bootstrap credential (a credential
    rotation, which also revokes all outstanding refresh tokens for that account)
    and delivers it to the device out-of-band, repeating first use.
  - API-key device accounts are intentionally different: the hashed API key is a
    **standing** credential by design (that is the constrained-device fallback),
    individually revocable and rotatable. The single-use rule applies to the JWT
    bootstrap password only.
- **Telemetry timing defaults (decided, TASK-0005):** The following defaults are
  accepted. All values are expressed in seconds in configuration, all comparisons
  use UTC, and server-side values support an optional per-machine override before
  Gate 1 exit.

  | Setting | Default | Valid range | Configured where |
  |---|---:|---|---|
  | Heartbeat interval | 60 s | 15 s – 3600 s | Agent configuration |
  | Offline threshold | 180 s | > 2 × heartbeat interval, ≤ 86400 s | Server (session logic), per-machine override |
  | Session-break threshold | 300 s | ≥ offline threshold, ≤ 86400 s | Server (session logic), per-machine override |
  | Clock-skew tolerance | 300 s | 0 s – 900 s | Server (applied when comparing client `SentAtUtc` to server `ReceivedAtUtc`) |
  | Detailed-telemetry interval | 900 s | ≥ heartbeat interval, ≤ 86400 s | Agent configuration |

  CPU and memory metrics ride on every heartbeat; the detailed-telemetry interval
  governs how often the heavier storage-volume snapshot is included. The offline
  threshold marks a machine as offline in current-status views; the session-break
  threshold is the gap that closes a runtime session (see
  [architecture-overview.md](./architecture-overview.md)).
- **Agent retry queue (decided, TASK-0006):** The [inital-spec.md](./inital-spec.md)
  proposal is adopted as-is: a **SQLite-backed durable queue** under the agent's
  durable data root, with a **7-day maximum age**, a **100 MB maximum size**, and
  a retry progression of **15 s, 30 s, 1 m, 5 m, 15 m** (then steady-state at
  15 m) with jitter applied to every delay. Overflow policy: when either cap is
  exceeded, evict **oldest-first deterministically** and count every eviction in
  an explicit data-loss metric. Poison-message policy: responses classified as
  terminal (validation, authorization, unsupported version) are **never
  retried** — the item is moved to a bounded dead-letter table inside the same
  store with a diagnostic record, and a poison item must never block later
  eligible items. Retryable classifications (network failure, `408`, `429`,
  `5xx`) follow the schedule and honor a valid `Retry-After` header.
- **Power-reading ingestion contract (decided, TASK-0007):** Power readings use a
  **separate endpoint** (`POST /api/v1/power-readings`); they are never embedded
  in heartbeat payloads. There is exactly **one canonical power-reading storage
  command**, and every ingestion path — agent polling now, any future direct or
  broker path — must normalize into it so idempotency (meter identity plus
  `MessageId`) is enforced in one place. Shelly support starts as **agent polling
  only**; direct/MQTT/webhook ingestion is deferred to the EPIC-15 evaluation
  (TASK-1505) and requires no contract change because of the single canonical
  command.
- **Minimum location/monitored-device workflow (decided):** The minimum operator
  workflow for locations and monitored devices is the owner CRUD and
  end-association surface defined by TASK-1307, plus the accessible portal
  workflows for meter registration, locations, monitored devices, and
  association timelines defined by TASK-1309. No additional operator tooling is
  in scope for the first release.

## Open Product Decisions

- None currently open. All previously listed open decisions were resolved on
  2026-08-30 under EPIC-00 (see [Decisions](#decisions) above and the EPIC-00
  task files under [backlog/tasks](./backlog/tasks/)). Record any new open
  product decision here and open a corresponding backlog task.
