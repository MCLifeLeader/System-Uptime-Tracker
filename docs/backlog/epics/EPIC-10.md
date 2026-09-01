---
id: EPIC-10
title: 'Ubuntu Systemd Delivery'
type: epic
status: not-started
release_gate: 'Gate 2'
depends_on: 'EPIC-07, EPIC-08'
---

# EPIC-10: Ubuntu Systemd Delivery

## Outcome

Produce a least-privilege Ubuntu daemon artifact and repeatable systemd install, upgrade, rollback, and uninstall workflow.

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
  TASK_1001[TASK-1001]
  TASK_1002[TASK-1002]
  TASK_1003[TASK-1003]
  TASK_1004[TASK-1004]
  TASK_1005[TASK-1005]
  TASK_1006[TASK-1006]
  TASK_1007[TASK-1007]
  TASK_1001 --> TASK_1002
  TASK_1002 --> TASK_1003
  TASK_1003 --> TASK_1004
  TASK_1004 --> TASK_1005
  TASK_1004 --> TASK_1006
  TASK_1004 --> TASK_1007
  TASK_1005 --> TASK_1007
  TASK_1006 --> TASK_1007
  UPSTREAM --> TASK_1001
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-1001](../tasks/TASK-1001.md) | Create the thin `SystemUptimeTracker.LinuxDaemon` host with systemd integration and shared agent-core registration | [TASK-0709](../tasks/TASK-0709.md), [TASK-0807](../tasks/TASK-0807.md) | Not started |
| [TASK-1002](../tasks/TASK-1002.md) | Finalize Linux service name, executable path, release path, state path, configuration path, and unprivileged user/group under `SystemUptimeTracker` naming | [TASK-1001](../tasks/TASK-1001.md) | Not started |
| [TASK-1003](../tasks/TASK-1003.md) | Configure self-contained single-file `linux-x64` publishing without trimming and include unit, configuration template, install/uninstall scripts, and README | [TASK-1002](../tasks/TASK-1002.md) | Not started |
| [TASK-1004](../tasks/TASK-1004.md) | Implement idempotent install/upgrade with versioned staging, ownership, permissions, daemon reload, enable/start, bounded readiness, and rollback | [TASK-1003](../tasks/TASK-1003.md) | Not started |
| [TASK-1005](../tasks/TASK-1005.md) | Harden the unit with an unprivileged account, restricted writable paths, `NoNewPrivileges`, private temp, filesystem protection, restart policy, and network ordering | [TASK-1004](../tasks/TASK-1004.md) | Not started |
| [TASK-1006](../tasks/TASK-1006.md) | Implement uninstall that disables/removes the unit and releases while retaining durable state unless explicit purge is requested | [TASK-1004](../tasks/TASK-1004.md) | Not started |
| [TASK-1007](../tasks/TASK-1007.md) | Add disposable Ubuntu lifecycle automation for install, repeat install, upgrade, rollback, reboot/autostart, restart recovery, stop, uninstall, and retained state | [TASK-1004](../tasks/TASK-1004.md), [TASK-1005](../tasks/TASK-1005.md), [TASK-1006](../tasks/TASK-1006.md) | Not started |

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
- An operator can install and support the daemon using only the published artifact and documented commands.
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
