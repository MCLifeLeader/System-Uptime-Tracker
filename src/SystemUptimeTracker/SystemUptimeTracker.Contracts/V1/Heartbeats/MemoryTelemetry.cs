using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Heartbeats;

/// <summary>
/// Memory metrics carried on every heartbeat (TASK-0203). Values are bytes.
/// </summary>
public sealed class MemoryTelemetry
{
    /// <summary>
    /// Total physical memory in bytes.
    /// </summary>
    [JsonPropertyName("totalBytes")]
    public required long TotalBytes { get; init; }

    /// <summary>
    /// Currently available physical memory in bytes.
    /// </summary>
    [JsonPropertyName("availableBytes")]
    public required long AvailableBytes { get; init; }
}
