# Story 10: Power-Meter Domain And API-Key Auth

## Objective

Extend the platform for independent power-meter registration and constrained
device authentication, while preserving the machine-monitoring baseline as a
first-class path that does not require Shelly support.

## Why This Story Follows Story 09

The product is operationally viable for computer monitoring after Story 09.
Power support is intentionally additive, so it begins only after the core path
is stable.

## Previous Story Reference

- Build on [story-09-operations-observability-and-deployment-hardening.md](./story-09-operations-observability-and-deployment-hardening.md).

## Source References

- [docs/product-scope.md](../../../product-scope.md)
- [docs/domain-model.md](../../../domain-model.md)
- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/admin/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/admin/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md)

## Dependencies

- Stories 01 through 09 completed.
- Device-account and owner-management flows already working.

## In Scope

- `PowerMeter` and `PowerReading` schema additions.
- Independent power-meter registration endpoints.
- Device-account API-key issuance, one-time reveal, revoke, and rotate flows.
- Basic Auth validation for constrained devices.

## Out Of Scope

- Shelly polling implementation.
- Association and location workflows.
- Rich portal power dashboards.

## Deliverables

- Persisted power-meter model.
- Secure API-key management workflow.
- Basic Auth ingestion path with hardened validation.
- Owner-facing portal support for issuing, revealing once, rotating, and
   revoking API keys.

## Backend Details

- Extend the schema and API surface for power-meter registration and hashed API
   key lifecycle management.
- Implement the Basic Auth validation path and hardening controls so
   constrained devices can authenticate safely.
- Expose owner-authorized endpoints that support one-time API-key reveal and
   later revocation or rotation behavior.

## Frontend Details

- Extend the existing device-account administration surface so owners can issue,
   reveal once, rotate, and revoke API keys from the portal instead of dropping
   to raw API tooling.
- If full power-meter management UX is deferred to Story 12, at least expose
   the minimal portal hooks required to consume the new key-management contract
   safely and consistently.

## Execution Steps

1. Extend the data model with `PowerMeter` and `PowerReading` entities, keeping
   registration independent from machines as required by the domain rules.
2. Add any new `DeviceAccount` fields or behaviors needed to support long-lived,
   hashed, revocable API keys alongside JWT-capable accounts.
3. Implement owner-authorized endpoints for creating, rotating, viewing once,
   disabling, and revoking API keys for constrained-device accounts.
4. Implement the Basic Auth authentication handler so it validates hashed API
   keys, applies the same restricted telemetry authorization scope as JWT
   devices, and never stores or logs plaintext keys.
5. Apply rate limiting and lockout or equivalent defensive controls on the
   Basic Auth path so it is not a weaker side door into the ingestion API.
6. Implement independent power-meter registration endpoints and basic read
   endpoints that let owners see the registered meter inventory.
7. Add tests for API-key lifecycle behavior, one-time reveal semantics,
   revocation, invalid-credential handling, and independent power-meter create
   and update flows.
8. Extend the owner portal's device-account administration workflows so owners
   can issue, reveal once, rotate, and revoke API keys without leaving the
   portal.
9. Add portal tests that prove one-time key reveal, revocation, and error
   handling behave correctly and do not leak key material after the initial
   display.

## Validation Steps

- Create a constrained-device account and issue an API key.
- Issue, reveal once, and revoke an API key through the portal workflow.
- Verify the plaintext key is only displayed once.
- Authenticate a test request through Basic Auth and confirm telemetry-only
  authorization.
- Register a power meter without attaching it to any machine.

## Completion Criteria

- Power meters are first-class domain records.
- Constrained devices have a secure, supported authentication path.
- Owners can manage constrained-device credentials through the supported admin
   surface rather than through raw backend calls.
- The power path still remains optional and separate from core machine
  monitoring.
