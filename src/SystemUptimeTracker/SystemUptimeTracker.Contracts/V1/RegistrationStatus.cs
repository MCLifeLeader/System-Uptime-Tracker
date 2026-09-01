using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1;

/// <summary>
/// Registration lifecycle state for machines and power meters, serialized as
/// a string on the wire. First-release transitions and actors are documented
/// in docs/domain-model.md (TASK-0001).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<RegistrationStatus>))]
public enum RegistrationStatus
{
    /// <summary>
    /// The entity may submit and appear in telemetry.
    /// </summary>
    Active,

    /// <summary>
    /// Owner-suspended; telemetry is rejected but history is retained.
    /// </summary>
    Disabled,

    /// <summary>
    /// Terminal state; history is retained and identity is never reused.
    /// </summary>
    Retired,

    /// <summary>
    /// Reserved for a deferred approval workflow (TASK-1507); unreachable in
    /// the first release.
    /// </summary>
    Discovered,

    /// <summary>
    /// Reserved for a deferred approval workflow (TASK-1507); unreachable in
    /// the first release.
    /// </summary>
    PendingApproval,
}
