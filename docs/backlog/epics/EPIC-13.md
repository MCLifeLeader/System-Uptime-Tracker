---
id: EPIC-13
title: 'Shelly And Association Workflows'
type: epic
status: not-started
release_gate: 'Gate 3'
depends_on: 'EPIC-05, EPIC-07, EPIC-12'
---

# EPIC-13: Shelly And Association Workflows

## Outcome

Collect Shelly Plug US Gen4 readings through an agent and manage historically correct machine, device, meter, and location relationships.

## Epic Completion Dependencies

- [EPIC-05](./EPIC-05.md): Machine Registration And Heartbeat Ingestion
- [EPIC-07](./EPIC-07.md): Shared Agent Runtime
- [EPIC-12](./EPIC-12.md): Power Telemetry Foundation
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
  TASK_1301[TASK-1301]
  TASK_1302[TASK-1302]
  TASK_1303[TASK-1303]
  TASK_1304[TASK-1304]
  TASK_1305[TASK-1305]
  TASK_1306[TASK-1306]
  TASK_1307[TASK-1307]
  TASK_1308[TASK-1308]
  TASK_1309[TASK-1309]
  TASK_1310[TASK-1310]
  TASK_1301 --> TASK_1302
  TASK_1302 --> TASK_1303
  TASK_1303 --> TASK_1304
  TASK_1304 --> TASK_1308
  TASK_1305 --> TASK_1306
  TASK_1306 --> TASK_1307
  TASK_1307 --> TASK_1308
  TASK_1307 --> TASK_1309
  TASK_1308 --> TASK_1310
  TASK_1309 --> TASK_1310
  UPSTREAM --> TASK_1301
  UPSTREAM --> TASK_1305
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-1301](../tasks/TASK-1301.md) | Create `SystemUptimeTracker.Power.Shelly` with a provider interface, bounded HTTP client, DTOs for supported Gen4 RPC responses, and normalized output | [TASK-0701](../tasks/TASK-0701.md), [TASK-1205](../tasks/TASK-1205.md) | Not started |
| [TASK-1302](../tasks/TASK-1302.md) | Load Shelly host and secret reference from validated configuration | [TASK-1301](../tasks/TASK-1301.md) | Not started |
| [TASK-1303](../tasks/TASK-1303.md) | Add optional independent Shelly polling schedules to agent core so disabled or failing power collection never blocks heartbeats | [TASK-0707](../tasks/TASK-0707.md), [TASK-1302](../tasks/TASK-1302.md) | Not started |
| [TASK-1304](../tasks/TASK-1304.md) | Queue and publish normalized readings with their own message IDs and retry classification | [TASK-0806](../tasks/TASK-0806.md), [TASK-1303](../tasks/TASK-1303.md) | Not started |
| [TASK-1305](../tasks/TASK-1305.md) | Implement `Location`, `MonitoredDevice`, `MachinePowerMeterAssociation`, `PowerMeterDeviceAssociation`, and `PowerMeterLocationHistory` with effective dates | [TASK-1203](../tasks/TASK-1203.md) | Not started |
| [TASK-1306](../tasks/TASK-1306.md) | Enforce non-overlapping active primary relationships and valid effective ranges transactionally | [TASK-1305](../tasks/TASK-1305.md) | Not started |
| [TASK-1307](../tasks/TASK-1307.md) | Implement owner CRUD and end-association endpoints for locations, monitored devices, meter placement, machine reporting, and powered-device relationships | [TASK-0408](../tasks/TASK-0408.md), [TASK-1306](../tasks/TASK-1306.md) | Not started |
| [TASK-1308](../tasks/TASK-1308.md) | Validate that the reporting machine is authorized for the meter relationship while measured power remains owned by the meter | [TASK-1304](../tasks/TASK-1304.md), [TASK-1307](../tasks/TASK-1307.md) | Not started |
| [TASK-1309](../tasks/TASK-1309.md) | Add accessible portal workflows for meter registration, reading history, locations, monitored devices, and association timelines | [TASK-1107](../tasks/TASK-1107.md), [TASK-1207](../tasks/TASK-1207.md), [TASK-1307](../tasks/TASK-1307.md) | Not started |
| [TASK-1310](../tasks/TASK-1310.md) | Add end-to-end scenarios for computer-only, meter-only, dedicated load, shared load, collector-only, reassignment, and delayed reading delivery | [TASK-1308](../tasks/TASK-1308.md), [TASK-1309](../tasks/TASK-1309.md) | Not started |

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
- Shelly support is optional, independently manageable, and historically correct across every supported relationship type.
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
