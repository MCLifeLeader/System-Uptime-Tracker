/**
 * Portal-side contract tests (TASK-0209). The golden JSON fixtures below are
 * the same examples pinned by the .NET contract tests in
 * `SystemUptimeTracker.Contracts.UnitTests`, so a wire-shape change fails CI
 * on both stacks.
 */

import { describe, expect, it } from "vitest";
import {
  apiKeyResponseSchema,
  createDeviceAccountRequestSchema,
  createMachinePowerMeterAssociationRequestSchema,
  createMachineRequestSchema,
  createPowerMeterDeviceAssociationRequestSchema,
  createPowerMeterRequestSchema,
  deviceAccountSummarySchema,
  deviceCredentialResponseSchema,
  endAssociationRequestSchema,
  errorContract,
  heartbeatSummarySchema,
  locationRequestSchema,
  locationSummarySchema,
  machinePowerMeterAssociationSummarySchema,
  machineSummarySchema,
  meterLocationPlacementRequestSchema,
  monitoredDeviceRequestSchema,
  monitoredDeviceSummarySchema,
  ownerLoginRequestSchema,
  pagedResponseSchema,
  paginationDefaults,
  powerMeterDeviceAssociationSummarySchema,
  powerMeterLocationHistorySummarySchema,
  powerMeterSummarySchema,
  powerReadingSummarySchema,
  problemDetailsSchema,
  refreshTokenRequestSchema,
  revokeTokenRequestSchema,
  runtimeSessionSummarySchema,
  tokenResponseSchema,
  updateMachineRequestSchema,
  updatePowerMeterRequestSchema,
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

  it("requires exactly one revoke option", () => {
    expect(
      revokeTokenRequestSchema.safeParse({ refreshToken: "example-refresh-token" })
        .success,
    ).toBe(true);
    expect(revokeTokenRequestSchema.safeParse({ revokeAll: true }).success).toBe(
      true,
    );
    // Neither option, or both at once, is not a well-formed revoke request.
    expect(revokeTokenRequestSchema.safeParse({}).success).toBe(false);
    expect(
      revokeTokenRequestSchema.safeParse({
        refreshToken: "example-refresh-token",
        revokeAll: true,
      }).success,
    ).toBe(false);
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

  it("accepts a power reading summary, tolerating sensor-rounded power factors", () => {
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
    // Read-side tolerance: the API stores plain doubles, so a sensor-rounded
    // power factor slightly outside [-1, 1] must not break rendering.
    expect(
      powerReadingSummarySchema.safeParse({ ...golden, powerFactor: 1.02 })
        .success,
    ).toBe(true);
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
    // Header names are case-insensitive on the wire; the API emits
    // "X-Trace-Id" and the shared web constant is lowercase.
    expect(errorContract.traceIdHeaderName.toLowerCase()).toBe("x-trace-id");
  });

  it("accepts a golden example for every remaining portal schema", () => {
    const cases: [string, { parse: (value: unknown) => unknown }, unknown][] = [
      [
        "ownerLoginRequest",
        ownerLoginRequestSchema,
        { email: "owner@example.test", password: "example-password" },
      ],
      [
        "refreshTokenRequest",
        refreshTokenRequestSchema,
        { refreshToken: "example-refresh-token" },
      ],
      ["revokeTokenRequest", revokeTokenRequestSchema, { revokeAll: true }],
      [
        "createDeviceAccountRequest",
        createDeviceAccountRequestSchema,
        { name: "DEV-WORKSTATION-01", allowedAuthenticationMethods: "Jwt" },
      ],
      [
        "createMachineRequest",
        createMachineRequestSchema,
        { machineName: "PLANNED-SERVER-02", deviceAccountId: null },
      ],
      [
        "updateMachineRequest",
        updateMachineRequestSchema,
        {
          machineName: "BUILD-SERVER-01",
          deviceAccountId: "5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01",
        },
      ],
      [
        "heartbeatSummary",
        heartbeatSummarySchema,
        {
          heartbeatId: "9c0a95f2-63d4-4b7e-8a3a-52f27cf7f5a1",
          machineId: "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
          sequenceNumber: 4211,
          sentAtUtc: "2026-07-25T15:30:00+00:00",
          receivedAtUtc: "2026-07-25T15:30:02+00:00",
          cpuUsagePercent: 14.7,
          totalMemoryBytes: 34359738368,
          availableMemoryBytes: 18253611008,
        },
      ],
      [
        "powerMeterSummary",
        powerMeterSummarySchema,
        {
          powerMeterId: "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
          vendor: "Shelly",
          externalDeviceId: "shellyplugus4-a8032ab12345",
          name: "Kitchen Plug",
          model: "Plug US Gen4",
          macAddress: "A8:03:2A:B1:23:45",
          ipAddress: "192.168.1.57",
          firmwareVersion: "1.4.2",
          connectionType: "AgentPolling",
          registrationStatus: "Active",
          firstSeenAtUtc: "2026-08-30T12:00:00+00:00",
          lastSeenAtUtc: "2026-08-30T12:05:00+00:00",
        },
      ],
      [
        "createPowerMeterRequest",
        createPowerMeterRequestSchema,
        {
          vendor: "Shelly",
          externalDeviceId: "shellyplugus4-a8032ab12345",
          name: "Kitchen Plug",
          connectionType: "AgentPolling",
          authenticationReference: "shelly/kitchen-plug",
        },
      ],
      [
        "updatePowerMeterRequest",
        updatePowerMeterRequestSchema,
        { name: "Kitchen Plug", connectionType: "AgentPolling" },
      ],
      [
        "locationRequest",
        locationRequestSchema,
        { name: "Home Office", locationType: "Room", timeZone: "America/Denver" },
      ],
      [
        "locationSummary",
        locationSummarySchema,
        {
          locationId: "2b3c4d5e-6f70-4181-92a3-b4c5d6e7f809",
          name: "Home Office",
          locationType: "Room",
          parentLocationId: null,
          timeZone: "America/Denver",
          description: null,
          isActive: true,
        },
      ],
      [
        "monitoredDeviceRequest",
        monitoredDeviceRequestSchema,
        { name: "Dev Workstation", deviceType: "Computer", isPowerConsumer: true },
      ],
      [
        "monitoredDeviceSummary",
        monitoredDeviceSummarySchema,
        {
          monitoredDeviceId: "9e8d7c6b-5a49-4382-b716-0f1e2d3c4b5a",
          name: "Dev Workstation",
          deviceType: "Computer",
          locationId: "2b3c4d5e-6f70-4181-92a3-b4c5d6e7f809",
          parentMonitoredDeviceId: null,
          machineId: "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
          manufacturer: null,
          model: null,
          isPowerConsumer: true,
          isActive: true,
        },
      ],
      [
        "machinePowerMeterAssociationSummary",
        machinePowerMeterAssociationSummarySchema,
        {
          machinePowerMeterAssociationId: "0a1b2c3d-4e5f-4061-8273-8495a6b7c8d9",
          machineId: "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
          powerMeterId: "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
          relationshipType: "DedicatedLoad",
          effectiveFromUtc: "2026-08-30T00:00:00+00:00",
          effectiveToUtc: null,
          isPrimary: true,
        },
      ],
      [
        "createPowerMeterDeviceAssociationRequest",
        createPowerMeterDeviceAssociationRequestSchema,
        {
          powerMeterId: "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
          monitoredDeviceId: "9e8d7c6b-5a49-4382-b716-0f1e2d3c4b5a",
          associationType: "Shared",
          estimatedSharePercent: 35,
          effectiveFromUtc: "2026-08-30T00:00:00+00:00",
          isPrimary: false,
          notes: "Monitor and dock share the plug",
        },
      ],
      [
        "powerMeterDeviceAssociationSummary",
        powerMeterDeviceAssociationSummarySchema,
        {
          associationId: "3c4d5e6f-7081-4293-a4b5-c6d7e8f90a1b",
          powerMeterId: "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
          monitoredDeviceId: "9e8d7c6b-5a49-4382-b716-0f1e2d3c4b5a",
          associationType: "Shared",
          estimatedSharePercent: 35,
          effectiveFromUtc: "2026-08-30T00:00:00+00:00",
          effectiveToUtc: null,
          isPrimary: false,
          notes: null,
        },
      ],
      [
        "endAssociationRequest",
        endAssociationRequestSchema,
        { effectiveToUtc: "2026-09-01T00:00:00+00:00" },
      ],
      [
        "meterLocationPlacementRequest",
        meterLocationPlacementRequestSchema,
        {
          locationId: "2b3c4d5e-6f70-4181-92a3-b4c5d6e7f809",
          effectiveFromUtc: "2026-08-30T00:00:00+00:00",
          notes: null,
        },
      ],
      [
        "powerMeterLocationHistorySummary",
        powerMeterLocationHistorySummarySchema,
        {
          powerMeterLocationHistoryId: "5e6f7081-92a3-44b5-86d7-e8f90a1b2c3d",
          powerMeterId: "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
          locationId: "2b3c4d5e-6f70-4181-92a3-b4c5d6e7f809",
          effectiveFromUtc: "2026-08-30T00:00:00+00:00",
          effectiveToUtc: null,
          notes: null,
        },
      ],
    ];

    for (const [name, schema, golden] of cases) {
      expect(schema.parse(golden), `schema '${name}' rejected its golden example`).toEqual(golden);
    }
  });
});
