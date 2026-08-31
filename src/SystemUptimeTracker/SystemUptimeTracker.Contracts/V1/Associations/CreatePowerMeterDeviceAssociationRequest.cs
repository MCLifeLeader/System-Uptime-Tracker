using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Associations;

/// <summary>
/// Request body for <c>POST /api/v1/power-meter-device-associations</c>
/// (TASK-0206): which monitored device is physically powered through a
/// meter.
/// </summary>
public sealed class CreatePowerMeterDeviceAssociationRequest
{
    /// <summary>
    /// The power meter.
    /// </summary>
    [JsonPropertyName("powerMeterId")]
    public required Guid PowerMeterId { get; init; }

    /// <summary>
    /// The powered monitored device.
    /// </summary>
    [JsonPropertyName("monitoredDeviceId")]
    public required Guid MonitoredDeviceId { get; init; }

    /// <summary>
    /// What-consumes association kind.
    /// </summary>
    [JsonPropertyName("associationType")]
    public required DeviceAssociationType AssociationType { get; init; }

    /// <summary>
    /// Optional estimated share (0–100) of the meter's measured power for a
    /// shared association. Always an estimate label, never a measurement.
    /// </summary>
    [JsonPropertyName("estimatedSharePercent")]
    public double? EstimatedSharePercent { get; init; }

    /// <summary>
    /// When the association takes effect (UTC).
    /// </summary>
    [JsonPropertyName("effectiveFromUtc")]
    public required DateTimeOffset EffectiveFromUtc { get; init; }

    /// <summary>
    /// True when this is the device's primary power source.
    /// </summary>
    [JsonPropertyName("isPrimary")]
    public required bool IsPrimary { get; init; }

    /// <summary>
    /// Optional free-text note.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}
