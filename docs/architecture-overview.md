# Architecture Overview

## System Summary

The preferred design is a three-application deployment model backed by shared libraries and a SQL Server database.

## Deployable Applications

- `SystemUptimeTracker.Api`: ASP.NET Core ingestion API.
- `SystemUptimeTracker.WindowsService`: Windows background service.
- `SystemUptimeTracker.LinuxDaemon`: Ubuntu systemd-managed daemon.

## Shared Libraries

- `SystemUptimeTracker.Agent.Core`: Shared worker behavior, retry, and publishing logic.
- `SystemUptimeTracker.Contracts`: Shared API contracts and payload models.
- `SystemUptimeTracker.Data`: Entity Framework Core data layer, entities, and migrations.
- `SystemUptimeTracker.Power.Shelly`: Shelly normalization and provider logic.

## Proposed Solution Shape

All implementation code, including automated tests, should live under `src/`.
Test projects should be differentiated by project name rather than by a separate `tests/` folder. The test type should be expressed in the project name, for example `UnitTests`, `IntegrationTests`, or `FunctionalTests`.
Test projects should be created where behavior warrants them. Thin host shells can rely on shared-core tests plus integration or packaging coverage until they contain meaningful platform-specific logic.

```text
src/
  SystemUptimeTracker.Api/
  SystemUptimeTracker.Api.UnitTests/
  SystemUptimeTracker.Api.IntegrationTests/
  SystemUptimeTracker.Api.FunctionalTests/
  SystemUptimeTracker.WindowsService/
  SystemUptimeTracker.LinuxDaemon/
  SystemUptimeTracker.Agent.Core/
  SystemUptimeTracker.Agent.Core.UnitTests/
  SystemUptimeTracker.Agent.Core.IntegrationTests/
  SystemUptimeTracker.Agent.Core.FunctionalTests/
  SystemUptimeTracker.Contracts/
  SystemUptimeTracker.Contracts.UnitTests/
  SystemUptimeTracker.Data/
  SystemUptimeTracker.Data.UnitTests/
  SystemUptimeTracker.Data.IntegrationTests/
  SystemUptimeTracker.Power.Shelly/
  SystemUptimeTracker.Power.Shelly.UnitTests/
```

The proposed solution shape is illustrative rather than exhaustive. Additional test projects or support libraries should be added only when they own distinct behavior or deployment concerns.

## Context Diagram

```mermaid
flowchart LR
    WS[Windows Service Agent]
    LD[Linux Daemon Agent]
    SH[Shelly Plug US Gen4]
    API[Telemetry API]
    DB[(SQL Server)]

    WS -->|HTTPS heartbeats and telemetry| API
    LD -->|HTTPS heartbeats and telemetry| API
    SH -->|Local HTTP RPC via agent polling| WS
    SH -->|Local HTTP RPC via agent polling| LD
    SH -.->|Future: MQTT or WebSocket, agent bypassed| API
    API --> DB
```

## Runtime Responsibilities

### Agent Applications

The Windows and Linux applications should be thin hosting shells over shared agent behavior. Their responsibilities are:

- Integrate with Windows Service Control Manager or systemd.
- Collect platform-specific machine telemetry.
- Persist agent identity locally.
- Queue and retry outbound telemetry when the API is unavailable.
- Optionally poll configured Shelly devices.
- Publish telemetry to the API over HTTPS.

### API Application

The API should:

