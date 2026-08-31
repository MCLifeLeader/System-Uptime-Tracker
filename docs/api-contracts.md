# API Contracts (`/api/v1`)

Created under TASK-0201 (2026-08-30). This document is the accepted route
catalog for the initial release: every `/api/v1` route appears exactly once,
with its caller type, authorization policy, status codes, idempotency
behavior, and owning epic. Request/response DTO shapes are defined by the
EPIC-02 contract tasks referenced per group; error shape, headers, and unit
conventions are standardized by TASK-0208; the OpenAPI document and executable
examples are owned by TASK-0209.

Decisions this catalog implements (see
[product-scope.md](./product-scope.md#decisions)):
self-service auto-approved registration (TASK-0001), multiple owners in one
trust domain with no per-owner isolation (TASK-0002), dedicated device account
per machine as the default (TASK-0003), single-use bootstrap credentials
(TASK-0004), accepted timing defaults (TASK-0005), the accepted retry policy
(TASK-0006), and a separate power-readings endpoint with one canonical storage
command (TASK-0007).

## Conventions

- **Base path and versioning:** all routes below live under `/api/v1`. A
  request whose `PayloadVersion` (where the DTO carries one) is not supported
  receives `422` with a Problem Details `type` of
  `urn:systemuptimetracker:error:unsupported-payload-version`.
- **Caller types:**
  - **Owner** — a human principal in the `Owner` role (portal or tooling),
    authenticated with an owner JWT bearer token.
  - **Device** — a `DeviceAccount` principal, authenticated with a device JWT
    bearer token or HTTP Basic Auth with a hashed API key. Both schemes land
    in the same telemetry-only scope.
  - **Anonymous** — no authentication; permitted only for health probes and
    the authentication entry points themselves.
- **Authorization policies** (route-group policies required by TASK-0408):
  - `OwnerOnly` — requires an authenticated `Owner`-role principal.
  - `DeviceTelemetry` — requires an authenticated device principal; grants
    telemetry submission only, never administrative access.
- **Errors (decided, TASK-0208):** RFC 9457 Problem Details
  (`application/problem+json`) on every non-2xx response, enriched with
  `traceId` and `requestId` extension members (see `ErrorContract` in
  `SystemUptimeTracker.Contracts`). Validation failures use `400` with the
  ASP.NET Core `ValidationProblemDetails` shape (an `errors` dictionary of
  field names to messages); authentication failures `401`; authorization
  failures `403`; missing resources `404`; state/uniqueness conflicts `409`;
  oversized payloads `413`; semantic rejections `422`; rate limiting `429`
  (with `Retry-After`); lockout `423`. An unsupported `payloadVersion` is
  `422` with `type` `urn:systemuptimetracker:error:unsupported-payload-version`.
- **Correlation (decided, TASK-0208):** callers propagate context with the
  standard W3C `traceparent` header; the API stamps `X-Trace-Id` (the W3C
  trace identifier) on **every** response, success or error, and the same
  value appears as the `traceId` Problem Details extension. Portal proxying
  preserves `X-Trace-Id` end to end.
- **Timestamps (decided, TASK-0208):** UTC ISO 8601 everywhere. Producers
  must send UTC (`Z` or `+00:00` offsets accepted); the API serializes
  `DateTimeOffset` values with a `+00:00` offset. Non-UTC offsets on
  device-submitted timestamps are rejected as validation errors.
- **Units (decided, TASK-0208):** one convention across all contracts —
  bytes for memory/storage capacity, percent 0–100 for CPU usage and
  estimated shares, watts (active power), volt-amps (apparent power), volts,
  amps, hertz, watt-hours (energy), Celsius (temperature), and seconds for
  every configured duration. Unit names are embedded in wire field names
  (`totalBytes`, `usagePercent`, `activePowerWatts`, `calculatedUptimeSeconds`)
  so payloads are self-describing.
- **Pagination:** list endpoints take `page` (1-based) and `pageSize`
  (default 50, maximum 200), return a bounded page envelope with a total
  count, and use deterministic ordering (documented per endpoint by
  TASK-0205). Telemetry history queries additionally require a bounded UTC
  date window.
- **Retry classification (for agents, TASK-0006/TASK-0804):** network
  failures, `408`, `429`, and `5xx` are retryable; `400`, `401` (after one
  reauthentication attempt), `403`, `404`, `409` (duplicate-create), `413`,
  and `422` are terminal for the queued item.

## Route Catalog

### Health (outside `/api/v1`)

| Route | Caller | Policy | Success | Errors | Idempotency | Owning epic |
|---|---|---|---|---|---|---|
| `GET /health/live` | Anonymous | none | `200` | `503` | Safe/read-only | EPIC-14 (TASK-1403) |
| `GET /health/ready` | Anonymous | none | `200` | `503` | Safe/read-only | EPIC-14 (TASK-1403) |

Health responses expose no sensitive detail to anonymous callers. Health
routes are never rate-limited or authenticated (TASK-0407).

### Authentication And Tokens (EPIC-04)

| Route | Caller | Policy | Success | Errors | Idempotency | Owning epic |
|---|---|---|---|---|---|---|
| `POST /api/v1/auth/owner/login` | Anonymous (owner credentials in body) | none (rate-limited, lockout) | `200` tokens + lifetimes | `400`, `401`, `423`, `429` | Not idempotent: each success issues new tokens | EPIC-04 (TASK-0404) |
| `POST /api/v1/auth/device/login` | Anonymous (device bootstrap credential in body) | none (rate-limited, lockout) | `200` tokens + lifetimes | `400`, `401` (bad or already-used bootstrap credential), `403` (disabled account), `423`, `429` | Not idempotent: the bootstrap credential is single-use and is invalidated by the first success (TASK-0004) | EPIC-04 (TASK-0404) |
| `POST /api/v1/auth/refresh` | Owner or Device (refresh token in body) | none (rate-limited) | `200` rotated token pair | `400`, `401` (expired, revoked, or replayed token), `429` | Not idempotent: rotation invalidates the presented refresh token; replay is rejected | EPIC-04 (TASK-0404) |
| `POST /api/v1/auth/revoke` | Owner or Device | authenticated (either policy) | `204` | `400`, `401` | Idempotent: revoking an already-revoked token still returns `204` | EPIC-04 (TASK-0404) |

Token responses contain access/refresh tokens and lifetime metadata only —
never stored secrets or hashes (TASK-0204 defines the DTOs). Basic Auth
callers do not use these endpoints; they present `Authorization: Basic
base64(deviceAccountName:apiKey)` directly on telemetry routes.

### Device-Account Administration (EPIC-04)

All routes `OwnerOnly`, caller type Owner. Any owner may administer any
device account (TASK-0002). DTOs from TASK-0204/TASK-0205.

| Route | Success | Errors | Idempotency | Owning epic |
|---|---|---|---|---|
| `GET /api/v1/device-accounts` | `200` page | `401`, `403` | Safe/read-only | EPIC-04 (TASK-0403) |
| `POST /api/v1/device-accounts` | `201` + one-time bootstrap credential or API key | `400`, `401`, `403`, `409` (duplicate name) | Not idempotent: repeat create with the same name returns `409` | EPIC-04 (TASK-0403) |
| `GET /api/v1/device-accounts/{id}` | `200` | `401`, `403`, `404` | Safe/read-only | EPIC-04 (TASK-0403) |
| `PUT /api/v1/device-accounts/{id}` | `200` | `400`, `401`, `403`, `404`, `409` | Idempotent for identical payloads | EPIC-04 (TASK-0403) |
| `POST /api/v1/device-accounts/{id}/disable` | `204` | `401`, `403`, `404` | Idempotent: disabling a disabled account returns `204` | EPIC-04 (TASK-0403) |
| `POST /api/v1/device-accounts/{id}/enable` | `204` | `401`, `403`, `404` | Idempotent | EPIC-04 (TASK-0403) |
| `DELETE /api/v1/device-accounts/{id}` | `204` (referencing machines keep history; `Machine.DeviceAccountId` becomes null or is reassigned per request body) | `401`, `403`, `404` | Idempotent: deleting an absent account returns `404` without side effects; repeat delete of the same id is `404` | EPIC-04 (TASK-0403) |
| `POST /api/v1/device-accounts/{id}/credentials/rotate` | `200` + new one-time bootstrap credential; revokes outstanding refresh tokens | `401`, `403`, `404`, `409` (JWT not an allowed method) | Not idempotent: each call issues a new credential and revokes prior tokens | EPIC-04 (TASK-0403/TASK-0404) |
| `POST /api/v1/device-accounts/{id}/api-key/rotate` | `200` + new one-time plaintext key (also used for first issue) | `401`, `403`, `404`, `409` (API key not an allowed method) | Not idempotent: each call replaces the stored hash | EPIC-04 (TASK-0405) |
| `POST /api/v1/device-accounts/{id}/api-key/revoke` | `204` | `401`, `403`, `404` | Idempotent | EPIC-04 (TASK-0405) |

### Machine Registration And Heartbeats (EPIC-05)

| Route | Caller | Policy | Success | Errors | Idempotency | Owning epic |
|---|---|---|---|---|---|---|
| `POST /api/v1/machines/register` | Device | `DeviceTelemetry` | `201` (new machine, assigned `MachineId`), `200` (existing `AgentId` reconciled) | `400`, `401`, `403` (disabled/retired account or machine), `409` (conflict per TASK-0202, e.g. `AgentId` bound to a different account without reassignment), `422` | Idempotent on durable `AgentId`: re-registration returns the same `MachineId` and mutates no lifecycle state (TASK-0001) | EPIC-05 (TASK-0501) |
| `POST /api/v1/heartbeats` | Device | `DeviceTelemetry` | `202` (accepted, persisted), `200` (duplicate replay, no new side effect) | `400`, `401`, `403` (machine not authorized for this account), `409` (same `AgentId + SequenceNumber` with different content), `413`, `422` (including unsupported payload version) | Idempotency key `AgentId + SequenceNumber` (TASK-0207): duplicates persist exactly one heartbeat and one storage-telemetry set | EPIC-05 (TASK-0502/TASK-0503) |

Lifecycle signals (agent start, graceful stop; TASK-0709) travel as optional
fields on the heartbeat payload rather than as separate routes; runtime
sessions remain server-authoritative. The server records `ReceivedAtUtc`
itself and never trusts client receipt times (TASK-0304).

### Owner Machine Administration And Telemetry Reads (EPIC-05 / EPIC-06)

All routes `OwnerOnly`, caller type Owner. DTOs from TASK-0205.

| Route | Success | Errors | Idempotency | Owning epic |
|---|---|---|---|---|
| `GET /api/v1/machines` | `200` page (registration state, last seen, OS, version, assigned account) | `401`, `403` | Safe/read-only | EPIC-05 (TASK-0505) |
| `POST /api/v1/machines` | `201` (owner pre-created record, no `AgentId` yet; TASK-0001) | `400`, `401`, `403`, `409` (duplicate pre-created identity) | Not idempotent: repeat create returns `409` | EPIC-05 (TASK-0501) |
| `GET /api/v1/machines/{id}` | `200` | `401`, `403`, `404` | Safe/read-only | EPIC-05 (TASK-0505) |
| `PUT /api/v1/machines/{id}` | `200` (metadata, device-account assignment) | `400`, `401`, `403`, `404`, `409` | Idempotent for identical payloads | EPIC-05 (TASK-0501) |
| `POST /api/v1/machines/{id}/disable` | `204` | `401`, `403`, `404`, `409` (retired is terminal) | Idempotent | EPIC-05 (TASK-0501) |
| `POST /api/v1/machines/{id}/enable` | `204` | `401`, `403`, `404`, `409` (retired is terminal) | Idempotent | EPIC-05 (TASK-0501) |
| `POST /api/v1/machines/{id}/retire` | `204` (terminal; history retained, `AgentId` never reused) | `401`, `403`, `404` | Idempotent | EPIC-05 (TASK-0501) |
| `GET /api/v1/machines/{id}/heartbeats` | `200` page (bounded UTC window, deterministic time order) | `400` (unbounded window), `401`, `403`, `404` | Safe/read-only | EPIC-05 (TASK-0505) |
| `GET /api/v1/machines/{id}/sessions` | `200` page (current and historical runtime sessions) | `400`, `401`, `403`, `404` | Safe/read-only | EPIC-06 (TASK-0608) |

### Power Meters And Readings (EPIC-12)

| Route | Caller | Policy | Success | Errors | Idempotency | Owning epic |
|---|---|---|---|---|---|---|
| `GET /api/v1/power-meters` | Owner | `OwnerOnly` | `200` page | `401`, `403` | Safe/read-only | EPIC-12 (TASK-1204) |
| `POST /api/v1/power-meters` | Owner | `OwnerOnly` | `201` | `400`, `401`, `403`, `409` (duplicate `Vendor + ExternalDeviceId` or MAC) | Not idempotent: repeat create returns `409` | EPIC-12 (TASK-1204) |
| `GET /api/v1/power-meters/{id}` | Owner | `OwnerOnly` | `200` (current state incl. last seen) | `401`, `403`, `404` | Safe/read-only | EPIC-12 (TASK-1204/TASK-1207) |
| `PUT /api/v1/power-meters/{id}` | Owner | `OwnerOnly` | `200` | `400`, `401`, `403`, `404`, `409` | Idempotent for identical payloads | EPIC-12 (TASK-1204) |
| `POST /api/v1/power-meters/{id}/disable` | Owner | `OwnerOnly` | `204` | `401`, `403`, `404`, `409` (retired is terminal) | Idempotent | EPIC-12 (TASK-1204) |
| `POST /api/v1/power-meters/{id}/enable` | Owner | `OwnerOnly` | `204` | `401`, `403`, `404`, `409` (retired is terminal) | Idempotent | EPIC-12 (TASK-1204) |
| `POST /api/v1/power-meters/{id}/retire` | Owner | `OwnerOnly` | `204` (terminal) | `401`, `403`, `404` | Idempotent | EPIC-12 (TASK-1204) |
| `GET /api/v1/power-meters/{id}/readings` | Owner | `OwnerOnly` | `200` page (bounded UTC window, deterministic time order, no duplicates) | `400`, `401`, `403`, `404` | Safe/read-only | EPIC-12 (TASK-1207) |
| `POST /api/v1/power-readings` | Device | `DeviceTelemetry` | `202` (accepted), `200` (duplicate replay) | `400`, `401`, `403` (reporting machine not authorized for the meter relationship, TASK-1308), `404` (unknown meter identity), `409` (same idempotency key, different content), `413`, `422` | Idempotency key: meter identity + `MessageId` (TASK-0207); every ingestion path (agent polling now, direct paths later) normalizes into this one canonical storage command (TASK-0007) | EPIC-12 (TASK-1205) |

### Locations, Monitored Devices, And Associations (EPIC-13)

All routes `OwnerOnly`, caller type Owner. Effective-dated association rules
(non-overlapping active primaries, valid ranges) are enforced transactionally
(TASK-1306); DTOs from TASK-0206.

| Route | Success | Errors | Idempotency | Owning epic |
|---|---|---|---|---|
| `GET /api/v1/locations` | `200` page | `401`, `403` | Safe/read-only | EPIC-13 (TASK-1307) |
| `POST /api/v1/locations` | `201` | `400`, `401`, `403`, `409` | Not idempotent: repeat create returns `409` | EPIC-13 (TASK-1307) |
| `GET /api/v1/locations/{id}` | `200` | `401`, `403`, `404` | Safe/read-only | EPIC-13 (TASK-1307) |
| `PUT /api/v1/locations/{id}` | `200` | `400`, `401`, `403`, `404`, `409` | Idempotent for identical payloads | EPIC-13 (TASK-1307) |
| `POST /api/v1/locations/{id}/deactivate` | `204` | `401`, `403`, `404` | Idempotent | EPIC-13 (TASK-1307) |
| `GET /api/v1/monitored-devices` | `200` page | `401`, `403` | Safe/read-only | EPIC-13 (TASK-1307) |
| `POST /api/v1/monitored-devices` | `201` | `400`, `401`, `403`, `409` | Not idempotent: repeat create returns `409` | EPIC-13 (TASK-1307) |
| `GET /api/v1/monitored-devices/{id}` | `200` | `401`, `403`, `404` | Safe/read-only | EPIC-13 (TASK-1307) |
| `PUT /api/v1/monitored-devices/{id}` | `200` | `400`, `401`, `403`, `404`, `409` | Idempotent for identical payloads | EPIC-13 (TASK-1307) |
| `POST /api/v1/monitored-devices/{id}/deactivate` | `204` | `401`, `403`, `404` | Idempotent | EPIC-13 (TASK-1307) |
| `GET /api/v1/power-meters/{id}/location-history` | `200` page (effective-dated) | `401`, `403`, `404` | Safe/read-only | EPIC-13 (TASK-1307) |
| `POST /api/v1/power-meters/{id}/location-history` | `201` (places the meter; ends the open placement) | `400`, `401`, `403`, `404`, `409` (overlapping range) | Not idempotent: repeat placement with an overlapping range returns `409` | EPIC-13 (TASK-1307) |
| `GET /api/v1/machine-power-meter-associations` | `200` page (filter by machine or meter; historical query support) | `400`, `401`, `403` | Safe/read-only | EPIC-13 (TASK-1307) |
| `POST /api/v1/machine-power-meter-associations` | `201` (`DedicatedLoad`, `SharedLoad`, or `CollectorOnly`) | `400`, `401`, `403`, `404`, `409` (overlapping active primary, TASK-1306) | Not idempotent: overlap returns `409` | EPIC-13 (TASK-1307) |
| `POST /api/v1/machine-power-meter-associations/{id}/end` | `204` (sets `EffectiveToUtc`) | `400` (end before start), `401`, `403`, `404` | Idempotent: ending an ended association returns `204` | EPIC-13 (TASK-1307) |
| `GET /api/v1/power-meter-device-associations` | `200` page (filter by meter or device) | `400`, `401`, `403` | Safe/read-only | EPIC-13 (TASK-1307) |
| `POST /api/v1/power-meter-device-associations` | `201` (`Dedicated` or `Shared`) | `400`, `401`, `403`, `404`, `409` | Not idempotent: overlap returns `409` | EPIC-13 (TASK-1307) |
| `POST /api/v1/power-meter-device-associations/{id}/end` | `204` | `400`, `401`, `403`, `404` | Idempotent | EPIC-13 (TASK-1307) |

## Existing Pre-v1 Baseline Routes

The repository's current Identity foundation exposes routes outside `/api/v1`:
`/api/identity/*` (ASP.NET Core `MapIdentityApi`, logout, and the first-owner
bootstrap endpoints `setup-status`, `self-create`, `bootstrap-admin`),
`/api/users` (user management), `/api/auth/antiforgery-token`, `/api/Info/*`,
and `/api/Operations/*`. These are the implementation baseline, not part of
the accepted v1 contract. Their disposition — adapting `MapIdentityApi` token
issuance to back `/api/v1/auth/*`, mapping legacy roles onto `Owner`, and
aligning the first-owner bootstrap with TASK-0410 — is owned by TASK-0204,
TASK-0401, and TASK-1101. No new client may take a dependency on pre-v1
routes.

## Route Ownership Summary

| Group | Routes | Owning epic |
|---|---:|---|
| Health | 2 | EPIC-14 |
| Authentication and tokens | 4 | EPIC-04 |
| Device-account administration | 10 | EPIC-04 |
| Registration and heartbeats | 2 | EPIC-05 |
| Owner machine administration and reads | 9 | EPIC-05 / EPIC-06 |
| Power meters and readings | 9 | EPIC-12 |
| Locations, monitored devices, associations | 18 | EPIC-13 |

Every route above appears exactly once. Adding, renaming, or removing a v1
route requires updating this catalog, the OpenAPI document (TASK-0209), and
the owning epic's contract tests in the same change.

## Related Documents

- [Product scope — decisions](./product-scope.md#decisions)
- [Architecture overview — authentication and authorization](./architecture-overview.md#authentication-and-authorization)
- [Domain model](./domain-model.md)
- [Delivery backlog — release gates](./delivery-backlog.md#release-gates)
