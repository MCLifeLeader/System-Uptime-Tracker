/**
 * Portal-side contract tests (TASK-0209). The golden JSON fixtures below are
 * the same examples pinned by the .NET contract tests in
 * `SystemUptimeTracker.Contracts.UnitTests`, so a wire-shape change fails CI
 * on both stacks.
 */

import { describe, expect, it } from "vitest";
import {
  apiKeyResponseSchema,
  createMachinePowerMeterAssociationRequestSchema,
  deviceAccountSummarySchema,
  deviceCredentialResponseSchema,
  errorContract,
  machineSummarySchema,
  pagedResponseSchema,
  paginationDefaults,
  powerReadingSummarySchema,
  problemDetailsSchema,
  runtimeSessionSummarySchema,
  tokenResponseSchema,
} from "./v1";

describe("api-contracts/v1", () => {
  it("accepts the golden token response with lifetime metadata", () => {
    const golden = {
      tokenType: "Bearer",
      accessToken: "example.access.token",
      expiresInSeconds: 900,
      refreshToken: "example-refresh-token",
      refreshTokenExpiresAtUtc: "2026-09-13T15:30:00+00:00",
    };

    expect(tokenResponseSchema.parse(golden)).toEqual(golden);
  });

  it("rejects a token response missing the refresh token", () => {
    const result = tokenResponseSchema.safeParse({
      tokenType: "Bearer",
      accessToken: "example.access.token",
      expiresInSeconds: 900,
      refreshTokenExpiresAtUtc: "2026-09-13T15:30:00+00:00",
    });

    expect(result.success).toBe(false);
  });

  it("accepts the golden one-time device credential response", () => {
    const golden = {
      deviceAccountId: "5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01",
      deviceAccountName: "DEV-WORKSTATION-01",
      bootstrapPassword: "example-one-time-bootstrap",
      issuedAtUtc: "2026-08-30T12:00:00+00:00",
    };

    expect(deviceCredentialResponseSchema.parse(golden)).toEqual(golden);
  });

  it("accepts the golden one-time API key response", () => {
    const golden = {
      deviceAccountId: "5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01",
      deviceAccountName: "SHELLY-PLUG-KITCHEN",
      apiKey: "example-one-time-api-key",
      issuedAtUtc: "2026-08-30T12:00:00+00:00",
    };

    expect(apiKeyResponseSchema.parse(golden)).toEqual(golden);
  });

  it("accepts the golden paged device-account envelope", () => {
    const golden = {
      items: [
        {
          deviceAccountId: "5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01",
          name: "DEV-WORKSTATION-01",
          allowedAuthenticationMethods: "Jwt",
          isActive: true,
          hasApiKey: false,
          apiKeyCreatedAtUtc: null,
          apiKeyLastUsedAtUtc: null,
          machineCount: 1,
          createdAtUtc: "2026-08-30T12:00:00+00:00",
        },
      ],
      page: paginationDefaults.firstPage,
      pageSize: paginationDefaults.defaultPageSize,
      totalItemCount: 1,
    };

    const schema = pagedResponseSchema(deviceAccountSummarySchema);
    expect(schema.parse(golden)).toEqual(golden);
  });

  it("rejects a page size beyond the bounded maximum", () => {
    const schema = pagedResponseSchema(deviceAccountSummarySchema);
    const result = schema.safeParse({
      items: [],
      page: 1,
      pageSize: paginationDefaults.maxPageSize + 1,
      totalItemCount: 0,
    });

    expect(result.success).toBe(false);
  });

  it("accepts a pre-created machine with null agent fields", () => {
    const golden = {
      machineId: "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
      agentId: null,
      machineName: "PLANNED-SERVER-02",
      operatingSystem: null,
      operatingSystemVersion: null,
      architecture: null,
      agentVersion: null,
      registrationStatus: "Active",
      firstSeenAtUtc: null,
      lastSeenAtUtc: null,
      deviceAccountId: "5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01",
    };

    expect(machineSummarySchema.parse(golden)).toEqual(golden);
  });

  it("accepts the golden running runtime session", () => {
    const golden = {
      runtimeSessionId: "0d9e6a11-1b8e-4d3c-9f7b-6a5d4c3b2a10",
      machineId: "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
      startedAtUtc: "2026-08-30T06:00:00+00:00",
      lastHeartbeatAtUtc: "2026-08-30T12:34:00+00:00",
      endedAtUtc: null,
      endReason: "Running",
      heartbeatCount: 394,
      calculatedUptimeSeconds: 23640,
    };

    expect(runtimeSessionSummarySchema.parse(golden)).toEqual(golden);
  });

  it("rejects an unknown session end reason", () => {
    const result = runtimeSessionSummarySchema.safeParse({
      runtimeSessionId: "0d9e6a11-1b8e-4d3c-9f7b-6a5d4c3b2a10",
      machineId: "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
      startedAtUtc: "2026-08-30T06:00:00+00:00",
      lastHeartbeatAtUtc: "2026-08-30T12:34:00+00:00",
      endedAtUtc: null,
      endReason: "PowerLoss",
      heartbeatCount: 394,
      calculatedUptimeSeconds: 23640,
    });

    expect(result.success).toBe(false);
  });

  it("accepts a power reading summary and rejects an out-of-range power factor", () => {
    const golden = {
      powerReadingId: "4d3c2b1a-0f9e-4d8c-b7a6-5e4f3a2b1c0d",
      powerMeterId: "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
      messageId: "b6a1f2c3-d4e5-4f60-8a9b-0c1d2e3f4a5b",
      measuredAtUtc: "2026-08-30T12:00:00+00:00",
      receivedAtUtc: "2026-08-30T12:00:03+00:00",
      activePowerWatts: 87.4,
      voltage: 119.8,
      currentAmps: 0.74,
      powerFactor: 0.98,
      frequencyHz: 60.0,
      totalEnergyWattHours: 15234.6,
      outputIsOn: true,
      deviceTemperatureCelsius: 41.2,
    };

    expect(powerReadingSummarySchema.parse(golden)).toEqual(golden);
    expect(
      powerReadingSummarySchema.safeParse({ ...golden, powerFactor: 1.5 })
        .success,
    ).toBe(false);
  });

  it("accepts each machine/meter relationship kind", () => {
    for (const relationshipType of [
      "DedicatedLoad",
      "SharedLoad",
      "CollectorOnly",
    ]) {
      const golden = {
        machineId: "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
        powerMeterId: "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
        relationshipType,
        effectiveFromUtc: "2026-08-30T00:00:00+00:00",
        isPrimary: true,
      };

      expect(
        createMachinePowerMeterAssociationRequestSchema.parse(golden),
      ).toEqual(golden);
    }
  });

  it("accepts a problem details payload with correlation extensions", () => {
    const golden = {
      type: errorContract.unsupportedPayloadVersionType,
      title: "Unsupported payload version.",
      status: 422,
      detail: "The request could not be completed. Trace ID: abc123.",
      instance: "POST /api/v1/heartbeats",
      traceId: "0af7651916cd43dd8448eb211c80319c",
      requestId: "0HN0000000000",
    };

    const parsed = problemDetailsSchema.parse(golden);
    expect(parsed.traceId).toBe("0af7651916cd43dd8448eb211c80319c");
    expect(errorContract.traceIdHeaderName).toBe("X-Trace-Id");
  });
});
