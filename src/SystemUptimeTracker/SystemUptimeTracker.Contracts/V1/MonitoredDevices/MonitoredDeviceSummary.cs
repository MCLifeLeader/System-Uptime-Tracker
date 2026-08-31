using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.MonitoredDevices;

/// <summary>
/// Owner-facing view of a monitored device (TASK-0206).
/// </summary>
public sealed class MonitoredDeviceSummary
{
    /// <summary>
    /// The monitored-device identifier.
    /// </summary>
    [JsonPropertyName("monitoredDeviceId")]
    public required Guid MonitoredDeviceId { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Kind of equipment.
    /// </summary>
    [JsonPropertyName("deviceType")]
    public required MonitoredDeviceType DeviceType { get; init; }

    /// <summary>
    /// Optional location the device sits in.
    /// </summary>
    [JsonPropertyName("locationId")]
    public Guid? LocationId { get; init; }

    /// <summary>
    /// Optional parent device.
    /// </summary>
    [JsonPropertyName("parentMonitoredDeviceId")]
    public Guid? ParentMonitoredDeviceId { get; init; }

    /// <summary>
    /// Optional reporting machine this device corresponds to.
    /// </summary>
    [JsonPropertyName("machineId")]
    public Guid? MachineId { get; init; }

    /// <summary>
    /// Optional manufacturer.
    /// </summary>
    [JsonPropertyName("manufacturer")]
    public string? Manufacturer { get; init; }

    /// <summary>
    /// Optional model.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; init; }

    /// <summary>
    /// True when the device consumes power.
    /// </summary>
    [JsonPropertyName("isPowerConsumer")]
    public required bool IsPowerConsumer { get; init; }

    /// <summary>
    /// False when the device record has been deactivated.
    /// </summary>
    [JsonPropertyName("isActive")]
    public required bool IsActive { get; init; }
}
