---
id: EPIC-07
title: 'Shared Agent Runtime'
type: epic
status: not-started
release_gate: 'Gate 1'
depends_on: 'EPIC-05'
---

# EPIC-07: Shared Agent Runtime

## Outcome

Implement one testable agent core used by thin Windows and Linux hosts for identity, scheduling, collection, authentication, and publishing.

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
  TASK_0701[TASK-0701]
  TASK_0702[TASK-0702]
  TASK_0703[TASK-0703]
  TASK_0704[TASK-0704]
  TASK_0705[TASK-0705]
  TASK_0706[TASK-0706]
  TASK_0707[TASK-0707]
  TASK_0708[TASK-0708]
  TASK_0709[TASK-0709]
  TASK_0701 --> TASK_0702
  TASK_0701 --> TASK_0703
  TASK_0702 --> TASK_0706
  TASK_0702 --> TASK_0707
  TASK_0703 --> TASK_0704
  TASK_0703 --> TASK_0705
  TASK_0703 --> TASK_0707
  TASK_0706 --> TASK_0707
  TASK_0707 --> TASK_0708
  TASK_0707 --> TASK_0709
  TASK_0708 --> TASK_0709
  UPSTREAM --> TASK_0701
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-0701](../tasks/TASK-0701.md) | Create `SystemUptimeTracker.Agent.Core` and its unit-test project with no Windows Service or systemd hosting dependency | [TASK-0103](../tasks/TASK-0103.md), [TASK-0202](../tasks/TASK-0202.md), [TASK-0203](../tasks/TASK-0203.md) | Not started |
| [TASK-0702](../tasks/TASK-0702.md) | Define the durable local identity-state boundary and implement atomic first-run `AgentId` creation and load with corrupt-file handling and an OS-supplied durable-state path | [TASK-0701](../tasks/TASK-0701.md) | Not started |
| [TASK-0703](../tasks/TASK-0703.md) | Define platform telemetry provider interfaces and normalized snapshots for OS, architecture, boot identity/time, CPU, memory, and storage | [TASK-0701](../tasks/TASK-0701.md) | Not started |
| [TASK-0704](../tasks/TASK-0704.md) | Implement Windows telemetry collection using least-privilege supported APIs and cancellation-aware asynchronous I/O | [TASK-0703](../tasks/TASK-0703.md) | Not started |
| [TASK-0705](../tasks/TASK-0705.md) | Implement Ubuntu telemetry collection from stable OS interfaces with bounded reads and explicit parsing failures | [TASK-0703](../tasks/TASK-0703.md) | Not started |
| [TASK-0706](../tasks/TASK-0706.md) | Implement bootstrap login, durable protected token and refresh-metadata storage, proactive access-token refresh, and disabled/revoked response handling | [TASK-0702](../tasks/TASK-0702.md), [TASK-0404](../tasks/TASK-0404.md) | Not started |
| [TASK-0707](../tasks/TASK-0707.md) | Implement the cancellation-aware worker loop with configurable interval, monotonic sequence numbers, and non-overlapping collection cycles | [TASK-0702](../tasks/TASK-0702.md), [TASK-0703](../tasks/TASK-0703.md), [TASK-0706](../tasks/TASK-0706.md) | Not started |
| [TASK-0708](../tasks/TASK-0708.md) | Implement the typed HTTPS publishing client with bounded timeouts, contract version header, correlation ID, and response classification | [TASK-0502](../tasks/TASK-0502.md), [TASK-0707](../tasks/TASK-0707.md) | Not started |
| [TASK-0709](../tasks/TASK-0709.md) | Add lifecycle signals for agent start and graceful stop while leaving runtime sessions server-authoritative | [TASK-0707](../tasks/TASK-0707.md), [TASK-0708](../tasks/TASK-0708.md) | Not started |

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
- One shared worker can collect and publish valid heartbeats on both target operating systems under deterministic tests.
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
