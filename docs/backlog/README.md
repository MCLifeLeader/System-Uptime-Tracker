# Delivery Backlog

## Purpose

This directory is the canonical task-level execution tree for System Uptime
Tracker. Every epic and task has its own file so implementation, review, and
completion evidence can be managed without editing one monolithic document.

Use [the delivery overview](../delivery-backlog.md) for the full program graph,
critical path, release gates, definition of done, and traceability matrix.

The split files are working records. `Split-DeliveryBacklog.ps1` refuses to
overwrite them by default because regeneration resets task status and
completion evidence. Use its `-Force` switch only when replacing that data is
intentional.

## Navigation

- [Epic files](./epics/)
- [Task files](./tasks/)
- [Task dependency execution tree](./dependency-tree.md)
- [Implementation plan](../implementation-plan.md)
- [Architecture overview](../architecture-overview.md)
- [Domain model](../domain-model.md)

## Dependency Rules

- Start a task only after every linked prerequisite task is complete.
- Task-file `depends_on` metadata controls task start order. Epic dependencies control epic completion and release-gate readiness.
- Tasks with satisfied predecessors may run in parallel, including tasks from
  different epics.
- Complete the acceptance evidence in the task file before marking it done.
- Keep IDs and file names stable; add IDs rather than renumbering existing
  work.
- Update the epic file and this index whenever a task is added, removed, or
  assigned a different predecessor.

## Epics

| Epic | Title | Completion depends on | Release gate | Tasks |
|---|---|---|---|---:|
| [EPIC-00](./epics/EPIC-00.md) | Decisions And Acceptance Baseline | None | Gate 0 | 8 |
| [EPIC-01](./epics/EPIC-01.md) | Solution And Engineering Foundation | EPIC-00 | Gate 0 | 8 |
| [EPIC-02](./epics/EPIC-02.md) | Versioned Contracts | EPIC-00 | Gate 0 | 9 |
| [EPIC-03](./epics/EPIC-03.md) | Telemetry Persistence | EPIC-01, EPIC-02 | Gate 1 | 8 |
| [EPIC-04](./epics/EPIC-04.md) | Identity And Authorization | EPIC-01, EPIC-02 | Gate 1 | 10 |
| [EPIC-05](./epics/EPIC-05.md) | Machine Registration And Heartbeat Ingestion | EPIC-03, EPIC-04 | Gate 1 | 8 |
| [EPIC-06](./epics/EPIC-06.md) | Runtime-Session Reconstruction | EPIC-05 | Gate 1 | 8 |
| [EPIC-07](./epics/EPIC-07.md) | Shared Agent Runtime | EPIC-05 | Gate 1 | 9 |
| [EPIC-08](./epics/EPIC-08.md) | Offline Queue And Resilient Delivery | EPIC-07 | Gate 1 | 8 |
| [EPIC-09](./epics/EPIC-09.md) | Windows Service Delivery | EPIC-07, EPIC-08 | Gate 2 | 9 |
| [EPIC-10](./epics/EPIC-10.md) | Ubuntu Systemd Delivery | EPIC-07, EPIC-08 | Gate 2 | 7 |
| [EPIC-11](./epics/EPIC-11.md) | Owner Portal MVP | EPIC-04, EPIC-05 | Gate 1 | 9 |
| [EPIC-12](./epics/EPIC-12.md) | Power Telemetry Foundation | EPIC-02, EPIC-03, EPIC-04 | Gate 3 | 8 |
| [EPIC-13](./epics/EPIC-13.md) | Shelly And Association Workflows | EPIC-05, EPIC-07, EPIC-12 | Gate 3 | 10 |
| [EPIC-14](./epics/EPIC-14.md) | Operations And Release Readiness | EPIC-06, EPIC-09, EPIC-10, EPIC-11, EPIC-13 | Gate 3 | 9 |
| [EPIC-15](./epics/EPIC-15.md) | Reporting And Future Ingestion | EPIC-06, EPIC-11, EPIC-13 | Gate 4 | 9 |

## Tasks

