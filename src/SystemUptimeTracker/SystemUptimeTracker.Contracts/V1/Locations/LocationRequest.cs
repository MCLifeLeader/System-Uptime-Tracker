using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Locations;

/// <summary>
/// Request body for <c>POST /api/v1/locations</c> and
/// <c>PUT /api/v1/locations/{id}</c> (TASK-0206).
/// </summary>
public sealed class LocationRequest
{
    /// <summary>
    /// Display name, unique within the parent location.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Kind of location.
    /// </summary>
    [JsonPropertyName("locationType")]
    public required LocationType LocationType { get; init; }

    /// <summary>
    /// Optional parent for nested hierarchies (site → building → room).
    /// </summary>
    [JsonPropertyName("parentLocationId")]
    public Guid? ParentLocationId { get; init; }

    /// <summary>
    /// Optional IANA time zone identifier, for example "America/Denver".
    /// </summary>
    [JsonPropertyName("timeZone")]
    public string? TimeZone { get; init; }

    /// <summary>
    /// Optional free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
}
