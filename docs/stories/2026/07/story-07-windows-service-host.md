# Story 07: Windows Service Host

## Objective

Create the Windows service host as a thin wrapper over the shared agent runtime,
including installation guidance, local state paths, and packaging validation.

## Why This Story Follows Story 06

The core runtime and monitoring path already exist by Story 05, and the portal
exists by Story 06. The next step is to make the product deployable on one of
its first-class target platforms.

## Previous Story Reference

- Build on [story-06-owner-portal-mvp.md](./story-06-owner-portal-mvp.md).

## Source References

- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/product-scope.md](../../../product-scope.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md)

## Dependencies

- Stories 01 through 06 completed.
- Shared agent runtime available from Story 04.

## In Scope

- Windows-specific host project.
- Windows Service Control Manager integration.
- Secure local configuration and state storage.
- Install, uninstall, upgrade, and smoke-test guidance.

## Out Of Scope

- Linux packaging.
- Power-meter polling.
- Owner portal changes beyond minor admin guidance if needed.

## Deliverables

- `SystemUptimeTracker.WindowsService` project.
- Windows installation documentation or automation.
- Packaging smoke tests.

## Backend Details

- Implement the Windows-specific host, lifecycle integration, local state
   handling, and packaging path on top of the shared agent runtime.
- Validate service behavior, restart recovery, and operator-facing diagnostics
   on Windows.

## Frontend Details

- No direct portal feature implementation is required in this story.
- Any owner-facing guidance added for Windows provisioning should stay limited
   to documentation or minor administrative copy, not new portal workflow scope.

## Execution Steps

1. Create the Windows host project and wire it to the shared agent runtime
   without duplicating runtime behavior inside the host.
2. Implement Windows-specific telemetry collectors, filesystem path resolution,
   service lifecycle handling, and any event-log integration required for basic
   diagnostics.
3. Define the local configuration and secret-storage approach for machine
   identity, token bootstrap, retry queue, and log locations.
4. Add installation and uninstallation automation or scripted guidance,
   including service account expectations and filesystem permissions.
5. Verify graceful service start, stop, restart, and failure recovery behavior.
6. Validate that retry state and identity state survive service restarts and
   host reboots.
7. Add packaging smoke tests and documentation for initial provisioning,
   upgrades, and log collection.
8. Feed any Windows-specific operational requirements into Story 09 rather than
   inventing a parallel runbook track.

## Validation Steps

- Install the service on a Windows test machine or equivalent environment.
- Confirm the service starts automatically and publishes heartbeats.
- Restart the service and verify local state recovery.
- Run packaging smoke tests for install and uninstall.

## Completion Criteria

- Windows becomes a real supported deployment target, not just a design claim.
- The host remains thin and reusable changes still belong in the shared runtime.
- Operators have enough guidance to provision the first Windows machine safely.
