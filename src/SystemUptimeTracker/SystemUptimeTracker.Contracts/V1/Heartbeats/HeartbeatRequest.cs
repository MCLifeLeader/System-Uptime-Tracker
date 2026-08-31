using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Heartbeats;

/// <summary>
/// Request body for <c>POST /api/v1/heartbeats</c> (TASK-0203). The
/// idempotency key is <see cref="AgentId"/> + <see cref="SequenceNumber"/>
/// (TASK-0207): a duplicate delivery persists nothing new and is answered
/// with the original logical result.
/// </summary>
public sealed class HeartbeatRequest
{
    /// <summary>
    /// Contract payload version; see <see cref="PayloadVersions"/>.
    /// </summary>
    [JsonPropertyName("payloadVersion")]
    public required int PayloadVersion { get; init; }

    /// <summary>
    /// The durable agent identity issued at first run and used at
    /// registration.
    /// </summary>
    [JsonPropertyName("agentId")]
    public required Guid AgentId { get; init; }

    /// <summary>
    /// Monotonically increasing per-agent sequence number. Combined with
    /// <see cref="AgentId"/> it forms the heartbeat idempotency key.
    /// </summary>
    [JsonPropertyName("sequenceNumber")]
    public required long SequenceNumber { get; init; }

    /// <summary>
    /// Client-reported send time (UTC). The server records its own
    /// authoritative receipt time separately and never trusts this value for
    /// receipt ordering (TASK-0304).
    /// </summary>
    [JsonPropertyName("sentAtUtc")]
    public required DateTimeOffset SentAtUtc { get; init; }

    /// <summary>
    /// When the reporting agent process started (UTC).
    /// </summary>
    [JsonPropertyName("agentStartedAtUtc")]
    public required DateTimeOffset AgentStartedAtUtc { get; init; }

    /// <summary>
    /// Operating-system boot time (UTC), used as reboot evidence by
    /// runtime-session reconstruction.
    /// </summary>
    [JsonPropertyName("systemBootTimeUtc")]
    public required DateTimeOffset SystemBootTimeUtc { get; init; }

    /// <summary>
    /// Operating-system reported machine name.
    /// </summary>
    [JsonPropertyName("machineName")]
    public required string MachineName { get; init; }

    /// <summary>
    /// Operating system product name.
    /// </summary>
    [JsonPropertyName("operatingSystem")]
    public required string OperatingSystem { get; init; }

    /// <summary>
    /// Optional operating system version detail.
    /// </summary>
    [JsonPropertyName("operatingSystemVersion")]
    public string? OperatingSystemVersion { get; init; }

    /// <summary>
    /// Processor architecture, for example "X64" or "Arm64".
    /// </summary>
    [JsonPropertyName("architecture")]
    public required string Architecture { get; init; }

    /// <summary>
    /// Version of the reporting agent.
    /// </summary>
    [JsonPropertyName("agentVersion")]
    public required string AgentVersion { get; init; }

    /// <summary>
    /// Processor metrics captured on every heartbeat.
    /// </summary>
    [JsonPropertyName("processor")]
    public required ProcessorTelemetry Processor { get; init; }

    /// <summary>
    /// Memory metrics captured on every heartbeat.
    /// </summary>
    [JsonPropertyName("memory")]
    public required MemoryTelemetry Memory { get; init; }

    /// <summary>
    /// Storage-volume snapshot. Optional: included only on detailed-telemetry
    /// heartbeats (default every 900 seconds, TASK-0005); null or absent on
    /// ordinary heartbeats.
    /// </summary>
    [JsonPropertyName("storage")]
    public IReadOnlyList<StorageVolumeTelemetry>? Storage { get; init; }
}
