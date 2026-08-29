# System Uptime Tracker

A small, cross-platform .NET system for tracking computer uptime and telemetry, with optional power-usage monitoring through smart plugs. A lightweight background agent runs on Windows and Ubuntu Linux, reports heartbeats and system telemetry over HTTPS to a central ASP.NET Core API, and the API persists everything to SQL Server for uptime history and reporting.

> **Status: design phase.** No application code has been written yet. The design work lives under [docs/](docs/): [docs/product-scope.md](docs/product-scope.md), [docs/architecture-overview.md](docs/architecture-overview.md), [docs/domain-model.md](docs/domain-model.md), and [docs/implementation-plan.md](docs/implementation-plan.md) are the structured, up-to-date references; [docs/inital-spec.md](docs/inital-spec.md) is the original design conversation plus later implementation amendments. This README reflects that intended direction. The repository currently carries a general-purpose Azure/.NET developer-environment scaffold (dev container, local dependency containers, DevOps pipeline placeholders, Copilot/Codex instruction library) that implementation will build on top of.

## What it does

- Tracks **uptime history** for each registered computer: when it was running, and when it went offline (shutdown, sleep/hibernate, lost network, or a stopped agent).
- Collects lightweight **system telemetry**: machine name, OS, architecture, boot time, CPU/memory/disk usage, and agent version.
- Optionally reports **power usage** from a **Shelly Plug US Gen4** smart plug — either dedicated to one computer or shared across multiple devices (monitors, power strips, network gear).
- Keeps computers and power meters as **independent entities** — either can be registered and used before the other exists, and association between them is optional.
- Is **outbound-only**: agents call the API; the server never opens a connection back to a monitored machine. No inbound firewall ports, agents can sit behind NAT.

## Planned architecture

Three independently deployable applications share common libraries and message contracts:

| Application | Target | Responsibility |
| --- | --- | --- |
| `SystemUptimeTracker.Api` | Azure App Service/Container Apps, IIS, or Linux + Kestrel/Nginx | Ingests heartbeats and power readings, owns the SQL Server data, exposes reporting endpoints |
| `SystemUptimeTracker.WindowsService` | Windows x64 | Runs under Windows Service Control Manager |
| `SystemUptimeTracker.LinuxDaemon` | Ubuntu x64/ARM64 | Runs under `systemd` |

Shared libraries: `Agent.Core` (worker logic used by both platform agents), `Contracts` (heartbeat/telemetry message contracts), `Data` (EF Core `DbContext`, entities, migrations), `Power.Shelly` (Shelly Plug normalization). See [docs/architecture-overview.md](docs/architecture-overview.md) for the full project/namespace list and proposed `src/` layout.

```text
Shelly Plug US Gen4 (optional)
          ↓ Local HTTP RPC
Windows / Linux Agent
          ↓ HTTPS (heartbeat + telemetry)
ASP.NET Core API
          ↓
SQL Server
```

Design notes preserve room to add an MQTT- or WebSocket-based ingestion path later (for power readings while a computer is off) without changing the existing agent-to-API contracts. See [docs/inital-spec.md](docs/inital-spec.md) for the full comparison of integration options.

## Tech stack

- .NET 10 Worker Service (`Microsoft.Extensions.Hosting.WindowsServices` / `.Systemd`) for the cross-platform agent
- ASP.NET Core Web API + Entity Framework Core for the ingestion service
- SQL Server / Azure SQL Database for persistence
- Self-contained, single-file `dotnet publish` for Windows Service and `systemd` daemon distribution
- Artifact-contained PowerShell installation for Windows, with idempotent
    upgrades, startup validation, rollback, and durable state outside the
    replaceable application directory. See
    [docs/windows-service-reference.md](docs/windows-service-reference.md).
