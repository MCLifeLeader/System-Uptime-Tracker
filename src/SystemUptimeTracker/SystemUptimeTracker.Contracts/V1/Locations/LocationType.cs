using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Locations;

/// <summary>
/// Kind of physical location (TASK-0206; values from the Location entity in
/// docs/domain-model.md). Serialized as a string.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<LocationType>))]
public enum LocationType
{
    Site,
    Building,
    Floor,
    Room,
    Office,
    Desk,
    Rack,
    Lab,
    Other,
}
