---
id: EPIC-05
title: 'Machine Registration And Heartbeat Ingestion'
type: epic
status: not-started
release_gate: 'Gate 1'
depends_on: 'EPIC-03, EPIC-04'
---

# EPIC-05: Machine Registration And Heartbeat Ingestion

## Outcome

Register machines independently and ingest validated, idempotent heartbeats through an authenticated end-to-end path.

## Epic Completion Dependencies

- [EPIC-03](./EPIC-03.md): Telemetry Persistence
- [EPIC-04](./EPIC-04.md): Identity And Authorization
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
  TASK_0501[TASK-0501]
  TASK_0502[TASK-0502]
  TASK_0503[TASK-0503]
  TASK_0504[TASK-0504]
  TASK_0505[TASK-0505]
  TASK_0506[TASK-0506]
  TASK_0507[TASK-0507]
  TASK_0508[TASK-0508]
  TASK_0502 --> TASK_0503
  TASK_0502 --> TASK_0504
  TASK_0502 --> TASK_0505
  TASK_0502 --> TASK_0506
  TASK_0503 --> TASK_0507
  TASK_0504 --> TASK_0507
  TASK_0505 --> TASK_0507
  TASK_0507 --> TASK_0508
  UPSTREAM --> TASK_0501
  UPSTREAM --> TASK_0502
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-0501](../tasks/TASK-0501.md) | Implement the selected machine registration workflow, including pre-created records, durable `AgentId`, status transitions, and device-account assignment | [TASK-0202](../tasks/TASK-0202.md), [TASK-0306](../tasks/TASK-0306.md), [TASK-0406](../tasks/TASK-0406.md) | Not started |
| [TASK-0502](../tasks/TASK-0502.md) | Implement `POST /api/v1/heartbeats` with payload-size limits, version validation, authenticated machine scope, and server receipt time | [TASK-0203](../tasks/TASK-0203.md), [TASK-0306](../tasks/TASK-0306.md), [TASK-0408](../tasks/TASK-0408.md) | Not started |
| [TASK-0503](../tasks/TASK-0503.md) | Make heartbeat processing atomic and idempotent under sequential and concurrent duplicate delivery | [TASK-0207](../tasks/TASK-0207.md), [TASK-0502](../tasks/TASK-0502.md) | Not started |
| [TASK-0504](../tasks/TASK-0504.md) | Normalize OS, architecture, machine name, agent version, CPU, memory, and storage values without silently coercing invalid data | [TASK-0502](../tasks/TASK-0502.md) | Not started |
| [TASK-0505](../tasks/TASK-0505.md) | Update machine `FirstSeenAtUtc`, `LastSeenAtUtc`, metadata, and registration state using server-authoritative rules | [TASK-0502](../tasks/TASK-0502.md) | Not started |
| [TASK-0506](../tasks/TASK-0506.md) | Attach or generate a correlation identifier and emit structured ingestion success/failure metrics without machine secrets or raw credentials | [TASK-0502](../tasks/TASK-0502.md) | Not started |
| [TASK-0507](../tasks/TASK-0507.md) | Add SQL Server integration tests for registration through heartbeat persistence, including retry and out-of-order delivery | [TASK-0503](../tasks/TASK-0503.md), [TASK-0504](../tasks/TASK-0504.md), [TASK-0505](../tasks/TASK-0505.md) | Not started |
| [TASK-0508](../tasks/TASK-0508.md) | Add an end-to-end smoke client that registers a machine, obtains authorization, posts a heartbeat, and reads it as an owner | [TASK-0507](../tasks/TASK-0507.md) | Not started |

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
- A permitted machine can register and submit telemetry repeatedly without duplication, privilege escalation, or loss of original event time.
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
