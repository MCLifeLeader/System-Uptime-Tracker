using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Locations;

/// <summary>
/// Owner-facing view of a location (TASK-0206).
/// </summary>
public sealed class LocationSummary
{
    /// <summary>
    /// The location identifier.
    /// </summary>
    [JsonPropertyName("locationId")]
    public required Guid LocationId { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Kind of location.
    /// </summary>
    [JsonPropertyName("locationType")]
    public required LocationType LocationType { get; init; }

    /// <summary>
    /// Optional parent location.
    /// </summary>
    [JsonPropertyName("parentLocationId")]
    public Guid? ParentLocationId { get; init; }

    /// <summary>
    /// Optional IANA time zone identifier.
    /// </summary>
    [JsonPropertyName("timeZone")]
    public string? TimeZone { get; init; }

    /// <summary>
    /// Optional free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// False when the location has been deactivated.
    /// </summary>
    [JsonPropertyName("isActive")]
    public required bool IsActive { get; init; }
}
