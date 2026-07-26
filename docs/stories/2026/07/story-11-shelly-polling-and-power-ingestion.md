# Story 11: Shelly Polling And Power Ingestion

## Objective

Implement agent-mediated Shelly Plug US Gen4 polling, response normalization,
and power-reading ingestion without changing the established machine-monitoring
contract.

## Why This Story Follows Story 10

Story 10 creates the power-meter domain and constrained-device auth model.
Story 11 uses that foundation to add the first concrete power provider.

## Previous Story Reference

- Build on [story-10-power-meter-domain-and-api-key-auth.md](./story-10-power-meter-domain-and-api-key-auth.md).

## Source References

- [docs/product-scope.md](../../../product-scope.md)
- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/domain-model.md](../../../domain-model.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)

## Dependencies

- Stories 01 through 10 completed.
- Shared agent runtime from Story 04.

## In Scope

- `SystemUptimeTracker.Power.Shelly` provider implementation.
- Agent-side polling configuration.
- Shelly response normalization.
- API storage path for power readings.
- Power-reading idempotency and diagnostics.

## Out Of Scope

- Direct Shelly-to-API ingestion.
- Location and device association workflows.
- Final portal power-management screens.

## Deliverables

- Working agent-side Shelly polling provider.
- Power-reading ingestion endpoint or accepted transport variant.
- Stored and queryable power readings.

## Backend Details

- Implement the Shelly provider library, the agent polling path, and the API
   ingestion and storage behavior for normalized power readings.
- Keep polling optional, retry-safe, and isolated from the machine-heartbeat
   critical path.
- Add diagnostics and integration coverage for polling, normalization, and
   storage behavior.

## Frontend Details

- No direct portal feature implementation is required in this story.
- Backend outputs from this story should make later portal power-data views and
   association workflows possible without revisiting payload semantics.

## Execution Steps

1. Create the Shelly provider library and isolate vendor-specific logic there,
   rather than scattering RPC handling across the shared agent runtime.
2. Implement local Shelly polling over HTTP RPC, including timeout handling,
   authentication-reference resolution, and safe retry behavior.
3. Normalize the Shelly response into the system's accepted power-reading
   contract and preserve raw payloads only where they aid supportability.
4. Implement the chosen API transport for power readings and keep it compatible
   with the existing telemetry-auth rules.
5. Persist power readings with duplicate protection, source correlation, and
   basic validation of meter identity.
6. Make the agent runtime treat Shelly polling as optional. Failures in polling
   must not break basic machine heartbeat submission.
7. Add integration tests for polling success, polling failure, normalized
   ingestion, duplicate-reading behavior, and disabled or unreachable meters.
8. Document the operator setup needed to add a Shelly device to an existing
   monitored machine or to a collector-only agent.

## Validation Steps

- Poll a test Shelly device or mock and ingest normalized readings.
- Confirm machine-only monitoring still works with no Shelly configuration.
- Confirm a polling failure does not stop heartbeat publishing.
- Verify stored readings can be queried back through the API.

## Completion Criteria

- Shelly support exists as an additive capability.
- The shared agent runtime remains resilient when the power path fails.
- Power readings are normalized into the platform's storage model rather than
  living as vendor-specific blobs.
