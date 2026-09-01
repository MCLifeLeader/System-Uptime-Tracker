---
id: EPIC-15
title: 'Reporting And Future Ingestion'
type: epic
status: not-started
release_gate: 'Gate 4'
depends_on: 'EPIC-06, EPIC-11, EPIC-13'
---

# EPIC-15: Reporting And Future Ingestion

## Outcome

Add bounded aggregate reporting and prepare alternate power paths without changing the established machine, meter, or reading ownership model.

## Epic Completion Dependencies

- [EPIC-06](./EPIC-06.md): Runtime-Session Reconstruction
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
  TASK_1501[TASK-1501]
  TASK_1502[TASK-1502]
  TASK_1503[TASK-1503]
  TASK_1504[TASK-1504]
  TASK_1505[TASK-1505]
  TASK_1506[TASK-1506]
  TASK_1507[TASK-1507]
  TASK_1508[TASK-1508]
  TASK_1509[TASK-1509]
  TASK_1501 --> TASK_1502
  TASK_1501 --> TASK_1504
  TASK_1502 --> TASK_1503
  TASK_1502 --> TASK_1508
  TASK_1503 --> TASK_1509
  TASK_1504 --> TASK_1509
  TASK_1505 --> TASK_1506
  TASK_1506 --> TASK_1509
  TASK_1507 --> TASK_1509
  TASK_1508 --> TASK_1509
  UPSTREAM --> TASK_1501
  UPSTREAM --> TASK_1505
  UPSTREAM --> TASK_1507
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-1501](../tasks/TASK-1501.md) | Define reporting questions, time zones, aggregation intervals, retention expectations, and measured-versus-estimated labeling | [TASK-0608](../tasks/TASK-0608.md), [TASK-1207](../tasks/TASK-1207.md), [TASK-1308](../tasks/TASK-1308.md) | Not started |
| [TASK-1502](../tasks/TASK-1502.md) | Implement indexed read models or queries for uptime totals, session trends, meter energy, and location summaries with bounded date windows | [TASK-1501](../tasks/TASK-1501.md) | Not started |
| [TASK-1503](../tasks/TASK-1503.md) | Add accessible portal reporting views and exports that preserve units, time-zone context, and measured/shared/estimated distinctions | [TASK-1502](../tasks/TASK-1502.md), [TASK-1107](../tasks/TASK-1107.md) | Not started |
| [TASK-1504](../tasks/TASK-1504.md) | Decide whether estimated allocation is needed | [TASK-1306](../tasks/TASK-1306.md), [TASK-1501](../tasks/TASK-1501.md) | Not started |
| [TASK-1505](../tasks/TASK-1505.md) | Evaluate MQTT, WebSocket, webhook, or broker ingestion against security, availability, cost, and constrained-device authentication requirements | [TASK-1205](../tasks/TASK-1205.md), [TASK-1301](../tasks/TASK-1301.md) | Not started |
| [TASK-1506](../tasks/TASK-1506.md) | If an alternate path is approved, normalize it through the same power-reading command and idempotency rules as agent polling | [TASK-1505](../tasks/TASK-1505.md) | Not started |
| [TASK-1507](../tasks/TASK-1507.md) | Implement discovered-machine and discovered-meter approval workflows only if deferred registration approval was selected | [TASK-0001](../tasks/TASK-0001.md), [TASK-1307](../tasks/TASK-1307.md) | Not started |
| [TASK-1508](../tasks/TASK-1508.md) | Add optional alert evaluation for offline machines, stale meters, and queue/ingestion failures after alert destinations and noise budgets are approved | [TASK-1405](../tasks/TASK-1405.md), [TASK-1502](../tasks/TASK-1502.md) | Not started |
| [TASK-1509](../tasks/TASK-1509.md) | Execute Gate 4 compatibility, scale, reporting, and alternate-ingestion review | [TASK-1503](../tasks/TASK-1503.md), [TASK-1504](../tasks/TASK-1504.md), [TASK-1506](../tasks/TASK-1506.md), [TASK-1507](../tasks/TASK-1507.md), [TASK-1508](../tasks/TASK-1508.md) | Not started |

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
- Reporting and alternate ingestion extend the platform without schema ownership reversal or silent contract breakage.
- The affected solution, integration, functional, and packaging suites pass.
- Security, accessibility, performance, observability, and operational review
  findings are resolved or explicitly accepted.
- Gate 4 evidence is updated when this epic contributes to that gate.

## Related Documents

- [Backlog index](../README.md)
- [Delivery backlog and dependency tree](../../delivery-backlog.md)
- [Implementation plan](../../implementation-plan.md)
- [Architecture overview](../../architecture-overview.md)
- [Domain model](../../domain-model.md)
