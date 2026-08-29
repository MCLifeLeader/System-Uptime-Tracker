# Documentation Set

This folder contains the working design and planning documents for the System Uptime Tracker project.

The current source material started as a design conversation captured in [inital-spec.md](./inital-spec.md). The documents below convert that raw discussion into an implementation-oriented set of references.

## Reading Order

1. [product-scope.md](./product-scope.md)
2. [architecture-overview.md](./architecture-overview.md)
3. [domain-model.md](./domain-model.md)
4. [implementation-plan.md](./implementation-plan.md)
5. [stories/2026/07/README.md](./stories/2026/07/README.md)
6. [delivery-backlog.md](./delivery-backlog.md)
7. [backlog/README.md](./backlog/README.md)
8. [backlog/dependency-tree.md](./backlog/dependency-tree.md)
9. [windows-service-reference.md](./windows-service-reference.md)
10. [inital-spec.md](./inital-spec.md)

## Document Purpose

- [product-scope.md](./product-scope.md): Project goals, scope boundaries, assumptions, and success criteria.
- [architecture-overview.md](./architecture-overview.md): Proposed system shape, runtime flows, deployment model, and cross-cutting concerns.
- [domain-model.md](./domain-model.md): Core entities, relationships, identity rules, and data lifecycle guidance.
- [implementation-plan.md](./implementation-plan.md): Phased execution plan, workstreams, deliverables, risks, and open questions.
- [stories/2026/07/README.md](./stories/2026/07/README.md): Reviewable
	delivery slices with explicit sequencing and frontend or backend work
	expectations.
- [delivery-backlog.md](./delivery-backlog.md): Program-level epic dependency
	graph, critical path, release gates, definition of done, and traceability.
- [backlog/README.md](./backlog/README.md): Canonical index of the separate epic
	and task documents, with stable IDs and direct dependency links.
- [backlog/dependency-tree.md](./backlog/dependency-tree.md): Topological task
	execution waves showing which work can proceed in parallel.
- [windows-service-reference.md](./windows-service-reference.md): Transferable Windows Service hosting, packaging, installer, and upgrade patterns from the local FamilyTools reference implementation.
- [inital-spec.md](./inital-spec.md): Original design conversation retained for
	traceability, with later implementation amendments where the project adopted
	a concrete reference design.

## Usage Notes

- Treat [inital-spec.md](./inital-spec.md) as source material and an amended
	specification record, not as the day-to-day working plan.
- Update the structured documents first when decisions change.
- Read [implementation-plan.md](./implementation-plan.md) as the phase and
	workstream summary and use the story set as reviewable delivery slices.
- Use [backlog/README.md](./backlog/README.md) and task-file `depends_on`
	metadata as the canonical task-level execution source.
- Add architectural decisions as amendments to the relevant document until a dedicated ADR process is introduced.
- Naming has moved on since the original conversation: [inital-spec.md](./inital-spec.md) uses placeholder names like `ComputerTelemetry.sln` and `computer-telemetry.service`. The structured documents supersede that naming with `SystemUptimeTracker.*` for solution, namespace, and shared-library names (see [architecture-overview.md](./architecture-overview.md)). Do not copy the old `ComputerTelemetry`/`computer-telemetry` strings into implementation.
- Authentication has also moved on: [inital-spec.md](./inital-spec.md) proposes a custom `Authorization: AgentKey ...` header. That is superseded by the decided approach — ASP.NET Core Identity local user accounts under an Owner/DeviceAccount ownership model, authenticating via JWT bearer tokens (primary) or HTTP Basic Auth with a hashed API key (fallback for constrained devices) — see [product-scope.md](./product-scope.md#decisions) and [architecture-overview.md](./architecture-overview.md#authentication-and-authorization).
