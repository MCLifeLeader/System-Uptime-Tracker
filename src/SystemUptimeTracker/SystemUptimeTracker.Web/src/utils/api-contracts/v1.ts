/**
 * Zod validators for the accepted `/api/v1` wire contracts (TASK-0209).
 *
 * These schemas mirror the DTOs owned by the `SystemUptimeTracker.Contracts`
 * project and the route catalog in `docs/api-contracts.md`. The same golden
 * JSON examples are pinned on both stacks: the .NET side in
 * `SystemUptimeTracker.Contracts.UnitTests` and the portal side in
 * `v1.test.ts`. Changing a field name, requiredness, or type is a versioned
 * contract change and must update both.
 *
 * @module utils/api-contracts/v1
 */

import { z } from "zod";

/** Correlation and error conventions (TASK-0208). */
export const errorContract = {
  traceIdHeaderName: "X-Trace-Id",
  traceIdExtensionKey: "traceId",
  requestIdExtensionKey: "requestId",
  problemContentType: "application/problem+json",
  unsupportedPayloadVersionType:
    "urn:systemuptimetracker:error:unsupported-payload-version",
} as const;

/** Bounded pagination rules (TASK-0205). */
export const paginationDefaults = {
  defaultPageSize: 50,
  maxPageSize: 200,
  firstPage: 1,
} as const;

/** The current (and only) accepted payload version for v1 contracts. */
export const payloadVersionV1 = 1;

const utcTimestamp = z.string().datetime({ offset: true });

/** RFC 9457 Problem Details with the v1 correlation extensions. */
export const problemDetailsSchema = z.object({
  type: z.string().nullable().optional(),
  title: z.string().nullable().optional(),
  status: z.number().int().nullable().optional(),
  detail: z.string().nullable().optional(),
  instance: z.string().nullable().optional(),
  traceId: z.string().optional(),
  requestId: z.string().optional(),
  errors: z.record(z.array(z.string())).optional(),
});

export const registrationStatusSchema = z.enum([
  "Active",
  "Disabled",
  "Retired",
  "Discovered",
  "PendingApproval",
]);

export const allowedAuthenticationMethodsSchema = z.enum([
  "Jwt",
  "ApiKey",
  "Both",
]);

export const sessionEndReasonSchema = z.enum([
  "Running",
  "GracefulShutdown",
  "ServiceStopped",
  "SleepOrHibernate",
  "HeartbeatTimeout",
  "AgentRestart",
  "MachineReboot",
  "Unknown",
]);

export const meterConnectionTypeSchema = z.enum([
  "AgentPolling",
  "Mqtt",
  "WebSocket",
  "Webhook",
  "ShellyCloud",
]);

export const machineMeterRelationshipTypeSchema = z.enum([
  "DedicatedLoad",
  "SharedLoad",
  "CollectorOnly",
]);

export const deviceAssociationTypeSchema = z.enum(["Dedicated", "Shared"]);

/**
 * Builds the bounded page envelope schema returned by every v1 list endpoint.
 *
 * @param itemSchema Schema for the items on the page.
 */
export function pagedResponseSchema<TItem extends z.ZodTypeAny>(
  itemSchema: TItem,
) {
  return z.object({
    items: z.array(itemSchema),
    page: z.number().int().min(paginationDefaults.firstPage),
    pageSize: z.number().int().min(1).max(paginationDefaults.maxPageSize),
    totalItemCount: z.number().int().min(0),
  });
}

export const ownerLoginRequestSchema = z.object({
  email: z.string().email(),
  password: z.string().min(1),
});

export const tokenResponseSchema = z.object({
  tokenType: z.literal("Bearer"),
  accessToken: z.string().min(1),
  expiresInSeconds: z.number().int().positive(),
  refreshToken: z.string().min(1),
  refreshTokenExpiresAtUtc: utcTimestamp,
});

export const refreshTokenRequestSchema = z.object({
  refreshToken: z.string().min(1),
});

export const revokeTokenRequestSchema = z.object({
  refreshToken: z.string().min(1).nullable().optional(),
  revokeAll: z.boolean().optional(),
});

export const deviceCredentialResponseSchema = z.object({
  deviceAccountId: z.string().uuid(),
  deviceAccountName: z.string().min(1),
  bootstrapPassword: z.string().min(1),
  issuedAtUtc: utcTimestamp,
});

export const apiKeyResponseSchema = z.object({
  deviceAccountId: z.string().uuid(),
  deviceAccountName: z.string().min(1),
  apiKey: z.string().min(1),
  issuedAtUtc: utcTimestamp,
});

export const deviceAccountSummarySchema = z.object({
  deviceAccountId: z.string().uuid(),
  name: z.string().min(1),
  allowedAuthenticationMethods: allowedAuthenticationMethodsSchema,
  isActive: z.boolean(),
  hasApiKey: z.boolean(),
  apiKeyCreatedAtUtc: utcTimestamp.nullable().optional(),
  apiKeyLastUsedAtUtc: utcTimestamp.nullable().optional(),
  machineCount: z.number().int().min(0),
  createdAtUtc: utcTimestamp,
});

