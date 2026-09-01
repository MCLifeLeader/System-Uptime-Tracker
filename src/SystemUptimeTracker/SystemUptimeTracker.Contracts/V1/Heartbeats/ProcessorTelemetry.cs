using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Heartbeats;

/// <summary>
/// Processor metrics carried on every heartbeat (TASK-0203).
/// </summary>
public sealed class ProcessorTelemetry
{
    /// <summary>
    /// Number of logical processors visible to the operating system.
    /// </summary>
    [JsonPropertyName("logicalProcessorCount")]
    public required int LogicalProcessorCount { get; init; }

    /// <summary>
    /// Total CPU usage as a percentage in the range 0–100.
    /// </summary>
    [JsonPropertyName("usagePercent")]
    public required double UsagePercent { get; init; }
}
