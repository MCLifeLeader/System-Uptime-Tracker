using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Associations;

/// <summary>
/// Request body for <c>POST /api/v1/machine-power-meter-associations</c>
/// (TASK-0206). Effective ranges for mutually exclusive primary
/// relationships must not overlap (TASK-1306); an overlap is rejected with
/// 409.
/// </summary>
public sealed class CreateMachinePowerMeterAssociationRequest
{
    /// <summary>
    /// The reporting machine.
    /// </summary>
    [JsonPropertyName("machineId")]
    public required Guid MachineId { get; init; }

    /// <summary>
    /// The power meter.
    /// </summary>
    [JsonPropertyName("powerMeterId")]
    public required Guid PowerMeterId { get; init; }

    /// <summary>
    /// Who-reports relationship kind.
    /// </summary>
    [JsonPropertyName("relationshipType")]
    public required MachineMeterRelationshipType RelationshipType { get; init; }

    /// <summary>
    /// When the relationship takes effect (UTC).
    /// </summary>
    [JsonPropertyName("effectiveFromUtc")]
    public required DateTimeOffset EffectiveFromUtc { get; init; }

    /// <summary>
    /// True when this is the machine's primary meter relationship; a machine
    /// has at most one active primary association.
    /// </summary>
    [JsonPropertyName("isPrimary")]
    public required bool IsPrimary { get; init; }
}
