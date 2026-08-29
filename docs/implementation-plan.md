# Implementation Plan

## Planning Objective

Deliver the first usable version in staged increments, starting with computer uptime monitoring and leaving room for optional Shelly-based power telemetry without redesigning the core architecture.

## Execution Backlog

The phase descriptions in this document define release intent. The canonical,
task-level execution order is maintained in the
[split delivery backlog](./backlog/README.md), which assigns stable epic and
task IDs, explicit predecessors, acceptance evidence, parallel work lanes, and
release gates. The [execution tree](./backlog/dependency-tree.md) lists every
task in topological scheduling waves.

When sequencing work, use the dependency tree in that backlog rather than the
phase number alone. A later-phase task may begin early when all of its declared
dependencies are complete, while no task may begin merely because its phase is
listed next.

## Delivery Strategy

Use phased implementation so the system becomes useful early:

1. Establish the heartbeat path and runtime-session model.
2. Add platform-specific service packaging and operational hardening.
3. Add optional Shelly integration on top of the stable machine-monitoring baseline.

## Phase 0: Architecture Baseline

### Phase 0 Outcome

Create a stable decision baseline before project scaffolding expands.

### Phase 0 Deliverables

- Structured documentation set in [docs](./).
- Agreed project scope, domain language, and data ownership rules.
- Initial project skeleton decision for applications, shared libraries, and flat `src/`-level test projects named by test type.

### Phase 0 Exit Criteria

- Core terminology is stable enough to scaffold the solution.
- Independent registration rules for machines and power meters are accepted.

## Phase 1: Core Uptime Monitoring MVP

### Phase 1 Outcome

Support Windows and Ubuntu computer monitoring without any power-meter dependency, while establishing the shared API and basic owner-management portal needed to operate the deployment.

### Phase 1 Work Items

