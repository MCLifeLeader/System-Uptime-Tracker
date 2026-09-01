using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Heartbeats;

/// <summary>
/// Storage metrics for one volume, included on detailed-telemetry heartbeats
/// (TASK-0203). Values are bytes.
/// </summary>
public sealed class StorageVolumeTelemetry
{
    /// <summary>
    /// Volume identifier, for example "C:" on Windows or "/" on Linux.
    /// </summary>
    [JsonPropertyName("volumeName")]
    public required string VolumeName { get; init; }

    /// <summary>
    /// Optional file-system name, for example "NTFS" or "ext4".
    /// </summary>
    [JsonPropertyName("fileSystem")]
    public string? FileSystem { get; init; }

    /// <summary>
    /// Total volume capacity in bytes.
    /// </summary>
    [JsonPropertyName("totalBytes")]
    public required long TotalBytes { get; init; }

    /// <summary>
    /// Currently available capacity in bytes.
    /// </summary>
    [JsonPropertyName("availableBytes")]
    public required long AvailableBytes { get; init; }
}
