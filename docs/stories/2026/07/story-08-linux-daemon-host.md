# Story 08: Linux Daemon Host

## Objective

Create the Ubuntu systemd-managed daemon host as a thin wrapper over the shared
agent runtime, with Linux-specific configuration, service management, and
packaging guidance.

## Why This Story Follows Story 07

The architecture names Windows and Ubuntu as the first supported operating
systems. Story 07 proves the host pattern on Windows; this story completes the
cross-platform monitoring baseline.

## Previous Story Reference

- Build on [story-07-windows-service-host.md](./story-07-windows-service-host.md).

## Source References

- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/product-scope.md](../../../product-scope.md)

## Dependencies

- Stories 01 through 07 completed.
- Shared agent runtime available from Story 04.

## In Scope

- Linux host project.
- systemd unit definition and service management.
- Linux filesystem layout for config, logs, state, and retry data.
- Packaging and smoke-test guidance.

## Out Of Scope

- Windows host changes other than shared-runtime fixes.
- Power-meter polling.

## Deliverables

- `SystemUptimeTracker.LinuxDaemon` project.
- systemd unit and installation guidance.
- Linux deployment smoke tests.

## Backend Details

- Implement the Ubuntu host, systemd integration, Linux filesystem layout, and
   daemon packaging path on top of the shared agent runtime.
- Validate reboot recovery, restart behavior, and offline retry handling in a
   Linux environment.

## Frontend Details

- No direct portal feature implementation is required in this story.
- Any owner-facing Linux deployment guidance should remain documentation-level
   unless a later story explicitly brings it into the portal.

## Execution Steps

1. Create the Linux host project and connect it to the shared runtime without
   forking business logic from the Windows host.
2. Implement Linux-specific telemetry collectors, path resolution, and host
   lifecycle behavior needed for systemd environments.
3. Define the daemon's install directory, state directory, and permissions
   model, including least-privilege filesystem access.
4. Create the systemd unit file and define restart behavior, environment-file
   handling, dependency ordering, and logging expectations.
5. Add installation and upgrade documentation or automation for Ubuntu.
6. Verify graceful startup, shutdown, restart, reboot recovery, and offline
   retry behavior under systemd.
7. Add Linux packaging smoke tests and basic diagnostics guidance.
8. Feed any shared operational improvements back into Story 09 rather than
   creating isolated Linux-only conventions unnecessarily.

## Validation Steps

- Install the daemon on a Linux test environment.
- Confirm systemd can start, stop, and restart it reliably.
- Verify heartbeats resume after daemon or machine restarts.
- Run smoke tests for install and removal paths.

## Completion Criteria

- Ubuntu becomes a real supported monitoring target.
- Linux-specific concerns are isolated to the host layer as intended.
- Cross-platform support remains one runtime with two deployment shells.