| Task | Epic | Objective | Depends on |
|---|---|---|---|
| [TASK-0001](./tasks/TASK-0001.md) | [EPIC-00](./epics/EPIC-00.md) | Decide whether first-release registration is pre-provisioned, self-service, or approval-based | None |
| [TASK-0002](./tasks/TASK-0002.md) | [EPIC-00](./epics/EPIC-00.md) | Decide whether the deployment supports one owner or multiple owners and whether data is isolated by owner | None |
| [TASK-0003](./tasks/TASK-0003.md) | [EPIC-00](./epics/EPIC-00.md) | Select the default device-account policy: shared account or one account per machine, while retaining both supported modes | None |
| [TASK-0004](./tasks/TASK-0004.md) | [EPIC-00](./epics/EPIC-00.md) | Decide whether the bootstrap password is single-use or a standing fallback after the first refresh token is issued | None |
| [TASK-0005](./tasks/TASK-0005.md) | [EPIC-00](./epics/EPIC-00.md) | Accept configurable defaults for heartbeat interval, offline threshold, session-break threshold, clock-skew tolerance, and detailed-telemetry interval | None |
| [TASK-0006](./tasks/TASK-0006.md) | [EPIC-00](./epics/EPIC-00.md) | Accept the retry store technology, 7-day age cap, 100 MB size cap, retry schedule, overflow policy, and poison-message policy | None |
| [TASK-0007](./tasks/TASK-0007.md) | [EPIC-00](./epics/EPIC-00.md) | Decide whether power readings use a separate endpoint, a combined heartbeat payload, or both | None |
| [TASK-0008](./tasks/TASK-0008.md) | [EPIC-00](./epics/EPIC-00.md) | Define Gate 1, Gate 2, Gate 3, and Gate 4 release evidence, including required automated suites and target environments | None |
| [TASK-0101](./tasks/TASK-0101.md) | [EPIC-01](./epics/EPIC-01.md) | Inventory the current API, web, data, common, AppHost, test, and QA projects | [TASK-0008](./tasks/TASK-0008.md) |
| [TASK-0102](./tasks/TASK-0102.md) | [EPIC-01](./epics/EPIC-01.md) | Decide whether `SystemUptimeTracker.Common` becomes the contracts library or whether to add `SystemUptimeTracker.Contracts` | [TASK-0101](./tasks/TASK-0101.md) |
| [TASK-0103](./tasks/TASK-0103.md) | [EPIC-01](./epics/EPIC-01.md) | Add planned `Agent.Core`, `WindowsService`, `LinuxDaemon`, and `Power.Shelly` projects to the solution only as their first behavior is implemented | [TASK-0101](./tasks/TASK-0101.md) |
| [TASK-0104](./tasks/TASK-0104.md) | [EPIC-01](./epics/EPIC-01.md) | Preserve the existing .NET 10, Next.js 16, React 19, Node 24, NUnit, Vitest, and Playwright toolchain unless a separate upgrade is approved | [TASK-0101](./tasks/TASK-0101.md) |
| [TASK-0105](./tasks/TASK-0105.md) | [EPIC-01](./epics/EPIC-01.md) | Establish the baseline commands for restore, build, .NET test, web lint/test/build, and QA smoke execution in contributor documentation | [TASK-0104](./tasks/TASK-0104.md) |
| [TASK-0106](./tasks/TASK-0106.md) | [EPIC-01](./epics/EPIC-01.md) | Configure CI jobs to run independent .NET, web, contract, migration, and packaging validations with dependency caching | [TASK-0105](./tasks/TASK-0105.md) |
| [TASK-0107](./tasks/TASK-0107.md) | [EPIC-01](./epics/EPIC-01.md) | Define configuration precedence and environment naming across API, web, agents, and AppHost | [TASK-0101](./tasks/TASK-0101.md) |
| [TASK-0108](./tasks/TASK-0108.md) | [EPIC-01](./epics/EPIC-01.md) | Define test ownership: unit for pure rules, integration for SQL/API boundaries, functional for workflows, and packaging tests for installed services | [TASK-0105](./tasks/TASK-0105.md) |
| [TASK-0201](./tasks/TASK-0201.md) | [EPIC-02](./epics/EPIC-02.md) | Create `docs/api-contracts.md` with the complete `/api/v1` route catalog, caller type, authorization policy, status codes, and idempotency behavior | [TASK-0001](./tasks/TASK-0001.md), [TASK-0002](./tasks/TASK-0002.md), [TASK-0003](./tasks/TASK-0003.md), [TASK-0004](./tasks/TASK-0004.md) |
| [TASK-0202](./tasks/TASK-0202.md) | [EPIC-02](./epics/EPIC-02.md) | Define machine registration request/response DTOs, durable `AgentId`, registration status, assigned `MachineId`, and conflict behavior | [TASK-0102](./tasks/TASK-0102.md) |
| [TASK-0203](./tasks/TASK-0203.md) | [EPIC-02](./epics/EPIC-02.md) | Define heartbeat DTOs for machine metadata, sequence number, sent time, agent start, boot time, CPU, memory, and storage | [TASK-0005](./tasks/TASK-0005.md), [TASK-0102](./tasks/TASK-0102.md) |
| [TASK-0204](./tasks/TASK-0204.md) | [EPIC-02](./epics/EPIC-02.md) | Define owner login, device login, refresh, revoke, and API-key issue/rotate responses without exposing stored secrets | [TASK-0004](./tasks/TASK-0004.md), [TASK-0102](./tasks/TASK-0102.md) |
| [TASK-0205](./tasks/TASK-0205.md) | [EPIC-02](./epics/EPIC-02.md) | Define owner read and administration contracts for device accounts, machines, sessions, and telemetry with bounded pagination and filtering | [TASK-0002](./tasks/TASK-0002.md), [TASK-0102](./tasks/TASK-0102.md) |
| [TASK-0206](./tasks/TASK-0206.md) | [EPIC-02](./epics/EPIC-02.md) | Define power-meter registration, power reading, location, monitored-device, and effective-dated association contracts | [TASK-0007](./tasks/TASK-0007.md), [TASK-0102](./tasks/TASK-0102.md) |
| [TASK-0207](./tasks/TASK-0207.md) | [EPIC-02](./epics/EPIC-02.md) | Define idempotency keys: `AgentId + SequenceNumber` for heartbeats and meter identity plus `MessageId` for readings | [TASK-0202](./tasks/TASK-0202.md), [TASK-0203](./tasks/TASK-0203.md), [TASK-0206](./tasks/TASK-0206.md) |
| [TASK-0208](./tasks/TASK-0208.md) | [EPIC-02](./epics/EPIC-02.md) | Standardize validation errors on Problem Details, correlation headers, UTC timestamp format, numeric units, and unsupported payload-version responses | [TASK-0201](./tasks/TASK-0201.md) |
| [TASK-0209](./tasks/TASK-0209.md) | [EPIC-02](./epics/EPIC-02.md) | Generate or maintain the API OpenAPI document, executable HTTP examples, and portal-consumable typed or Zod validators for the accepted v1 surface | [TASK-0202](./tasks/TASK-0202.md), [TASK-0203](./tasks/TASK-0203.md), [TASK-0204](./tasks/TASK-0204.md), [TASK-0205](./tasks/TASK-0205.md), [TASK-0206](./tasks/TASK-0206.md), [TASK-0207](./tasks/TASK-0207.md), [TASK-0208](./tasks/TASK-0208.md) |
| [TASK-0301](./tasks/TASK-0301.md) | [EPIC-03](./epics/EPIC-03.md) | Define the telemetry `DbContext` ownership and migration strategy alongside the existing Identity context | [TASK-0107](./tasks/TASK-0107.md), [TASK-0202](./tasks/TASK-0202.md), [TASK-0203](./tasks/TASK-0203.md) |
| [TASK-0302](./tasks/TASK-0302.md) | [EPIC-03](./epics/EPIC-03.md) | Implement `Machine`, `Heartbeat`, `StorageTelemetry`, and `RuntimeSession` entities with audit fields and domain-model nullability | [TASK-0301](./tasks/TASK-0301.md) |
| [TASK-0303](./tasks/TASK-0303.md) | [EPIC-03](./epics/EPIC-03.md) | Add unique indexes for populated `AgentId` and heartbeat idempotency, plus indexes for machine/time and session queries | [TASK-0302](./tasks/TASK-0302.md) |
| [TASK-0304](./tasks/TASK-0304.md) | [EPIC-03](./epics/EPIC-03.md) | Store server-authoritative `ReceivedAtUtc` | [TASK-0302](./tasks/TASK-0302.md) |
| [TASK-0305](./tasks/TASK-0305.md) | [EPIC-03](./epics/EPIC-03.md) | Configure storage telemetry as heartbeat-owned history with explicit delete behavior and no accidental cascade from account deletion | [TASK-0302](./tasks/TASK-0302.md) |
| [TASK-0306](./tasks/TASK-0306.md) | [EPIC-03](./epics/EPIC-03.md) | Create and review the initial telemetry migration and SQL script for least-privilege deployment | [TASK-0303](./tasks/TASK-0303.md), [TASK-0304](./tasks/TASK-0304.md), [TASK-0305](./tasks/TASK-0305.md) |
| [TASK-0307](./tasks/TASK-0307.md) | [EPIC-03](./epics/EPIC-03.md) | Add migration rollback/reapply and model-snapshot drift tests to CI | [TASK-0306](./tasks/TASK-0306.md) |
| [TASK-0308](./tasks/TASK-0308.md) | [EPIC-03](./epics/EPIC-03.md) | Define retention boundaries for raw heartbeats, storage telemetry, sessions, and later power readings without implementing destructive defaults | [TASK-0306](./tasks/TASK-0306.md) |
| [TASK-0401](./tasks/TASK-0401.md) | [EPIC-04](./epics/EPIC-04.md) | Map existing `Admin`, `Manager`, `Contributor`, and `Read` roles to the decided `Owner` and telemetry-only device policies | [TASK-0002](./tasks/TASK-0002.md), [TASK-0101](./tasks/TASK-0101.md), [TASK-0204](./tasks/TASK-0204.md) |
| [TASK-0402](./tasks/TASK-0402.md) | [EPIC-04](./epics/EPIC-04.md) | Implement `DeviceAccount` as a domain companion to `ApplicationUser`, including owner, allowed methods, API-key metadata, active state, and audit fields | [TASK-0301](./tasks/TASK-0301.md), [TASK-0401](./tasks/TASK-0401.md) |
| [TASK-0403](./tasks/TASK-0403.md) | [EPIC-04](./epics/EPIC-04.md) | Implement owner-authorized create, list, update, disable, delete/reassign, and credential-rotation services with ownership checks | [TASK-0402](./tasks/TASK-0402.md) |
| [TASK-0404](./tasks/TASK-0404.md) | [EPIC-04](./epics/EPIC-04.md) | Implement device credential exchange and refresh-token rotation with configured access/refresh lifetimes and revocation | [TASK-0204](./tasks/TASK-0204.md), [TASK-0402](./tasks/TASK-0402.md) |
| [TASK-0405](./tasks/TASK-0405.md) | [EPIC-04](./epics/EPIC-04.md) | Implement cryptographically random API-key issuance, salted hashing, constant-time verification, one-time display, rotation, and revocation | [TASK-0204](./tasks/TASK-0204.md), [TASK-0402](./tasks/TASK-0402.md) |
| [TASK-0406](./tasks/TASK-0406.md) | [EPIC-04](./epics/EPIC-04.md) | Build device claims from server-side account and machine authorization data | [TASK-0404](./tasks/TASK-0404.md), [TASK-0405](./tasks/TASK-0405.md) |
| [TASK-0407](./tasks/TASK-0407.md) | [EPIC-04](./epics/EPIC-04.md) | Apply lockout and partitioned rate limits to password, token, refresh, and Basic Auth entry points without blocking health probes | [TASK-0404](./tasks/TASK-0404.md), [TASK-0405](./tasks/TASK-0405.md) |
| [TASK-0408](./tasks/TASK-0408.md) | [EPIC-04](./epics/EPIC-04.md) | Require authentication on every non-health route and explicit owner/device policies by route group | [TASK-0401](./tasks/TASK-0401.md), [TASK-0406](./tasks/TASK-0406.md) |
| [TASK-0409](./tasks/TASK-0409.md) | [EPIC-04](./epics/EPIC-04.md) | Audit logs for account creation, disablement, key issue/rotation/revocation, failed authentication, and denied authorization using identifiers rather than secrets | [TASK-0403](./tasks/TASK-0403.md), [TASK-0408](./tasks/TASK-0408.md) |
| [TASK-0410](./tasks/TASK-0410.md) | [EPIC-04](./epics/EPIC-04.md) | Implement a one-time first-owner bootstrap path using deployment-supplied secret material, explicit startup validation, and automatic closure after an owner exists | [TASK-0301](./tasks/TASK-0301.md), [TASK-0401](./tasks/TASK-0401.md) |
| [TASK-0501](./tasks/TASK-0501.md) | [EPIC-05](./epics/EPIC-05.md) | Implement the selected machine registration workflow, including pre-created records, durable `AgentId`, status transitions, and device-account assignment | [TASK-0202](./tasks/TASK-0202.md), [TASK-0306](./tasks/TASK-0306.md), [TASK-0406](./tasks/TASK-0406.md) |
| [TASK-0502](./tasks/TASK-0502.md) | [EPIC-05](./epics/EPIC-05.md) | Implement `POST /api/v1/heartbeats` with payload-size limits, version validation, authenticated machine scope, and server receipt time | [TASK-0203](./tasks/TASK-0203.md), [TASK-0306](./tasks/TASK-0306.md), [TASK-0408](./tasks/TASK-0408.md) |
| [TASK-0503](./tasks/TASK-0503.md) | [EPIC-05](./epics/EPIC-05.md) | Make heartbeat processing atomic and idempotent under sequential and concurrent duplicate delivery | [TASK-0207](./tasks/TASK-0207.md), [TASK-0502](./tasks/TASK-0502.md) |
| [TASK-0504](./tasks/TASK-0504.md) | [EPIC-05](./epics/EPIC-05.md) | Normalize OS, architecture, machine name, agent version, CPU, memory, and storage values without silently coercing invalid data | [TASK-0502](./tasks/TASK-0502.md) |
| [TASK-0505](./tasks/TASK-0505.md) | [EPIC-05](./epics/EPIC-05.md) | Update machine `FirstSeenAtUtc`, `LastSeenAtUtc`, metadata, and registration state using server-authoritative rules | [TASK-0502](./tasks/TASK-0502.md) |
| [TASK-0506](./tasks/TASK-0506.md) | [EPIC-05](./epics/EPIC-05.md) | Attach or generate a correlation identifier and emit structured ingestion logs, success/failure metrics, and health diagnostics without machine secrets or raw credentials | [TASK-0502](./tasks/TASK-0502.md) |
| [TASK-0507](./tasks/TASK-0507.md) | [EPIC-05](./epics/EPIC-05.md) | Add SQL Server integration tests for registration through heartbeat persistence, including retry and out-of-order delivery | [TASK-0503](./tasks/TASK-0503.md), [TASK-0504](./tasks/TASK-0504.md), [TASK-0505](./tasks/TASK-0505.md) |
| [TASK-0508](./tasks/TASK-0508.md) | [EPIC-05](./epics/EPIC-05.md) | Add an end-to-end smoke client that registers a machine, obtains authorization, posts a heartbeat, and reads it as an owner | [TASK-0507](./tasks/TASK-0507.md) |
| [TASK-0601](./tasks/TASK-0601.md) | [EPIC-06](./epics/EPIC-06.md) | Specify the session state machine for first heartbeat, continuation, timeout, reboot, agent restart, suspend/resume, graceful stop, and out-of-order receipt | [TASK-0005](./tasks/TASK-0005.md), [TASK-0503](./tasks/TASK-0503.md) |
| [TASK-0602](./tasks/TASK-0602.md) | [EPIC-06](./epics/EPIC-06.md) | Implement the pure session-transition calculator using UTC instants and injected thresholds | [TASK-0601](./tasks/TASK-0601.md) |
| [TASK-0603](./tasks/TASK-0603.md) | [EPIC-06](./epics/EPIC-06.md) | Integrate session updates in the heartbeat transaction or an idempotent post-ingestion processor with explicit concurrency control | [TASK-0602](./tasks/TASK-0602.md), [TASK-0503](./tasks/TASK-0503.md) |
| [TASK-0604](./tasks/TASK-0604.md) | [EPIC-06](./epics/EPIC-06.md) | Implement timeout closure using a scheduled, restart-safe process and an injectable server clock | [TASK-0603](./tasks/TASK-0603.md) |
| [TASK-0605](./tasks/TASK-0605.md) | [EPIC-06](./epics/EPIC-06.md) | Handle delayed queue uploads by preserving event order and avoiding false uptime across gaps | [TASK-0603](./tasks/TASK-0603.md) |
| [TASK-0606](./tasks/TASK-0606.md) | [EPIC-06](./epics/EPIC-06.md) | Calculate `HeartbeatCount`, `LastHeartbeatAtUtc`, `EndedAtUtc`, and uptime duration with documented boundary semantics | [TASK-0603](./tasks/TASK-0603.md) |
| [TASK-0607](./tasks/TASK-0607.md) | [EPIC-06](./epics/EPIC-06.md) | Add SQL Server integration tests for reboot, restart, timeout, duplicate, concurrent, delayed, and clock-skew scenarios | [TASK-0604](./tasks/TASK-0604.md), [TASK-0605](./tasks/TASK-0605.md), [TASK-0606](./tasks/TASK-0606.md) |
| [TASK-0608](./tasks/TASK-0608.md) | [EPIC-06](./epics/EPIC-06.md) | Expose owner-authorized current and historical session queries with pagination and deterministic ordering | [TASK-0607](./tasks/TASK-0607.md) |
| [TASK-0701](./tasks/TASK-0701.md) | [EPIC-07](./epics/EPIC-07.md) | Create `SystemUptimeTracker.Agent.Core` and its unit-test project with no Windows Service or systemd hosting dependency | [TASK-0103](./tasks/TASK-0103.md), [TASK-0202](./tasks/TASK-0202.md), [TASK-0203](./tasks/TASK-0203.md) |
| [TASK-0702](./tasks/TASK-0702.md) | [EPIC-07](./epics/EPIC-07.md) | Define the durable local identity-state boundary and implement atomic first-run `AgentId` creation and load with corrupt-file handling and an OS-supplied durable-state path | [TASK-0701](./tasks/TASK-0701.md) |
| [TASK-0703](./tasks/TASK-0703.md) | [EPIC-07](./epics/EPIC-07.md) | Define platform telemetry provider interfaces and normalized snapshots for OS, architecture, boot identity/time, CPU, memory, and storage | [TASK-0701](./tasks/TASK-0701.md) |
| [TASK-0704](./tasks/TASK-0704.md) | [EPIC-07](./epics/EPIC-07.md) | Implement Windows telemetry collection using least-privilege supported APIs and cancellation-aware asynchronous I/O | [TASK-0703](./tasks/TASK-0703.md) |
| [TASK-0705](./tasks/TASK-0705.md) | [EPIC-07](./epics/EPIC-07.md) | Implement Ubuntu telemetry collection from stable OS interfaces with bounded reads and explicit parsing failures | [TASK-0703](./tasks/TASK-0703.md) |
| [TASK-0706](./tasks/TASK-0706.md) | [EPIC-07](./epics/EPIC-07.md) | Implement bootstrap login, durable protected token and refresh-metadata storage, proactive access-token refresh, and disabled/revoked response handling | [TASK-0702](./tasks/TASK-0702.md), [TASK-0404](./tasks/TASK-0404.md) |
| [TASK-0707](./tasks/TASK-0707.md) | [EPIC-07](./epics/EPIC-07.md) | Implement the cancellation-aware worker loop with configurable interval, monotonic sequence numbers, and non-overlapping collection cycles | [TASK-0702](./tasks/TASK-0702.md), [TASK-0703](./tasks/TASK-0703.md), [TASK-0706](./tasks/TASK-0706.md) |
| [TASK-0708](./tasks/TASK-0708.md) | [EPIC-07](./epics/EPIC-07.md) | Implement the typed HTTPS publishing client with bounded timeouts, contract version header, correlation ID, and response classification | [TASK-0502](./tasks/TASK-0502.md), [TASK-0707](./tasks/TASK-0707.md) |
| [TASK-0709](./tasks/TASK-0709.md) | [EPIC-07](./epics/EPIC-07.md) | Add lifecycle signals for agent start and graceful stop while leaving runtime sessions server-authoritative | [TASK-0707](./tasks/TASK-0707.md), [TASK-0708](./tasks/TASK-0708.md) |
| [TASK-0801](./tasks/TASK-0801.md) | [EPIC-08](./epics/EPIC-08.md) | Define a durable queue interface and record envelope containing original event time, sequence/idempotency key, payload version, attempt count, and next attempt time | [TASK-0006](./tasks/TASK-0006.md), [TASK-0701](./tasks/TASK-0701.md) |
| [TASK-0802](./tasks/TASK-0802.md) | [EPIC-08](./epics/EPIC-08.md) | Implement the selected local queue under the durable data root with single-process locking, crash-safe writes, and restrictive file permissions | [TASK-0801](./tasks/TASK-0801.md) |
| [TASK-0803](./tasks/TASK-0803.md) | [EPIC-08](./epics/EPIC-08.md) | Enforce age and size caps with a deterministic oldest-first eviction policy and explicit data-loss metrics | [TASK-0802](./tasks/TASK-0802.md) |
| [TASK-0804](./tasks/TASK-0804.md) | [EPIC-08](./epics/EPIC-08.md) | Classify retryable network/`408`/`429`/`5xx` failures separately from terminal validation/authorization failures | [TASK-0708](./tasks/TASK-0708.md), [TASK-0802](./tasks/TASK-0802.md) |
| [TASK-0805](./tasks/TASK-0805.md) | [EPIC-08](./epics/EPIC-08.md) | Implement jittered bounded backoff, honor valid `Retry-After`, and prevent a poison item from blocking later eligible items | [TASK-0803](./tasks/TASK-0803.md), [TASK-0804](./tasks/TASK-0804.md) |
| [TASK-0806](./tasks/TASK-0806.md) | [EPIC-08](./epics/EPIC-08.md) | Drain queued events in original per-agent sequence before or alongside live events without starving current collection | [TASK-0707](./tasks/TASK-0707.md), [TASK-0805](./tasks/TASK-0805.md) |
| [TASK-0807](./tasks/TASK-0807.md) | [EPIC-08](./epics/EPIC-08.md) | Flush in-flight queue state on cancellation within a bounded shutdown period | [TASK-0806](./tasks/TASK-0806.md) |
| [TASK-0808](./tasks/TASK-0808.md) | [EPIC-08](./epics/EPIC-08.md) | Emit queue depth, oldest age, retry count, eviction count, and last successful publish telemetry | [TASK-0803](./tasks/TASK-0803.md), [TASK-0805](./tasks/TASK-0805.md), [TASK-0806](./tasks/TASK-0806.md) |
| [TASK-0901](./tasks/TASK-0901.md) | [EPIC-09](./epics/EPIC-09.md) | Create the thin `SystemUptimeTracker.WindowsService` host and configure `AddWindowsService` with service name `SystemUptimeTrackerAgent` | [TASK-0709](./tasks/TASK-0709.md), [TASK-0807](./tasks/TASK-0807.md) |
| [TASK-0902](./tasks/TASK-0902.md) | [EPIC-09](./epics/EPIC-09.md) | Configure self-contained single-file `win-x64` publishing without trimming and include non-secret configuration plus operator documentation | [TASK-0901](./tasks/TASK-0901.md) |
| [TASK-0903](./tasks/TASK-0903.md) | [EPIC-09](./epics/EPIC-09.md) | Implement `Install-SystemUptimeTrackerWindowsService.ps1` as an advanced, elevation-checked, parameter-validated, `SupportsShouldProcess` script using its own artifact directory | [TASK-0902](./tasks/TASK-0902.md) |
| [TASK-0904](./tasks/TASK-0904.md) | [EPIC-09](./epics/EPIC-09.md) | Stage immutable versioned releases under `Program Files`, keep durable state under `ProgramData`, and prevent source/application/data path overlap | [TASK-0903](./tasks/TASK-0903.md) |
| [TASK-0905](./tasks/TASK-0905.md) | [EPIC-09](./epics/EPIC-09.md) | Create or update the service with LocalService by default, automatic startup, description, restart-on-failure actions, and least-privilege ACLs | [TASK-0904](./tasks/TASK-0904.md) |
| [TASK-0906](./tasks/TASK-0906.md) | [EPIC-09](./epics/EPIC-09.md) | Replace fixed waits with bounded state polling | [TASK-0905](./tasks/TASK-0905.md) |
| [TASK-0907](./tasks/TASK-0907.md) | [EPIC-09](./epics/EPIC-09.md) | Preserve the prior binary path and release until startup validation succeeds | [TASK-0906](./tasks/TASK-0906.md) |
| [TASK-0908](./tasks/TASK-0908.md) | [EPIC-09](./epics/EPIC-09.md) | Implement uninstall that removes service registration and releases while retaining `ProgramData` unless an explicit confirmed purge is requested | [TASK-0904](./tasks/TASK-0904.md), [TASK-0906](./tasks/TASK-0906.md) |
| [TASK-0909](./tasks/TASK-0909.md) | [EPIC-09](./epics/EPIC-09.md) | Add disposable Windows lifecycle automation for install, repeat install, upgrade, failed upgrade, reboot/autostart, recovery, stop, uninstall, and retained state | [TASK-0907](./tasks/TASK-0907.md), [TASK-0908](./tasks/TASK-0908.md) |
| [TASK-1001](./tasks/TASK-1001.md) | [EPIC-10](./epics/EPIC-10.md) | Create the thin `SystemUptimeTracker.LinuxDaemon` host with systemd integration and shared agent-core registration | [TASK-0709](./tasks/TASK-0709.md), [TASK-0807](./tasks/TASK-0807.md) |
| [TASK-1002](./tasks/TASK-1002.md) | [EPIC-10](./epics/EPIC-10.md) | Finalize Linux service name, executable path, release path, state path, configuration path, and unprivileged user/group under `SystemUptimeTracker` naming | [TASK-1001](./tasks/TASK-1001.md) |
| [TASK-1003](./tasks/TASK-1003.md) | [EPIC-10](./epics/EPIC-10.md) | Configure self-contained single-file `linux-x64` publishing without trimming and include unit, configuration template, install/uninstall scripts, and README | [TASK-1002](./tasks/TASK-1002.md) |
| [TASK-1004](./tasks/TASK-1004.md) | [EPIC-10](./epics/EPIC-10.md) | Implement idempotent install/upgrade with versioned staging, ownership, permissions, daemon reload, enable/start, bounded readiness, and rollback | [TASK-1003](./tasks/TASK-1003.md) |
| [TASK-1005](./tasks/TASK-1005.md) | [EPIC-10](./epics/EPIC-10.md) | Harden the unit with an unprivileged account, restricted writable paths, `NoNewPrivileges`, private temp, filesystem protection, restart policy, and network ordering | [TASK-1004](./tasks/TASK-1004.md) |
| [TASK-1006](./tasks/TASK-1006.md) | [EPIC-10](./epics/EPIC-10.md) | Implement uninstall that disables/removes the unit and releases while retaining durable state unless explicit purge is requested | [TASK-1004](./tasks/TASK-1004.md) |
| [TASK-1007](./tasks/TASK-1007.md) | [EPIC-10](./epics/EPIC-10.md) | Add disposable Ubuntu lifecycle automation for install, repeat install, upgrade, rollback, reboot/autostart, restart recovery, stop, uninstall, and retained state | [TASK-1004](./tasks/TASK-1004.md), [TASK-1005](./tasks/TASK-1005.md), [TASK-1006](./tasks/TASK-1006.md) |
| [TASK-1101](./tasks/TASK-1101.md) | [EPIC-11](./epics/EPIC-11.md) | Adapt existing Next.js authentication to the owner API flow and establish typed service modules with runtime validation from the accepted OpenAPI contract | [TASK-0209](./tasks/TASK-0209.md), [TASK-0401](./tasks/TASK-0401.md), [TASK-0404](./tasks/TASK-0404.md) |
| [TASK-1102](./tasks/TASK-1102.md) | [EPIC-11](./epics/EPIC-11.md) | Implement CSRF protection for cookie-authenticated state changes and tightly scoped CORS when portal and API origins differ | [TASK-1101](./tasks/TASK-1101.md) |
| [TASK-1103](./tasks/TASK-1103.md) | [EPIC-11](./epics/EPIC-11.md) | Build device-account list/create/edit/disable/delete/reassign and API-key rotate/revoke flows, with one-time key presentation | [TASK-0403](./tasks/TASK-0403.md), [TASK-1101](./tasks/TASK-1101.md) |
| [TASK-1104](./tasks/TASK-1104.md) | [EPIC-11](./epics/EPIC-11.md) | Build paginated machine inventory and detail views showing registration, last seen, OS, version, and assigned account | [TASK-0505](./tasks/TASK-0505.md), [TASK-1101](./tasks/TASK-1101.md) |
| [TASK-1105](./tasks/TASK-1105.md) | [EPIC-11](./epics/EPIC-11.md) | Add heartbeat and runtime-session views with UTC/local-time clarity, current status text, and bounded date filters | [TASK-0608](./tasks/TASK-0608.md), [TASK-1104](./tasks/TASK-1104.md) |
| [TASK-1106](./tasks/TASK-1106.md) | [EPIC-11](./epics/EPIC-11.md) | Normalize API Problem Details into actionable field and page errors | [TASK-0208](./tasks/TASK-0208.md), [TASK-1101](./tasks/TASK-1101.md) |
| [TASK-1107](./tasks/TASK-1107.md) | [EPIC-11](./epics/EPIC-11.md) | Complete responsive keyboard, screen-reader semantics, visible focus, skip navigation, and contrast review for MVP routes | [TASK-1103](./tasks/TASK-1103.md), [TASK-1104](./tasks/TASK-1104.md), [TASK-1105](./tasks/TASK-1105.md) |
| [TASK-1108](./tasks/TASK-1108.md) | [EPIC-11](./epics/EPIC-11.md) | Add owner-login, device-account, machine, heartbeat, and session Playwright journeys to QA automation | [TASK-1102](./tasks/TASK-1102.md), [TASK-1107](./tasks/TASK-1107.md) |
| [TASK-1109](./tasks/TASK-1109.md) | [EPIC-11](./epics/EPIC-11.md) | Define standalone portal build/start configuration, API base URL validation, forwarded headers, health/readiness, and deployment guidance | [TASK-1108](./tasks/TASK-1108.md) |
| [TASK-1201](./tasks/TASK-1201.md) | [EPIC-12](./epics/EPIC-12.md) | Implement `PowerMeter` and `PowerReading` entities, audit fields, connection type, status, and secret reference without storing polling credentials | [TASK-0206](./tasks/TASK-0206.md), [TASK-0301](./tasks/TASK-0301.md) |
| [TASK-1202](./tasks/TASK-1202.md) | [EPIC-12](./epics/EPIC-12.md) | Add unique meter identity constraints for vendor/external ID and optional MAC, plus reading idempotency and meter/time indexes | [TASK-1201](./tasks/TASK-1201.md) |
| [TASK-1203](./tasks/TASK-1203.md) | [EPIC-12](./epics/EPIC-12.md) | Create and validate the power-foundation migration against empty and existing telemetry databases | [TASK-1202](./tasks/TASK-1202.md) |
| [TASK-1204](./tasks/TASK-1204.md) | [EPIC-12](./epics/EPIC-12.md) | Implement owner-authorized meter create/list/detail/update/disable/retire endpoints independent of machines | [TASK-0406](./tasks/TASK-0406.md), [TASK-1203](./tasks/TASK-1203.md) |
| [TASK-1205](./tasks/TASK-1205.md) | [EPIC-12](./epics/EPIC-12.md) | Implement authenticated power-reading ingestion with supported units, server receipt time, payload/version limits, and idempotency | [TASK-0206](./tasks/TASK-0206.md), [TASK-0207](./tasks/TASK-0207.md), [TASK-1203](./tasks/TASK-1203.md), [TASK-0408](./tasks/TASK-0408.md) |
| [TASK-1206](./tasks/TASK-1206.md) | [EPIC-12](./epics/EPIC-12.md) | Preserve optional raw vendor payload only under an explicit size, redaction, and retention policy | [TASK-1205](./tasks/TASK-1205.md) |
| [TASK-1207](./tasks/TASK-1207.md) | [EPIC-12](./epics/EPIC-12.md) | Add owner read endpoints for current meter state and paginated historical readings with deterministic time ordering | [TASK-1204](./tasks/TASK-1204.md), [TASK-1205](./tasks/TASK-1205.md) |
| [TASK-1208](./tasks/TASK-1208.md) | [EPIC-12](./epics/EPIC-12.md) | Extend API and database health/metrics with power ingestion count, failures, duplicates, and last-seen state | [TASK-1207](./tasks/TASK-1207.md) |
| [TASK-1301](./tasks/TASK-1301.md) | [EPIC-13](./epics/EPIC-13.md) | Create `SystemUptimeTracker.Power.Shelly` with a provider interface, bounded HTTP client, DTOs for supported Gen4 RPC responses, and normalized output | [TASK-0701](./tasks/TASK-0701.md), [TASK-1205](./tasks/TASK-1205.md) |
| [TASK-1302](./tasks/TASK-1302.md) | [EPIC-13](./epics/EPIC-13.md) | Load Shelly host and secret reference from validated configuration | [TASK-1301](./tasks/TASK-1301.md) |
| [TASK-1303](./tasks/TASK-1303.md) | [EPIC-13](./epics/EPIC-13.md) | Add optional independent Shelly polling schedules to agent core so disabled or failing power collection never blocks heartbeats | [TASK-0707](./tasks/TASK-0707.md), [TASK-1302](./tasks/TASK-1302.md) |
| [TASK-1304](./tasks/TASK-1304.md) | [EPIC-13](./epics/EPIC-13.md) | Queue and publish normalized readings with their own message IDs and retry classification | [TASK-0806](./tasks/TASK-0806.md), [TASK-1303](./tasks/TASK-1303.md) |
| [TASK-1305](./tasks/TASK-1305.md) | [EPIC-13](./epics/EPIC-13.md) | Implement `Location`, `MonitoredDevice`, `MachinePowerMeterAssociation`, `PowerMeterDeviceAssociation`, and `PowerMeterLocationHistory` with effective dates | [TASK-1203](./tasks/TASK-1203.md) |
| [TASK-1306](./tasks/TASK-1306.md) | [EPIC-13](./epics/EPIC-13.md) | Enforce non-overlapping active primary relationships and valid effective ranges transactionally | [TASK-1305](./tasks/TASK-1305.md) |
| [TASK-1307](./tasks/TASK-1307.md) | [EPIC-13](./epics/EPIC-13.md) | Implement owner CRUD and end-association endpoints for locations, monitored devices, meter placement, machine reporting, and powered-device relationships | [TASK-0408](./tasks/TASK-0408.md), [TASK-1306](./tasks/TASK-1306.md) |
| [TASK-1308](./tasks/TASK-1308.md) | [EPIC-13](./epics/EPIC-13.md) | Validate that the reporting machine is authorized for the meter relationship while measured power remains owned by the meter | [TASK-1304](./tasks/TASK-1304.md), [TASK-1307](./tasks/TASK-1307.md) |
| [TASK-1309](./tasks/TASK-1309.md) | [EPIC-13](./epics/EPIC-13.md) | Add accessible portal workflows for meter registration, reading history, locations, monitored devices, and association timelines | [TASK-1107](./tasks/TASK-1107.md), [TASK-1207](./tasks/TASK-1207.md), [TASK-1307](./tasks/TASK-1307.md) |
| [TASK-1310](./tasks/TASK-1310.md) | [EPIC-13](./epics/EPIC-13.md) | Add end-to-end scenarios for computer-only, meter-only, dedicated load, shared load, collector-only, reassignment, and delayed reading delivery | [TASK-1308](./tasks/TASK-1308.md), [TASK-1309](./tasks/TASK-1309.md) |
| [TASK-1401](./tasks/TASK-1401.md) | [EPIC-14](./epics/EPIC-14.md) | Publish environment-specific configuration references for API, portal, Windows agent, and Linux agent with secret provisioning separated from artifacts and command lines | [TASK-0107](./tasks/TASK-0107.md), [TASK-0909](./tasks/TASK-0909.md), [TASK-1007](./tasks/TASK-1007.md), [TASK-1109](./tasks/TASK-1109.md) |
| [TASK-1402](./tasks/TASK-1402.md) | [EPIC-14](./epics/EPIC-14.md) | Standardize trace/correlation IDs and structured logs across portal, API, agent, heartbeat, and reading operations | [TASK-0506](./tasks/TASK-0506.md), [TASK-0808](./tasks/TASK-0808.md), [TASK-1208](./tasks/TASK-1208.md) |
| [TASK-1403](./tasks/TASK-1403.md) | [EPIC-14](./epics/EPIC-14.md) | Implement API, database, portal, and dependency liveness/readiness checks with startup grace and no sensitive detail for anonymous callers | [TASK-0307](./tasks/TASK-0307.md), [TASK-1109](./tasks/TASK-1109.md) |
| [TASK-1404](./tasks/TASK-1404.md) | [EPIC-14](./epics/EPIC-14.md) | Write start, stop, status, logs, configuration, credential rotation, queue diagnosis, upgrade, rollback, uninstall, and state-recovery runbooks for both agents | [TASK-0909](./tasks/TASK-0909.md), [TASK-1007](./tasks/TASK-1007.md) |
| [TASK-1405](./tasks/TASK-1405.md) | [EPIC-14](./epics/EPIC-14.md) | Define actionable metrics and alert thresholds for ingestion failure, auth failure, offline machines, queue age, migration failure, and inactive meters | [TASK-0608](./tasks/TASK-0608.md), [TASK-0808](./tasks/TASK-0808.md), [TASK-1208](./tasks/TASK-1208.md), [TASK-1402](./tasks/TASK-1402.md) |
| [TASK-1406](./tasks/TASK-1406.md) | [EPIC-14](./epics/EPIC-14.md) | Perform a secret/PII logging review and dependency vulnerability scan for .NET, Node, container, and packaging artifacts | [TASK-0409](./tasks/TASK-0409.md), [TASK-1402](./tasks/TASK-1402.md) |
| [TASK-1407](./tasks/TASK-1407.md) | [EPIC-14](./epics/EPIC-14.md) | Build a release pipeline that produces checksummed API, portal, Windows, and Linux artifacts only after required tests pass | [TASK-0909](./tasks/TASK-0909.md), [TASK-1007](./tasks/TASK-1007.md), [TASK-1108](./tasks/TASK-1108.md), [TASK-1310](./tasks/TASK-1310.md), [TASK-1403](./tasks/TASK-1403.md) |
| [TASK-1408](./tasks/TASK-1408.md) | [EPIC-14](./epics/EPIC-14.md) | Test backup, database migration, application rollback, retained agent state, and restore procedures in a disposable environment | [TASK-0307](./tasks/TASK-0307.md), [TASK-1404](./tasks/TASK-1404.md), [TASK-1407](./tasks/TASK-1407.md) |
| [TASK-1409](./tasks/TASK-1409.md) | [EPIC-14](./epics/EPIC-14.md) | Execute Gate 3 release review against security, accessibility, performance, operations, and product acceptance criteria | [TASK-1405](./tasks/TASK-1405.md), [TASK-1406](./tasks/TASK-1406.md), [TASK-1408](./tasks/TASK-1408.md) |
| [TASK-1501](./tasks/TASK-1501.md) | [EPIC-15](./epics/EPIC-15.md) | Define reporting questions, time zones, aggregation intervals, retention expectations, and measured-versus-estimated labeling | [TASK-0608](./tasks/TASK-0608.md), [TASK-1207](./tasks/TASK-1207.md), [TASK-1308](./tasks/TASK-1308.md) |
| [TASK-1502](./tasks/TASK-1502.md) | [EPIC-15](./epics/EPIC-15.md) | Implement indexed read models or queries for uptime totals, session trends, meter energy, and location summaries with bounded date windows | [TASK-1501](./tasks/TASK-1501.md) |
| [TASK-1503](./tasks/TASK-1503.md) | [EPIC-15](./epics/EPIC-15.md) | Add accessible portal reporting views and exports that preserve units, time-zone context, and measured/shared/estimated distinctions | [TASK-1502](./tasks/TASK-1502.md), [TASK-1107](./tasks/TASK-1107.md) |
| [TASK-1504](./tasks/TASK-1504.md) | [EPIC-15](./epics/EPIC-15.md) | Decide whether estimated allocation is needed | [TASK-1306](./tasks/TASK-1306.md), [TASK-1501](./tasks/TASK-1501.md) |
| [TASK-1505](./tasks/TASK-1505.md) | [EPIC-15](./epics/EPIC-15.md) | Evaluate MQTT, WebSocket, webhook, or broker ingestion against security, availability, cost, and constrained-device authentication requirements | [TASK-1205](./tasks/TASK-1205.md), [TASK-1301](./tasks/TASK-1301.md) |
| [TASK-1506](./tasks/TASK-1506.md) | [EPIC-15](./epics/EPIC-15.md) | If an alternate path is approved, normalize it through the same power-reading command and idempotency rules as agent polling | [TASK-1505](./tasks/TASK-1505.md) |
| [TASK-1507](./tasks/TASK-1507.md) | [EPIC-15](./epics/EPIC-15.md) | Implement discovered-machine and discovered-meter approval workflows only if deferred registration approval was selected | [TASK-0001](./tasks/TASK-0001.md), [TASK-1307](./tasks/TASK-1307.md) |
| [TASK-1508](./tasks/TASK-1508.md) | [EPIC-15](./epics/EPIC-15.md) | Add optional alert evaluation for offline machines, stale meters, and queue/ingestion failures after alert destinations and noise budgets are approved | [TASK-1405](./tasks/TASK-1405.md), [TASK-1502](./tasks/TASK-1502.md) |
| [TASK-1509](./tasks/TASK-1509.md) | [EPIC-15](./epics/EPIC-15.md) | Execute Gate 4 compatibility, scale, reporting, and alternate-ingestion review | [TASK-1503](./tasks/TASK-1503.md), [TASK-1504](./tasks/TASK-1504.md), [TASK-1506](./tasks/TASK-1506.md), [TASK-1507](./tasks/TASK-1507.md), [TASK-1508](./tasks/TASK-1508.md) |