- Scaffold API, Windows Service, Linux daemon, and shared libraries.
- Scaffold the NodeJS management portal.
- Implement agent identity persistence.
- Implement heartbeat contract and publishing client.
- Implement ASP.NET Core Identity with local user accounts, the `Owner` role, and the `DeviceAccount` entity (see [architecture-overview.md](./architecture-overview.md#authentication-and-authorization) and [domain-model.md](./domain-model.md)). Support both a shared device account and dedicated per-device accounts, at the owner's discretion.
- Implement a token endpoint issuing JWT access and refresh tokens for device accounts, used by the Windows Service and Linux daemon agents; support periodic access-token rotation via refresh rather than resending the original credential.
- Implement owner-account login for the NodeJS portal against the same API, plus the minimum owner-facing API endpoints and portal views needed to create/manage device accounts and inspect registered machines.
- Implement account lockout on repeated failed authentication attempts and rate limiting on the token endpoint, as baseline brute-force protection for the most exposed part of the system.
- Persist machines, heartbeats, runtime sessions, and storage telemetry in SQL Server.
- Implement heartbeat gap handling and runtime-session reconstruction.
- Add health checks and structured logging.

### Phase 1 Exit Criteria

- A Windows or Ubuntu machine can register and send heartbeats over HTTPS.
- An owner can sign into the NodeJS portal and manage device accounts through the shared API.
- The API stores heartbeat history and derives runtime sessions.
- The agent can survive temporary API outages through retry behavior.

## Phase 2: Service Packaging And Operational Hardening

### Phase 2 Outcome

Make the monitoring system operationally deployable across target environments.

### Phase 2 Work Items

- Implement Windows Service hosting, publishing, and lifecycle behavior using
  [windows-service-reference.md](./windows-service-reference.md) as the concrete
  design baseline.
- Publish a self-contained, single-file `win-x64` artifact containing the
  executable, non-secret configuration template, operator README, and named
  PowerShell install and uninstall entry points.
- Implement idempotent first-install and upgrade behavior with validated named
  parameters, elevation checks, bounded service-state waits, versioned release
  staging, checked native-command results, startup validation, and rollback.
- Configure the `SystemUptimeTrackerAgent` service for automatic startup,
  restart-on-failure recovery, and the explicit least-privilege service
  identity and ACL contract defined by the architecture.
- Keep application releases under `Program Files` separate from durable
  identity, retry, and diagnostic state under `ProgramData`; retain durable
  state on uninstall unless an explicit purge is requested.
- Add a disposable Windows packaging test covering install, repeat install,
  upgrade, failed-upgrade rollback, start, stop, uninstall, and state retention.
- Add systemd unit definition and installation guidance.
- Add NodeJS portal build, packaging, and deployment guidance.
- Define local configuration model for production deployment.
- Define portal-to-API configuration, origin strategy, and session/token handling model.
- Finalize filesystem locations for state, logs, and retry queue storage.
- Add deployment-time configuration validation.
- Add basic operational runbooks for start, stop, upgrade, and diagnostics.

### Phase 2 Exit Criteria

- The Windows agent artifact supports tested install, repeat install, upgrade,
  failed-start rollback, uninstall, automatic startup, recovery configuration,
  clean shutdown, and durable-state retention behavior.
- The Ubuntu agent is installable as a systemd-managed daemon.
- The NodeJS portal can be deployed and connected to the shared API.
- API, portal, and agents expose enough health and logs for first-line troubleshooting.

## Phase 3: Independent Power-Meter Support

### Phase 3 Outcome

Add Shelly support without changing the machine-monitoring core contract.

### Phase 3 Work Items

- Implement power-meter registration model, including secret-reference storage for device credentials (never the credential itself).
- Implement power-reading ingestion contract and storage.
- Implement agent-side Shelly polling provider.
- Implement HTTP Basic Auth with hashed API-key validation as a second authentication scheme for `DeviceAccount`s that cannot perform the JWT login/refresh flow, plus owner-facing endpoints to issue, view (once), and revoke a `DeviceAccount`'s API key.
- Implement machine-to-meter and meter-to-device association endpoints, and the owner-facing device-account management endpoints (create/remove/reassign device accounts) these depend on.
- Implement location model and effective-dated meter placement.
- Extend the NodeJS portal with owner-facing screens for power-meter registration, association management, and collected power-data views.
- Validate dedicated, shared, and collector-only relationship handling.

### Phase 3 Exit Criteria

- A Shelly meter can be added independently of any machine.
- A machine can remain unassociated with any meter.
- A reporting machine can optionally submit Shelly readings under an approved relationship.

## Phase 4: Extended Ingestion And Reporting Readiness

### Phase 4 Outcome

Prepare for scale, alternate telemetry paths, and operator workflows.

### Phase 4 Work Items

- Evaluate MQTT or direct-ingestion Shelly path.
- Add approval workflows for discovered machines and meters.
- Add aggregate reporting queries or read models.
- Add optional estimated power-allocation support (`PowerAllocationRule`; see [domain-model.md](./domain-model.md)).
- Add alerting and operational dashboards if required.

### Phase 4 Exit Criteria

- The system can evolve beyond agent-mediated Shelly polling without schema redesign.
- Administrative data quality workflows are defined.

## Workstreams

## API And Contracts

- Registration endpoints.
- Heartbeat ingestion.
- Power-reading ingestion.
- Association management endpoints.
- Owner-facing administrative and data-read endpoints consumed by the NodeJS portal.
- Token issuance/refresh (JWT) and device-account/API-key management endpoints.
- Validation and idempotency rules.

## Frontend Portal

- Owner authentication flow against the shared API.
- Device-account management UI.
- Machine and telemetry data views.
- Power-meter, location, and association management views.
- Session, token, and API-integration hardening.

## Agent Runtime

- Scheduling.
- Telemetry collection.
- Retry queue.
- API publishing.
- Optional power-provider abstraction.

## Data And Persistence

- Entity model.
- Migrations.
- Query boundaries.
- Session derivation logic.
- Historical association enforcement.

## Deployment And Operations

- Publishing profiles.
- Artifact-contained Windows install and uninstall scripts plus the operator
  runbook defined in [windows-service-reference.md](./windows-service-reference.md).
- Configuration handling.
- Health checks and diagnostics.

## Testing

- `*.UnitTests` projects for session derivation, contracts, and normalization.
- `*.IntegrationTests` projects for heartbeat ingestion and power-meter association rules.
- `*.FunctionalTests` projects for end-to-end API and agent workflow verification where needed.
- Portal unit, integration, and functional tests for owner login, device-account management, and core data-view workflows.
- Packaging smoke tests for Windows and Ubuntu deployment paths, including the
  complete Windows service lifecycle on a disposable Windows environment.
- Thin Windows Service and Linux daemon host projects should default to shared-core coverage plus integration and packaging checks unless platform-specific logic grows large enough to justify dedicated test projects.

## Recommended Implementation Sequence

The executable sequence is the
[task dependency tree](./backlog/dependency-tree.md). Its critical path is
summarized in [delivery-backlog.md](./delivery-backlog.md):

1. Close product decisions and define release evidence (`EPIC-00`).
2. Align the solution and freeze versioned contracts (`EPIC-01`, `EPIC-02`).
3. Implement persistence and least-privilege identity in parallel (`EPIC-03`,
  `EPIC-04`).
4. Complete machine registration and heartbeat ingestion (`EPIC-05`).
5. Build runtime sessions, the agent core, and the owner portal on the stable
  heartbeat path (`EPIC-06`, `EPIC-07`, `EPIC-11`).
6. Add durable retry and offline recovery (`EPIC-08`).
7. Package Windows and Ubuntu agents in parallel (`EPIC-09`, `EPIC-10`).
8. Add independent power persistence, then Shelly collection and associations
  (`EPIC-12`, `EPIC-13`).
9. Complete operational release readiness (`EPIC-14`).
10. Add approved reporting and alternate-ingestion capabilities (`EPIC-15`).

## Major Risks

- Cross-platform telemetry collection may diverge more than expected.
- Session reconstruction rules may need tuning once real-world heartbeat gaps are observed.
- Enrollment and authentication complexity may expand quickly if unauthenticated discovery is allowed.
- Supporting two authentication schemes (JWT and Basic Auth/API key) doubles the surface that must be kept hardened — a weakness in the less-used API-key path (for example, a missed rate limit or an unhashed key in a log) undermines the security goal even if the JWT path is solid.
- A separate NodeJS portal introduces an additional session and deployment surface; if it bypasses the shared API contract or mishandles owner tokens, the system's authorization model becomes inconsistent even if the API itself is correct.
- Power-meter identity conflicts may arise if meters are discovered from multiple ingestion paths.
- Historical association rules can become difficult to enforce without clear administrative workflows.

## Open Technical Questions

- Will SQL Server be available locally through containers for everyday development and tests?
- Should the retry queue start in SQLite or in a simpler file-based format? [inital-spec.md](./inital-spec.md) already recommends a SQLite-backed queue with a 7-day/100 MB cap and a 15s/30s/1m/5m/15m backoff progression — this question is whether to adopt that recommendation as-is or revisit it, not whether to start from a blank slate.
- Is the first agent registration flow self-service, pre-provisioned, or approval-based?
- Should power readings travel inside heartbeat payloads, through separate endpoints, or both?
- What minimum owner-facing administrative and data-read API surface is required for the first management-portal release?
- Should the NodeJS portal act purely as a server-rendered/BFF-style client to the API, or is direct browser-to-API access acceptable for selected read operations?
- What are the accepted default values for heartbeat interval, offline threshold, and session-break threshold? [inital-spec.md](./inital-spec.md) proposes 60 seconds, 3 minutes, and 5 minutes respectively as illustrative starting points; [architecture-overview.md](./architecture-overview.md) repeats them but they are not yet a confirmed decision.
- No document yet enumerates the concrete API routes, request/response payloads, and token endpoint contract (login/refresh request and response shapes, JWT claims such as `AgentId`/`MachineId`, access-token lifetime, Basic Auth header format) as an accepted contract — today these only exist as examples inside the raw [inital-spec.md](./inital-spec.md) transcript, and that transcript predates the Owner/DeviceAccount/JWT/Basic-Auth decision entirely. A dedicated API contracts document (or an addition to this plan) should be produced before or during Phase 1 so the API and agent implementations build against the same accepted shapes.
- Should device accounts be provisioned one-per-machine by default, or is a shared account the default with dedicated per-device accounts as an opt-in? (See [product-scope.md](./product-scope.md).)
- Is more than one owner account expected in the first release, and if so, must each owner's devices/data be isolated from other owners'? (See [product-scope.md](./product-scope.md).)
- Is the initial device-account credential single-use (invalidated after the first JWT login) or a standing fallback credential? (See [product-scope.md](./product-scope.md).)

## Definition Of Ready For Implementation

- Project naming and solution structure are agreed.
- Initial authentication approach is selected: ASP.NET Core Identity local accounts, an `Owner`/`DeviceAccount` ownership model, JWT bearer tokens as the primary scheme, and HTTP Basic Auth with a hashed API key as a fallback for devices that cannot rotate JWTs (decided; see [product-scope.md](./product-scope.md)). The concrete token endpoint contract and default account-provisioning policy remain open — see the questions above.
- Runtime-session rules are agreed well enough to implement tests.
- Minimum machine and power-meter data fields are accepted.
- Deployment targets for the first environment are identified.

## Definition Of Done For The First Release

- Windows and Ubuntu agents can run as managed background services.
- Heartbeats are persisted with reconstructed runtime sessions.
- SQL Server schema and migrations are repeatable.
- API health checks and logs support operational diagnosis.
- Shelly support remains optional and does not block computer-only deployments.
