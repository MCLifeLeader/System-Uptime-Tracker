using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Machines;

/// <summary>
/// Owner-facing view of a machine for inventory and detail endpoints
/// (TASK-0205).
/// </summary>
public sealed class MachineSummary
{
    /// <summary>
    /// Server-assigned machine identifier.
    /// </summary>
    [JsonPropertyName("machineId")]
    public required Guid MachineId { get; init; }

    /// <summary>
    /// Durable agent identity; null for an owner pre-created machine no agent
    /// has registered against yet (TASK-0001).
    /// </summary>
    [JsonPropertyName("agentId")]
    public Guid? AgentId { get; init; }

    /// <summary>
    /// Operating-system reported machine name.
    /// </summary>
    [JsonPropertyName("machineName")]
    public required string MachineName { get; init; }

    /// <summary>
    /// Operating system product name; null until first agent contact.
    /// </summary>
    [JsonPropertyName("operatingSystem")]
    public string? OperatingSystem { get; init; }

    /// <summary>
    /// Optional operating system version detail.
    /// </summary>
    [JsonPropertyName("operatingSystemVersion")]
    public string? OperatingSystemVersion { get; init; }

    /// <summary>
    /// Processor architecture; null until first agent contact.
    /// </summary>
    [JsonPropertyName("architecture")]
    public string? Architecture { get; init; }

    /// <summary>
    /// Reporting agent version; null until first agent contact.
    /// </summary>
    [JsonPropertyName("agentVersion")]
    public string? AgentVersion { get; init; }

    /// <summary>
    /// Registration lifecycle state.
    /// </summary>
    [JsonPropertyName("registrationStatus")]
    public required RegistrationStatus RegistrationStatus { get; init; }

    /// <summary>
    /// When telemetry was first received (UTC); null before first contact.
    /// </summary>
    [JsonPropertyName("firstSeenAtUtc")]
    public DateTimeOffset? FirstSeenAtUtc { get; init; }

    /// <summary>
    /// When telemetry was last received (UTC); null before first contact.
    /// Server-authoritative and never moved backward by delayed queue uploads
    /// (TASK-0505).
    /// </summary>
    [JsonPropertyName("lastSeenAtUtc")]
    public DateTimeOffset? LastSeenAtUtc { get; init; }

    /// <summary>
    /// The device account currently authorized to report for this machine;
    /// null when unassigned.
    /// </summary>
    [JsonPropertyName("deviceAccountId")]
    public Guid? DeviceAccountId { get; init; }
}