export const createDeviceAccountRequestSchema = z.object({
  name: z.string().min(1),
  allowedAuthenticationMethods: allowedAuthenticationMethodsSchema,
});

export const updateDeviceAccountRequestSchema =
  createDeviceAccountRequestSchema;

export const machineSummarySchema = z.object({
  machineId: z.string().uuid(),
  agentId: z.string().uuid().nullable().optional(),
  machineName: z.string().min(1),
  operatingSystem: z.string().nullable().optional(),
  operatingSystemVersion: z.string().nullable().optional(),
  architecture: z.string().nullable().optional(),
  agentVersion: z.string().nullable().optional(),
  registrationStatus: registrationStatusSchema,
  firstSeenAtUtc: utcTimestamp.nullable().optional(),
  lastSeenAtUtc: utcTimestamp.nullable().optional(),
  deviceAccountId: z.string().uuid().nullable().optional(),
});

export const createMachineRequestSchema = z.object({
  machineName: z.string().min(1),
  deviceAccountId: z.string().uuid().nullable().optional(),
});

export const updateMachineRequestSchema = z.object({
  machineName: z.string().min(1),
  deviceAccountId: z.string().uuid().nullable().optional(),
});

export const heartbeatSummarySchema = z.object({
  heartbeatId: z.string().uuid(),
  machineId: z.string().uuid(),
  sequenceNumber: z.number().int().min(0),
  sentAtUtc: utcTimestamp,
  receivedAtUtc: utcTimestamp,
  cpuUsagePercent: z.number().min(0).max(100),
  totalMemoryBytes: z.number().int().min(0),
  availableMemoryBytes: z.number().int().min(0),
});

export const runtimeSessionSummarySchema = z.object({
  runtimeSessionId: z.string().uuid(),
  machineId: z.string().uuid(),
  startedAtUtc: utcTimestamp,
  lastHeartbeatAtUtc: utcTimestamp,
  endedAtUtc: utcTimestamp.nullable().optional(),
  endReason: sessionEndReasonSchema,
  heartbeatCount: z.number().int().min(0),
  calculatedUptimeSeconds: z.number().int().min(0),
});

export const powerMeterSummarySchema = z.object({
  powerMeterId: z.string().uuid(),
  vendor: z.string().min(1),
  externalDeviceId: z.string().min(1),
  name: z.string().min(1),
  model: z.string().nullable().optional(),
  macAddress: z.string().nullable().optional(),
  ipAddress: z.string().nullable().optional(),
  firmwareVersion: z.string().nullable().optional(),
  connectionType: meterConnectionTypeSchema,
  registrationStatus: registrationStatusSchema,
  firstSeenAtUtc: utcTimestamp.nullable().optional(),
  lastSeenAtUtc: utcTimestamp.nullable().optional(),
});

export const createPowerMeterRequestSchema = z.object({
  vendor: z.string().min(1),
  externalDeviceId: z.string().min(1),
  name: z.string().min(1),
  model: z.string().nullable().optional(),
  macAddress: z.string().nullable().optional(),
  ipAddress: z.string().nullable().optional(),
  connectionType: meterConnectionTypeSchema,
  authenticationReference: z.string().nullable().optional(),
});

export const updatePowerMeterRequestSchema = z.object({
  name: z.string().min(1),
  model: z.string().nullable().optional(),
  macAddress: z.string().nullable().optional(),
  ipAddress: z.string().nullable().optional(),
  connectionType: meterConnectionTypeSchema,
  authenticationReference: z.string().nullable().optional(),
});

export const powerReadingSummarySchema = z.object({
  powerReadingId: z.string().uuid(),
  powerMeterId: z.string().uuid(),
  messageId: z.string().uuid(),
  measuredAtUtc: utcTimestamp,
  receivedAtUtc: utcTimestamp,
  activePowerWatts: z.number(),
  voltage: z.number().nullable().optional(),
  currentAmps: z.number().nullable().optional(),
  powerFactor: z.number().min(-1).max(1).nullable().optional(),
  frequencyHz: z.number().nullable().optional(),
  totalEnergyWattHours: z.number().nullable().optional(),
  outputIsOn: z.boolean().nullable().optional(),
  deviceTemperatureCelsius: z.number().nullable().optional(),
});

export const locationTypeSchema = z.enum([
  "Site",
  "Building",
  "Floor",
  "Room",
  "Office",
  "Desk",
  "Rack",
  "Lab",
  "Other",
]);

export const locationRequestSchema = z.object({
  name: z.string().min(1),
  locationType: locationTypeSchema,
  parentLocationId: z.string().uuid().nullable().optional(),
  timeZone: z.string().nullable().optional(),
  description: z.string().nullable().optional(),
});

