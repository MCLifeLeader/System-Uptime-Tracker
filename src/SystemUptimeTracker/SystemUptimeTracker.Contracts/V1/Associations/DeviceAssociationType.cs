using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Associations;

/// <summary>
/// How a monitored device is powered through a meter — what consumes the
/// power (TASK-0206). Intentionally distinct from
/// <see cref="MachineMeterRelationshipType"/>, which describes who reports
/// the reading.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<DeviceAssociationType>))]
public enum DeviceAssociationType
{
    /// <summary>
    /// The meter powers only this device.
    /// </summary>
    Dedicated,

    /// <summary>
    /// The device shares the meter with other consumers.
    /// </summary>
    Shared,
}
