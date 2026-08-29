---
id: EPIC-00
title: 'Decisions And Acceptance Baseline'
type: epic
status: not-started
release_gate: 'Gate 0'
depends_on: 'None'
---

# EPIC-00: Decisions And Acceptance Baseline

## Outcome

Resolve decisions that affect schema, authorization, contracts, queue behavior, and deployment before dependent implementation begins.

## Epic Completion Dependencies

- None.
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
  TASK_0001[TASK-0001]
  TASK_0002[TASK-0002]
  TASK_0003[TASK-0003]
  TASK_0004[TASK-0004]
  TASK_0005[TASK-0005]
  TASK_0006[TASK-0006]
  TASK_0007[TASK-0007]
  TASK_0008[TASK-0008]
  START --> TASK_0001
  START --> TASK_0002
  START --> TASK_0003
  START --> TASK_0004
  START --> TASK_0005
  START --> TASK_0006
  START --> TASK_0007
  START --> TASK_0008
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-0001](../tasks/TASK-0001.md) | Decide whether first-release registration is pre-provisioned, self-service, or approval-based | None | Not started |
| [TASK-0002](../tasks/TASK-0002.md) | Decide whether the deployment supports one owner or multiple owners and whether data is isolated by owner | None | Not started |
| [TASK-0003](../tasks/TASK-0003.md) | Select the default device-account policy: shared account or one account per machine, while retaining both supported modes | None | Not started |
| [TASK-0004](../tasks/TASK-0004.md) | Decide whether the bootstrap password is single-use or a standing fallback after the first refresh token is issued | None | Not started |
| [TASK-0005](../tasks/TASK-0005.md) | Accept configurable defaults for heartbeat interval, offline threshold, session-break threshold, clock-skew tolerance, and detailed-telemetry interval | None | Not started |
| [TASK-0006](../tasks/TASK-0006.md) | Accept the retry store technology, 7-day age cap, 100 MB size cap, retry schedule, overflow policy, and poison-message policy | None | Not started |
| [TASK-0007](../tasks/TASK-0007.md) | Decide whether power readings use a separate endpoint, a combined heartbeat payload, or both | None | Not started |
| [TASK-0008](../tasks/TASK-0008.md) | Define Gate 1, Gate 2, Gate 3, and Gate 4 release evidence, including required automated suites and target environments | None | Not started |

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
- All decisions are merged into maintained documents, and no dependent task requires a product or architecture choice during coding.
- The affected solution, integration, functional, and packaging suites pass.
- Security, accessibility, performance, observability, and operational review
  findings are resolved or explicitly accepted.
- Gate 0 evidence is updated when this epic contributes to that gate.

## Related Documents

- [Backlog index](../README.md)
- [Delivery backlog and dependency tree](../../delivery-backlog.md)
- [Implementation plan](../../implementation-plan.md)
- [Architecture overview](../../architecture-overview.md)
- [Domain model](../../domain-model.md)
