using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Heartbeats;

/// <summary>
/// Response body for <c>POST /api/v1/heartbeats</c> (TASK-0203). Returned
/// with 202 when the heartbeat was accepted and persisted, and 200 with
/// <see cref="Duplicate"/> set when the idempotency key was already stored.
/// </summary>
public sealed class HeartbeatResponse
{
    /// <summary>
    /// Server-assigned identifier of the persisted heartbeat. For a duplicate
    /// delivery this is the identifier of the originally persisted heartbeat.
    /// </summary>
    [JsonPropertyName("heartbeatId")]
    public required Guid HeartbeatId { get; init; }

    /// <summary>
    /// The machine the heartbeat was recorded against.
    /// </summary>
    [JsonPropertyName("machineId")]
    public required Guid MachineId { get; init; }

    /// <summary>
    /// Echo of the request sequence number.
    /// </summary>
    [JsonPropertyName("sequenceNumber")]
    public required long SequenceNumber { get; init; }

    /// <summary>
    /// Server-authoritative receipt time (UTC). Never derived from the
    /// client-supplied send time (TASK-0304).
    /// </summary>
    [JsonPropertyName("receivedAtUtc")]
    public required DateTimeOffset ReceivedAtUtc { get; init; }

    /// <summary>
    /// True when this delivery matched an already-persisted idempotency key
    /// and produced no new side effect (TASK-0207).
    /// </summary>
    [JsonPropertyName("duplicate")]
    public required bool Duplicate { get; init; }
}