- ASP.NET Core Identity with local user accounts (Owner and Device accounts); devices authenticate with JWT bearer tokens (primary, with periodic rotation) or HTTP Basic Auth with a hashed API key (fallback for constrained devices like the Shelly plug); HTTPS-only. See [docs/architecture-overview.md](docs/architecture-overview.md#authentication-and-authorization).

## Data model highlights

Core entities are independent first-class records, associated only when a real-world relationship exists:

- **Machine** — a registered computer (identified by a persisted `AgentId`, never by hostname alone).
- **DeviceAccount** — the credential/ownership record a machine (or Shelly plug, eventually) authenticates with; owned by exactly one Owner account, and shareable across machines or dedicated to one.
- **PowerMeter** — a Shelly plug (or future smart-meter vendor), independent of any computer.
- **MonitoredDevice** — general equipment inventory (computer, monitor, power strip, network switch, UPS, etc.), optionally linked to a `Machine`.
- **Location** — a nested physical hierarchy (site → building → room → desk) that meters and devices can be placed in.
- **RuntimeSession** — heartbeats are grouped into sessions rather than treated as isolated points, so gaps naturally surface shutdowns, sleep, or lost connectivity.
- Time-aware association tables (`MachinePowerMeterAssociation`, `PowerMeterDeviceAssociation`, `PowerMeterLocationHistory`) connect the entities above and preserve history when equipment moves.

The guiding principle: **measured power belongs to the meter**; location and device associations provide context; any per-device estimate is explicitly labeled as an estimate, never treated as a direct measurement.

## Recommended MVP scope

1. Cross-platform Worker Service agent
2. Windows Service artifact with tested install, upgrade, rollback, and uninstall
3. Ubuntu `systemd` installation
4. Persistent agent identifier
5. HTTPS heartbeat submission
6. ASP.NET Core ingestion API
7. SQL Server persistence
8. Machine and heartbeat records
9. Server-side runtime-session calculation
10. Local retry queue (SQLite-backed) for API outages
11. ASP.NET Core Identity authentication (Owner/Device accounts) with JWT bearer tokens, plus HTTP Basic Auth with API keys as a fallback for constrained devices
12. CPU, memory, disk, OS, boot, and agent-version telemetry
13. Health-check endpoint
14. Structured logging

**Deliberately deferred:** web dashboard, real-time alerts, software inventory, process monitoring, remote commands, automatic updater, exact electrical power usage, multi-tenant organizations, mobile interface.

## Repository layout

- [docs/](docs/) — project documentation; start with [docs/README.md](docs/README.md) for the reading order.
- [src/](src/) — application source (not yet populated; will hold the solution described above).
- [containers/](containers/) — Docker Compose services for local dependencies (SQL Server, Seq, WireMock, and others not all needed by this project — trim what you don't use).
- [devops/](devops/) — CI/CD pipeline and infrastructure-as-code scaffolding.
- [.github/](.github/) — Copilot/Codex instructions, agent personas, prompts, and skills used while developing this repository.

This layout, the dev container, and the local dependency containers were inherited from a general-purpose Azure/.NET template. Nothing here should be treated as production-ready until it has been reviewed against this project's actual requirements — see [AGENTS.md](AGENTS.md).

## Development environment setup

Use one of the options below depending on your platform and preference. Install command references:

- [INSTALL-WINDOWS-WINGET.md](INSTALL-WINDOWS-WINGET.md) — Windows-only winget commands for required and optional tools.
- [INSTALL-LINUX-APT.md](INSTALL-LINUX-APT.md) — Linux-only apt and npm commands for required and optional tools.

### Option A: Dev Containers (recommended)

1. Install Docker Desktop.
2. Install Visual Studio Code and the Dev Containers extension.
3. Open this repository in VS Code and select "Reopen in Container."
4. The container runs post-create setup and installs the tools described in `.devcontainer/devcontainer.json` (.NET, Node.js, Azure CLI, Terraform, Docker, PowerShell).

### Option B: Local setup on Windows (winget)

1. Clone the repository.
2. Install tools using winget (.NET SDK 10, PowerShell, Git, Docker Desktop, Visual Studio 2022, SQL Server Express if not using containers).
3. Start the containerized dependencies: run `docker_setup.ps1`.
4. Stop containers when finished: run `docker_down.ps1`.

### Option C: Local setup on Linux (apt)

1. Install prerequisites using apt (Git, Docker Engine/Compose, PowerShell, .NET SDK 10).
2. Start the containerized dependencies: run `docker_setup.sh`.
3. Stop containers when finished: run `docker_down.sh`.

## Database and local services

The Docker Compose setup runs a local SQL Server instance and other supporting services. The SQL Server container exposes localhost port `10433` and currently initializes a placeholder database named `ProjectExample` — rename this to match the project (for example, `SystemUptimeTracker`) as part of implementing the API's data layer.

Example connection string for local development:

```text
Server=127.0.0.1,10433;Database=ProjectExample;User Id=sa;Password=P@ssword123!;TrustServerCertificate=True;
```

Security note: the provided password is for local development only. Do not reuse it in production. Store secrets in your secret manager or environment configuration.

WireMock HTTPS certificate generation uses `keytool`, provided by a Java Development Kit. Install OpenJDK if you need to generate WireMock certificates locally.

## Documentation map

- [docs/README.md](docs/README.md) — index and recommended reading order for the documents below.
- [docs/product-scope.md](docs/product-scope.md) — goals, non-goals, scope boundaries, assumptions, and success criteria.
- [docs/architecture-overview.md](docs/architecture-overview.md) — system shape, runtime flows, deployment model, and cross-cutting concerns.
- [docs/domain-model.md](docs/domain-model.md) — entities, relationships, identity rules, and data lifecycle guidance.
- [docs/implementation-plan.md](docs/implementation-plan.md) — phased execution plan, workstreams, risks, and open questions.
- [docs/inital-spec.md](docs/inital-spec.md) — the original raw design conversation the structured documents above were distilled from; kept for traceability, not as the working plan.
- [.github/readme.md](.github/readme.md) — repository automation, Copilot configuration, and GitHub-specific guidance.
- [containers/readme.md](containers/readme.md) — Docker Compose services and local dependency containers.
- [containers/certs/readme.md](containers/certs/readme.md) — certificate setup for HTTPS and WireMock scenarios.
- [devops/readme.md](devops/readme.md) — DevOps folder overview and how to use it in CI/CD.
- [devops/terraform/readme.md](devops/terraform/readme.md) — Terraform layout and infrastructure guidance.
- [src/readme.md](src/readme.md) — application source layout expectations.

## Next steps

The solution skeleton, shared heartbeat contracts, initial database entities, and a functioning agent-to-API heartbeat path (without Shelly integration) are the recommended starting point. Shelly Plug support, location/device inventory, and richer reporting can follow once that path works end-to-end. See [docs/implementation-plan.md](docs/implementation-plan.md) for the full phased plan, and its "Open Technical Questions" section for decisions that should be settled before or during Phase 1.

## Troubleshooting

- Ensure Docker Desktop or Docker Engine is running before starting containers.
- If SQL Server does not start, check container logs and confirm the environment variables in `containers/.env`.
- If WireMock certificate generation fails, verify OpenJDK is installed and `keytool` is available on PATH.
- For script execution issues on Windows, set the PowerShell execution policy to allow local scripts.

## Additional resources

- [AUTHORS.md](AUTHORS.md)
- [CHANGELOG.md](CHANGELOG.md)
- [CONTRIBUTING.md](CONTRIBUTING.md)
- [Dev Containers documentation](https://code.visualstudio.com/docs/devcontainers/containers)

Built with accessibility in mind, but accessibility issues may still exist; please review and test with tools like Accessibility Insights.

## AI skill tooling (Codex + Copilot)

This repository uses the Skills CLI ecosystem at `https://skills.sh/` for skill discovery and installation.

Common commands:

```bash
npx skills find <query>
npx skills list -g
npx skills add <owner/repo@skill> -g -y
npx skills check
```

Reference: `.github/skills/INDEX.md` (canonical skill discovery map).

### Install shared skills globally (Codex + Copilot)

```powershell
npx skills add microsoft/github-copilot-for-azure@appinsights-instrumentation -g -y
npx skills add microsoft/github-copilot-for-azure@azure-resource-lookup -g -y
npx skills add microsoft/github-copilot-for-azure@azure-resource-visualizer -g -y
npx skills add microsoft/github-copilot-for-azure@microsoft-code-reference -g -y
npx skills add microsoft/github-copilot-for-azure@microsoft-docs -g -y
npx skills add anthropics/skills@webapp-testing -g -y
```

### Install Codex skills globally (Skills.sh)

```powershell
npx skills add microsoft/github-copilot-for-azure@azure-ai -g -y
npx skills add microsoft/github-copilot-for-azure@azure-diagnostics -g -y
npx skills add microsoft/github-copilot-for-azure@azure-observability -g -y
npx skills add microsoft/github-copilot-for-azure@azure-rbac -g -y
npx skills add vercel-labs/skills@find-skills -g -y
npx skills add microsoft/github-copilot-for-azure@microsoft-foundry -g -y
```

### Install Copilot skills globally (Skills.sh)

```powershell
npx skills add microsoft/github-copilot-for-azure@azure-role-selector -g -y
npx skills add microsoft/github-copilot-for-azure@azure-static-web-apps -g -y
npx skills add microsoft/github-copilot-for-azure@github-issues -g -y
npx skills add microsoft/github-copilot-for-azure@make-skill-template -g -y
npx skills add microsoft/github-copilot-for-azure@nuget-manager -g -y
npx skills add microsoft/github-copilot-for-azure@vscode-ext-commands -g -y
npx skills add microsoft/github-copilot-for-azure@vscode-ext-localization -g -y
npx skills add microsoft/github-copilot-for-azure@web-design-reviewer -g -y
```

Optional verification:

```powershell
npx skills list -g
npx skills check
```
