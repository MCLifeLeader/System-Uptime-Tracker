# Story 03: Identity And Persistence Foundation

## Objective

Implement the SQL Server schema, Entity Framework model, and ASP.NET Core
Identity foundation required for owners, device accounts, machines,
heartbeats, runtime sessions, and storage telemetry.

## Why This Story Follows Story 02

The system cannot issue credentials, authorize devices, or persist telemetry
without a stable schema. Story 02 defines the contract surface; this story
builds the persistence and identity substrate that contract depends on.

## Previous Story Reference

- Build on [story-02-api-v1-and-auth-contract-baseline.md](./story-02-api-v1-and-auth-contract-baseline.md).

## Source References

- [docs/domain-model.md](../../../domain-model.md)
- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/product-scope.md](../../../product-scope.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Models/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Models/ReadMe.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Services/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Services/ReadMe.md)

## Dependencies

- Story 01 completed.
- Story 02 completed.
- SQL Server remains the system of record.

## In Scope

- Identity-user extension strategy.
- `DeviceAccount` entity and ownership model.
- Machine, heartbeat, runtime-session, and storage entities.
- EF Core configuration, indexes, and constraints.
- Initial migrations and bootstrap admin setup.

## Out Of Scope

- Platform agent code.
- Owner portal pages.
- Shelly entities and associations beyond placeholders if needed.

## Deliverables

- Repeatable EF Core migrations for the initial schema.
- Identity role and ownership foundations for `Owner` and device-scoped access.
- Data-access seams that later API stories can use without reworking the schema.

## Backend Details

- Implement the Identity and EF Core schema needed for owners, device accounts,
   machines, heartbeats, runtime sessions, and storage telemetry.
- Define indexes, uniqueness rules, migrations, and bootstrap-admin behavior so
   later API work builds on a stable persistence model.
- Expose data-access and query seams that later backend stories can use without
   hiding important domain rules.

## Frontend Details

- No direct portal-page implementation is in scope in this story.
- Preserve stable entity names, ownership semantics, and query intent so later
   portal workflows can be built without renaming or reshaping core concepts.

## Execution Steps

1. Create or refine the application DbContext strategy so ASP.NET Core Identity
   and domain entities coexist without blurring responsibilities.
2. Model `DeviceAccount` as the domain-owned companion to the framework Identity
   schema, including ownership, allowed authentication methods, API-key fields,
   and activity status.
3. Implement the machine-monitoring core entities: `Machine`, `Heartbeat`,
   `RuntimeSession`, and `StorageTelemetry`, including audit columns, required
   uniqueness constraints, and efficient foreign-key relationships.
4. Add indexes and uniqueness rules that reflect the domain model, such as
   durable uniqueness for `AgentId` when present and efficient lookups by
   `MachineId`, `SentAtUtc`, and `ReceivedAtUtc`.
5. Implement the owner bootstrap path so the first administrative account can be
   created safely, then constrained by role-based authorization afterward.
6. Create migrations and verify they apply cleanly to a fresh local SQL Server
   instance and to an update path from an empty baseline.
7. Add repository or query abstractions only where they clarify boundaries.
   Avoid prematurely introducing a generic data-access layer that hides core
   model semantics.
8. Add tests for entity configuration, migration application, uniqueness rules,
   and basic owner and device-account lifecycle operations.

## Validation Steps

- Apply migrations to a clean database.
- Verify the Identity tables and domain tables coexist as intended.
- Confirm the schema can represent shared and dedicated device-account patterns.
- Run data-layer tests that cover key constraints and ownership rules.

## Completion Criteria

- The system can represent owners, device accounts, machines, and telemetry
  history in SQL Server without schema ambiguity.
- Future auth and ingestion stories can build on migrations rather than
  inventing schema incrementally.
- The database foundation matches the documented domain model closely enough
  that later stories can extend it instead of reworking it.
