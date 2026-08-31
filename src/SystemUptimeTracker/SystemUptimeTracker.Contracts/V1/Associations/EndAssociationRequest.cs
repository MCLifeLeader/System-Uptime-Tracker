using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Associations;

/// <summary>
/// Request body for the association <c>/end</c> routes and meter
/// location-history closure (TASK-0206). Ending an already-ended association
/// is idempotent.
/// </summary>
public sealed class EndAssociationRequest
{
    /// <summary>
    /// When the association ends (UTC). Must not precede the association's
    /// effective start.
    /// </summary>
    [JsonPropertyName("effectiveToUtc")]
    public required DateTimeOffset EffectiveToUtc { get; init; }
}
