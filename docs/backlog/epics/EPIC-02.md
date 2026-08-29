---
id: EPIC-02
title: 'Versioned Contracts'
type: epic
status: not-started
release_gate: 'Gate 0'
depends_on: 'EPIC-00'
---

# EPIC-02: Versioned Contracts

## Outcome

Freeze interoperable request, response, error, authentication, idempotency, and pagination contracts before API and agent implementations diverge.

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
  TASK_0201[TASK-0201]
  TASK_0202[TASK-0202]
  TASK_0203[TASK-0203]
  TASK_0204[TASK-0204]
  TASK_0205[TASK-0205]
  TASK_0206[TASK-0206]
  TASK_0207[TASK-0207]
  TASK_0208[TASK-0208]
  TASK_0209[TASK-0209]
  TASK_0201 --> TASK_0208
  TASK_0202 --> TASK_0207
  TASK_0202 --> TASK_0209
  TASK_0203 --> TASK_0207
  TASK_0203 --> TASK_0209
  TASK_0204 --> TASK_0209
  TASK_0205 --> TASK_0209
  TASK_0206 --> TASK_0207
  TASK_0206 --> TASK_0209
  TASK_0208 --> TASK_0209
  UPSTREAM --> TASK_0201
  UPSTREAM --> TASK_0202
  UPSTREAM --> TASK_0203
  UPSTREAM --> TASK_0204
  UPSTREAM --> TASK_0205
  UPSTREAM --> TASK_0206
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-0201](../tasks/TASK-0201.md) | Create `docs/api-contracts.md` with the complete `/api/v1` route catalog, caller type, authorization policy, status codes, and idempotency behavior | [TASK-0001](../tasks/TASK-0001.md), [TASK-0002](../tasks/TASK-0002.md), [TASK-0003](../tasks/TASK-0003.md), [TASK-0004](../tasks/TASK-0004.md) | Not started |
| [TASK-0202](../tasks/TASK-0202.md) | Define machine registration request/response DTOs, durable `AgentId`, registration status, assigned `MachineId`, and conflict behavior | [TASK-0102](../tasks/TASK-0102.md) | Not started |
| [TASK-0203](../tasks/TASK-0203.md) | Define heartbeat DTOs for machine metadata, sequence number, sent time, agent start, boot time, CPU, memory, and storage | [TASK-0005](../tasks/TASK-0005.md), [TASK-0102](../tasks/TASK-0102.md) | Not started |
| [TASK-0204](../tasks/TASK-0204.md) | Define owner login, device login, refresh, revoke, and API-key issue/rotate responses without exposing stored secrets | [TASK-0004](../tasks/TASK-0004.md), [TASK-0102](../tasks/TASK-0102.md) | Not started |
| [TASK-0205](../tasks/TASK-0205.md) | Define owner read and administration contracts for device accounts, machines, sessions, and telemetry with bounded pagination and filtering | [TASK-0002](../tasks/TASK-0002.md), [TASK-0102](../tasks/TASK-0102.md) | Not started |
| [TASK-0206](../tasks/TASK-0206.md) | Define power-meter registration, power reading, location, monitored-device, and effective-dated association contracts | [TASK-0007](../tasks/TASK-0007.md), [TASK-0102](../tasks/TASK-0102.md) | Not started |
| [TASK-0207](../tasks/TASK-0207.md) | Define idempotency keys: `AgentId + SequenceNumber` for heartbeats and meter identity plus `MessageId` for readings | [TASK-0202](../tasks/TASK-0202.md), [TASK-0203](../tasks/TASK-0203.md), [TASK-0206](../tasks/TASK-0206.md) | Not started |
| [TASK-0208](../tasks/TASK-0208.md) | Standardize validation errors on Problem Details, correlation headers, UTC timestamp format, numeric units, and unsupported payload-version responses | [TASK-0201](../tasks/TASK-0201.md) | Not started |
| [TASK-0209](../tasks/TASK-0209.md) | Generate or maintain the API OpenAPI document and add a compatibility test for the accepted v1 surface | [TASK-0202](../tasks/TASK-0202.md), [TASK-0203](../tasks/TASK-0203.md), [TASK-0204](../tasks/TASK-0204.md), [TASK-0205](../tasks/TASK-0205.md), [TASK-0206](../tasks/TASK-0206.md), [TASK-0208](../tasks/TASK-0208.md) | Not started |

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
- API, portal, and agents can implement against accepted v1 contracts without sharing runtime implementation assemblies.
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
