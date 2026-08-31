using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Associations;

/// <summary>
/// Request body for <c>POST /api/v1/power-meters/{id}/location-history</c>
/// (TASK-0206). Placing a meter closes its open placement at the new
/// placement's effective start.
/// </summary>
public sealed class MeterLocationPlacementRequest
{
    /// <summary>
    /// The location the meter now sits in.
    /// </summary>
    [JsonPropertyName("locationId")]
    public required Guid LocationId { get; init; }

    /// <summary>
    /// When the placement takes effect (UTC).
    /// </summary>
    [JsonPropertyName("effectiveFromUtc")]
    public required DateTimeOffset EffectiveFromUtc { get; init; }

    /// <summary>
    /// Optional free-text note.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}
