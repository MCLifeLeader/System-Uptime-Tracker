using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Machines;

/// <summary>
/// Response body for <c>POST /api/v1/machines/register</c> (TASK-0202).
/// Returned with 201 when a machine record was created and 200 when an
/// existing <see cref="AgentId"/> was reconciled.
/// </summary>
public sealed class MachineRegistrationResponse
{
    /// <summary>
    /// Server-assigned machine identifier used by owner-facing endpoints.
    /// </summary>
    [JsonPropertyName("machineId")]
    public required Guid MachineId { get; init; }

    /// <summary>
    /// The durable agent identity the machine registered with.
    /// </summary>
    [JsonPropertyName("agentId")]
    public required Guid AgentId { get; init; }

    /// <summary>
    /// Current registration lifecycle state; always Active after a successful
    /// first-release registration (TASK-0001).
    /// </summary>
    [JsonPropertyName("registrationStatus")]
    public required RegistrationStatus RegistrationStatus { get; init; }

    /// <summary>
    /// True when this call created the machine record; false when the call
    /// idempotently reconciled an existing registration.
    /// </summary>
    [JsonPropertyName("wasCreated")]
    public required bool WasCreated { get; init; }
}
