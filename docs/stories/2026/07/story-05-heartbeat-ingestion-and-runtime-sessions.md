# Story 05: Heartbeat Ingestion And Runtime Sessions

## Objective

Deliver the first complete machine-monitoring vertical slice: device
authentication, machine registration or resolution, heartbeat ingestion,
storage-telemetry persistence, and runtime-session reconstruction.

## Why This Story Follows Story 04

Stories 01 through 04 establish topology, contracts, schema, and shared agent
runtime. This story turns those foundations into the first product capability
that the system must provide.

## Previous Story Reference

- Build on [story-04-shared-contracts-and-agent-core.md](./story-04-shared-contracts-and-agent-core.md).

## Source References

- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/domain-model.md](../../../domain-model.md)
- [docs/product-scope.md](../../../product-scope.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Controllers/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Controllers/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Services/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Services/ReadMe.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Factories/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Factories/ReadMe.md)

## Dependencies

- Stories 01 through 04 completed.
- Token, refresh, and device-account rules accepted from Story 02.

## In Scope

- Device-authenticated heartbeat ingestion.
- Machine resolution and upsert behavior.
- Storage-telemetry persistence.
- Runtime-session creation, continuation, and closure rules.
- Idempotency and retry-safe ingestion behavior.

## Out Of Scope

- Owner portal pages.
- Windows or Linux packaging specifics.
- Power readings.

## Deliverables

- Working ingestion endpoints.
- Runtime-session derivation logic with test coverage.
- A local end-to-end flow from shared agent runtime to SQL persistence.

## Backend Details

- Implement device-authenticated ingestion endpoints, machine resolution,
   storage-telemetry persistence, and runtime-session derivation.
- Add idempotency, retry-safe behavior, authorization checks, and diagnostic
   logging to the first monitoring vertical slice.
- Validate the complete backend and agent path against SQL persistence and
   outage-retry scenarios.

## Frontend Details

- No direct owner-portal UI work is in scope in this story.
- Backend outputs from this story should be stable enough that the next story
   can build owner-facing administrative and read workflows against real data
   rather than mocks.

## Execution Steps

1. Implement the ingestion controllers and services as thin controller surfaces
   over application logic, following the API project folder guidance.
2. Implement device-account authorization checks that ensure telemetry-scoped
   callers cannot reach administrative operations and can only submit allowed
   telemetry.
3. Add machine resolution logic that can create a previously unknown machine or
   update an existing one based on the accepted registration rules and durable
   `AgentId` semantics.
4. Persist heartbeat payloads and storage details in a retry-safe way. Handle
   duplicate or replayed submissions explicitly instead of leaving behavior to
   database accidents.
5. Implement runtime-session logic using the agreed thresholds for heartbeat
   interval, offline gap, session-break behavior, and end-reason calculation.
6. Make the shared agent runtime call the real token and heartbeat endpoints,
   including token refresh and outage retry handling.
7. Add integration tests that exercise device authentication, heartbeat ingest,
   session continuity, timeout-driven session breaks, duplicate submissions,
   and storage-telemetry persistence.
8. Add operator-observable logs and health signals so failures in the first
   monitoring path are diagnosable.

## Validation Steps

- Start the API locally, run a test agent flow, and verify machine and
  heartbeat records appear in SQL Server.
- Run tests that prove runtime-session rules behave correctly for gaps,
  restarts, and clean continuation.
- Confirm ingestion remains idempotent under simulated retries.

## Completion Criteria

- A Windows or Linux host can eventually use the shared agent runtime to submit
  authenticated telemetry.
- The API stores heartbeat history and derives sessions instead of treating
  heartbeats as disconnected events.
- The product has its first end-to-end useful capability.
