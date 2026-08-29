# Telemetry Application Design Source Transcript

> Source: [ChatGPT shared conversation](https://chatgpt.com/share/6a64dcaf-17b8-83e8-a452-e8545a4a4358)
> Retrieved: 2026-07-25
> Conversation created: 2026-07-25

## Status

This file is an archival transcript of the original design conversation.

For the maintained working documentation, use:

- [README.md](./README.md)
- [product-scope.md](./product-scope.md)
- [architecture-overview.md](./architecture-overview.md)
- [domain-model.md](./domain-model.md)
- [implementation-plan.md](./implementation-plan.md)

## Archived Transcript

````markdown

## User

I am looking to create a small simple application that can run on windows and on Ubuntu Linux. It needs to run as a demon process or Windows service.

I need to be able to compile a window service application version and a Lennox daemon background working process service and be able to register it with Linux.

The application will need to talk to a web service to report data and detail details about the computer that Webb service will capture the data as a web post and store that detail and information in a database

The purpose is to track computer up time and some telemetry about the computer, fairly simple lightweight, but will provide on ongoing uptime history when the computer is running and then it won’t when the computer is off or in hibernation or sleep mode

If there’s a way to acquire power usage information that would be great, but it at least needs to be able to report the computer name and the time in which the message was sent and received to provide up time data and detail details about the computer

So part of this project will need a Webb service that will be running on either as your or a Windows IIS server

I want to use DOTNET C sharp for the coding language and that will be both for the background service and the web service  start working on that and putting some details together, and we will continue to iterate from there on design requirements

## ChatGPT

I’ll frame this as a cross-platform .NET worker plus ASP.NET Core ingestion API, then define the first-pass architecture, data model, and service-registration approach.

## Recommended First-Pass Architecture

Use one .NET solution with three primary applications/libraries:

```text
ComputerTelemetry.sln

src/
  ComputerTelemetry.Agent/
  ComputerTelemetry.Api/
  ComputerTelemetry.Contracts/
  ComputerTelemetry.Infrastructure/

tests/
  ComputerTelemetry.Agent.Tests/
  ComputerTelemetry.Api.Tests/
```

### 1. Computer Telemetry Agent

A cross-platform **.NET 10 Worker Service** running as:

- A Windows Service on Windows
- A `systemd` daemon on Ubuntu Linux
- A console application during development and troubleshooting

.NET Worker Services are designed for long-running, cross-platform background processes and include configuration, logging, dependency injection, and application-lifetime management. ([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers?utm_source=chatgpt.com))

The same source code should support both operating systems. Separate publish packages will be generated:

```text
win-x64
linux-x64
```

ARM Linux support could later add:

```text
linux-arm64
```

### 2. Computer Telemetry API

An ASP.NET Core Web API that receives heartbeat and telemetry messages.

It can run on:

- Azure App Service
- Azure Container Apps
- Windows Server with IIS
- Windows Server as a standalone Windows Service
- Ubuntu with Kestrel and Nginx
- Docker

Kestrel is cross-platform, and Microsoft documents production hosting on both Windows Services and Linux/Nginx. ([Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-10.0&utm_source=chatgpt.com))

### 3. Database

For your environment, I recommend starting with:

- SQL Server
- Entity Framework Core
- Azure SQL Database when hosted in Azure

For a lightweight proof of concept, SQLite would work, but SQL Server is the better long-term choice for reporting, multiple organizations, retention policies, and future dashboards.

---

# Agent Responsibilities

The agent wakes at a configurable interval—probably every 60 seconds—and sends a heartbeat.

A basic heartbeat could look like this:

```json
{
  "agentId": "3a812c1a-9dfd-42e7-97f4-8a47d68971e4",
  "machineName": "BUILD-SERVER-01",
  "sentAtUtc": "2026-07-25T15:30:00Z",
  "agentStartedAtUtc": "2026-07-25T12:10:34Z",
  "operatingSystem": "Ubuntu 24.04.3 LTS",
  "architecture": "X64",
  "agentVersion": "1.0.0",
  "systemBootTimeUtc": "2026-07-24T18:42:11Z",
  "memory": {
    "totalBytes": 34359738368,
    "availableBytes": 18253611008
  },
  "processor": {
    "logicalProcessorCount": 16,
    "usagePercent": 14.7
  },
  "storage": [
    {
      "name": "/",
      "totalBytes": 1073741824000,
      "availableBytes": 584115552256
    }
  ]
}
```

The server should independently record:

```text
ReceivedAtUtc
```

That gives us both:

- Client-reported send time
- Server-authoritative receive time

This matters because the computer clock could be incorrect, deliberately changed, or out of sync.

---

# Uptime Tracking Model

Do not treat a heartbeat as an individual uptime record only. Group heartbeats into **runtime sessions**.

A session starts when:

- The agent service starts
- The computer boots
- The agent resumes after sleep or hibernation
- A heartbeat arrives after a sufficiently large gap

A session ends when:

- The agent shuts down cleanly
- The agent reports a suspend or shutdown event
- No heartbeat is received within the timeout window

For example:

```text
Heartbeat interval:       60 seconds
Offline threshold:        3 minutes
Session break threshold:  5 minutes
```

If the last heartbeat arrived at 10:01 AM and the next arrived at 2:15 PM, the system should not count those four hours as uptime. Instead:

```text
Session 1 ends approximately 10:02 AM
Session 2 begins approximately 2:15 PM
```

This naturally identifies shutdown, sleep, hibernation, lost network access, or a stopped agent. It cannot always distinguish those conditions without additional operating-system event monitoring, but it accurately identifies when the machine was no longer reporting.

---

# Initial Database Model

## Machine

```text
MachineId
AgentId
MachineName
OperatingSystem
OperatingSystemVersion
Architecture
FirstSeenAtUtc
LastSeenAtUtc
AgentVersion
IsActive
CreatedAtUtc
UpdatedAtUtc
```

## Heartbeat

```text
HeartbeatId
MachineId
SequenceNumber
SentAtUtc
ReceivedAtUtc
AgentStartedAtUtc
SystemBootTimeUtc
CpuUsagePercent
TotalMemoryBytes
AvailableMemoryBytes
PayloadVersion
CreatedAtUtc
```

## RuntimeSession

```text
RuntimeSessionId
MachineId
StartedAtUtc
LastHeartbeatAtUtc
EndedAtUtc
EndReason
HeartbeatCount
CalculatedUptimeSeconds
```

Possible `EndReason` values:

```text
Running
GracefulShutdown
ServiceStopped
SleepOrHibernate
HeartbeatTimeout
AgentRestart
MachineReboot
Unknown
```

## StorageTelemetry

```text
StorageTelemetryId
HeartbeatId
VolumeName
FileSystem
TotalBytes
AvailableBytes
```

Initially, detailed telemetry could be stored only every 5–15 minutes while lightweight heartbeats are stored every minute. That prevents unnecessary database growth.

---

# Agent Identity

The agent should create a random GUID on first run and persist it locally:

### Windows

```text
C:\ProgramData\ComputerTelemetry\agent.json
```

### Ubuntu

```text
/var/lib/computer-telemetry/agent.json
```

Example:

```json
{
  "agentId": "3a812c1a-9dfd-42e7-97f4-8a47d68971e4",
  "registeredAtUtc": "2026-07-25T15:00:00Z"
}
```

Do not identify a machine solely by hostname. Hostnames can be changed, duplicated, or reused.

---

# Communication and Security

Recommended endpoint:

```http
POST /api/v1/heartbeats
```

Initial authentication can use a per-agent API key:

```http
Authorization: AgentKey xxxxxxxxxxxxxxxxx
```

A stronger long-term design would use:

1. A one-time enrollment token
2. Agent registration
3. A unique client credential issued to that agent
4. Credential rotation and revocation

All communication must use HTTPS.

Additional protections:

- Validate payload size
- Rate-limit clients
- Reject unsupported payload versions
- Hash stored API keys
- Record authentication failures
- Use idempotency through `AgentId + SequenceNumber`
- Never allow the agent to submit a trusted `ReceivedAtUtc`

---

# Platform Service Hosting

## Windows Service `Program.cs`

The Windows host integrates with Windows Service Control Manager and delegates
worker behavior to the shared agent core:

```csharp
using SystemUptimeTracker.Agent.Core;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
  options.ServiceName = "SystemUptimeTrackerAgent";
});

builder.Services.AddHttpClient<TelemetryApiClient>();
builder.Services.AddSingleton<MachineIdentityProvider>();
builder.Services.AddSingleton<SystemTelemetryCollector>();
builder.Services.AddHostedService<TelemetryWorker>();

IHost host = builder.Build();

await host.RunAsync();
```

The Windows host requires:

```xml
<PackageReference
    Include="Microsoft.Extensions.Hosting.WindowsServices"
    Version="10.0.*" />
```

The separate Linux host registers the same shared agent services and adds
systemd integration through:

```xml
<PackageReference
    Include="Microsoft.Extensions.Hosting.Systemd"
    Version="10.0.*" />
```

The business logic remains identical. Windows Service Control Manager and Linux `systemd` handle process startup, shutdown, restart, and recovery.

---

# Windows Publication and Installation

The local `C:\Code\Personal\FamilyTools` repository provides the reference
deployment shape. Its
`src/ComputerTaskHandler/Task.Handler.Client/Install_Service.ps1` script ships
beside the service executable and performs a create-or-update deployment: it
uses the script directory as the artifact source, creates the target directory,
stops an existing service, replaces application files, creates the service with
automatic startup when it is absent, and starts it.

System Uptime Tracker should preserve that artifact-contained and rerunnable
workflow without copying the reference script unchanged. The initial Windows
contract is:

| Setting | Default |
|---|---|
| Service name | `SystemUptimeTrackerAgent` |
| Display name | `System Uptime Tracker Agent` |
| Executable | `SystemUptimeTracker.WindowsService.exe` |
| Application root | `C:\Program Files\SystemUptimeTracker\Agent` |
| Release directories | `C:\Program Files\SystemUptimeTracker\Agent\releases\<version>` |
| Durable data root | `C:\ProgramData\SystemUptimeTracker\Agent` |
| Default identity | `NT AUTHORITY\LocalService` unless a documented telemetry provider requires additional rights |
| Startup | Automatic, with restart-on-failure recovery actions |

Publish the dedicated Windows host as a self-contained, single-file
executable:

```powershell
dotnet publish src/SystemUptimeTracker/SystemUptimeTracker.WindowsService/SystemUptimeTracker.WindowsService.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:PublishTrimmed=false `
  --output artifacts/windows-service/win-x64
```

Single-file publishing is supported through runtime-specific `dotnet publish` operations. ([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?utm_source=chatgpt.com))

The published artifact must include:

- `SystemUptimeTracker.WindowsService.exe`
- `Install-SystemUptimeTrackerWindowsService.ps1`
- `Uninstall-SystemUptimeTrackerWindowsService.ps1`
- A non-secret configuration template
- An operator README containing install, upgrade, rollback, diagnostics, and
  uninstall procedures

The install script must be an advanced PowerShell script with named, validated
parameters, `SupportsShouldProcess`, terminating error handling, and an explicit
elevation check. Running the same command for a new install or an upgrade must
be safe. A typical invocation is:

```powershell
pwsh ./Install-SystemUptimeTrackerWindowsService.ps1 `
  -PackageVersion "1.0.0"
```

The installer must:

1. Validate elevation, parameters, the source executable, and non-overlapping
  source, application, and durable-data paths before changing the machine.
2. Detect the service with `Get-Service`; stop it only when present and wait for
  `Stopped` with a bounded timeout rather than a fixed sleep.
3. Stage and validate the new files in a versioned release directory, unblock
  only those packaged files, and leave `C:\ProgramData\SystemUptimeTracker`
  untouched during upgrades.
4. Create the service when absent or update its binary path when present. The
  service name used by Windows Service Control Manager must match the name
  configured by `AddWindowsService` in the host.
5. Configure the display name, description, automatic startup, and restart
  recovery actions. Every native command must have its exit code checked.
6. Configure and verify the selected service identity, deny unnecessary
  interactive access, and grant only the application-read, data-write,
  event-log, and outbound-network permissions the agent requires.
7. Start the service, wait for `Running` with a bounded timeout, and verify an
  observable startup signal before declaring success.
8. If startup validation fails, restore the previous binary path and release,
  restart the previous version, and return a terminating error.
9. Avoid accepting credentials on the command line or writing configuration
  objects, tokens, passwords, or API keys to installer output. Bootstrap
  credentials through a separate ACL-protected provisioning step.

Uninstall must stop and remove the service registration and application
releases. Durable identity, queue, and diagnostic data under `ProgramData` must
be retained by default and removed only through an explicit purge switch.

The FamilyTools script's positional `$args`, fixed sleeps, deletion of the live
directory before validation, unchecked `sc.exe` result, and post-deployment
identity reminder are findings to improve, not patterns to reproduce. See
[windows-service-reference.md](./windows-service-reference.md) for the detailed
comparison and packaging test expectations.

---

# Ubuntu Publication and Installation

Publish:

```bash
dotnet publish src/ComputerTelemetry.Agent \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=false \
  --output artifacts/linux-x64
```

Install under:

```text
/opt/computer-telemetry/
```

Example `systemd` unit:

```ini
[Unit]
Description=Computer Telemetry Agent
After=network-online.target
Wants=network-online.target

[Service]
Type=notify
ExecStart=/opt/computer-telemetry/ComputerTelemetry.Agent
WorkingDirectory=/opt/computer-telemetry
User=computertelemetry
Group=computertelemetry
Restart=always
RestartSec=10
Environment=DOTNET_ENVIRONMENT=Production

NoNewPrivileges=true
PrivateTmp=true
ProtectSystem=strict
ProtectHome=true
ReadWritePaths=/var/lib/computer-telemetry
StateDirectory=computer-telemetry

[Install]
WantedBy=multi-user.target
```

Register and start:

```bash
sudo cp computer-telemetry.service \
  /etc/systemd/system/computer-telemetry.service

sudo systemctl daemon-reload
sudo systemctl enable computer-telemetry
sudo systemctl start computer-telemetry
```

Check status:

```bash
sudo systemctl status computer-telemetry
```

Review logs:

```bash
sudo journalctl \
  --unit computer-telemetry \
  --follow
```

---

# Sleep and Hibernation Behavior

The heartbeat model already exposes gaps, but we can add explicit lifecycle detection later.

### Windows

Possible sources:

- Windows power broadcast events
- Windows Event Log
- WMI/CIM events
- Service shutdown notifications

### Linux

Possible sources:

- `systemd-logind` D-Bus events
- `/sys/power`
- Boot ID from `/proc/sys/kernel/random/boot_id`
- Uptime from `/proc/uptime`
- system journal events

For the MVP, I recommend:

- Record the OS boot identifier
- Record system uptime
- Record agent process start time
- Detect unexplained heartbeat gaps server-side

This provides dependable uptime history without immediately introducing complicated platform-specific event listeners.

---

# Power Usage

Actual electrical consumption is the least portable requirement.

Software may be able to collect:

- Battery charge and discharge rate on laptops
- Battery estimated remaining time
- CPU package energy on some Linux hardware through RAPL
- UPS load through vendor APIs or Network UPS Tools
- Hardware sensor information through platform-specific libraries

It generally cannot reliably determine whole-computer wall power consumption across arbitrary desktops and servers.

For accurate wattage, the best future integrations would be:

- Smart plug with a local API
- Network-connected PDU
- UPS with SNMP or USB monitoring
- Server hardware management such as iDRAC, iLO, or IPMI

I recommend making power telemetry optional:

```csharp
public interface IPowerTelemetryProvider
{
    Task<PowerTelemetry?> GetTelemetryAsync(
        CancellationToken cancellationToken);
}
```

Then implement providers only where supported.

---

# Reliability Requirements

The agent should continue working when the API is temporarily unavailable.

Recommended behavior:

```text
1. Generate heartbeat.
2. Try to send it.
3. If sending fails, save it to a small local queue.
4. Retry with exponential backoff.
5. Send queued heartbeats when connectivity returns.
6. Cap local storage by age and size.
```

A lightweight SQLite queue is preferable to keeping pending messages only in memory.

Suggested limits:

```text
Maximum queue age:   7 days
Maximum database:    100 MB
Retry progression:   15 sec, 30 sec, 1 min, 5 min, 15 min
```

The queue should distinguish:

- The heartbeat’s original occurrence time
- The later API receipt time

This prevents a network outage from appearing as a computer outage once queued records are uploaded.

---

# Recommended MVP Scope

The first usable release should include:

1. Cross-platform Worker Service
2. Windows Service installation
3. Ubuntu `systemd` installation
4. Persistent agent identifier
5. HTTPS heartbeat submission
6. ASP.NET Core ingestion API
7. SQL Server persistence
8. Machine and heartbeat records
9. Server-side runtime session calculation
10. Local retry queue
11. API-key authentication
12. CPU, memory, disk, OS, boot and agent-version telemetry
13. Health-check endpoint
14. Structured logging

ASP.NET Core health checks can verify application dependencies such as databases and external endpoints. ([Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-10.0&utm_source=chatgpt.com))

## Deliberately Deferred

- Web dashboard
- Real-time alerts
- Software inventory
- Process monitoring
- Remote commands
- Automatic updater
- Exact electrical power usage
- Multi-tenant organizations
- Mobile interface

---

# Key Design Decision

I recommend keeping the system **outbound-only**:

```text
Agent → HTTPS API
```

The server should not open connections back to monitored computers.

That makes deployment safer because:

- No inbound firewall ports are required
- Machines can remain behind NAT
- The agent has a smaller attack surface
- Azure or IIS hosting is straightforward
- It avoids accidentally becoming a remote administration tool

The next implementation step should be the solution skeleton, shared heartbeat contracts, initial database entities, and a functioning agent-to-API heartbeat path.

## User

I see needing three applications,
1. Web application API (Azure or IIS hosted)
2. Windows Service application that can register as a service
3. Systemd installable version that is compatible with Ubuntu.

Database will be SQL Server

For power consumption watt metering etc, I believe the "Shelly Plug US Gen4" may be able to be used. This device can integrate with other services, but I'm not sure how. The API would need to support messages from these smart wall plugs.

## ChatGPT

I’ll refine the design around three deployable applications and verify how the Shelly Plug US Gen4 can report real-time and historical power data into the API.

Yes—those should be treated as **three independently deployable applications**, while sharing common libraries and message contracts.

## Revised Solution Structure

```text
ComputerTelemetry.sln

src/
  ComputerTelemetry.Api/
  ComputerTelemetry.WindowsService/
  ComputerTelemetry.LinuxDaemon/

  ComputerTelemetry.Agent.Core/
  ComputerTelemetry.Contracts/
  ComputerTelemetry.Data/
  ComputerTelemetry.Shelly/
```

### Deployable applications

| Application | Target | Responsibility |
|---|---|---|
| `ComputerTelemetry.Api` | Azure, IIS, or Linux hosting | Receives computer heartbeats and Shelly power telemetry |
| `ComputerTelemetry.WindowsService` | Windows x64 | Runs through Windows Service Control Manager |
| `ComputerTelemetry.LinuxDaemon` | Ubuntu x64/ARM64 | Runs through `systemd` |

Although the Windows and Linux agents will be separate executables, most of their logic should live in `ComputerTelemetry.Agent.Core`.

That gives us:

```text
Windows-specific hosting
        ↓
Shared agent core
        ↑
Linux-specific hosting
```

The platform projects should mainly contain:

- Service registration
- Platform-specific lifecycle detection
- Platform-specific telemetry providers
- Installation and publication configuration

---

# SQL Server Architecture

The API will use:

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server or Azure SQL
- SQL migrations
- Health checks
- Structured logging

Recommended project ownership:

```text
ComputerTelemetry.Data
  ├── TelemetryDbContext
  ├── Database entities
  ├── Entity configurations
  ├── Migrations
  └── Repository/query services
```

---

# Shelly Plug US Gen4

The Shelly Plug US Gen4 is a good match for this system.

It includes a built-in power meter and supports:

- Instantaneous power measurement
- Energy measurement
- MQTT
- HTTP RPC
- Outbound WebSocket
- Webhooks
- Device scripts
- Cloud connectivity

The plug supports loads up to 15 amps or 1,800 watts. ([Shelly API Documentation](https://shelly-api-docs.shelly.cloud/gen2/Devices/Gen4/ShellyPlugUSG4/))

## Available Shelly Integration Models

There are three practical ways to integrate it.

### Option 1: Computer Agent Polls the Shelly Plug

```text
Shelly Plug
     ↓ Local HTTP
Computer Agent
     ↓ HTTPS POST
Telemetry API
```

The Windows or Linux agent discovers or is configured with the Shelly plug’s local IP address and periodically requests its status.

A local request would use Shelly’s RPC API:

```http
POST http://192.168.1.50/rpc
Content-Type: application/json
```

```json
{
  "id": 1,
  "method": "Shelly.GetStatus"
}
```

Shelly supports one-shot HTTP RPC calls through its `/rpc` endpoint. ([Shelly API Documentation](https://shelly-api-docs.shelly.cloud/gen2/General/RPCChannels/))

### Advantages

- Simplest MVP
- No MQTT broker required
- Power usage naturally correlates with the computer heartbeat
- Continues working without Shelly Cloud
- The API remains HTTPS-only and internet-facing
- The Shelly device does not need direct internet access to our API

### Limitation

When the computer is off, the agent cannot poll the plug. This means we would not receive power readings during shutdown unless the plug reports separately.

That may actually be useful behavior for uptime tracking, but it does not capture standby power consumption while the computer is off.

---

### Option 2: Shelly Reports Through MQTT

```text
Shelly Plug
       ↓ MQTT/TLS
MQTT Broker
       ↓
Shelly Ingestion Worker
       ↓
SQL Server
```

Shelly can connect outbound to an MQTT broker and publish:

- RPC notifications
- Component status changes
- Online and offline status
- Power-related status updates

Shelly supports MQTT QoS 1, which provides at-least-once message delivery. It can publish complete component status under topics such as:

```text
<device-prefix>/status/switch:0
```

It also publishes connection status under:

```text
<device-prefix>/online
```

([Shelly API Documentation](https://shelly-api-docs.shelly.cloud/gen2/ComponentsAndServices/Mqtt))

Possible broker choices include:

- Azure Event Grid MQTT
- Azure IoT Hub
- Self-hosted Mosquitto
- EMQX
- HiveMQ Cloud

### Advantages

- Plug reports while the computer is off
- Near-real-time readings
- Strong device presence tracking
- Supports many plugs efficiently
- Better long-term IoT architecture

### Disadvantages

- Requires an MQTT broker
- More infrastructure
- Requires certificate and credential management
- QoS 1 means duplicate messages are possible and must be handled idempotently

---

### Option 3: Shelly Outbound WebSocket

```text
Shelly Plug
       ↓ Secure WebSocket
Telemetry API WebSocket endpoint
       ↓
SQL Server
```

Shelly Gen2+ devices can establish and maintain an outbound WebSocket connection. The connection supports RPC communication and unsolicited status notifications. ([Shelly API Documentation](https://shelly-api-docs.shelly.cloud/gen2/ComponentsAndServices/Ws/))

### Advantages

- No separate MQTT broker
- Plug can report directly
- Persistent real-time connection
- The API can request current plug status

### Disadvantages

- More difficult to scale than standard HTTP ingestion
- Persistent connection handling is required
- Azure App Service configuration becomes more important
- Authentication and connection identity require careful design
- Reconnection and load balancing must be handled

---

# Recommended Approach

I recommend supporting two Shelly paths.

## Phase 1: Agent-Mediated Shelly Collection

The Windows and Linux agents poll the Shelly plug over the local network.

```text
Shelly Plug → Agent → API
```

This provides the fastest path to a functioning product.

Agent configuration:

```json
{
  "TelemetryApi": {
    "BaseUrl": "https://telemetry.example.com",
    "ApiKey": "agent-key"
  },
  "Shelly": {
    "Enabled": true,
    "Devices": [
      {
        "DeviceId": "office-computer-plug",
        "Host": "192.168.1.50",
        "Username": "admin",
        "Password": "protected-secret",
        "ComputerAssociation": "local"
      }
    ],
    "PollingIntervalSeconds": 30
  }
}
```

## Phase 2: Direct MQTT Integration

Add an MQTT ingestion process for standalone Shelly reporting.

This could initially run inside the API application as a hosted service:

```csharp
public sealed class ShellyMqttIngestionService : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        // Connect to broker.
        // Subscribe to Shelly topics.
        // Normalize messages.
        // Store readings.
    }
}
```

At larger scale, it should become a separate deployable service:

```text
ComputerTelemetry.ShellyIngestion
```

For the initial scope, it does not need to count as a fourth application.

---

# API Endpoint Design

The API should not create a completely separate endpoint for every device brand. Instead, it should have normalized telemetry endpoints plus optional vendor-specific ingestion endpoints.

## Computer heartbeat

```http
POST /api/v1/agents/{agentId}/heartbeats
```

## Agent-collected power reading

```http
POST /api/v1/agents/{agentId}/power-readings
```

## Combined heartbeat

The agent could also include power data with the machine heartbeat:

```json
{
  "messageId": "3e67c926-c597-4c8a-b90c-c1f585ca84cb",
  "agentId": "15a41025-8209-4767-a957-1415fb117d29",
  "machineId": "cd55641e-756e-49fc-999b-2a183107cfb4",
  "sentAtUtc": "2026-07-25T16:32:00Z",
  "system": {
    "machineName": "OFFICE-PC",
    "uptimeSeconds": 287400,
    "cpuUsagePercent": 18.4,
    "memoryUsagePercent": 54.7
  },
  "powerMeters": [
    {
      "meterId": "office-computer-plug",
      "vendor": "Shelly",
      "model": "Shelly Plug US Gen4",
      "measuredAtUtc": "2026-07-25T16:31:59Z",
      "isOutputOn": true,
      "voltage": 121.3,
      "currentAmps": 1.84,
      "activePowerWatts": 214.7,
      "totalEnergyWattHours": 41862.3
    }
  ]
}
```

## Direct Shelly ingestion

For WebSocket or a future Shelly script/webhook integration:

```http
POST /api/v1/integrations/shelly/events
```

The vendor-specific endpoint translates Shelly payloads into the normalized internal model.

---

# Power Data Model

## PowerMeterDevice

```text
PowerMeterDeviceId
ExternalDeviceId
Vendor
Model
DeviceName
MacAddress
FirmwareVersion
ConnectionType
FirstSeenAtUtc
LastSeenAtUtc
IsActive
```

Possible `ConnectionType` values:

```text
AgentPolling
Mqtt
WebSocket
Webhook
ShellyCloud
```

## MachinePowerMeterAssociation

```text
AssociationId
MachineId
PowerMeterDeviceId
EffectiveFromUtc
EffectiveToUtc
IsPrimary
```

This association table is important. A plug may be moved from one computer to another, so the plug should not permanently contain a `MachineId`.

## PowerReading

```text
PowerReadingId
PowerMeterDeviceId
MessageId
MeasuredAtUtc
ReceivedAtUtc
Voltage
CurrentAmps
ActivePowerWatts
ApparentPowerVoltAmps
PowerFactor
FrequencyHz
TotalEnergyWattHours
ReturnedEnergyWattHours
OutputIsOn
DeviceTemperatureCelsius
RawPayload
```

Not every Shelly payload will supply every field, so most measurement columns should be nullable.

## PowerUsageInterval

Aggregated intervals can be generated later:

```text
PowerUsageIntervalId
PowerMeterDeviceId
IntervalStartUtc
IntervalEndUtc
AverageWatts
MinimumWatts
MaximumWatts
EnergyUsedWattHours
ReadingCount
```

This supports reports such as:

- Total kilowatt-hours by computer
- Average running wattage
- Standby power consumption
- Cost per day
- Cost per month
- Power consumed during active uptime
- Power consumed while the computer is apparently off
- Unexpected energy spikes

---

# Correlating Power and Uptime

The combined data permits more accurate state detection.

Example rules:

| Heartbeats | Power | Inferred state |
|---|---:|---|
| Present | Above operating threshold | Running |
| Missing | Near zero | Shut down or unplugged |
| Missing | Low stable wattage | Sleeping, hibernating, or soft-off |
| Missing | High wattage | Agent failure, OS issue, or network issue |
| Present | Zero watts | Meter association or power-reading problem |

We should make the thresholds configurable per machine:

```json
{
  "PowerStateThresholds": {
    "OffMaximumWatts": 3,
    "SleepMaximumWatts": 15,
    "RunningMinimumWatts": 20
  }
}
```

We should not hard-code these globally because different workstations, servers, laptops, monitors, and peripherals have very different power profiles.

---

# Service Project Design

## Shared worker

```csharp
public sealed class TelemetryWorker : BackgroundService
{
    private readonly ISystemTelemetryCollector _systemCollector;
    private readonly IEnumerable<IPowerMeterProvider> _powerProviders;
    private readonly ITelemetryPublisher _publisher;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        do
        {
            var systemTelemetry =
                await _systemCollector.CollectAsync(stoppingToken);

            var powerReadings = new List<PowerMeterReading>();

            foreach (IPowerMeterProvider provider in _powerProviders)
            {
                PowerMeterReading? reading =
                    await provider.ReadAsync(stoppingToken);

                if (reading is not null)
                {
                    powerReadings.Add(reading);
                }
            }

            await _publisher.PublishAsync(
                systemTelemetry,
                powerReadings,
                stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
```

## Shelly provider

```csharp
public interface IPowerMeterProvider
{
    string ProviderName { get; }

    Task<PowerMeterReading?> ReadAsync(
        CancellationToken cancellationToken);
}
```

```csharp
public sealed class ShellyPowerMeterProvider
    : IPowerMeterProvider
{
    private readonly HttpClient _httpClient;

    public string ProviderName => "Shelly";

    public async Task<PowerMeterReading?> ReadAsync(
        CancellationToken cancellationToken)
    {
        // Invoke Shelly.GetStatus.
        // Read switch:0 telemetry.
        // Convert vendor payload to normalized model.
        // Return normalized reading.
        throw new NotImplementedException();
    }
}
```

This plugin-style interface lets us later support:

- Shelly
- UPS devices
- IPMI
- iDRAC
- iLO
- SNMP PDUs
- Other smart plugs

without changing the core worker.

---

# Updated MVP Boundary

## Application 1: API

Includes:

- Computer heartbeat endpoint
- Power-reading endpoint
- Shelly integration endpoint
- SQL Server persistence
- Runtime-session calculation
- Machine and meter association
- Authentication
- Health checks
- Basic administrative API

## Application 2: Windows Service

Includes:

- Windows Service registration support
- Artifact-contained PowerShell install and uninstall entry points
- Idempotent first install and versioned upgrades
- Automatic startup, restart-on-failure recovery, and startup validation
- Failed-upgrade rollback with durable state retained under `ProgramData`
- Explicit least-privilege service identity and filesystem ACL configuration
- Windows telemetry
- Local Shelly polling
- Local retry queue
- Sleep, resume and shutdown awareness

## Application 3: Ubuntu Daemon

Includes:

- `systemd` integration
- Linux telemetry
- Local Shelly polling
- Local retry queue
- Linux boot ID and uptime collection
- Suspend and resume awareness

## Shared libraries

Includes:

- API contracts
- Agent worker logic
- SQL Server data layer
- Shelly normalization
- Authentication
- Validation

---

# Primary Architectural Recommendation

Start with this flow:

```text
Shelly Plug US Gen4
          ↓ Local HTTP RPC
Windows/Linux Agent
          ↓ HTTPS
ASP.NET Core API
          ↓
SQL Server
```

Design the database and API so that MQTT can be added without changing existing computer-agent contracts:

```text
Shelly Plug
     ↓ MQTT/TLS
MQTT Broker
     ↓
Shelly ingestion service
     ↓
Same normalized power-reading storage
```

This gives us a simple initial deployment while preserving the ability to monitor power when the computer itself is shut down.

## User

Shelly Plug US Gen4 will need to have two options for data. One option is were I can identify a single Shelly Plug US Gen4 with a single computer or device. This is ideal and an API contract should be available for this scenario were I can link Shelly Plug US Gen4 1:1 with a computer. However, the reality is that I have multiple computers, monitors, and power strips connected. Shelly Plug US Gen4 will also need a more general endpoint that is simply reporting power across multiple devices.

In the database I will want / need a way to link multiple devices to a single reporting Shelly Plug US Gen4. I will want location metadata that can link the location plus the devices to a single Shelly Plug US Gen4.

## ChatGPT

Agreed. The Shelly plug should be modeled as an independent **power meter**, not as a child of a computer. Computers, monitors, power strips, and other equipment should then be associated with that meter.

## Core Relationship Model

The database should support both:

```text
1 Shelly plug → 1 computer
```

and:

```text
1 Shelly plug → many devices
```

The generalized model is:

```text
Location
   └── Power Meter
          └── Meter Associations
                 ├── Computer
                 ├── Monitor
                 ├── Network switch
                 ├── Power strip
                 └── Other equipment
```

The 1:1 computer scenario is simply a meter with one active association.

---

# Recommended Database Model

## Location

Represents the physical area where equipment is installed.

```text
Location
--------
LocationId
ParentLocationId
Name
LocationType
Description
TimeZone
AddressLine1
AddressLine2
City
State
PostalCode
CountryCode
IsActive
CreatedAtUtc
UpdatedAtUtc
```

Example hierarchy:

```text
Home
 └── Basement Office
      ├── West Desk
      └── Server Rack

Business Office
 └── Engineering Area
      └── Desk 14
```

`ParentLocationId` allows nested locations without forcing a rigid structure.

Possible `LocationType` values:

```text
Site
Building
Floor
Room
Office
Desk
Rack
Lab
Other
```

---

## MonitoredDevice

A general inventory record for anything consuming power.

```text
MonitoredDevice
---------------
MonitoredDeviceId
LocationId
DeviceType
Name
Description
Manufacturer
Model
SerialNumber
AssetTag
MachineId
IsPowerConsumer
IsActive
CreatedAtUtc
UpdatedAtUtc
```

Possible `DeviceType` values:

```text
Computer
Server
Monitor
PowerStrip
NetworkSwitch
Router
Printer
StorageDevice
UPS
Peripheral
Appliance
Other
```

For agent-enabled computers:

```text
MonitoredDevice.MachineId
```

links to the machine reporting uptime and telemetry.

For monitors and other non-agent devices, `MachineId` remains null.

---

## PowerMeter

Represents the physical Shelly Plug US Gen4.

```text
PowerMeter
----------
PowerMeterId
LocationId
ExternalDeviceId
Vendor
Model
Name
MacAddress
IpAddress
FirmwareVersion
ConnectionType
AuthenticationReference
FirstSeenAtUtc
LastSeenAtUtc
IsActive
CreatedAtUtc
UpdatedAtUtc
```

Example:

```text
Name: Basement Office Main Desk Plug
Vendor: Shelly
Model: Plug US Gen4
Location: Basement Office / West Desk
```

Avoid storing the Shelly password directly in this table. Store a secret reference or use encrypted secret storage.

---

## PowerMeterDeviceAssociation

This is the central relationship table.

```text
PowerMeterDeviceAssociation
---------------------------
AssociationId
PowerMeterId
MonitoredDeviceId
EffectiveFromUtc
EffectiveToUtc
AssociationType
EstimatedSharePercent
Notes
IsPrimary
CreatedAtUtc
UpdatedAtUtc
```

This supports both scenarios.

### One computer per plug

```text
Shelly Plug 1
 └── Desktop Computer
```

```text
AssociationType: Dedicated
EstimatedSharePercent: 100
IsPrimary: true
```

### Multiple devices per plug

```text
Shelly Plug 2
 ├── Desktop Computer
 ├── Monitor 1
 ├── Monitor 2
 ├── Speakers
 └── Desk Power Strip
```

```text
AssociationType: Shared
```

The measured wattage belongs to the meter as a whole. The system should not pretend it knows each device’s individual consumption unless you later provide allocation rules or additional meters.

---

# Important Modeling Rule

Do not assign every power reading to each associated device.

That would multiply the measured energy incorrectly.

For example, a Shelly reading of:

```text
320 watts
```

with four associated devices does not mean each device consumed 320 watts.

The authoritative record should be:

```text
PowerMeter 17 consumed 320 watts
```

The device associations explain what equipment was likely included in that measurement.

Any device-level allocation should be handled separately as an estimate.

---

# Optional Allocation Model

If you eventually want estimated consumption per device, add:

```text
PowerAllocationRule
-------------------
PowerAllocationRuleId
AssociationId
AllocationMethod
FixedWatts
Percentage
Priority
EffectiveFromUtc
EffectiveToUtc
```

Possible methods:

```text
None
Percentage
FixedBaseline
AgentActivityWeighted
Manual
```

Example:

| Device | Allocation |
|---|---:|
| Computer | 70% |
| Monitor 1 | 12% |
| Monitor 2 | 12% |
| Speakers | 6% |

This should always be labeled **estimated**, not measured.

A better future calculation could use computer telemetry:

```text
Computer off + meter reading 45 watts
```

That 45 watts can become the estimated baseline for monitors, peripherals, and standby consumption.

---

# API Contract Strategy

I recommend one general power-reading endpoint and one convenience endpoint for the dedicated-computer scenario.

## 1. General Meter Reading Endpoint

This is the primary endpoint.

```http
POST /api/v1/power-meters/{powerMeterId}/readings
```

```json
{
  "messageId": "af1f36dc-5d72-4a84-91ef-63c4d03c3f85",
  "externalDeviceId": "shellyplugusg4-a1b2c3",
  "measuredAtUtc": "2026-07-25T16:31:59Z",
  "sentAtUtc": "2026-07-25T16:32:00Z",
  "outputIsOn": true,
  "voltage": 121.3,
  "currentAmps": 2.64,
  "activePowerWatts": 319.7,
  "apparentPowerVoltAmps": 326.4,
  "powerFactor": 0.979,
  "frequencyHz": 60.0,
  "totalEnergyWattHours": 52146.8,
  "temperatureCelsius": 41.2,
  "source": "AgentPolling"
}
```

The server adds:

```text
ReceivedAtUtc
```

This endpoint does not care whether the plug powers one device or twenty devices.

The database associations determine the reporting context.

---

## 2. Dedicated Computer Power Endpoint

For the 1:1 case:

```http
POST /api/v1/machines/{machineId}/power-readings
```

```json
{
  "messageId": "e52dcbdc-93eb-419b-abab-7a8a59c75591",
  "powerMeterExternalId": "shellyplugusg4-a1b2c3",
  "measuredAtUtc": "2026-07-25T16:31:59Z",
  "activePowerWatts": 186.4,
  "voltage": 120.8,
  "currentAmps": 1.57,
  "totalEnergyWattHours": 21830.7
}
```

The API should validate that:

```text
PowerMeter → Machine association is active
AssociationType = Dedicated
```

This endpoint is a convenience contract. Internally, it should normalize into the same `PowerReading` entity used by the general endpoint.

---

# Device Association API

## Associate one device

```http
POST /api/v1/power-meters/{powerMeterId}/device-associations
```

```json
{
  "monitoredDeviceId": "2ea602ce-52d0-4e23-bef2-b319092cda39",
  "associationType": "Dedicated",
  "isPrimary": true,
  "estimatedSharePercent": 100,
  "effectiveFromUtc": "2026-07-25T16:00:00Z",
  "notes": "Dedicated office workstation"
}
```

## Associate multiple devices

```http
PUT /api/v1/power-meters/{powerMeterId}/device-associations
```

```json
{
  "effectiveFromUtc": "2026-07-25T16:00:00Z",
  "devices": [
    {
      "monitoredDeviceId": "desktop-id",
      "associationType": "Shared",
      "isPrimary": true
    },
    {
      "monitoredDeviceId": "monitor-one-id",
      "associationType": "Shared",
      "isPrimary": false
    },
    {
      "monitoredDeviceId": "monitor-two-id",
      "associationType": "Shared",
      "isPrimary": false
    },
    {
      "monitoredDeviceId": "power-strip-id",
      "associationType": "Shared",
      "isPrimary": false
    }
  ]
}
```

The API should close previous active associations by setting:

```text
EffectiveToUtc
```

This preserves historical accuracy when equipment is moved.

---

# Location API

## Create a location

```http
POST /api/v1/locations
```

```json
{
  "name": "West Desk",
  "locationType": "Desk",
  "parentLocationId": "basement-office-id",
  "description": "Primary development workstation",
  "timeZone": "America/Denver"
}
```

## Assign meter to location

```http
PUT /api/v1/power-meters/{powerMeterId}/location
```

```json
{
  "locationId": "west-desk-location-id",
  "effectiveFromUtc": "2026-07-25T16:00:00Z"
}
```

For historical location tracking, use a separate table rather than only updating `PowerMeter.LocationId`.

```text
PowerMeterLocationHistory
-------------------------
PowerMeterLocationHistoryId
PowerMeterId
LocationId
EffectiveFromUtc
EffectiveToUtc
Notes
```

This lets reports accurately state where energy was consumed at the time.

---

# Reporting Views

The API should support reporting from three perspectives.

## Meter-level

```http
GET /api/v1/power-meters/{powerMeterId}/usage
```

Returns total measured power for everything connected to the plug.

## Location-level

```http
GET /api/v1/locations/{locationId}/power-usage
```

Aggregates all meters currently or historically associated with that location.

## Device-context view

```http
GET /api/v1/monitored-devices/{deviceId}/power-context
```

Returns readings from meters associated with the device, with an explicit classification:

```json
{
  "deviceId": "desktop-id",
  "meterId": "main-desk-plug-id",
  "measurementType": "SharedMeter",
  "measuredEnergyWattHours": 3840.4,
  "estimatedDeviceEnergyWattHours": null,
  "associatedDevices": [
    "Desktop",
    "Monitor 1",
    "Monitor 2",
    "Speakers"
  ]
}
```

For a dedicated meter:

```json
{
  "measurementType": "DedicatedMeter",
  "measuredEnergyWattHours": 3840.4,
  "estimatedDeviceEnergyWattHours": 3840.4
}
```

---

# Revised Entity Relationships

```text
Location
  1 ─────── * MonitoredDevice

Location
  1 ─────── * PowerMeterLocationHistory

PowerMeter
  1 ─────── * PowerReading

PowerMeter
  1 ─────── * PowerMeterDeviceAssociation

MonitoredDevice
  1 ─────── * PowerMeterDeviceAssociation

Machine
  1 ─────── 0..1 MonitoredDevice
```

Conceptually, this is a time-aware many-to-many relationship:

```text
PowerMeter * ↔ * MonitoredDevice
```

A device could theoretically be associated with more than one meter—for example:

- Computer connected through a metered UPS
- UPS connected through a Shelly plug
- Rack also monitored through a PDU

The data model should allow this, although the API can warn about potentially overlapping measurements.

---

# Power Strip Handling

A power strip can be represented in two ways.

## Simple approach

Treat it as another monitored device:

```text
Shelly Plug
 └── Power Strip
      ├── Computer
      ├── Monitor
      └── Speakers
```

This requires device-to-device hierarchy:

```text
MonitoredDevice.ParentDeviceId
```

Recommended field:

```text
ParentMonitoredDeviceId
```

Then the meter can associate directly with the power strip, and reports can expand its child devices.

## Direct association approach

Associate every powered device directly with the Shelly meter.

```text
Shelly Plug
 ├── Computer
 ├── Monitor
 └── Speakers
```

I recommend supporting both but using the hierarchical model when the physical topology matters.

---

# Recommended Final Model

Add these primary tables:

```text
Location
MonitoredDevice
Machine
PowerMeter
PowerReading
PowerMeterDeviceAssociation
PowerMeterLocationHistory
PowerAllocationRule
```

Add device hierarchy through:

```text
MonitoredDevice.ParentMonitoredDeviceId
```

The API should treat meter readings as authoritative at the **power-meter level**, while the association structure provides:

- Dedicated computer measurement
- Shared multi-device measurement
- Location-based reporting
- Historical movement tracking
- Power-strip topology
- Optional estimated allocation
- Future support for UPS, PDU, SNMP, and other smart meters

The most important design distinction is:

```text
Measured power belongs to the meter.
Context belongs to its location and device associations.
Estimated power may optionally be allocated to individual devices.
```

## User

I would like to be able to link / associate the reporting computer from the background service to a Shelly Plug US Gen4. This should be optional but allowed.

## ChatGPT

Yes. The **reporting computer should optionally associate itself with a Shelly power meter**, while keeping the Shelly meter independent from the computer.

## Recommended Relationship

```text
Reporting Agent
      ↓ identifies
Machine
      ↓ optional association
Power Meter
      ↓ measures
One or more Monitored Devices
```

A reporting computer can therefore be:

1. Directly powered by the Shelly plug.
2. One of several devices powered through that plug.
3. Merely responsible for polling the Shelly plug, without being powered by it.
4. Unassociated with any Shelly plug.

These distinctions should be explicit.

## Machine-to-Power-Meter Association

Add a time-aware association table:

```text
MachinePowerMeterAssociation
----------------------------
MachinePowerMeterAssociationId
MachineId
PowerMeterId
RelationshipType
EffectiveFromUtc
EffectiveToUtc
IsPrimary
CreatedAtUtc
UpdatedAtUtc
```

Recommended `RelationshipType` values:

```text
DedicatedLoad
SharedLoad
CollectorOnly
```

### Meaning

| Relationship | Meaning |
|---|---|
| `DedicatedLoad` | The Shelly plug exclusively powers the reporting computer |
| `SharedLoad` | The reporting computer is one of several devices powered by the plug |
| `CollectorOnly` | The computer polls/reports the Shelly data but is not powered by that plug |

This avoids confusing **who reports the reading** with **what consumes the power**.

## Connection to MonitoredDevice

Every reporting machine should also have a corresponding `MonitoredDevice` record:

```text
Machine
  1 ─────── 1 MonitoredDevice
```

For example:

```text
Machine
-------
MachineId: M-100
MachineName: DEV-WORKSTATION-01

MonitoredDevice
---------------
MonitoredDeviceId: D-100
MachineId: M-100
DeviceType: Computer
Name: Development Workstation
```

The power-meter device association then represents the physical load:

```text
PowerMeterDeviceAssociation
---------------------------
PowerMeterId: P-200
MonitoredDeviceId: D-100
AssociationType: Dedicated
```

The machine-level association represents the operational relationship:

```text
MachinePowerMeterAssociation
----------------------------
MachineId: M-100
PowerMeterId: P-200
RelationshipType: DedicatedLoad
```

This is slightly redundant, but it gives the API a clear machine-centric contract while retaining the generalized equipment inventory model.

## Dedicated Computer Scenario

```text
Shelly Plug P-200
  └── Development Workstation D-100

Reporting Machine M-100
  ├── corresponds to D-100
  ├── polls P-200
  └── reports P-200 telemetry
```

Associations:

```text
MachinePowerMeterAssociation:
  MachineId: M-100
  PowerMeterId: P-200
  RelationshipType: DedicatedLoad

PowerMeterDeviceAssociation:
  PowerMeterId: P-200
  MonitoredDeviceId: D-100
  AssociationType: Dedicated
```

## Shared Equipment Scenario

```text
Shelly Plug P-300
  └── Desk Power Strip
       ├── Development Workstation
       ├── Monitor 1
       ├── Monitor 2
       └── Speakers
```

The reporting computer is one of the loads:

```text
MachinePowerMeterAssociation:
  MachineId: M-100
  PowerMeterId: P-300
  RelationshipType: SharedLoad
```

The physical device associations include:

```text
P-300 → Development Workstation
P-300 → Monitor 1
P-300 → Monitor 2
P-300 → Speakers
```

The reading still belongs only to `P-300`. It is not duplicated across the associated devices.

## Collector-Only Scenario

A Linux server might poll a Shelly plug powering unrelated equipment:

```text
Linux telemetry server
      ↓ polls
Shelly Plug
      ↓ powers
Network equipment
```

Association:

```text
MachinePowerMeterAssociation:
  MachineId: M-500
  PowerMeterId: P-600
  RelationshipType: CollectorOnly
```

Physical associations:

```text
P-600 → Router
P-600 → Network Switch
P-600 → Wireless Access Point
```

The reporting server is not included as a power consumer.

# Agent Configuration

The Windows or Linux service should allow the relationship to be configured locally:

```json
{
  "Shelly": {
    "Devices": [
      {
        "ExternalDeviceId": "shellyplugusg4-a1b2c3",
        "Host": "192.168.1.50",
        "RelationshipToReportingMachine": "DedicatedLoad",
        "IsPrimary": true
      }
    ]
  }
}
```

Shared example:

```json
{
  "Shelly": {
    "Devices": [
      {
        "ExternalDeviceId": "shellyplugusg4-d4e5f6",
        "Host": "192.168.1.51",
        "RelationshipToReportingMachine": "SharedLoad",
        "IsPrimary": true
      }
    ]
  }
}
```

Collector-only example:

```json
{
  "Shelly": {
    "Devices": [
      {
        "ExternalDeviceId": "shellyplugusg4-g7h8i9",
        "Host": "192.168.1.52",
        "RelationshipToReportingMachine": "CollectorOnly",
        "IsPrimary": false
      }
    ]
  }
}
```

The entire `Shelly` section remains optional.

# API Contracts

## Associate a Meter with a Machine

```http
POST /api/v1/machines/{machineId}/power-meter-associations
```

```json
{
  "powerMeterId": "4e00a5a9-dc2e-47e8-b670-cff1c0cab207",
  "relationshipType": "SharedLoad",
  "isPrimary": true,
  "effectiveFromUtc": "2026-07-25T16:00:00Z"
}
```

## Retrieve Machine Associations

```http
GET /api/v1/machines/{machineId}/power-meter-associations
```

Example response:

```json
{
  "machineId": "54769f81-e5bb-4aa5-a3ed-b055d6211161",
  "powerMeters": [
    {
      "powerMeterId": "4e00a5a9-dc2e-47e8-b670-cff1c0cab207",
      "name": "West Desk Power",
      "vendor": "Shelly",
      "model": "Plug US Gen4",
      "relationshipType": "SharedLoad",
      "isPrimary": true,
      "location": {
        "locationId": "a6df1cf6-a852-47c9-91c4-f3ebd36f3742",
        "name": "West Desk"
      },
      "associatedDevices": [
        "Development Workstation",
        "Monitor 1",
        "Monitor 2",
        "Speakers"
      ]
    }
  ]
}
```

## Remove or End an Association

Prefer ending the association historically rather than deleting it:

```http
DELETE /api/v1/machines/{machineId}/power-meter-associations/{associationId}
```

The API sets:

```text
EffectiveToUtc = current UTC time
```

# Agent Self-Registration

The background service can optionally declare its Shelly relationship during agent registration:

```http
POST /api/v1/agents/register
```

```json
{
  "agentId": "ddc7ca89-11c4-455d-a2c4-2113829de101",
  "machine": {
    "machineName": "DEV-WORKSTATION-01",
    "operatingSystem": "Windows 11",
    "architecture": "X64"
  },
  "powerMeters": [
    {
      "externalDeviceId": "shellyplugusg4-a1b2c3",
      "vendor": "Shelly",
      "model": "Plug US Gen4",
      "relationshipType": "DedicatedLoad",
      "isPrimary": true
    }
  ]
}
```

The API can then:

1. Find or create the machine.
2. Find or register the power meter.
3. Create the machine-to-meter association.
4. Create the corresponding monitored-device association when appropriate.

For security, an agent should not be allowed to freely reassign an already registered meter unless its credentials authorize that operation.

# Heartbeat Contract

The heartbeat can reference associated meters without redefining the relationship each time:

```json
{
  "messageId": "c20ef222-aea3-4ec0-8be3-f00aec27e41a",
  "machineId": "54769f81-e5bb-4aa5-a3ed-b055d6211161",
  "sentAtUtc": "2026-07-25T16:32:00Z",
  "uptimeSeconds": 287400,
  "powerReadings": [
    {
      "powerMeterId": "4e00a5a9-dc2e-47e8-b670-cff1c0cab207",
      "measuredAtUtc": "2026-07-25T16:31:59Z",
      "activePowerWatts": 319.7,
      "voltage": 121.3,
      "currentAmps": 2.64,
      "totalEnergyWattHours": 52146.8
    }
  ]
}
```

The server validates that the agent is authorized to report for that meter.

## Recommended Rules

- A machine can have zero, one, or multiple Shelly associations.
- One association can be marked as the machine’s primary power meter.
- Only one active `IsPrimary` association should exist per machine.
- A Shelly plug can be associated with multiple monitored devices.
- A Shelly plug may be reported by a machine it does not power.
- Machine and device associations should be historically effective-dated.
- Power readings remain owned by the power meter.
- Machine-level power is considered directly measured only for `DedicatedLoad`.
- `SharedLoad` readings must be labeled as shared aggregate measurements.
- `CollectorOnly` readings must never be attributed to the reporting machine’s consumption.

This preserves a clean distinction between **reporting**, **physical power consumption**, and **device inventory**.

## User

I should be able to add Shelly Plug US Gen4 independent of computers and in the converse, I should be able to add computers independent of computers.

When I first deploy this, I will only have the background services monitoring computers without the Shelly Plug US Gen4. These will come later.

However, someone may want to add Shelly Plug US Gen4 first and then use the computer services later.

## ChatGPT

Correct. **Computers and Shelly plugs must be independent first-class entities.** Neither should require the other to exist.

I’m interpreting “add computers independent of computers” as **add computers independently of Shelly plugs**.

## Required Lifecycle Support

The system must support all of these sequences:

### Computer-first

```text
1. Install Windows or Linux agent
2. Agent registers computer
3. Computer reports uptime and telemetry
4. Shelly plug is added later
5. Computer and Shelly plug are optionally associated
```

### Shelly-first

```text
1. Register Shelly plug
2. Shelly reports aggregate power data
3. Add location and connected equipment
4. Computer agent is installed later
5. Existing computer and Shelly records are associated
```

### Independent operation

```text
Computer without Shelly
Shelly without computer
Computer with dedicated Shelly
Computer sharing a Shelly with other devices
Shelly monitored by an unrelated collector
```

## Independent Core Entities

```text
Machine
PowerMeter
MonitoredDevice
Location
```

Each entity should be creatable independently.

### Machine

Created either through:

- Automatic background-agent registration
- Manual administration
- API import

```text
Machine
-------
MachineId
AgentId
MachineName
OperatingSystem
Architecture
FirstSeenAtUtc
LastSeenAtUtc
RegistrationStatus
IsActive
```

No `PowerMeterId` should exist directly on this table.

### PowerMeter

Created through:

- Manual administration
- Shelly discovery
- Shelly webhook or MQTT registration
- Agent-assisted discovery

```text
PowerMeter
----------
PowerMeterId
ExternalDeviceId
Vendor
Model
Name
MacAddress
FirmwareVersion
FirstSeenAtUtc
LastSeenAtUtc
RegistrationStatus
IsActive
```

No `MachineId` should exist directly on this table.

## Associations Added Separately

Relationships should be created only when needed.

```text
MachinePowerMeterAssociation
PowerMeterDeviceAssociation
PowerMeterLocationHistory
```

This avoids making either entity dependent on the other.

## Recommended Entity Relationships

```text
Machine
   │
   │ optional
   ▼
MachinePowerMeterAssociation
   ▲
   │ optional
PowerMeter
   │
   ├── PowerReading
   │
   ├── PowerMeterDeviceAssociation
   │        └── MonitoredDevice
   │
   └── PowerMeterLocationHistory
            └── Location
```

A machine can exist with zero associations:

```text
Machine → no Shelly plug
```

A Shelly plug can also exist with zero computer associations:

```text
Shelly plug → no reporting computer
```

## Registration APIs

### Register a computer independently

```http
POST /api/v1/machines
```

```json
{
  "machineName": "DEV-WORKSTATION-01",
  "operatingSystem": "Windows 11",
  "architecture": "X64"
}
```

For automatic agent enrollment:

```http
POST /api/v1/agents/register
```

```json
{
  "agentId": "ddc7ca89-11c4-455d-a2c4-2113829de101",
  "machineName": "DEV-WORKSTATION-01",
  "operatingSystem": "Windows 11",
  "architecture": "X64",
  "agentVersion": "1.0.0"
}
```

No Shelly information is required.

### Register a Shelly plug independently

```http
POST /api/v1/power-meters
```

```json
{
  "externalDeviceId": "shellyplugusg4-a1b2c3",
  "vendor": "Shelly",
  "model": "Plug US Gen4",
  "name": "West Desk Power",
  "macAddress": "A1:B2:C3:D4:E5:F6",
  "locationId": "optional-location-id"
}
```

No computer or agent information is required.

### Associate them later

```http
POST /api/v1/machine-power-meter-associations
```

```json
{
  "machineId": "machine-guid",
  "powerMeterId": "power-meter-guid",
  "relationshipType": "SharedLoad",
  "isPrimary": true,
  "effectiveFromUtc": "2026-07-25T16:00:00Z"
}
```

## Computer Agent Behavior

The Windows and Linux services should not assume a Shelly plug exists.

Default configuration:

```json
{
  "TelemetryApi": {
    "BaseUrl": "https://telemetry.example.com",
    "ApiKey": "agent-key"
  },
  "Heartbeat": {
    "IntervalSeconds": 60
  },
  "Shelly": {
    "Enabled": false,
    "Devices": []
  }
}
```

The service should fully operate with:

```text
Shelly.Enabled = false
```

It will still report:

- Machine identity
- Operating system
- Boot time
- Agent uptime
- CPU usage
- Memory usage
- Disk usage
- Heartbeat send time
- Network availability
- Agent version

Shelly support becomes an optional capability.

## Shelly-Only Operation

A Shelly plug may report through:

- MQTT
- WebSocket
- Shelly webhook or script
- Another computer acting as a collector
- A future dedicated Shelly ingestion service

In this scenario:

```text
PowerMeter exists
PowerReadings exist
Machine association may be null
```

The plug can still be associated with:

- A location
- One or more monitors
- A power strip
- Network equipment
- An appliance
- A manually entered computer
- Other inventory devices

## Registration Status

Both machines and meters should support lifecycle states:

```text
Discovered
PendingApproval
Active
Disabled
Retired
```

For example, an agent might automatically register a computer as:

```text
PendingApproval
```

An administrator can then approve and name it.

Similarly, an MQTT message from an unknown Shelly device could create:

```text
PowerMeter:
  RegistrationStatus = Discovered
```

This lets the system capture data without immediately trusting or fully configuring the device.

## Identity Matching

When associations are added later, the system must avoid duplicate records.

### Machine identity priority

```text
1. Persistent AgentId
2. Machine hardware identifier where available
3. Machine name plus organization context
```

### Shelly identity priority

```text
1. Shelly device ID
2. MAC address
3. Configured external identifier
```

A Shelly IP address should not be treated as identity because DHCP addresses can change.

## Manual Computer Records

A computer may be entered before its agent is installed:

```http
POST /api/v1/monitored-devices
```

```json
{
  "deviceType": "Computer",
  "name": "Accounting Workstation",
  "manufacturer": "Dell",
  "model": "Precision 3680",
  "assetTag": "PC-1042",
  "locationId": "accounting-desk-location"
}
```

Later, when the agent registers, the administrator can link the machine record to that inventory device:

```http
POST /api/v1/machines/{machineId}/monitored-device
```

```json
{
  "monitoredDeviceId": "existing-device-guid"
}
```

That prevents creating duplicate entries for the same physical computer.

## Recommended Database Constraints

### Machine

```text
AgentId unique when not null
```

### PowerMeter

```text
Vendor + ExternalDeviceId unique
MAC address unique when available
```

### Active association

```text
Only one active primary power-meter association per machine
```

### Optional relationships

All foreign keys involving machines and power meters should reside in association tables, not in the core records.

## Final Domain Principle

```text
Machines are independent telemetry producers.

Power meters are independent telemetry producers.

Monitored devices represent physical equipment.

Locations represent physical context.

Associations connect them only when the real-world relationship exists.
```

This structure supports your initial deployment with computer agents only and allows Shelly devices to be introduced incrementally without redesigning the API or database.
````
