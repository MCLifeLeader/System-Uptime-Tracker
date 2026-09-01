# Windows Service Implementation Reference

## Purpose

The local `C:\Code\Personal\FamilyTools` repository contains a working .NET
Windows background service and PowerShell installer. It is a design reference
for `SystemUptimeTracker.WindowsService`, not a source dependency and not a
production-ready package to copy unchanged.

## Reference Surfaces

The relevant FamilyTools implementation is under
`src/ComputerTaskHandler/Task.Handler.Client`:

- `Message.Handler.Client.csproj` targets Windows x64, publishes a single-file
  executable, references `Microsoft.Extensions.Hosting.WindowsServices`, and
  includes the installer in its build output.
- `RegisterDependentServices.cs` connects the .NET host to Windows Service
  Control Manager and assigns the configured service name.
- `ServicesResolver.cs` registers a `BackgroundService` through
  `AddHostedService` while keeping worker behavior behind an interface.
- `RemoteHostServerService.cs` demonstrates cancellation-aware startup,
  execution, and shutdown lifecycle hooks.
- `Install_Service.ps1` demonstrates an idempotent create-or-update flow that
  stops an existing service, deploys files, registers the service when needed,
  and starts it.

## Patterns To Adopt

The System Uptime Tracker implementation should adapt these patterns:

1. Create a dedicated `SystemUptimeTracker.WindowsService` host project that
   contains Windows hosting and packaging concerns only. Heartbeat collection,
   retry, identity, and publishing behavior remains in the shared agent core.
2. Use the repository's .NET version and
   `Microsoft.Extensions.Hosting.WindowsServices`; configure a stable
   `SystemUptimeTracker` service name that exactly matches installer
   registration.
3. Publish an explicit `win-x64` artifact as the self-contained, single-file
   executable already selected in the root architecture.
4. Ship an advanced PowerShell installer in the artifact. Use named, validated
   parameters and make install or upgrade safe to rerun.
5. Support bounded stop/start status polling, service creation or update,
   startup mode, recovery actions, event-log integration, and explicit failure
   reporting.
6. Store configuration, durable agent identity, retry state, and logs outside
   the replaceable application directory. Apply least-privilege ACLs for the
   selected service identity and never write credentials to installer output.
7. Preserve the previous executable package until the replacement service has
   started successfully so a failed upgrade can be rolled back.
8. Add packaging tests that inspect the published artifact and installer tests
   that exercise install, repeat install, upgrade, start, stop, rollback, and
   uninstall behavior on a disposable Windows environment.

## Installer Contract

The initial deployment identifiers are:

| Setting | Default |
|---|---|
| Service name | `SystemUptimeTrackerAgent` |
| Display name | `System Uptime Tracker Agent` |
| Executable | `SystemUptimeTracker.WindowsService.exe` |
| Application root | `C:\Program Files\SystemUptimeTracker\Agent` |
| Release root | `C:\Program Files\SystemUptimeTracker\Agent\releases` |
| Durable data root | `C:\ProgramData\SystemUptimeTracker\Agent` |
| Service identity | `NT AUTHORITY\LocalService` by default |

`Install-SystemUptimeTrackerWindowsService.ps1` uses its own directory as the
package source, following FamilyTools. It must expose named parameters for the
package version and any supported overrides rather than positional `$args`.
Credentials are outside the installer contract and must be provisioned through
an ACL-protected mechanism that does not expose them in process arguments,
transcripts, or logs.

For both first install and upgrade, the installer validates all inputs before
changing the machine, stages the package into a versioned release directory,
stops an existing service with a bounded wait, creates or updates the service,
applies startup, recovery, identity, and ACL settings, starts the service, and
waits for an observable startup signal. If startup fails, it restores the
previous binary path and restarts the previous release.

`Uninstall-SystemUptimeTrackerWindowsService.ps1` removes the service and
application releases. It retains `ProgramData` by default; deleting identity,
retry, or diagnostic state requires an explicit purge switch and confirmation.

## Patterns To Modernize

The FamilyTools project is useful evidence for the deployment shape, but the
following details should not be copied:

- Positional installer arguments and unvalidated paths. Use a `param` block,
  validation attributes, `CmdletBinding`, and `SupportsShouldProcess`.
- Fixed sleeps while waiting for service state. Poll with a timeout and report
  the state that prevented progress.
- Deleting the active installation before the replacement is known to be
  usable. Stage, validate, switch, health-check, and retain rollback material.
- Logging complete configuration objects, because they may contain credentials
  or endpoint secrets.
- Blocking asynchronous work with `Task.WaitAll`. Await worker tasks and pass
  cancellation tokens through every I/O and delay operation.
- Ignoring background-service exceptions or calling `Environment.Exit` from
  worker logic. Let the host apply an explicit failure policy so Windows
  Service recovery can restart a failed process.
- Running under an unspecified identity. Require an explicit service-account
  decision, deny interactive logon where applicable, and grant only the file,
  event-log, and network permissions the agent needs.

## Expected Deliverables

Phase 2 Windows packaging is complete when the repository contains:

- A Windows service host project and focused lifecycle tests.
- A reproducible publish command in the build pipeline.
- Install, upgrade, rollback, and uninstall PowerShell entry points.
- A versioned artifact containing the executable, installer, configuration
  template, and operator README.
- A disposable Windows packaging test that verifies service registration,
  repeat installation, automatic startup, recovery behavior, clean shutdown,
  failed-upgrade rollback, uninstall, and retained state across upgrades.
