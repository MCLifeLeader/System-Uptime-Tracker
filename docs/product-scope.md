# Product Scope

## Purpose

System Uptime Tracker is intended to collect machine uptime and lightweight telemetry from Windows and Ubuntu computers, send that data to a central API, and store it in SQL Server for historical reporting and operational analysis.

The system also includes an owner-facing web management portal running on NodeJS. That portal is the primary human interface for managing device accounts, reviewing machine and power-meter records, and interacting with the data collected by the platform.

The system also needs an extensible path for optional power telemetry, starting with Shelly Plug US Gen4 devices, without making power-meter support a prerequisite for computer monitoring.

## Primary Goals

- Monitor Windows computers through a Windows Service.
- Monitor Ubuntu computers through a systemd-managed daemon.
- Receive and persist heartbeat and telemetry data through a .NET web API.
- Provide a NodeJS web management portal for owner-facing administration and operational data access.
- Preserve historical uptime through runtime-session reconstruction, not by treating each heartbeat as an isolated event.
- Support optional, independent registration of Shelly power meters.
- Allow later association between machines, monitored devices, locations, and power meters.

## Non-Goals For The Initial Release

- Remote administration of monitored machines.
- Power-based billing or highly precise device-level power attribution.
- Rich dashboarding, ad hoc analytics, and advanced reporting UI beyond the basic management portal.
- Software inventory, patch management, or process inspection.
- Automatic agent updates.
- Real-time alerting beyond basic health visibility.
- A multi-tenant SaaS platform (isolated organizations, billing, cross-tenant data partitioning). The owner/device-account model described under [Decisions](#decisions) is single-deployment ownership and access control, not a multi-tenant hosting model — see the open question below on whether more than one owner account is even expected in the first release.

## Product Principles

### Outbound-Only Monitoring

Monitored computers and optional power integrations should send data to the API. The server should not require inbound access to monitored machines.

### Independent First-Class Entities

Machines, power meters, monitored devices, and locations must all be creatable independently. Associations are optional and added later when the real-world relationship exists.

### Accurate Historical Context

Time-aware associations and session modeling are required so the system can answer both current-state and historical questions.

### Normalized Telemetry Ownership

Measured power belongs to the power meter. Measured uptime belongs to the machine heartbeat and runtime-session model. Context is created through associations rather than by duplicating telemetry across related entities.

### Minimal Attack Surface

The ingestion API is the most exposed part of the system and should be hardened accordingly: every non-health endpoint requires authentication, device-facing credentials carry the least privilege needed to submit telemetry (never administrative access), credentials are stored hashed rather than in plaintext, and authentication endpoints are protected against brute-force attempts (lockout, rate limiting). Convenience for constrained devices (see the Basic Auth/API-key fallback under [Decisions](#decisions)) must not come at the cost of these baseline protections.

## Initial Scope Boundary

### In Scope

- ASP.NET Core ingestion API.
- SQL Server persistence model.
- Windows Service packaging and installation path.
- Ubuntu daemon packaging and systemd installation path.
- Agent identity, heartbeat scheduling, retry queue, and telemetry publishing.
- Machine telemetry including uptime context, OS metadata, CPU, memory, and storage.
- Optional Shelly polling support through the agent.
- Optional independent Shelly registration and future non-agent ingestion paths.
- NodeJS web management portal for owner login, device-account administration, and interaction with collected machine and power data.
- Owner-facing administrative and data-read API endpoints that the portal uses
  for the same workflows.
- Minimum location and monitored-device management needed to associate power meters to real-world equipment when Shelly support is introduced.
- Documentation for design, planning, and implementation sequence.

### Deferred But Supported By The Design

- Dedicated power-meter ingestion service.
- MQTT-based Shelly ingestion.
- Device-level estimated power allocation (see `PowerAllocationRule` in [domain-model.md](./domain-model.md)).
- Administrative workflows for approval and lifecycle management.
- Power-aware state inference across large fleets.
- Broad software inventory beyond the minimum physical device and location context needed for power-meter associations.
- Rich dashboarding and custom reporting beyond the basic management portal.

## Target Users And Operators

- A system owner who needs machine uptime history.
- A system owner or operator who needs a web portal to manage device accounts and inspect collected data.
- An administrator deploying Windows and Ubuntu background services.
- An operator who wants to add power telemetry later without redesigning the system.
- A future maintainer who needs a clean, explicit architecture boundary between agents, API, and data model.

## Success Criteria

- A machine can be monitored with no Shelly configuration present.
- A Shelly power meter can be registered with no machine agent present.
- An owner can sign into the management portal and use it to manage device accounts and review collected telemetry through the same API surface used by devices.
- A reporting machine can optionally be linked to a power meter as dedicated load, shared load, or collector only.
- Runtime sessions can be derived from heartbeat data with reliable gap handling.
- The design supports staged delivery, starting with computer telemetry and adding power telemetry later.

## Assumptions

- The implementation will use .NET and C# for API and agent workloads.
- The owner-facing management portal will run on NodeJS.
- SQL Server is the system of record for production data.
- HTTPS is required for all agent-to-API communication.
- Windows and Ubuntu are the first supported operating systems.
- Power telemetry is optional for the first deployment wave.
- Authentication uses ASP.NET Core Identity with local user accounts and JWT bearer tokens; no external identity provider is in scope for the first release.

## Constraints

- The first usable deployment should stay operationally simple.
- Cross-platform agent logic should be shared where possible.
- Platform-specific behavior should be isolated to hosting, installation, and OS-specific telemetry collection.
- Security controls should be strong enough for unattended service-to-API communication.
- Portal features must consume the shared API contract rather than introducing
  direct database access or a second backend rule set.

## Implementation Approach

The first release should be delivered in paired backend and frontend slices.

- Backend or platform work should lead when new schema, routes, authentication,
  authorization, or host-runtime behavior is required.
- Frontend work should follow closely behind each backend slice so owner-facing
  workflows are validated against the real API and not against temporary mocks.
- Detailed execution order lives in [implementation-plan.md](./implementation-plan.md)
  and [stories/2026/07/README.md](./stories/2026/07/README.md).

## Decisions

- **Authentication model (decided):** The API authenticates callers through ASP.NET Core Identity local user accounts — no external identity provider. There are two kinds of account:
  - **Owner account**: a human user who administers the deployment. An owner creates and removes device accounts, and decides whether devices share one account or each get their own.
  - **Device account**: the credential a reporting agent (or other telemetry-producing device) uses to call the ingestion API. Every device account is owned by exactly one owner account. An owner may create one device account per machine, or a single shared device account used by many machines — both are supported, and the choice is the owner's, not a fixed system default.
  - Device accounts authenticate through **either** of two schemes, chosen per device account based on what the device can support:
    1. **JWT bearer tokens (primary).** The device is given its account's credentials out-of-band when the Windows Service or systemd daemon is first registered, uses them once to obtain an access token and refresh token, and rotates the access token periodically thereafter — it does not resend the original credential on every call.
    2. **HTTP Basic Auth with a long-lived API key (fallback).** For devices that cannot perform a login/refresh flow — for example, a Shelly Plug US Gen4 driven by a webhook or on-device script — the device account's "password" is a hashed, individually revocable API key rather than a real rotating credential.
  - Both schemes authorize into the same restricted, telemetry-only scope; neither can reach administrative endpoints (association management, location management, device-account management), which require an owner account.
  - The NodeJS management portal uses owner accounts against the same API rather than a separate backend. Device ingestion and human administration share one API surface, separated by route purpose and authorization scope rather than by separate servers.
  - This supersedes the `Authorization: AgentKey ...` scheme shown in [inital-spec.md](./inital-spec.md). See [architecture-overview.md](./architecture-overview.md#authentication-and-authorization) for the design and [domain-model.md](./domain-model.md) for the `DeviceAccount` entity and how a `Machine` links to it.

## Open Product Decisions

- Whether agent registration is auto-approved or requires an approval workflow.
- Whether device accounts are provisioned one-per-machine by default, or a shared account is the default with dedicated per-device accounts as an opt-in — the owner can choose either way, but the first-run default still needs to be picked.
- Whether more than one owner account is expected in the first release, and if so, whether each owner's devices and data must be isolated from other owners' (which would be a light form of multi-tenancy) or whether all owners share full visibility. Today the model assumes ownership without necessarily assuming isolation; this needs an explicit answer before it affects query/authorization design.
- Whether the initial device-account credential (used only to bootstrap the first JWT) should be treated as one-time/single-use and invalidated after first successful login, or remain a standing password the device can always fall back to.
- Whether Shelly support starts as agent polling only or also includes an early direct-ingestion path.
- What the minimum operator workflow should be for managing locations and monitored-device associations once power-meter support is enabled.
- Whether the retry-queue technology and limits proposed in [inital-spec.md](./inital-spec.md) (SQLite-backed, 7-day/100 MB cap, 15s/30s/1m/5m/15m backoff) should be adopted as-is; see the corresponding open question in [implementation-plan.md](./implementation-plan.md).
