using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Associations;

/// <summary>
/// Owner-facing view of where a meter was located during a period
/// (TASK-0206).
/// </summary>
public sealed class PowerMeterLocationHistorySummary
{
    /// <summary>
    /// The placement record identifier.
    /// </summary>
    [JsonPropertyName("powerMeterLocationHistoryId")]
    public required Guid PowerMeterLocationHistoryId { get; init; }

    /// <summary>
    /// The power meter.
    /// </summary>
    [JsonPropertyName("powerMeterId")]
    public required Guid PowerMeterId { get; init; }

    /// <summary>
    /// The location the meter sat in.
    /// </summary>
    [JsonPropertyName("locationId")]
    public required Guid LocationId { get; init; }

    /// <summary>
    /// When the placement took effect (UTC).
    /// </summary>
    [JsonPropertyName("effectiveFromUtc")]
    public required DateTimeOffset EffectiveFromUtc { get; init; }

    /// <summary>
    /// When the placement ended (UTC); null while current.
    /// </summary>
    [JsonPropertyName("effectiveToUtc")]
    public DateTimeOffset? EffectiveToUtc { get; init; }

    /// <summary>
    /// Optional free-text note.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}
