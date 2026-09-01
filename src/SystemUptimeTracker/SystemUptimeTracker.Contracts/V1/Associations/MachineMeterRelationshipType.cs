using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Associations;

/// <summary>
/// How a reporting machine relates to a power meter — who reports the
/// reading (TASK-0206). Intentionally distinct from
/// <see cref="DeviceAssociationType"/>, which describes what consumes the
/// power.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<MachineMeterRelationshipType>))]
public enum MachineMeterRelationshipType
{
    /// <summary>
    /// The meter powers only the reporting machine; readings may be reported
    /// as directly measured machine power.
    /// </summary>
    DedicatedLoad,

    /// <summary>
    /// The reporting machine is one of several devices powered by the meter;
    /// readings are shared aggregate measurements.
    /// </summary>
    SharedLoad,

    /// <summary>
    /// The machine reports meter data but is not powered by the meter;
    /// readings must never appear as this machine's consumption.
    /// </summary>
    CollectorOnly,
}
