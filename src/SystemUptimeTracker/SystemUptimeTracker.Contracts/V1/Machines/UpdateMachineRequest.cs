using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Machines;

/// <summary>
/// Request body for <c>PUT /api/v1/machines/{id}</c> (TASK-0205). Lifecycle
/// transitions (disable/enable/retire) use their dedicated routes.
/// </summary>
public sealed class UpdateMachineRequest
{
    /// <summary>
    /// New owner-maintained machine name.
    /// </summary>
    [JsonPropertyName("machineName")]
    public required string MachineName { get; init; }

    /// <summary>
    /// The device account authorized to report for this machine; null clears
    /// the assignment without deleting history (see the domain-model
    /// integrity rules).
    /// </summary>
    [JsonPropertyName("deviceAccountId")]
    public Guid? DeviceAccountId { get; init; }
}
