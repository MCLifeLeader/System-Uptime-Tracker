# Story 02: API v1 And Auth Contract Baseline

## Objective

Produce an accepted, versioned API and authentication contract for both device
ingestion and owner-facing web workflows before deeper implementation begins.

## Why This Story Follows Story 01

Once the project topology exists, the next major risk is contract drift. The
planning documents explicitly call out that route lists, request and response
shapes, token flows, and Basic Auth details are not yet captured as an accepted
contract. The API, web shell, and future agents all depend on that baseline.

## Previous Story Reference

- Build on [story-01-solution-topology-alignment.md](./story-01-solution-topology-alignment.md).

## Source References

- [docs/architecture-overview.md](../../../architecture-overview.md)
- [docs/implementation-plan.md](../../../implementation-plan.md)
- [docs/product-scope.md](../../../product-scope.md)
- [docs/inital-spec.md](../../../inital-spec.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Controllers/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Controllers/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Models/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Models/ReadMe.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Services/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Services/ReadMe.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Api/Factories/ReadMe.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Api/Factories/ReadMe.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/auth/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/auth/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/[...routeparts]/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/app/api/[...routeparts]/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/services/readme.md)
- [src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/errors/readme.md](../../../../src/SystemUptimeTracker/SystemUptimeTracker.Web/src/utils/errors/readme.md)

## Dependencies

- Story 01 completed.
- Agreement that all routes are versioned under `/api/v1`.

## In Scope

- Route taxonomy.
- Request and response models.
- Trace ID and error response conventions.
- JWT login, refresh, and device authorization claims.
- Basic Auth plus API key fallback for constrained devices.
- Portal proxy and server-fetch integration rules.

## Out Of Scope

- Database schema implementation.
- Full controller and service coding.
- UI page implementation.

## Deliverables

- A dedicated API contract document or equivalent source-of-truth artifact.
- Accepted request and response shapes for heartbeat ingestion, owner login,
  device-account management, machine reads, power-meter registration, and
  future association endpoints.
- A documented security contract for JWT tokens, refresh tokens, and Basic Auth
  API-key usage.
- A clear portal integration rule set for Next.js server fetches and passthrough
  routes.

## Backend Details

- Define versioned route groups, request and response DTOs, authorization
  requirements, error envelopes, and idempotency rules on the API side.
- Specify JWT login and refresh contracts, owner-login behavior, Basic Auth API
  key behavior, and the claims required for device and owner authorization.
- Produce API-facing executable artifacts such as HTTP files, OpenAPI output,
  or equivalent backend contract examples.

## Frontend Details

- Define how the portal consumes the contract through typed services, Zod
  validators, passthrough routes, or server-side fetch utilities.
- Specify secure owner-authentication behavior for the portal, including
  session, cookie, and trace-correlation expectations.
- Lock the frontend-visible validation and generic-error contract so the portal
  does not invent its own request or error semantics.

## Execution Steps

1. Define route groups for health, identity, owner administration, ingestion,
   machine reads, power reads, and future association management.
2. Convert the planning-doc examples into explicit request and response models,
   removing legacy `AgentKey` assumptions that were superseded by the
   `Owner` and `DeviceAccount` design.
3. Specify the authentication flows in detail:
   JWT login, JWT refresh, owner login behavior, logout or revocation behavior,
   and Basic Auth credential format for API-key devices.
4. Define the exact claims or equivalent identifiers needed in JWTs so the API
   can authorize device and owner requests without ambiguous interpretation.
5. Standardize the error contract across API and web layers. Include trace IDs,
   generic user-facing messages, validation-failure structure, and rate-limit
   or lockout response semantics.
6. Define idempotency and correlation expectations for ingestion endpoints,
   including any headers, payload fields, or duplicate-detection strategy.
7. Produce parallel contract artifacts for both runtimes: C# DTOs or OpenAPI on
   the API side, and Zod-backed request and response validators on the web side.
8. Add HTTP files, sample payloads, or other executable examples so the
   contract can be verified without first building the full product.

## Validation Steps

- Review the contract against all Phase 1 work items in
  [docs/implementation-plan.md](../../../implementation-plan.md).
- Confirm the contract can support both the retained Next.js proxy pattern and
  direct agent-to-API calls.
- Confirm no route or payload still depends on the superseded `AgentKey` model.
- Verify the contract is explicit enough that API and web developers could work
  in parallel.

## Completion Criteria

- The accepted contract resolves the open gap called out in the implementation
  plan.
- The API, web, and agent stories that follow can cite one stable source of
  truth for payloads and security behavior.
- Error, trace, and authorization conventions are consistent across all flows.
