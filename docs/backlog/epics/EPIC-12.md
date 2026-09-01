---
id: EPIC-12
title: 'Power Telemetry Foundation'
type: epic
status: not-started
release_gate: 'Gate 3'
depends_on: 'EPIC-02, EPIC-03, EPIC-04'
---

# EPIC-12: Power Telemetry Foundation

## Outcome

Register power meters independently and persist idempotent readings without coupling machine uptime to power availability.

## Epic Completion Dependencies

- [EPIC-02](./EPIC-02.md): Versioned Contracts
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
  TASK_1201[TASK-1201]
  TASK_1202[TASK-1202]
  TASK_1203[TASK-1203]
  TASK_1204[TASK-1204]
  TASK_1205[TASK-1205]
  TASK_1206[TASK-1206]
  TASK_1207[TASK-1207]
  TASK_1208[TASK-1208]
  TASK_1201 --> TASK_1202
  TASK_1202 --> TASK_1203
  TASK_1203 --> TASK_1204
  TASK_1203 --> TASK_1205
  TASK_1204 --> TASK_1207
  TASK_1205 --> TASK_1206
  TASK_1205 --> TASK_1207
  TASK_1207 --> TASK_1208
  UPSTREAM --> TASK_1201
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-1201](../tasks/TASK-1201.md) | Implement `PowerMeter` and `PowerReading` entities, audit fields, connection type, status, and secret reference without storing polling credentials | [TASK-0206](../tasks/TASK-0206.md), [TASK-0301](../tasks/TASK-0301.md) | Not started |
| [TASK-1202](../tasks/TASK-1202.md) | Add unique meter identity constraints for vendor/external ID and optional MAC, plus reading idempotency and meter/time indexes | [TASK-1201](../tasks/TASK-1201.md) | Not started |
| [TASK-1203](../tasks/TASK-1203.md) | Create and validate the power-foundation migration against empty and existing telemetry databases | [TASK-1202](../tasks/TASK-1202.md) | Not started |
| [TASK-1204](../tasks/TASK-1204.md) | Implement owner-authorized meter create/list/detail/update/disable/retire endpoints independent of machines | [TASK-0406](../tasks/TASK-0406.md), [TASK-1203](../tasks/TASK-1203.md) | Not started |
| [TASK-1205](../tasks/TASK-1205.md) | Implement authenticated power-reading ingestion with supported units, server receipt time, payload/version limits, and idempotency | [TASK-0206](../tasks/TASK-0206.md), [TASK-0207](../tasks/TASK-0207.md), [TASK-1203](../tasks/TASK-1203.md), [TASK-0408](../tasks/TASK-0408.md) | Not started |
| [TASK-1206](../tasks/TASK-1206.md) | Preserve optional raw vendor payload only under an explicit size, redaction, and retention policy | [TASK-1205](../tasks/TASK-1205.md) | Not started |
| [TASK-1207](../tasks/TASK-1207.md) | Add owner read endpoints for current meter state and paginated historical readings with deterministic time ordering | [TASK-1204](../tasks/TASK-1204.md), [TASK-1205](../tasks/TASK-1205.md) | Not started |
| [TASK-1208](../tasks/TASK-1208.md) | Extend API and database health/metrics with power ingestion count, failures, duplicates, and last-seen state | [TASK-1207](../tasks/TASK-1207.md) | Not started |

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
- A power meter operates as a first-class entity with no machine or agent dependency.
- The affected solution, integration, functional, and packaging suites pass.
- Security, accessibility, performance, observability, and operational review
  findings are resolved or explicitly accepted.
- Gate 3 evidence is updated when this epic contributes to that gate.

## Related Documents

- [Backlog index](../README.md)
- [Delivery backlog and dependency tree](../../delivery-backlog.md)
- [Implementation plan](../../implementation-plan.md)
- [Architecture overview](../../architecture-overview.md)
- [Domain model](../../domain-model.md)
