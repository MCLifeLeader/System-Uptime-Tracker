using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.MonitoredDevices;

/// <summary>
/// Kind of monitored physical equipment (TASK-0206; values from the
/// MonitoredDevice entity in docs/domain-model.md). Serialized as a string.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<MonitoredDeviceType>))]
public enum MonitoredDeviceType
{
    Computer,
    Server,
    Monitor,
    PowerStrip,
    NetworkSwitch,
    Router,
    Printer,
    StorageDevice,
    UPS,
    Peripheral,
    Appliance,
    Other,
}