export const locationSummarySchema = locationRequestSchema.extend({
  locationId: z.string().uuid(),
  isActive: z.boolean(),
});

export const monitoredDeviceTypeSchema = z.enum([
  "Computer",
  "Server",
  "Monitor",
  "PowerStrip",
  "NetworkSwitch",
  "Router",
  "Printer",
  "StorageDevice",
  "UPS",
  "Peripheral",
  "Appliance",
  "Other",
]);

export const monitoredDeviceRequestSchema = z.object({
  name: z.string().min(1),
  deviceType: monitoredDeviceTypeSchema,
  locationId: z.string().uuid().nullable().optional(),
  parentMonitoredDeviceId: z.string().uuid().nullable().optional(),
  machineId: z.string().uuid().nullable().optional(),
  description: z.string().nullable().optional(),
  manufacturer: z.string().nullable().optional(),
  model: z.string().nullable().optional(),
  serialNumber: z.string().nullable().optional(),
  isPowerConsumer: z.boolean(),
});

export const monitoredDeviceSummarySchema = z.object({
  monitoredDeviceId: z.string().uuid(),
  name: z.string().min(1),
  deviceType: monitoredDeviceTypeSchema,
  locationId: z.string().uuid().nullable().optional(),
  parentMonitoredDeviceId: z.string().uuid().nullable().optional(),
  machineId: z.string().uuid().nullable().optional(),
  manufacturer: z.string().nullable().optional(),
  model: z.string().nullable().optional(),
  isPowerConsumer: z.boolean(),
  isActive: z.boolean(),
});

export const createMachinePowerMeterAssociationRequestSchema = z.object({
  machineId: z.string().uuid(),
  powerMeterId: z.string().uuid(),
  relationshipType: machineMeterRelationshipTypeSchema,
  effectiveFromUtc: utcTimestamp,
  isPrimary: z.boolean(),
});

export const machinePowerMeterAssociationSummarySchema = z.object({
  machinePowerMeterAssociationId: z.string().uuid(),
  machineId: z.string().uuid(),
  powerMeterId: z.string().uuid(),
  relationshipType: machineMeterRelationshipTypeSchema,
  effectiveFromUtc: utcTimestamp,
  effectiveToUtc: utcTimestamp.nullable().optional(),
  isPrimary: z.boolean(),
});

export const createPowerMeterDeviceAssociationRequestSchema = z.object({
  powerMeterId: z.string().uuid(),
  monitoredDeviceId: z.string().uuid(),
  associationType: deviceAssociationTypeSchema,
  estimatedSharePercent: z.number().min(0).max(100).nullable().optional(),
  effectiveFromUtc: utcTimestamp,
  isPrimary: z.boolean(),
  notes: z.string().nullable().optional(),
});

export const powerMeterDeviceAssociationSummarySchema = z.object({
  associationId: z.string().uuid(),
  powerMeterId: z.string().uuid(),
  monitoredDeviceId: z.string().uuid(),
  associationType: deviceAssociationTypeSchema,
  estimatedSharePercent: z.number().min(0).max(100).nullable().optional(),
  effectiveFromUtc: utcTimestamp,
  effectiveToUtc: utcTimestamp.nullable().optional(),
  isPrimary: z.boolean(),
  notes: z.string().nullable().optional(),
});

export const endAssociationRequestSchema = z.object({
  effectiveToUtc: utcTimestamp,
});

export const meterLocationPlacementRequestSchema = z.object({
  locationId: z.string().uuid(),
  effectiveFromUtc: utcTimestamp,
  notes: z.string().nullable().optional(),
});

export const powerMeterLocationHistorySummarySchema = z.object({
  powerMeterLocationHistoryId: z.string().uuid(),
  powerMeterId: z.string().uuid(),
  locationId: z.string().uuid(),
  effectiveFromUtc: utcTimestamp,
  effectiveToUtc: utcTimestamp.nullable().optional(),
  notes: z.string().nullable().optional(),
});

export type TokenResponse = z.infer<typeof tokenResponseSchema>;
export type DeviceAccountSummary = z.infer<typeof deviceAccountSummarySchema>;
export type MachineSummary = z.infer<typeof machineSummarySchema>;
export type HeartbeatSummary = z.infer<typeof heartbeatSummarySchema>;
export type RuntimeSessionSummary = z.infer<
  typeof runtimeSessionSummarySchema
>;
export type PowerMeterSummary = z.infer<typeof powerMeterSummarySchema>;
export type PowerReadingSummary = z.infer<typeof powerReadingSummarySchema>;
export type LocationSummary = z.infer<typeof locationSummarySchema>;
export type MonitoredDeviceSummary = z.infer<
  typeof monitoredDeviceSummarySchema
>;
export type ProblemDetails = z.infer<typeof problemDetailsSchema>;
