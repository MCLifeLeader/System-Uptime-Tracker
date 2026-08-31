using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Heartbeats;

/// <summary>
/// Owner-facing view of a persisted heartbeat for
/// <c>GET /api/v1/machines/{id}/heartbeats</c> (TASK-0205).
/// </summary>
public sealed class HeartbeatSummary
{
    /// <summary>
    /// The persisted heartbeat identifier.
    /// </summary>
    [JsonPropertyName("heartbeatId")]
    public required Guid HeartbeatId { get; init; }

    /// <summary>
    /// The machine the heartbeat belongs to.
    /// </summary>
    [JsonPropertyName("machineId")]
    public required Guid MachineId { get; init; }

    /// <summary>
    /// Per-agent sequence number.
    /// </summary>
    [JsonPropertyName("sequenceNumber")]
    public required long SequenceNumber { get; init; }

    /// <summary>
    /// Client-reported send time (UTC).
    /// </summary>
    [JsonPropertyName("sentAtUtc")]
    public required DateTimeOffset SentAtUtc { get; init; }

    /// <summary>
    /// Server-authoritative receipt time (UTC).
    /// </summary>
    [JsonPropertyName("receivedAtUtc")]
    public required DateTimeOffset ReceivedAtUtc { get; init; }

    /// <summary>
    /// CPU usage percentage (0–100) reported on the heartbeat.
    /// </summary>
    [JsonPropertyName("cpuUsagePercent")]
    public required double CpuUsagePercent { get; init; }

    /// <summary>
    /// Total physical memory in bytes.
    /// </summary>
    [JsonPropertyName("totalMemoryBytes")]
    public required long TotalMemoryBytes { get; init; }

    /// <summary>
    /// Available physical memory in bytes.
    /// </summary>
    [JsonPropertyName("availableMemoryBytes")]
    public required long AvailableMemoryBytes { get; init; }
}
