# Story 13: Reporting And Extended Ingestion Readiness

## Objective

Prepare the system for the next stage after the first release by defining and,
where appropriate, implementing approval workflows, reporting read models,
alternate ingestion-path readiness, and operational expansion points.

## Why This Story Follows Story 12

This is the first story that intentionally looks beyond the initial usable
deployment. It only makes sense once the core machine and power flows are in
place.

## Previous Story Reference

- Build on [story-12-location-associations-and-portal-power-workflows.md](./story-12-location-associations-and-portal-power-workflows.md).

## Source References

- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/product-scope.md](../../../product-scope.md)
- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/domain-model.md](../../../domain-model.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md)

## Dependencies

- Stories 01 through 12 completed.

## In Scope

- Approval workflow design and initial implementation where justified.
- Aggregate reporting and read-model design.
- Alternate Shelly or broker ingestion readiness analysis.
- Alerting and dashboard readiness planning.

## Out Of Scope

- Broad multi-tenant SaaS redesign.
- Large-scale analytics platform work unrelated to the first roadmap.

## Deliverables

- A concrete readiness package for Phase 4 work.
- Optional initial implementations for the highest-value reporting or approval
  capabilities.
- A documented decision path for direct or brokered power ingestion.

## Backend Details

- Design and, where justified, implement approval workflows, reporting read
   models, and alternate-ingestion readiness on the backend side.
- Keep reporting paths efficient enough that portal views do not depend on
   expensive ad hoc transactional queries.
- Validate future-ingestion assumptions against the current auth and schema
   model before expanding protocols.

## Frontend Details

- Build the smallest useful owner-facing approval and reporting views that prove
   the new read models answer real operator questions.
- Keep portal workflow scope bounded to what the backend can already support
   stably, rather than creating speculative UI ahead of settled read semantics.
- Add portal validation and functional checks for any newly introduced review or
   reporting workflows.

## Execution Steps

1. Review real data and operator workflows from Stories 05 through 12 and use
   that evidence to decide which approval and reporting gaps are worth closing
   immediately.
2. Design aggregate read models or query surfaces for the most important owner
   questions, such as recent uptime health, offline machines, recent power use,
   and unresolved registration or approval items.
3. Implement the smallest useful set of read APIs and portal views that prove
   the system can support reporting without forcing ad hoc heavy queries onto
   hot transactional tables.
4. Define the discovery and approval workflow for newly seen machines or power
   meters if the product chooses not to auto-approve everything.
5. Evaluate the future direct-ingestion options called out in the planning
   documents, such as MQTT or device-direct paths, and document which schema and
   auth assumptions are already ready versus what still needs work.
6. Add any initial alerting hooks, dashboards, or health summaries needed for
   practical operations, but keep them bounded to proven user need.
7. Write a go-forward decision note that separates first-release done criteria
   from second-wave optimization work.
8. Add tests and operational checks for any new read models or approval flows
   introduced in this story.

## Validation Steps

- Verify the selected reporting read models answer real owner questions.
- Confirm approval workflow decisions match the security and operational goals
  captured earlier in the docs.
- Review the alternate-ingestion readiness note for unresolved blockers.

## Completion Criteria

- The project has a credible path beyond the first release.
- Reporting and approval work is grounded in the implemented product rather than
  speculative design.
- Future ingestion expansion can proceed without rediscovering the same design
  questions.
