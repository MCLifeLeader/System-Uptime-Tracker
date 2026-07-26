# Story 01: Solution Topology Alignment

## Objective

Align the current repository's starter solution to the documented target
product architecture so later stories can build product behavior without first
untangling project boundaries.

## Why This Story Comes First

The planning documents describe deployables and shared libraries that do not
yet cleanly match the current solution. The current repo contains an API
starter, a retained Next.js web shell, an Aspire AppHost, a broad `Common`
library, a single `Tests` project, and no dedicated Windows agent, Linux agent,
  or agent-core projects. All later work depends on fixing that mismatch.

## Previous Story Reference

- None. This story starts the sequence.

## Source References

- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/product-scope.md](../../../product-scope.md)
- [src/readme.md](../../../../src/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Common/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Common/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/wwwroot/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/wwwroot/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Tests/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Tests/readme.md)

## Current Implementation Anchors

- [SystemUptimeTracker.sln](../../../../SystemUptimeTracker.sln)
- [src/SystemUptimeTracker/SystemUptimeTracker.AppHost/AppHost.cs](../../../../src/SystemUptimeTracker/SystemUptimeTracker.AppHost/AppHost.cs)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/SystemUptimeTracker.Api.csproj](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/SystemUptimeTracker.Api.csproj)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/package.json](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/package.json)

## Dependencies

- No prior implementation story.
- Requires agreement that the current `SystemUptimeTracker.Web` project will
  serve the documented portal role unless a deliberate rename is performed.

## In Scope

- Project and solution restructuring.
- Project naming alignment.
- Shared library boundary definition.
- Test-project topology changes.
- README and local architecture notes needed to make the new shape clear.

## Out Of Scope

- Implementing telemetry domain behavior.
- Implementing owner workflows.
- Implementing service installers.
- Implementing Shelly support.

## Deliverables

- A solution structure that matches the documented product architecture closely
  enough for feature work to proceed.
- A clear decision about whether `SystemUptimeTracker.Web` remains the portal
  name or is renamed to `SystemUptimeTracker.Portal`.
- Dedicated project placeholders for `Contracts`, `Agent.Core`, Windows host,
  Linux host, and power integration seams.
- Replaced or decomposed starter projects where their current scope is too
  broad.

## Backend Details

- Define the ownership boundaries for API, data, contracts, shared runtime,
  power-provider libraries, and host applications.
- Resolve whether broad starter libraries such as `SystemUptimeTracker.Common`
  should be decomposed so backend responsibilities do not stay mixed together.
- Establish test-project placement for API, data, shared-runtime, and host
  behavior before product code starts to accumulate.

## Frontend Details

- Decide whether `SystemUptimeTracker.Web` stays as the project name for the
  documented portal role or is renamed later as part of topology alignment.
- Define where portal features, services, typed API clients, and portal tests
  belong so frontend work does not spread into backend projects.
- Preserve the retained Next.js shell, localization seams, and portal-specific
  test structure as first-class parts of the target architecture.

## Execution Steps

1. Inventory the current solution and map every existing project to one of four
   buckets: keep as-is, keep but narrow, rename, or replace.
2. Decide the canonical product topology and record it in a short architecture
   note. At minimum, account for `Api`, `Web` or `Portal`, `Data`,
   `Contracts`, `Agent.Core`, `WindowsService`, `LinuxDaemon`,
   `Power.Shelly`, `ServiceDefaults`, `AppHost`, and targeted test projects.
3. Create any missing projects as empty but buildable shells with README files
   describing their ownership boundaries.
4. Break the current `SystemUptimeTracker.Common` project apart where needed.
   Move API-only or agent-only concerns into the projects that should own them,
   leaving only true cross-solution building blocks in any retained common
   library.
5. Replace the single broad `SystemUptimeTracker.Tests` project with the test
   layout documented in [docs/architecture-overview.md](../../../architecture-overview.md).
   If the full target matrix is too large for one story, at least split the
   test project into domain-specific project shells and migrate the starter test
   conventions forward.
6. Verify the solution still builds and that the Aspire AppHost continues to
   orchestrate the retained API and web projects after any renames or moves.
7. Update root and project-level documentation to explain the new structure,
   including how the current Next.js web shell satisfies the documented portal
   requirement.
8. Capture any intentionally deferred topology cleanups as explicit follow-on
   tasks in the story notes rather than leaving them implicit.

## Validation Steps

- Build the solution after restructuring.
- Launch the AppHost and confirm it still starts the API and web projects.
- Confirm all new project references resolve cleanly in the solution.
- Confirm documentation no longer leaves ambiguity about where new code should
  land.

## Completion Criteria

- The solution shape no longer conflicts with the documented product shape in a
  way that would block later stories.
- Every later story has a clear home for its code.
- The current starter assets that remain are explicitly retained for a reason,
  not by accident.
