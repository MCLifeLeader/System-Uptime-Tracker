# Story 06: Owner Administrative API And Portal MVP

## Objective

Implement the first owner-facing management slice end to end: the
owner-authenticated API surface required for administration and machine reads,
plus the portal MVP on the retained Next.js web shell that consumes it.

## Why This Story Follows Story 05

The product scope requires a human administration surface that uses the same
API as devices. That surface is only meaningful once the API can authenticate,
store, and expose real machine data.

## Previous Story Reference

- Build on [story-05-heartbeat-ingestion-and-runtime-sessions.md](./story-05-heartbeat-ingestion-and-runtime-sessions.md).

## Source References

- [docs/product-scope.md](../../../product-scope.md)
- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/admin/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/admin/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/auth/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/auth/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/[...routeparts]/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/[...routeparts]/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/errors/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/errors/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/components/generic/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/components/generic/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/components/generic/hooks/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/components/generic/hooks/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/public/strings/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/public/strings/readme.md)

## Dependencies

- Stories 01 through 05 completed.
- Owner-authenticated admin and read endpoints exposed by the API.

## In Scope

- Owner login flow.
- Session and secure-cookie handling.
- Device-account CRUD workflows.
- Machine-list and machine-detail views.
- Trace-aware, generic error presentation.
- Localization-ready strings and feature-module placement.

## Out Of Scope

- Power-meter administration.
- Advanced dashboarding.
- Direct browser-to-API experiments that bypass the accepted security pattern.

## Deliverables

- Working owner-authenticated administrative and read endpoints.
- Working owner sign-in and sign-out experience.
- Device-account management screens.
- Machine inventory and telemetry-read screens.
- Tests for core portal flows.

## Backend Details

- Implement owner authorization policies and the minimum owner-account login,
   device-account administration, and machine-read endpoints needed by the
   portal MVP.
- Ensure the owner-facing API surface is distinct from telemetry-only routes,
   while still living on the same shared API contract and authorization model.
- Add integration coverage for owner authorization, device-account lifecycle
   operations, machine reads, and trace-aware failure behavior.

## Frontend Details

- Implement owner authentication, secure session handling, typed service
   modules, and the first owner-facing feature modules in the portal.
- Build device-account administration and machine-read views against the real
   shared API surface.
- Apply localization, generic error handling, and portal test coverage to the
   MVP workflows so the admin surface is usable, not just technically present.

## Execution Steps

1. Implement owner authorization policies and the owner-authenticated endpoint
   set required for device-account administration and machine-read workflows.
2. Implement integration tests that verify owner-only access, device-account
   lifecycle behavior, and machine-read behavior over the shared API surface.
3. Implement owner authentication against the API using the retained web auth
   and secure-cookie patterns. Keep long-lived credentials out of browser
   storage.
4. Build typed service modules for the portal's initial admin workflows,
   using server-side fetch or passthrough routes consistently with the accepted
   contract and caching rules.
5. Implement the first admin feature module under the web features structure,
   including screens for listing, creating, disabling, rotating, and removing
   device accounts.
6. Implement owner-readable machine views that show status, last heartbeat,
   current runtime-session state, and key hardware storage metrics.
7. Integrate trace-aware error handling so user-visible failures remain generic
   but can still be correlated to backend logs using trace IDs.
8. Externalize user-facing strings so the new portal surfaces fit the retained
   localization model and do not bury text inside components.
9. Add feature flags only where they protect partially complete admin surfaces.
   Do not use flags as a substitute for finishing the MVP slice.
10. Add unit, integration, and functional tests for login, protected navigation,
   device-account administration, machine data reads, localization loading, and
   error-state handling.

## Validation Steps

- Sign in as an owner in a local environment.
- Exercise the owner-authenticated device-account and machine-read endpoints
   directly or through integration tests.
- Create and manage device accounts through the portal.
- View real machine data produced by Story 05.
- Verify error pages or banners preserve trace IDs without exposing raw backend
  exception details.

## Completion Criteria

- Owners can use the product for real administrative work instead of raw API
  calls.
- The owner-facing backend surface and the portal MVP are both present and
   consistent with one another.
- The retained web starter has become the documented portal surface in fact,
  not just in name.
- The portal and API now share one enforceable authorization model.
