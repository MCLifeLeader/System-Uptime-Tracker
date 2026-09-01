---
id: EPIC-14
title: 'Operations And Release Readiness'
type: epic
status: not-started
release_gate: 'Gate 3'
depends_on: 'EPIC-06, EPIC-09, EPIC-10, EPIC-11, EPIC-13'
---

# EPIC-14: Operations And Release Readiness

## Outcome

Make the completed feature set observable, configurable, deployable, recoverable, and supportable in the selected first environment.

## Epic Completion Dependencies

- [EPIC-06](./EPIC-06.md): Runtime-Session Reconstruction
- [EPIC-09](./EPIC-09.md): Windows Service Delivery
- [EPIC-10](./EPIC-10.md): Ubuntu Systemd Delivery
- [EPIC-11](./EPIC-11.md): Owner Portal MVP
- [EPIC-13](./EPIC-13.md): Shelly And Association Workflows
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
  TASK_1401[TASK-1401]
  TASK_1402[TASK-1402]
  TASK_1403[TASK-1403]
  TASK_1404[TASK-1404]
  TASK_1405[TASK-1405]
  TASK_1406[TASK-1406]
  TASK_1407[TASK-1407]
  TASK_1408[TASK-1408]
  TASK_1409[TASK-1409]
  TASK_1402 --> TASK_1405
  TASK_1402 --> TASK_1406
  TASK_1403 --> TASK_1407
  TASK_1404 --> TASK_1408
  TASK_1405 --> TASK_1409
  TASK_1406 --> TASK_1409
  TASK_1407 --> TASK_1408
  TASK_1408 --> TASK_1409
  UPSTREAM --> TASK_1401
  UPSTREAM --> TASK_1402
  UPSTREAM --> TASK_1403
  UPSTREAM --> TASK_1404
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-1401](../tasks/TASK-1401.md) | Publish environment-specific configuration references for API, portal, Windows agent, and Linux agent with secret provisioning separated from artifacts and command lines | [TASK-0107](../tasks/TASK-0107.md), [TASK-0909](../tasks/TASK-0909.md), [TASK-1007](../tasks/TASK-1007.md), [TASK-1109](../tasks/TASK-1109.md) | Not started |
| [TASK-1402](../tasks/TASK-1402.md) | Standardize trace/correlation IDs and structured logs across portal, API, agent, heartbeat, and reading operations | [TASK-0506](../tasks/TASK-0506.md), [TASK-0808](../tasks/TASK-0808.md), [TASK-1208](../tasks/TASK-1208.md) | Not started |
| [TASK-1403](../tasks/TASK-1403.md) | Implement API, database, portal, and dependency liveness/readiness checks with startup grace and no sensitive detail for anonymous callers | [TASK-0307](../tasks/TASK-0307.md), [TASK-1109](../tasks/TASK-1109.md) | Not started |
| [TASK-1404](../tasks/TASK-1404.md) | Write start, stop, status, logs, configuration, credential rotation, queue diagnosis, upgrade, rollback, uninstall, and state-recovery runbooks for both agents | [TASK-0909](../tasks/TASK-0909.md), [TASK-1007](../tasks/TASK-1007.md) | Not started |
| [TASK-1405](../tasks/TASK-1405.md) | Define actionable metrics and alert thresholds for ingestion failure, auth failure, offline machines, queue age, migration failure, and inactive meters | [TASK-0608](../tasks/TASK-0608.md), [TASK-0808](../tasks/TASK-0808.md), [TASK-1208](../tasks/TASK-1208.md), [TASK-1402](../tasks/TASK-1402.md) | Not started |
| [TASK-1406](../tasks/TASK-1406.md) | Perform a secret/PII logging review and dependency vulnerability scan for .NET, Node, container, and packaging artifacts | [TASK-0409](../tasks/TASK-0409.md), [TASK-1402](../tasks/TASK-1402.md) | Not started |
| [TASK-1407](../tasks/TASK-1407.md) | Build a release pipeline that produces checksummed API, portal, Windows, and Linux artifacts only after required tests pass | [TASK-0909](../tasks/TASK-0909.md), [TASK-1007](../tasks/TASK-1007.md), [TASK-1108](../tasks/TASK-1108.md), [TASK-1310](../tasks/TASK-1310.md), [TASK-1403](../tasks/TASK-1403.md) | Not started |
| [TASK-1408](../tasks/TASK-1408.md) | Test backup, database migration, application rollback, retained agent state, and restore procedures in a disposable environment | [TASK-0307](../tasks/TASK-0307.md), [TASK-1404](../tasks/TASK-1404.md), [TASK-1407](../tasks/TASK-1407.md) | Not started |
| [TASK-1409](../tasks/TASK-1409.md) | Execute Gate 3 release review against security, accessibility, performance, operations, and product acceptance criteria | [TASK-1405](../tasks/TASK-1405.md), [TASK-1406](../tasks/TASK-1406.md), [TASK-1408](../tasks/TASK-1408.md) | Not started |

## Execution Guidance

- Begin only tasks whose linked predecessors are complete.
- Tasks with all predecessors satisfied may proceed in parallel.
- Keep implementation inside the owning architecture boundary; use the shared
  contracts rather than creating an epic-specific integration surface.
- Validate each task independently before starting a dependent task.
- Treat task-file acceptance criteria as required evidence, not illustrative
  guidance.

## Epic Acceptance

- Every task in this epic is complete with linked evidence.
- The release can be deployed, observed, upgraded, rolled back, and recovered by following versioned automation and runbooks.
- The affected solution, integration, functional, and packaging suites pass.
- Security, accessibility, performance, observability, and operational review
  findings are resolved or explicitly accepted.
- Gate 3 evidence is updated when this epic contributes to that gate.

## Related Documents

- [Backlog index](../README.md)
- [Delivery backlog and dependency tree](../../delivery-backlog.md)
- [Implementation plan](../../implementation-plan.md)
- [Architecture overview](../../architecture-overview.md)
- [Domain model](../../domain-model.md)
