# Implementation Plan

## Planning Objective

Deliver the first usable version in staged increments, starting with computer uptime monitoring and leaving room for optional Shelly-based power telemetry without redesigning the core architecture.

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

Support Windows and Ubuntu computer monitoring without any power-meter dependency.

### Phase 1 Work Items

- Scaffold API, Windows Service, Linux daemon, and shared libraries.
- Implement agent identity persistence.
- Implement heartbeat contract and publishing client.
- Implement API authentication for agents.
- Persist machines, heartbeats, runtime sessions, and storage telemetry in SQL Server.
- Implement heartbeat gap handling and runtime-session reconstruction.
- Add health checks and structured logging.

### Phase 1 Exit Criteria

- A Windows or Ubuntu machine can register and send heartbeats over HTTPS.
- The API stores heartbeat history and derives runtime sessions.
- The agent can survive temporary API outages through retry behavior.

## Phase 2: Service Packaging And Operational Hardening

### Phase 2 Outcome

Make the monitoring system operationally deployable across target environments.

### Phase 2 Work Items

- Add Windows Service installation guidance and packaging.
- Add systemd unit definition and installation guidance.
- Define local configuration model for production deployment.
- Finalize filesystem locations for state, logs, and retry queue storage.
- Add deployment-time configuration validation.
- Add basic operational runbooks for start, stop, upgrade, and diagnostics.

### Phase 2 Exit Criteria

- The Windows agent is installable as a service.
- The Ubuntu agent is installable as a systemd-managed daemon.
- API and agents expose enough health and logs for first-line troubleshooting.

## Phase 3: Independent Power-Meter Support

### Phase 3 Outcome

Add Shelly support without changing the machine-monitoring core contract.

### Phase 3 Work Items

- Implement power-meter registration model.
- Implement power-reading ingestion contract and storage.
- Implement agent-side Shelly polling provider.
- Implement machine-to-meter and meter-to-device association endpoints.
- Implement location model and effective-dated meter placement.
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
- Add optional estimated power-allocation support.
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
- Validation and idempotency rules.

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
- Service installation scripts or instructions.
- Configuration handling.
- Health checks and diagnostics.

## Testing

- `*.UnitTests` projects for session derivation, contracts, and normalization.
- `*.IntegrationTests` projects for heartbeat ingestion and power-meter association rules.
- `*.FunctionalTests` projects for end-to-end API and agent workflow verification where needed.
- Packaging smoke tests for Windows and Ubuntu deployment paths.
- Thin Windows Service and Linux daemon host projects should default to shared-core coverage plus integration and packaging checks unless platform-specific logic grows large enough to justify dedicated test projects.

## Recommended Implementation Sequence

1. Create the solution skeleton and shared contracts.
2. Implement machine registration and heartbeat ingestion end to end.
3. Implement runtime-session calculation with tests.
4. Add retry queue and resilient agent publishing.
5. Add Windows and Ubuntu service-hosting specifics.
6. Add independent power-meter registration and storage.
7. Add Shelly provider integration and association APIs.
8. Add location and richer inventory support.

## Major Risks

- Cross-platform telemetry collection may diverge more than expected.
- Session reconstruction rules may need tuning once real-world heartbeat gaps are observed.
- Enrollment and authentication complexity may expand quickly if unauthenticated discovery is allowed.
- Power-meter identity conflicts may arise if meters are discovered from multiple ingestion paths.
- Historical association rules can become difficult to enforce without clear administrative workflows.

## Open Technical Questions

- Will SQL Server be available locally through containers for everyday development and tests?
- Should the retry queue start in SQLite or in a simpler file-based format?
- Is the first agent registration flow self-service, pre-provisioned, or approval-based?
- Should power readings travel inside heartbeat payloads, through separate endpoints, or both?
- What minimum administrative API surface is required before any UI exists?

## Definition Of Ready For Implementation

- Project naming and solution structure are agreed.
- Initial authentication approach is selected.
- Runtime-session rules are agreed well enough to implement tests.
- Minimum machine and power-meter data fields are accepted.
- Deployment targets for the first environment are identified.

## Definition Of Done For The First Release

- Windows and Ubuntu agents can run as managed background services.
- Heartbeats are persisted with reconstructed runtime sessions.
- SQL Server schema and migrations are repeatable.
- API health checks and logs support operational diagnosis.
- Shelly support remains optional and does not block computer-only deployments.
