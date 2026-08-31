using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Power;

/// <summary>
/// Response body for <c>POST /api/v1/power-readings</c> (TASK-0206).
/// Returned with 202 when accepted and 200 with <see cref="Duplicate"/> set
/// when the idempotency key was already stored (TASK-0207).
/// </summary>
public sealed class PowerReadingResponse
{
    /// <summary>
    /// Server-assigned identifier of the persisted reading. For a duplicate
    /// delivery this is the originally persisted reading's identifier.
    /// </summary>
    [JsonPropertyName("powerReadingId")]
    public required Guid PowerReadingId { get; init; }

    /// <summary>
    /// The registered meter the reading was recorded against.
    /// </summary>
    [JsonPropertyName("powerMeterId")]
    public required Guid PowerMeterId { get; init; }

    /// <summary>
    /// Echo of the request message identifier.
    /// </summary>
    [JsonPropertyName("messageId")]
    public required Guid MessageId { get; init; }

    /// <summary>
    /// Server-authoritative receipt time (UTC).
    /// </summary>
    [JsonPropertyName("receivedAtUtc")]
    public required DateTimeOffset ReceivedAtUtc { get; init; }

    /// <summary>
    /// True when this delivery matched an already-persisted idempotency key
    /// and produced no new side effect.
    /// </summary>
    [JsonPropertyName("duplicate")]
    public required bool Duplicate { get; init; }
}
