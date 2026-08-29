---
id: EPIC-04
title: 'Identity And Authorization'
type: epic
status: not-started
release_gate: 'Gate 1'
depends_on: 'EPIC-01, EPIC-02'
---

# EPIC-04: Identity And Authorization

## Outcome

Adapt the existing Identity implementation into explicit owner and device security boundaries with JWT as primary authentication and hashed API keys as the constrained-device fallback.

## Epic Completion Dependencies

- [EPIC-01](./EPIC-01.md): Solution And Engineering Foundation
- [EPIC-02](./EPIC-02.md): Versioned Contracts
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
  TASK_0401[TASK-0401]
  TASK_0402[TASK-0402]
  TASK_0403[TASK-0403]
  TASK_0404[TASK-0404]
  TASK_0405[TASK-0405]
  TASK_0406[TASK-0406]
  TASK_0407[TASK-0407]
  TASK_0408[TASK-0408]
  TASK_0409[TASK-0409]
  TASK_0410[TASK-0410]
  TASK_0401 --> TASK_0402
  TASK_0401 --> TASK_0408
  TASK_0401 --> TASK_0410
  TASK_0402 --> TASK_0403
  TASK_0402 --> TASK_0404
  TASK_0402 --> TASK_0405
  TASK_0403 --> TASK_0409
  TASK_0404 --> TASK_0406
  TASK_0404 --> TASK_0407
  TASK_0405 --> TASK_0406
  TASK_0405 --> TASK_0407
  TASK_0406 --> TASK_0408
  TASK_0408 --> TASK_0409
  UPSTREAM --> TASK_0401
~~~

UPSTREAM represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
| [TASK-0401](../tasks/TASK-0401.md) | Map existing `Admin`, `Manager`, `Contributor`, and `Read` roles to the decided `Owner` and telemetry-only device policies | [TASK-0002](../tasks/TASK-0002.md), [TASK-0101](../tasks/TASK-0101.md), [TASK-0204](../tasks/TASK-0204.md) | Not started |
| [TASK-0402](../tasks/TASK-0402.md) | Implement `DeviceAccount` as a domain companion to `ApplicationUser`, including owner, allowed methods, API-key metadata, active state, and audit fields | [TASK-0301](../tasks/TASK-0301.md), [TASK-0401](../tasks/TASK-0401.md) | Not started |
| [TASK-0403](../tasks/TASK-0403.md) | Implement owner-authorized create, list, update, disable, delete/reassign, and credential-rotation services with ownership checks | [TASK-0402](../tasks/TASK-0402.md) | Not started |
| [TASK-0404](../tasks/TASK-0404.md) | Implement device credential exchange and refresh-token rotation with configured access/refresh lifetimes and revocation | [TASK-0204](../tasks/TASK-0204.md), [TASK-0402](../tasks/TASK-0402.md) | Not started |
| [TASK-0405](../tasks/TASK-0405.md) | Implement cryptographically random API-key issuance, salted hashing, constant-time verification, one-time display, rotation, and revocation | [TASK-0204](../tasks/TASK-0204.md), [TASK-0402](../tasks/TASK-0402.md) | Not started |
| [TASK-0406](../tasks/TASK-0406.md) | Build device claims from server-side account and machine authorization data | [TASK-0404](../tasks/TASK-0404.md), [TASK-0405](../tasks/TASK-0405.md) | Not started |
| [TASK-0407](../tasks/TASK-0407.md) | Apply lockout and partitioned rate limits to password, token, refresh, and Basic Auth entry points without blocking health probes | [TASK-0404](../tasks/TASK-0404.md), [TASK-0405](../tasks/TASK-0405.md) | Not started |
| [TASK-0408](../tasks/TASK-0408.md) | Require authentication on every non-health route and explicit owner/device policies by route group | [TASK-0401](../tasks/TASK-0401.md), [TASK-0406](../tasks/TASK-0406.md) | Not started |
| [TASK-0409](../tasks/TASK-0409.md) | Audit logs for account creation, disablement, key issue/rotation/revocation, failed authentication, and denied authorization using identifiers rather than secrets | [TASK-0403](../tasks/TASK-0403.md), [TASK-0408](../tasks/TASK-0408.md) | Not started |
| [TASK-0410](../tasks/TASK-0410.md) | Implement a one-time first-owner bootstrap path using deployment-supplied secret material, explicit startup validation, and automatic closure after an owner exists | [TASK-0301](../tasks/TASK-0301.md), [TASK-0401](../tasks/TASK-0401.md) | Not started |

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
- Owner and device callers authenticate through supported schemes, receive only required permissions, and cannot cross account or machine boundaries.
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
