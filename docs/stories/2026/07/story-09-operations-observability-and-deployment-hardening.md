# Story 09: Operations, Observability, And Deployment Hardening

## Objective

Make the API, portal, and agent hosts operationally deployable by finalizing
configuration rules, health checks, diagnostics, secure environment handling,
and runbook-quality deployment guidance.

## Why This Story Follows Story 08

By this point the product has core monitoring behavior and both target agent
hosts. The next gap is operational readiness: being able to deploy, diagnose,
and support the system reliably.

## Previous Story Reference

- Build on [story-08-linux-daemon-host.md](./story-08-linux-daemon-host.md).

## Source References

- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/product-scope.md](../../../product-scope.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/logging/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/logging/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Tests/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Tests/readme.md)

## Current Implementation Anchors

- [src/SystemUptimeTracker/SystemUptimeTracker.AppHost/AppHost.cs](../../../../src/SystemUptimeTracker/SystemUptimeTracker.AppHost/AppHost.cs)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/package.json](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/package.json)

## Dependencies

- Stories 01 through 08 completed.

## In Scope

- App configuration hardening.
- Health checks and structured telemetry.
- Trace correlation across API, portal, and agents.
- Deployment-time validation.
- Runbooks for start, stop, upgrade, and diagnostics.
- QA automation and smoke-test alignment.

## Out Of Scope

- New business workflows unrelated to operations.
- Direct Shelly support.

## Deliverables

- Production-ready configuration model.
- Operational runbooks.
- Hardened deployment validation and observability baselines.

## Backend Details

- Finalize API and host configuration validation, health checks, structured
   logs, metrics, and trace correlation.
- Harden production-facing infrastructure concerns such as CORS, forwarded
   headers, rate limiting, and startup validation.
- Align automation and smoke-test coverage with the real backend runtime.

## Frontend Details

- Finalize portal deployment packaging, origin strategy, and secure
   session-handling expectations.
- Ensure portal logs and user-visible failures preserve trace IDs that can be
   followed into API and host diagnostics.
- Add portal deployment smoke tests and operator-facing troubleshooting notes
   for proxy, auth, and environment-configuration failures.

## Execution Steps

1. Finalize the configuration contract for API, portal, Windows host, and Linux
   host. Make environment-specific values explicit and keep secrets out of the
   repository.
2. Validate that the AppHost and ServiceDefaults setup still serves local
   orchestration needs without becoming an implicit production dependency.
3. Implement or refine health endpoints for app, database, and critical
   dependency readiness, and document how operators should interpret them.
4. Ensure structured logging, metrics, and traces preserve a correlation path
   from owner-browser errors to portal server logs to API logs and, where
   relevant, agent logs.
5. Add configuration validation at startup so missing connection strings,
   signing keys, or critical URLs fail fast and diagnostically.
6. Harden deployment details for CORS, forwarded headers, cookie handling,
   CSRF protections where cookies are used, and rate limiting around the most
   exposed authentication and ingestion edges.
7. Write or update runbooks for first deployment, certificate setup, start,
   stop, restart, upgrade, token issues, database connectivity failures, and
   basic telemetry troubleshooting.
8. Align existing QA automation and smoke-test projects with the now-real
   monitoring flows so deployments can be validated automatically.

## Validation Steps

- Run a local orchestrated environment and verify diagnostics appear in logs and
  traces across web and API boundaries.
- Intentionally misconfigure critical settings and confirm startup validation
  fails clearly.
- Execute smoke tests for login, heartbeat, and basic admin workflows.
- Review the runbooks to confirm they are executable by someone who did not
  write the code.

## Completion Criteria

- The system is diagnosable and deployable, not just runnable in a dev shell.
- Operators have a documented path for first-line troubleshooting.
- Later power stories inherit a stable operational baseline.