- Issue and validate JWT bearer tokens for reporting agents, operators, and other telemetry producers, backed by ASP.NET Core Identity local accounts (see [Authentication And Authorization](#authentication-and-authorization)).
- Accept machine heartbeats and optional power readings.
- Accept independent power-meter registration and future direct power ingestion.
- Normalize payloads into the shared data model.
- Calculate or update runtime sessions based on heartbeat continuity.
- Expose health endpoints and administrative endpoints needed for association management.
- Version all routes under `/api/v1` so future contract changes do not silently break deployed agents. See [inital-spec.md](./inital-spec.md) for the endpoint shapes this convention was drawn from; a dedicated API contracts document should formalize the accepted route list before Phase 1 implementation begins.

### Database Layer

SQL Server is the authoritative store for:

- Machine telemetry history.
- Runtime-session history.
- Power-meter registrations and readings.
- Inventory, location, and association state.
- Historical effective-dated relationships.

## Key Architectural Decisions

### Decision 1: Separate Deployables For Windows And Linux

Windows Service and Ubuntu daemon packaging should be separate applications even if most business logic is shared. This avoids conditional deployment behavior leaking into the shared worker.

### Decision 2: Shared Agent Core

Agent behavior should live in one shared library to keep scheduling, retry, publishing, and contract generation consistent across platforms.

### Decision 3: Power Telemetry Is Optional

The machine-monitoring path must work with no Shelly presence. Shelly support is an additive capability.

### Decision 4: Independent Registration Model

Machines and power meters must not depend on each other for creation. Associations are explicit and time-aware.

### Decision 5: Outbound-Only Collection

Agents and power producers send data to the API. The server does not initiate control-plane traffic to monitored devices.

## Core Data Flows

### Device Account Provisioning And First Connection Flow

1. Owner creates a `DeviceAccount` (dedicated to one machine, or shared across several), choosing JWT, API key, or both as its allowed authentication methods.
2. The resulting initial credential — a password for JWT-capable devices, or an API key for Basic Auth devices — is supplied out-of-band into the Windows Service or systemd daemon's local configuration at install time.
3. On first run, a JWT-capable agent exchanges that initial credential once at the token endpoint for an access token and a refresh token.
4. The agent persists the refresh token (or re-derives access tokens from it) and rotates its access token periodically thereafter, without resending the initial credential on every call. Whether the initial credential remains usable as a standing fallback or is invalidated after first use is an open question (see [product-scope.md](./product-scope.md)).
5. A Basic Auth device instead sends its API key on every call; there is no rotation step unless the owner manually issues a new key.

### Machine Heartbeat Flow

1. Agent starts and loads local identity.
2. Agent collects machine telemetry.
3. Agent posts heartbeat payload to the API, authenticated with its current bearer token or API key.
4. API validates sender and stores machine and heartbeat data.
5. API updates runtime-session state based on prior heartbeats.

### Shelly Via Agent Flow

1. Agent polls a configured Shelly Plug US Gen4 over local HTTP RPC.
2. Agent normalizes the Shelly response into a power-reading contract.
3. Agent posts the power reading with its heartbeat or through a related endpoint (see the open question in [implementation-plan.md](./implementation-plan.md) on which transport wins).
4. API stores the reading against the power meter and validates any machine relationship.

### Future Direct Shelly Flow

1. Shelly device or broker-connected ingestion service sends power telemetry directly.
2. API or ingestion worker resolves meter identity.
3. Reading is normalized into the same storage model used by agent-mediated readings.

## Deployment View

### Windows Agent

- Publish as a Windows-targeted service executable.
- Install through Windows Service registration.
- Persist local state under a service-owned data directory.

### Ubuntu Agent

- Publish as a Linux-targeted daemon executable.
- Install under a fixed application directory.
- Register under systemd with restart and minimal required filesystem access.

### API

- Support local containerized development.
- Support IIS, Azure App Service, or Linux-hosted Kestrel deployments.
- Use SQL Server in all durable environments.

Concrete deployment identifiers (Windows service name, systemd unit name, install and data directories, config file name) still need to be defined under the `SystemUptimeTracker` naming established in this document. [inital-spec.md](./inital-spec.md) uses placeholder `ComputerTelemetry`/`computer-telemetry` names from before the project was renamed; do not carry those literal strings into implementation.

## Cross-Cutting Concerns

### Authentication And Authorization

**Decided:** the API authenticates callers using **ASP.NET Core Identity** with **local user accounts** — no external identity provider (Azure AD, Google, etc.) is in scope for the first release. The top priority driving this design is minimizing the ingestion API's attack surface: every non-health endpoint requires authentication, credentials are never stored in plaintext, and device-facing credentials carry only enough privilege to submit telemetry. This supersedes the `Authorization: AgentKey ...` header scheme shown in [inital-spec.md](./inital-spec.md).

#### Account Model: Owner And Device Accounts

- **Owner account**: a human user (an ASP.NET Core Identity account in the `Owner` role) who administers the deployment. An owner creates and removes device accounts, and decides whether devices share one account or each get their own.
- **Device account**: modeled by the `DeviceAccount` entity (see [domain-model.md](./domain-model.md)), which wraps an Identity user with the extra metadata authentication needs — which owner manages it, which authentication methods it's allowed to use, and (if enabled) its hashed API key. A `Machine` references the `DeviceAccount` currently authorized to report on its behalf via `Machine.DeviceAccountId`; multiple machines may point at the same device account (shared) or each may have its own (dedicated) — the owner's choice, not a fixed system rule.
- The machine's own `AgentId` — not the account used to authenticate — remains the durable identity written onto every heartbeat and reading. The device account proves the caller is allowed to write; `AgentId` says which machine the data is about.

#### Two Authentication Schemes, One Authorization Scope

Every device account is configured to use one (or both) of two schemes, chosen based on what the device can support. Both land in the same restricted, telemetry-only authorization scope — neither can reach administrative endpoints (association, location, or device-account management), which require an owner account instead.

1. **JWT bearer tokens (primary).** Used by the Windows Service and Linux daemon agents. When a service is first registered, it is supplied its device account's credentials out-of-band (placed into local configuration at install time). On first run, the agent exchanges those credentials once at a token endpoint for a signed JWT access token plus a refresh token, backed by ASP.NET Core Identity. From then on the agent presents the access token as a bearer token on every heartbeat/telemetry call and rotates it via the refresh token on a periodic basis — it does not resend the original credential on every request. ASP.NET Core's built-in Identity API endpoints (`MapIdentityApi<TUser>()`) are the natural fit for issuing these tokens without a separate token-service dependency; the concrete endpoint/claims/lifetime contract is tracked in [implementation-plan.md](./implementation-plan.md).
2. **HTTP Basic Auth with an API key (fallback).** For devices that cannot perform a login/refresh flow — for example, a Shelly Plug US Gen4 driven by a webhook, MQTT bridge, or on-device script — the device account's "password" is a long-lived, hashed, individually revocable **API key**, never a real changing password. The client sends `Authorization: Basic base64(deviceAccountName:apiKey)` over HTTPS. This trades some of the JWT flow's exposure-reduction for compatibility with constrained clients, so it is offered per device account rather than as the default.

#### Hardening Baseline

- HTTPS-only for all traffic, without exception — this matters even more once Basic Auth is in play, since it carries the credential (base64-encoded, not encrypted) on every request.
- API keys and Identity password hashes are salted/hashed at rest; a plaintext API key is shown to the owner exactly once, at creation or rotation time, and never again.
- Account lockout after repeated failed authentication attempts (ASP.NET Core Identity's built-in lockout), plus rate limiting on the token and Basic Auth entry points, to blunt credential-stuffing and brute-force attempts against the most exposed part of the system.
- API keys are individually revocable and rotatable by the owning owner account without needing to touch the underlying Identity user.
- Role/scope-based authorization is enforced the same way regardless of which of the two schemes authenticated the caller: device-scoped principals only ever reach ingestion endpoints.

### Security

- No trusted client-supplied server timestamps.
- Idempotent handling for retried messages.
- Never store power-meter credentials (for example, a Shelly device password) directly on the `PowerMeter` record. Store a reference to a secret manager or encrypted secret store instead — see `AuthenticationReference` in [domain-model.md](./domain-model.md). This is a distinct concern from API authentication above: it is the credential the *agent* uses to poll the Shelly plug locally, not a credential for calling this API.

### Reliability

- Local retry queue for agents. [inital-spec.md](./inital-spec.md) recommends a SQLite-backed queue with a maximum age of 7 days, a maximum size of 100 MB, and a retry backoff progression of 15s, 30s, 1m, 5m, 15m; these defaults have not yet been formally accepted (see the open question in [implementation-plan.md](./implementation-plan.md)).
- Gap-based runtime-session logic for missed heartbeats, using a default heartbeat interval of 60 seconds, an offline threshold of 3 minutes, and a session-break threshold of 5 minutes as illustrative starting points from the original design conversation. These thresholds should be confirmed and made configurable per machine before Phase 1 exit.
- Health endpoints for API and database dependencies.

### Observability

- Structured logging in agents and API.
- Consistent correlation identifiers for heartbeats and power readings.
- Operational metrics for ingestion success, retry backlog, and registration failures.

### Extensibility

- Power telemetry providers should be pluggable.
- Direct-ingestion power paths should normalize into the same domain entities.
- Additional device types and locations should fit the same association model without schema redesign.
