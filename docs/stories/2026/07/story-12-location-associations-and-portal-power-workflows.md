# Story 12: Location, Associations, And Portal Power Workflows

## Objective

Add the contextual domain needed to make power telemetry useful: locations,
monitored devices, effective-dated associations, and owner-facing portal
workflows for managing those relationships.

## Why This Story Follows Story 11

Power meters and readings are valuable only when operators can relate them to
machines, physical devices, and locations. Those contextual workflows depend on
the baseline power support being present first.

## Previous Story Reference

- Build on [story-11-shelly-polling-and-power-ingestion.md](./story-11-shelly-polling-and-power-ingestion.md).

## Source References

- [docs/domain-model.md](../../../domain-model.md)
- [docs/product-scope.md](../../../product-scope.md)
- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/admin/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/features/admin/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/public/strings/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/public/strings/readme.md)

## Dependencies

- Stories 01 through 11 completed.

## In Scope

- `Location` and `MonitoredDevice` entities.
- Effective-dated machine, meter, and device associations.
- Owner-facing APIs for association management.
- Portal workflows for managing locations, devices, and power relationships.

## Out Of Scope

- Advanced reporting dashboards.
- Alternate direct-ingestion paths.

## Deliverables

- Data model and API support for contextual relationships.
- Portal screens that let owners manage those relationships.
- Tests for relationship rules and effective-dating behavior.

## Backend Details

- Implement the schema, validation rules, and owner-facing API endpoints for
   locations, monitored devices, and effective-dated associations.
- Enforce dedicated-load, shared-load, and collector-only relationship rules in
   the backend so portal UX is not the only protection against bad data.
- Add integration coverage for lifecycle, overlap prevention, and historical
   read behavior.

## Frontend Details

- Implement portal workflows for location registration, power-meter placement,
   machine-to-device linking, and current or recent relationship views.
- Apply localization, form validation, and trace-aware error handling to the
   new power-management workflows.
- Add functional coverage that validates the operator workflow end to end on
   the shared API surface.

## Execution Steps

1. Extend the schema with `Location`, `MonitoredDevice`, and the association
   tables required to relate machines, meters, and devices without collapsing
   those independent concepts into one record.
2. Implement effective-dated association rules so the system can answer both
   current-state and historical questions accurately.
3. Define API endpoints for creating, editing, closing, and reading location,
   device, and association records under owner authorization.
4. Add validation rules for dedicated load, shared load, and collector-only
   relationships, including conflict detection for invalid active overlaps.
5. Extend the portal with workflows for registering locations, placing power
   meters, linking machines to monitored devices, and viewing current and recent
   power relationships.
6. Externalize all new user-facing strings and keep the admin UX consistent
   with the retained feature-module and generic-component guidance.
7. Add integration and functional tests for association lifecycle operations,
   overlap prevention, historical reads, and owner-facing UI flows.
8. Document the minimum operational workflow for introducing power telemetry to
   an existing machine-monitoring deployment.

## Validation Steps

- Create locations and monitored devices.
- Register associations and verify effective dates behave as expected.
- Confirm invalid overlapping relationships are rejected.
- Use the portal to manage the full workflow without direct database access.

## Completion Criteria

- Power telemetry now has usable context.
- Associations remain explicit, historical, and administratively manageable.
- The portal supports the real operator workflow needed for power features.
