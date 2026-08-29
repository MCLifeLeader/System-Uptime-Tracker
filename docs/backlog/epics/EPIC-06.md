---
id: EPIC-06
title: 'Runtime-Session Reconstruction'
type: epic
status: not-started
release_gate: 'Gate 1'
depends_on: 'EPIC-05'
---

# EPIC-06: Runtime-Session Reconstruction

## Outcome

Derive deterministic uptime sessions from persisted heartbeat continuity and lifecycle evidence.

## Epic Completion Dependencies

- [EPIC-05](./EPIC-05.md): Machine Registration And Heartbeat Ingestion
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
  TASK_0601[TASK-0601]
  TASK_0602[TASK-0602]
  TASK_0603[TASK-0603]
  TASK_0604[TASK-0604]
  TASK_0605[TASK-0605]
  TASK_0606[TASK-0606]
  TASK_0607[TASK-0607]
  TASK_0608[TASK-0608]
  TASK_0601 --> TASK_0602
  TASK_0602 --> TASK_0603
  TASK_0603 --> TASK_0604
  TASK_0603 --> TASK_0605
  TASK_0603 --> TASK_0606
  TASK_0604 --> TASK_0607
  TASK_0605 --> TASK_0607
  TASK_0606 --> TASK_0607
  TASK_0607 --> TASK_0608
  UPSTREAM --> TASK_0601
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-0601](../tasks/TASK-0601.md) | Specify the session state machine for first heartbeat, continuation, timeout, reboot, agent restart, suspend/resume, graceful stop, and out-of-order receipt | [TASK-0005](../tasks/TASK-0005.md), [TASK-0503](../tasks/TASK-0503.md) | Not started |
| [TASK-0602](../tasks/TASK-0602.md) | Implement the pure session-transition calculator using UTC instants and injected thresholds | [TASK-0601](../tasks/TASK-0601.md) | Not started |
| [TASK-0603](../tasks/TASK-0603.md) | Integrate session updates in the heartbeat transaction or an idempotent post-ingestion processor with explicit concurrency control | [TASK-0602](../tasks/TASK-0602.md), [TASK-0503](../tasks/TASK-0503.md) | Not started |
| [TASK-0604](../tasks/TASK-0604.md) | Implement timeout closure using a scheduled, restart-safe process and an injectable server clock | [TASK-0603](../tasks/TASK-0603.md) | Not started |
| [TASK-0605](../tasks/TASK-0605.md) | Handle delayed queue uploads by preserving event order and avoiding false uptime across gaps | [TASK-0603](../tasks/TASK-0603.md) | Not started |
| [TASK-0606](../tasks/TASK-0606.md) | Calculate `HeartbeatCount`, `LastHeartbeatAtUtc`, `EndedAtUtc`, and uptime duration with documented boundary semantics | [TASK-0603](../tasks/TASK-0603.md) | Not started |
| [TASK-0607](../tasks/TASK-0607.md) | Add SQL Server integration tests for reboot, restart, timeout, duplicate, concurrent, delayed, and clock-skew scenarios | [TASK-0604](../tasks/TASK-0604.md), [TASK-0605](../tasks/TASK-0605.md), [TASK-0606](../tasks/TASK-0606.md) | Not started |
| [TASK-0608](../tasks/TASK-0608.md) | Expose owner-authorized current and historical session queries with pagination and deterministic ordering | [TASK-0607](../tasks/TASK-0607.md) | Not started |

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
- The same heartbeat history always yields the same non-overlapping runtime sessions, including retries and delayed delivery.
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
