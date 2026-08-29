# Story 04: Shared Contracts And Agent Core

## Objective

Create the shared telemetry-contract project and platform-neutral agent core so
Windows and Linux hosts can remain thin shells over one runtime behavior model.

## Why This Story Follows Story 03

The schema and auth model now exist. The next dependency is a stable shared
runtime and contract layer so the heartbeat path can be implemented once and
hosted twice.

## Previous Story Reference

- Build on [story-03-identity-and-persistence-foundation.md](./story-03-identity-and-persistence-foundation.md).

## Source References

- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/product-scope.md](../../../product-scope.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Common/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Common/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Tests/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Tests/readme.md)

## Dependencies

- Stories 01 through 03 completed.
- Accepted API contract from Story 02.

## In Scope

- Shared DTOs or contract models.
- Agent scheduling and publishing abstractions.
- Local identity and token-state persistence abstractions.
- Retry-queue abstraction and storage strategy.
- Telemetry collector interfaces for OS-specific implementations.

## Out Of Scope

- Final Windows or Linux service packaging.
- Portal UI behavior.
- Power-meter polling implementation.

## Deliverables

- `SystemUptimeTracker.Contracts` project.
- `SystemUptimeTracker.Agent.Core` project.
- Tests for serialization, scheduling, retry logic, and state transitions.

## Backend Details

- Create the shared contract models used by the API and hosts.
- Implement the platform-neutral agent runtime, including scheduling, retry,
   identity state, token refresh, publishing, and OS abstraction seams.
- Add serialization and runtime tests that keep agent and API payload behavior
   aligned over time.

## Frontend Details

- No direct portal UI implementation is in scope in this story.
- Publish contract shapes and versioning rules clearly enough that portal-side
   request or response validators can later mirror the same payload semantics.

## Execution Steps

1. Create the shared contract project and add the API request and response
   models that must be shared across agent and API code.
2. Define clear versioning rules inside the contracts project so payload
   evolution remains compatible with `/api/v1`.
3. Create the platform-neutral agent runtime with abstractions for scheduling,
   heartbeat capture, local state, credential refresh, outbound publishing,
   retry, and graceful shutdown.
4. Decide the initial retry-queue implementation strategy, even if the precise
   storage technology remains configurable. Encode limits and backoff policies as
   settings, not hard-coded constants buried inside the runtime.
5. Introduce interfaces for OS-specific telemetry capture, filesystem paths,
   and service-host lifecycle hooks so the shared runtime does not absorb
   platform conditionals.
6. Add local identity-state handling that can persist `AgentId`, issued tokens,
   refresh metadata, and retry-queue state securely.
7. Add focused unit tests for retry rules, scheduling cadence, token refresh
   decision logic, serialization stability, and offline-state transitions.
8. Add a small end-to-end harness that proves the shared runtime can run against
   a fake publisher before any real Windows or Linux host is introduced.

## Validation Steps

- Build the shared projects independently of the final hosts.
- Run unit tests for contract serialization and runtime scheduling.
- Confirm the shared runtime has no Windows-only or Linux-only dependencies.
- Confirm later host stories can reference this project without copying logic.

## Completion Criteria

- The shared runtime owns the behavior documented in the architecture plan.
- The Windows and Linux stories can remain thin-host stories instead of runtime
  redesign stories.
- Shared contracts stop API and agent code from diverging on payload shape.
