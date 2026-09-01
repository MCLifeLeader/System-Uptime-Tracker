---
id: EPIC-09
title: 'Windows Service Delivery'
type: epic
status: not-started
release_gate: 'Gate 2'
depends_on: 'EPIC-07, EPIC-08'
---

# EPIC-09: Windows Service Delivery

## Outcome

Produce a self-contained Windows Service artifact with idempotent install, upgrade, rollback, recovery, uninstall, and retained durable state.

## Epic Completion Dependencies

- [EPIC-07](./EPIC-07.md): Shared Agent Runtime
- [EPIC-08](./EPIC-08.md): Offline Queue And Resilient Delivery
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
  TASK_0901[TASK-0901]
  TASK_0902[TASK-0902]
  TASK_0903[TASK-0903]
  TASK_0904[TASK-0904]
  TASK_0905[TASK-0905]
  TASK_0906[TASK-0906]
  TASK_0907[TASK-0907]
  TASK_0908[TASK-0908]
  TASK_0909[TASK-0909]
  TASK_0901 --> TASK_0902
  TASK_0902 --> TASK_0903
  TASK_0903 --> TASK_0904
  TASK_0904 --> TASK_0905
  TASK_0904 --> TASK_0908
  TASK_0905 --> TASK_0906
  TASK_0906 --> TASK_0907
  TASK_0906 --> TASK_0908
  TASK_0907 --> TASK_0909
  TASK_0908 --> TASK_0909
  UPSTREAM --> TASK_0901
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-0901](../tasks/TASK-0901.md) | Create the thin `SystemUptimeTracker.WindowsService` host and configure `AddWindowsService` with service name `SystemUptimeTrackerAgent` | [TASK-0709](../tasks/TASK-0709.md), [TASK-0807](../tasks/TASK-0807.md) | Not started |
| [TASK-0902](../tasks/TASK-0902.md) | Configure self-contained single-file `win-x64` publishing without trimming and include non-secret configuration plus operator documentation | [TASK-0901](../tasks/TASK-0901.md) | Not started |
| [TASK-0903](../tasks/TASK-0903.md) | Implement `Install-SystemUptimeTrackerWindowsService.ps1` as an advanced, elevation-checked, parameter-validated, `SupportsShouldProcess` script using its own artifact directory | [TASK-0902](../tasks/TASK-0902.md) | Not started |
| [TASK-0904](../tasks/TASK-0904.md) | Stage immutable versioned releases under `Program Files`, keep durable state under `ProgramData`, and prevent source/application/data path overlap | [TASK-0903](../tasks/TASK-0903.md) | Not started |
| [TASK-0905](../tasks/TASK-0905.md) | Create or update the service with LocalService by default, automatic startup, description, restart-on-failure actions, and least-privilege ACLs | [TASK-0904](../tasks/TASK-0904.md) | Not started |
| [TASK-0906](../tasks/TASK-0906.md) | Replace fixed waits with bounded state polling | [TASK-0905](../tasks/TASK-0905.md) | Not started |
| [TASK-0907](../tasks/TASK-0907.md) | Preserve the prior binary path and release until startup validation succeeds | [TASK-0906](../tasks/TASK-0906.md) | Not started |
| [TASK-0908](../tasks/TASK-0908.md) | Implement uninstall that removes service registration and releases while retaining `ProgramData` unless an explicit confirmed purge is requested | [TASK-0904](../tasks/TASK-0904.md), [TASK-0906](../tasks/TASK-0906.md) | Not started |
| [TASK-0909](../tasks/TASK-0909.md) | Add disposable Windows lifecycle automation for install, repeat install, upgrade, failed upgrade, reboot/autostart, recovery, stop, uninstall, and retained state | [TASK-0907](../tasks/TASK-0907.md), [TASK-0908](../tasks/TASK-0908.md) | Not started |

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
- The packaged Windows agent satisfies every expected deliverable and installer contract in `windows-service-reference.md`.
- The affected solution, integration, functional, and packaging suites pass.
- Security, accessibility, performance, observability, and operational review
  findings are resolved or explicitly accepted.
- Gate 2 evidence is updated when this epic contributes to that gate.

## Related Documents

- [Backlog index](../README.md)
- [Delivery backlog and dependency tree](../../delivery-backlog.md)
- [Implementation plan](../../implementation-plan.md)
- [Architecture overview](../../architecture-overview.md)
- [Domain model](../../domain-model.md)
