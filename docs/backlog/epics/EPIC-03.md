---
id: EPIC-03
title: 'Telemetry Persistence'
type: epic
status: not-started
release_gate: 'Gate 1'
depends_on: 'EPIC-01, EPIC-02'
---

# EPIC-03: Telemetry Persistence

## Outcome

Provide a repeatable SQL Server schema with constraints that preserve identity, history, idempotency, and time-aware relationships.

## Epic Completion Dependencies

- [EPIC-01](./EPIC-01.md): Solution And Engineering Foundation
- [EPIC-02](./EPIC-02.md): Versioned Contracts
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
  TASK_0301[TASK-0301]
  TASK_0302[TASK-0302]
  TASK_0303[TASK-0303]
  TASK_0304[TASK-0304]
  TASK_0305[TASK-0305]
  TASK_0306[TASK-0306]
  TASK_0307[TASK-0307]
  TASK_0308[TASK-0308]
  TASK_0301 --> TASK_0302
  TASK_0302 --> TASK_0303
  TASK_0302 --> TASK_0304
  TASK_0302 --> TASK_0305
  TASK_0303 --> TASK_0306
  TASK_0304 --> TASK_0306
  TASK_0305 --> TASK_0306
  TASK_0306 --> TASK_0307
  TASK_0306 --> TASK_0308
  UPSTREAM --> TASK_0301
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-0301](../tasks/TASK-0301.md) | Define the telemetry `DbContext` ownership and migration strategy alongside the existing Identity context | [TASK-0107](../tasks/TASK-0107.md), [TASK-0202](../tasks/TASK-0202.md), [TASK-0203](../tasks/TASK-0203.md) | Not started |
| [TASK-0302](../tasks/TASK-0302.md) | Implement `Machine`, `Heartbeat`, `StorageTelemetry`, and `RuntimeSession` entities with audit fields and domain-model nullability | [TASK-0301](../tasks/TASK-0301.md) | Not started |
| [TASK-0303](../tasks/TASK-0303.md) | Add unique indexes for populated `AgentId` and heartbeat idempotency, plus indexes for machine/time and session queries | [TASK-0302](../tasks/TASK-0302.md) | Not started |
| [TASK-0304](../tasks/TASK-0304.md) | Store server-authoritative `ReceivedAtUtc` | [TASK-0302](../tasks/TASK-0302.md) | Not started |
| [TASK-0305](../tasks/TASK-0305.md) | Configure storage telemetry as heartbeat-owned history with explicit delete behavior and no accidental cascade from account deletion | [TASK-0302](../tasks/TASK-0302.md) | Not started |
| [TASK-0306](../tasks/TASK-0306.md) | Create and review the initial telemetry migration and SQL script for least-privilege deployment | [TASK-0303](../tasks/TASK-0303.md), [TASK-0304](../tasks/TASK-0304.md), [TASK-0305](../tasks/TASK-0305.md) | Not started |
| [TASK-0307](../tasks/TASK-0307.md) | Add migration rollback/reapply and model-snapshot drift tests to CI | [TASK-0306](../tasks/TASK-0306.md) | Not started |
| [TASK-0308](../tasks/TASK-0308.md) | Define retention boundaries for raw heartbeats, storage telemetry, sessions, and later power readings without implementing destructive defaults | [TASK-0306](../tasks/TASK-0306.md) | Not started |

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
- A clean or existing Identity database can be migrated repeatably, and persistence invariants are covered by SQL Server integration tests.
- The affected solution, integration, functional, and packaging suites pass.
- Security, accessibility, performance, observability, and operational review
  findings are resolved or explicitly accepted.
- Gate 1 evidence is updated when this epic contributes to that gate.

## Related Documents

- [Backlog index](../README.md)
- [Delivery backlog and dependency tree](../../delivery-backlog.md)
- [Implementation plan](../../implementation-plan.md)
- [Architecture overview](../../architecture-overview.md)
- [Domain model](../../domain-model.md)
