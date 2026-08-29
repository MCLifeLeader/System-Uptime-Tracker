---
id: EPIC-01
title: 'Solution And Engineering Foundation'
type: epic
status: not-started
release_gate: 'Gate 0'
depends_on: 'EPIC-00'
---

# EPIC-01: Solution And Engineering Foundation

## Outcome

Align the existing repository with the actual implementation boundaries and establish repeatable validation before feature expansion.

## Epic Completion Dependencies

- [EPIC-00](./EPIC-00.md): Decisions And Acceptance Baseline
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
  TASK_0101[TASK-0101]
  TASK_0102[TASK-0102]
  TASK_0103[TASK-0103]
  TASK_0104[TASK-0104]
  TASK_0105[TASK-0105]
  TASK_0106[TASK-0106]
  TASK_0107[TASK-0107]
  TASK_0108[TASK-0108]
  TASK_0101 --> TASK_0102
  TASK_0101 --> TASK_0103
  TASK_0101 --> TASK_0104
  TASK_0101 --> TASK_0107
  TASK_0104 --> TASK_0105
  TASK_0105 --> TASK_0106
  TASK_0105 --> TASK_0108
  UPSTREAM --> TASK_0101
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-0101](../tasks/TASK-0101.md) | Inventory the current API, web, data, common, AppHost, test, and QA projects | [TASK-0008](../tasks/TASK-0008.md) | Not started |
| [TASK-0102](../tasks/TASK-0102.md) | Decide whether `SystemUptimeTracker.Common` becomes the contracts library or whether to add `SystemUptimeTracker.Contracts` | [TASK-0101](../tasks/TASK-0101.md) | Not started |
| [TASK-0103](../tasks/TASK-0103.md) | Add planned `Agent.Core`, `WindowsService`, `LinuxDaemon`, and `Power.Shelly` projects to the solution only as their first behavior is implemented | [TASK-0101](../tasks/TASK-0101.md) | Not started |
| [TASK-0104](../tasks/TASK-0104.md) | Preserve the existing .NET 10, Next.js 16, React 19, Node 24, NUnit, Vitest, and Playwright toolchain unless a separate upgrade is approved | [TASK-0101](../tasks/TASK-0101.md) | Not started |
| [TASK-0105](../tasks/TASK-0105.md) | Establish the baseline commands for restore, build, .NET test, web lint/test/build, and QA smoke execution in contributor documentation | [TASK-0104](../tasks/TASK-0104.md) | Not started |
| [TASK-0106](../tasks/TASK-0106.md) | Configure CI jobs to run independent .NET, web, contract, migration, and packaging validations with dependency caching | [TASK-0105](../tasks/TASK-0105.md) | Not started |
| [TASK-0107](../tasks/TASK-0107.md) | Define configuration precedence and environment naming across API, web, agents, and AppHost | [TASK-0101](../tasks/TASK-0101.md) | Not started |
| [TASK-0108](../tasks/TASK-0108.md) | Define test ownership: unit for pure rules, integration for SQL/API boundaries, functional for workflows, and packaging tests for installed services | [TASK-0105](../tasks/TASK-0105.md) | Not started |

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
- The solution structure matches the repository and maintained architecture, and baseline validation runs locally and in CI.
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
