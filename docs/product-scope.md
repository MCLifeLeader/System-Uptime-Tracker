# Product Scope

## Purpose

System Uptime Tracker is intended to collect machine uptime and lightweight telemetry from Windows and Ubuntu computers, send that data to a central API, and store it in SQL Server for historical reporting and operational analysis.

The system also needs an extensible path for optional power telemetry, starting with Shelly Plug US Gen4 devices, without making power-meter support a prerequisite for computer monitoring.

## Primary Goals

- Monitor Windows computers through a Windows Service.
- Monitor Ubuntu computers through a systemd-managed daemon.
- Receive and persist heartbeat and telemetry data through a .NET web API.
- Preserve historical uptime through runtime-session reconstruction, not by treating each heartbeat as an isolated event.
- Support optional, independent registration of Shelly power meters.
- Allow later association between machines, monitored devices, locations, and power meters.

## Non-Goals For The Initial Release

- Remote administration of monitored machines.
- Power-based billing or highly precise device-level power attribution.
- Full dashboard and reporting UI.
- Software inventory, patch management, or process inspection.
- Automatic agent updates.
- Real-time alerting beyond basic health visibility.

## Product Principles

## Outbound-Only Monitoring

Monitored computers and optional power integrations should send data to the API. The server should not require inbound access to monitored machines.

## Independent First-Class Entities

Machines, power meters, monitored devices, and locations must all be creatable independently. Associations are optional and added later when the real-world relationship exists.

## Accurate Historical Context

Time-aware associations and session modeling are required so the system can answer both current-state and historical questions.

## Normalized Telemetry Ownership

Measured power belongs to the power meter. Measured uptime belongs to the machine heartbeat and runtime-session model. Context is created through associations rather than by duplicating telemetry across related entities.

## Initial Scope Boundary

## In Scope

- ASP.NET Core ingestion API.
- SQL Server persistence model.
- Windows Service packaging and installation path.
- Ubuntu daemon packaging and systemd installation path.
- Agent identity, heartbeat scheduling, retry queue, and telemetry publishing.
- Machine telemetry including uptime context, OS metadata, CPU, memory, and storage.
- Optional Shelly polling support through the agent.
- Optional independent Shelly registration and future non-agent ingestion paths.
- Minimum location and monitored-device management needed to associate power meters to real-world equipment when Shelly support is introduced.
- Documentation for design, planning, and implementation sequence.

## Deferred But Supported By The Design

- Dedicated power-meter ingestion service.
- MQTT-based Shelly ingestion.
- Device-level estimated power allocation.
- Administrative workflows for approval and lifecycle management.
- Power-aware state inference across large fleets.
- Broad software inventory beyond the minimum physical device and location context needed for power-meter associations.
- Reporting UI and dashboards.

## Target Users And Operators

- A system owner who needs machine uptime history.
- An administrator deploying Windows and Ubuntu background services.
- An operator who wants to add power telemetry later without redesigning the system.
- A future maintainer who needs a clean, explicit architecture boundary between agents, API, and data model.

## Success Criteria

- A machine can be monitored with no Shelly configuration present.
- A Shelly power meter can be registered with no machine agent present.
- A reporting machine can optionally be linked to a power meter as dedicated load, shared load, or collector only.
- Runtime sessions can be derived from heartbeat data with reliable gap handling.
- The design supports staged delivery, starting with computer telemetry and adding power telemetry later.

## Assumptions

- The implementation will use .NET and C# for API and agent workloads.
- SQL Server is the system of record for production data.
- HTTPS is required for all agent-to-API communication.
- Windows and Ubuntu are the first supported operating systems.
- Power telemetry is optional for the first deployment wave.

## Constraints

- The first usable deployment should stay operationally simple.
- Cross-platform agent logic should be shared where possible.
- Platform-specific behavior should be isolated to hosting, installation, and OS-specific telemetry collection.
- Security controls should be strong enough for unattended service-to-API communication.

## Open Product Decisions

- Whether agent registration is auto-approved or requires an approval workflow.
- Whether the first API authentication model is per-agent API keys only or includes enrollment workflows immediately.
- Whether Shelly support starts as agent polling only or also includes an early direct-ingestion path.
- What the minimum operator workflow should be for managing locations and monitored-device associations once power-meter support is enabled.
