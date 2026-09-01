using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Associations;

/// <summary>
/// Owner-facing view of an effective-dated machine/meter relationship
/// (TASK-0206).
/// </summary>
public sealed class MachinePowerMeterAssociationSummary
{
    /// <summary>
    /// The association identifier.
    /// </summary>
    [JsonPropertyName("machinePowerMeterAssociationId")]
    public required Guid MachinePowerMeterAssociationId { get; init; }

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
    /// When the relationship took effect (UTC).
    /// </summary>
    [JsonPropertyName("effectiveFromUtc")]
    public required DateTimeOffset EffectiveFromUtc { get; init; }

    /// <summary>
    /// When the relationship ended (UTC); null while active.
    /// </summary>
    [JsonPropertyName("effectiveToUtc")]
    public DateTimeOffset? EffectiveToUtc { get; init; }

    /// <summary>
    /// True when this is the machine's primary meter relationship.
    /// </summary>
    [JsonPropertyName("isPrimary")]
    public required bool IsPrimary { get; init; }
}
