# Documentation Set

This folder contains the working design and planning documents for the System Uptime Tracker project.

The current source material started as a design conversation captured in [inital-spec.md](./inital-spec.md). The documents below convert that raw discussion into an implementation-oriented set of references.

## Reading Order

1. [product-scope.md](./product-scope.md)
2. [architecture-overview.md](./architecture-overview.md)
3. [domain-model.md](./domain-model.md)
4. [implementation-plan.md](./implementation-plan.md)
5. [stories/2026/07/README.md](./stories/2026/07/README.md)
6. [inital-spec.md](./inital-spec.md)

## Document Purpose

- [product-scope.md](./product-scope.md): Project goals, scope boundaries, assumptions, and success criteria.
- [architecture-overview.md](./architecture-overview.md): Proposed system shape, runtime flows, deployment model, and cross-cutting concerns.
- [domain-model.md](./domain-model.md): Core entities, relationships, identity rules, and data lifecycle guidance.
- [implementation-plan.md](./implementation-plan.md): Phased execution plan, workstreams, deliverables, risks, and open questions.
- [stories/2026/07/README.md](./stories/2026/07/README.md): Execution-level implementation stories with explicit sequencing, dependency links, and frontend or backend work expectations.
- [inital-spec.md](./inital-spec.md): Raw source conversation retained for traceability.

## Usage Notes

- Treat [inital-spec.md](./inital-spec.md) as source material, not as the day-to-day working plan.
- Update the structured documents first when decisions change.
- Use [stories/2026/07/README.md](./stories/2026/07/README.md) and its child story files as the execution source of truth once work moves from design into implementation.
- Read [implementation-plan.md](./implementation-plan.md) as the phase and workstream summary, then use the story set for the detailed backend, frontend, agent, and operations sequence.
- Add architectural decisions as amendments to the relevant document until a dedicated ADR process is introduced.
- Naming has moved on since the original conversation: [inital-spec.md](./inital-spec.md) uses placeholder names like `ComputerTelemetry.sln` and `computer-telemetry.service`. The structured documents supersede that naming with `SystemUptimeTracker.*` for solution, namespace, and shared-library names (see [architecture-overview.md](./architecture-overview.md)). Do not copy the old `ComputerTelemetry`/`computer-telemetry` strings into implementation.
- Authentication has also moved on: [inital-spec.md](./inital-spec.md) proposes a custom `Authorization: AgentKey ...` header. That is superseded by the decided approach — ASP.NET Core Identity local user accounts under an Owner/DeviceAccount ownership model, authenticating via JWT bearer tokens (primary) or HTTP Basic Auth with a hashed API key (fallback for constrained devices) — see [product-scope.md](./product-scope.md#decisions) and [architecture-overview.md](./architecture-overview.md#authentication-and-authorization).
