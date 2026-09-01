using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Associations;

/// <summary>
/// Owner-facing view of an effective-dated meter/device power association
/// (TASK-0206).
/// </summary>
public sealed class PowerMeterDeviceAssociationSummary
{
    /// <summary>
    /// The association identifier.
    /// </summary>
    [JsonPropertyName("associationId")]
    public required Guid AssociationId { get; init; }

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
    /// Optional estimated share (0–100); always an estimate label.
    /// </summary>
    [JsonPropertyName("estimatedSharePercent")]
    public double? EstimatedSharePercent { get; init; }

    /// <summary>
    /// When the association took effect (UTC).
    /// </summary>
    [JsonPropertyName("effectiveFromUtc")]
    public required DateTimeOffset EffectiveFromUtc { get; init; }

    /// <summary>
    /// When the association ended (UTC); null while active.
    /// </summary>
    [JsonPropertyName("effectiveToUtc")]
    public DateTimeOffset? EffectiveToUtc { get; init; }

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
