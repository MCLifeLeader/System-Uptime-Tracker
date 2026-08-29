---
id: EPIC-11
title: 'Owner Portal MVP'
type: epic
status: not-started
release_gate: 'Gate 1'
depends_on: 'EPIC-04, EPIC-05'
---

# EPIC-11: Owner Portal MVP

## Outcome

Deliver an accessible owner workflow for authentication, device accounts, machine inventory, heartbeat history, and runtime sessions through the shared API only.

## Epic Completion Dependencies

- [EPIC-04](./EPIC-04.md): Identity And Authorization
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
  TASK_1101[TASK-1101]
  TASK_1102[TASK-1102]
  TASK_1103[TASK-1103]
  TASK_1104[TASK-1104]
  TASK_1105[TASK-1105]
  TASK_1106[TASK-1106]
  TASK_1107[TASK-1107]
  TASK_1108[TASK-1108]
  TASK_1109[TASK-1109]
  TASK_1101 --> TASK_1102
  TASK_1101 --> TASK_1103
  TASK_1101 --> TASK_1104
  TASK_1101 --> TASK_1106
  TASK_1102 --> TASK_1108
  TASK_1103 --> TASK_1107
  TASK_1104 --> TASK_1105
  TASK_1104 --> TASK_1107
  TASK_1105 --> TASK_1107
  TASK_1107 --> TASK_1108
  TASK_1108 --> TASK_1109
  UPSTREAM --> TASK_1101
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-1101](../tasks/TASK-1101.md) | Adapt existing Next.js authentication to the owner API flow | [TASK-0401](../tasks/TASK-0401.md), [TASK-0404](../tasks/TASK-0404.md) | Not started |
| [TASK-1102](../tasks/TASK-1102.md) | Implement CSRF protection for cookie-authenticated state changes and tightly scoped CORS when portal and API origins differ | [TASK-1101](../tasks/TASK-1101.md) | Not started |
| [TASK-1103](../tasks/TASK-1103.md) | Build device-account list/create/edit/disable/delete/reassign and API-key rotate/revoke flows, with one-time key presentation | [TASK-0403](../tasks/TASK-0403.md), [TASK-1101](../tasks/TASK-1101.md) | Not started |
| [TASK-1104](../tasks/TASK-1104.md) | Build paginated machine inventory and detail views showing registration, last seen, OS, version, and assigned account | [TASK-0505](../tasks/TASK-0505.md), [TASK-1101](../tasks/TASK-1101.md) | Not started |
| [TASK-1105](../tasks/TASK-1105.md) | Add heartbeat and runtime-session views with UTC/local-time clarity, current status text, and bounded date filters | [TASK-0608](../tasks/TASK-0608.md), [TASK-1104](../tasks/TASK-1104.md) | Not started |
| [TASK-1106](../tasks/TASK-1106.md) | Normalize API Problem Details into actionable field and page errors | [TASK-0208](../tasks/TASK-0208.md), [TASK-1101](../tasks/TASK-1101.md) | Not started |
| [TASK-1107](../tasks/TASK-1107.md) | Complete responsive keyboard, screen-reader semantics, visible focus, skip navigation, and contrast review for MVP routes | [TASK-1103](../tasks/TASK-1103.md), [TASK-1104](../tasks/TASK-1104.md), [TASK-1105](../tasks/TASK-1105.md) | Not started |
| [TASK-1108](../tasks/TASK-1108.md) | Add owner-login, device-account, machine, heartbeat, and session Playwright journeys to QA automation | [TASK-1102](../tasks/TASK-1102.md), [TASK-1107](../tasks/TASK-1107.md) | Not started |
| [TASK-1109](../tasks/TASK-1109.md) | Define standalone portal build/start configuration, API base URL validation, forwarded headers, health/readiness, and deployment guidance | [TASK-1108](../tasks/TASK-1108.md) | Not started |

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
- An owner can securely operate the computer-monitoring MVP from the portal without direct database access.
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
