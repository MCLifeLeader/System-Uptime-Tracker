# July 2026 Story Set

## Purpose

This story set converts the repository's current documentation and starter
projects into a sequenced implementation backlog for delivering the first
usable System Uptime Tracker product.

## Planning Principles

- Start from the current repository baseline, not the idealized end state.
- Reconcile the current starter solution to the target architecture before
  adding product-specific behavior.
- Keep every story independently completable, while making each story consume
  the outputs of the prior story.
- Treat API, web, data, agent, packaging, and operations work as one connected
  delivery stream.
- State backend details and frontend details explicitly in each story. If one
   lane is intentionally out of scope for a story, say that directly rather than
   leaving it implicit.

## Source Set Used

- [docs/README.md](../../../README.md)
- [docs/product-scope.md](../../../product-scope.md)
- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/domain-model.md](../../../domain-model.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/inital-spec.md](../../../inital-spec.md)
- [src/readme.md](../../../../src/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Common/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Common/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Controllers/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Controllers/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Services/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Services/ReadMe.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Models/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Models/ReadMe.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Factories/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Factories/ReadMe.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/wwwroot/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/wwwroot/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/logging/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/logging/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/errors/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/errors/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/admin/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/admin/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/components/generic/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/components/generic/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/components/generic/hooks/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/components/generic/hooks/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/auth/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/auth/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/[...routeparts]/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/[...routeparts]/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/public/strings/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/public/strings/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Tests/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Tests/readme.md)

## Story Sequence

1. [story-01-solution-topology-alignment.md](./story-01-solution-topology-alignment.md)
   Align the current starter solution to the documented target architecture.
2. [story-02-api-v1-and-auth-contract-baseline.md](./story-02-api-v1-and-auth-contract-baseline.md)
   Lock the API, auth, and portal integration contract before deeper feature
   work starts.
3. [story-03-identity-and-persistence-foundation.md](./story-03-identity-and-persistence-foundation.md)
   Build the SQL Server and ASP.NET Core Identity foundation for devices and
   owners.
4. [story-04-shared-contracts-and-agent-core.md](./story-04-shared-contracts-and-agent-core.md)
   Create the shared telemetry contracts and platform-neutral agent runtime.
5. [story-05-heartbeat-ingestion-and-runtime-sessions.md](./story-05-heartbeat-ingestion-and-runtime-sessions.md)
   Deliver the end-to-end machine heartbeat path and uptime reconstruction.
6. [story-06-owner-portal-mvp.md](./story-06-owner-portal-mvp.md)
   Implement the owner administrative API surface and portal MVP on the shared
   API contract.
7. [story-07-windows-service-host.md](./story-07-windows-service-host.md)
   Add the Windows service host and installer path on top of the shared agent.
8. [story-08-linux-daemon-host.md](./story-08-linux-daemon-host.md)
   Add the Ubuntu systemd daemon host and packaging path.
9. [story-09-operations-observability-and-deployment-hardening.md](./story-09-operations-observability-and-deployment-hardening.md)
   Finalize deployability, diagnostics, and configuration validation.
10. [story-10-power-meter-domain-and-api-key-auth.md](./story-10-power-meter-domain-and-api-key-auth.md)
    Add independent power-meter registration and constrained-device
    authentication.
11. [story-11-shelly-polling-and-power-ingestion.md](./story-11-shelly-polling-and-power-ingestion.md)
    Implement agent-mediated Shelly polling and normalized power ingestion.
12. [story-12-location-associations-and-portal-power-workflows.md](./story-12-location-associations-and-portal-power-workflows.md)
    Add the inventory, location, association, and portal workflows needed for
    power telemetry to be useful.
13. [story-13-reporting-and-extended-ingestion-readiness.md](./story-13-reporting-and-extended-ingestion-readiness.md)
    Prepare the system for alternate ingestion paths, reporting, and approval
    workflows.

## Phase Mapping

- Phase 0: stories 01 and 02
- Phase 1: stories 03 through 06
- Phase 2: stories 07 through 09
- Phase 3: stories 10 through 12
- Phase 4: story 13

## Dependency Rules

- Do not start a later story by assuming outputs that an earlier story did not
  explicitly produce.
- Keep shared abstractions ahead of platform-specific implementations.
- Keep schema, contract, and authorization changes ahead of UI workflows that
  depend on them.
- Keep operational packaging behind at least one working local end-to-end flow.

## Definition Of A Good Story In This Set

- One clear delivery outcome.
- One explicit dependency chain.
- Explicit backend details and frontend details, or an explicit statement that
   one lane is intentionally out of scope.
- Concrete implementation steps tied to current repository seams.
- Validation steps that prove the story is complete.
- Bounded scope so work can finish without borrowing unfinished pieces from a
  later story.
