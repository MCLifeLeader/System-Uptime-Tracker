using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Machines;

/// <summary>
/// Request body for <c>POST /api/v1/machines/register</c> (TASK-0202).
/// Registration is idempotent on the durable <see cref="AgentId"/>: repeating
/// the call returns the existing machine without a duplicate record.
/// </summary>
public sealed class MachineRegistrationRequest
{
    /// <summary>
    /// Contract payload version; see <see cref="PayloadVersions"/>.
    /// </summary>
    [JsonPropertyName("payloadVersion")]
    public required int PayloadVersion { get; init; }

    /// <summary>
    /// The durable agent identity created on the machine at first run. It is
    /// written onto every heartbeat and never changes for the machine's
    /// lifetime.
    /// </summary>
    [JsonPropertyName("agentId")]
    public required Guid AgentId { get; init; }

    /// <summary>
    /// Operating-system reported machine name. Identity never relies on this
    /// value alone.
    /// </summary>
    [JsonPropertyName("machineName")]
    public required string MachineName { get; init; }

    /// <summary>
    /// Operating system product name, for example "Ubuntu 24.04.3 LTS".
    /// </summary>
    [JsonPropertyName("operatingSystem")]
    public required string OperatingSystem { get; init; }

    /// <summary>
    /// Optional operating system version detail when it is not part of
    /// <see cref="OperatingSystem"/>.
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
}
