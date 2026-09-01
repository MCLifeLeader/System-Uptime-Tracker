using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Power;

/// <summary>
/// Owner-facing view of a power meter (TASK-0206). Never carries polling
/// credentials; only the secret-store reference name appears.
/// </summary>
public sealed class PowerMeterSummary
{
    /// <summary>
    /// Server-assigned meter identifier.
    /// </summary>
    [JsonPropertyName("powerMeterId")]
    public required Guid PowerMeterId { get; init; }

    /// <summary>
    /// Vendor name.
    /// </summary>
    [JsonPropertyName("vendor")]
    public required string Vendor { get; init; }

    /// <summary>
    /// Vendor-specific durable device identifier.
    /// </summary>
    [JsonPropertyName("externalDeviceId")]
    public required string ExternalDeviceId { get; init; }

    /// <summary>
    /// Owner-facing display name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Optional vendor model.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; init; }

    /// <summary>
    /// Optional MAC address.
    /// </summary>
    [JsonPropertyName("macAddress")]
    public string? MacAddress { get; init; }

    /// <summary>
    /// Current IP address (runtime connectivity data).
    /// </summary>
    [JsonPropertyName("ipAddress")]
    public string? IpAddress { get; init; }

    /// <summary>
    /// Last reported firmware version.
    /// </summary>
    [JsonPropertyName("firmwareVersion")]
    public string? FirmwareVersion { get; init; }

    /// <summary>
    /// How readings reach the platform.
    /// </summary>
    [JsonPropertyName("connectionType")]
    public required MeterConnectionType ConnectionType { get; init; }

    /// <summary>
    /// Registration lifecycle state.
    /// </summary>
    [JsonPropertyName("registrationStatus")]
    public required RegistrationStatus RegistrationStatus { get; init; }

    /// <summary>
    /// When a reading was first received (UTC); null before first data.
    /// </summary>
    [JsonPropertyName("firstSeenAtUtc")]
    public DateTimeOffset? FirstSeenAtUtc { get; init; }

    /// <summary>
    /// When a reading was last received (UTC); null before first data.
    /// </summary>
    [JsonPropertyName("lastSeenAtUtc")]
    public DateTimeOffset? LastSeenAtUtc { get; init; }
}
