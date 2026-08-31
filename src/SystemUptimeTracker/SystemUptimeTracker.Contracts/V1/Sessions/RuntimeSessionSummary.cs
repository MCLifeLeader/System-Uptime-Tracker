using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Sessions;

/// <summary>
/// Owner-facing view of a reconstructed runtime session for
/// <c>GET /api/v1/machines/{id}/sessions</c> (TASK-0205; sessions are
/// server-derived, never client-authored).
/// </summary>
public sealed class RuntimeSessionSummary
{
    /// <summary>
    /// The session identifier.
    /// </summary>
    [JsonPropertyName("runtimeSessionId")]
    public required Guid RuntimeSessionId { get; init; }

    /// <summary>
    /// The machine the session belongs to.
    /// </summary>
    [JsonPropertyName("machineId")]
    public required Guid MachineId { get; init; }

    /// <summary>
    /// When the session started (UTC).
    /// </summary>
    [JsonPropertyName("startedAtUtc")]
    public required DateTimeOffset StartedAtUtc { get; init; }

    /// <summary>
    /// Receipt time of the most recent heartbeat in the session (UTC).
    /// </summary>
    [JsonPropertyName("lastHeartbeatAtUtc")]
    public required DateTimeOffset LastHeartbeatAtUtc { get; init; }

    /// <summary>
    /// When the session ended (UTC); null while the session is running.
    /// </summary>
    [JsonPropertyName("endedAtUtc")]
    public DateTimeOffset? EndedAtUtc { get; init; }

    /// <summary>
    /// Why the session ended; Running while open.
    /// </summary>
    [JsonPropertyName("endReason")]
    public required SessionEndReason EndReason { get; init; }

    /// <summary>
    /// Number of heartbeats attributed to the session.
    /// </summary>
    [JsonPropertyName("heartbeatCount")]
    public required int HeartbeatCount { get; init; }

    /// <summary>
    /// Calculated uptime for the session in seconds (boundary semantics per
    /// TASK-0606).
    /// </summary>
    [JsonPropertyName("calculatedUptimeSeconds")]
    public required long CalculatedUptimeSeconds { get; init; }
}
