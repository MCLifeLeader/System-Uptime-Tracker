using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Machines;

/// <summary>
/// Request body for owner pre-creation via <c>POST /api/v1/machines</c>
/// (TASK-0205; pre-created records per TASK-0001). The machine has no
/// AgentId until an agent registers against it.
/// </summary>
public sealed class CreateMachineRequest
{
    /// <summary>
    /// Expected machine name used to bind the first matching registration.
    /// </summary>
    [JsonPropertyName("machineName")]
    public required string MachineName { get; init; }

    /// <summary>
    /// Optional device account to authorize for this machine ahead of
    /// registration.
    /// </summary>
    [JsonPropertyName("deviceAccountId")]
    public Guid? DeviceAccountId { get; init; }
}
