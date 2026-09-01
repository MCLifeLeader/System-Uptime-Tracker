using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.MonitoredDevices;

/// <summary>
/// Request body for <c>POST /api/v1/monitored-devices</c> and
/// <c>PUT /api/v1/monitored-devices/{id}</c> (TASK-0206).
/// </summary>
public sealed class MonitoredDeviceRequest
{
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
    /// Optional parent device (for example a power strip feeding this
    /// device).
    /// </summary>
    [JsonPropertyName("parentMonitoredDeviceId")]
    public Guid? ParentMonitoredDeviceId { get; init; }

    /// <summary>
    /// Optional reporting machine this device record corresponds to. Not
    /// every monitored device is a reporting machine.
    /// </summary>
    [JsonPropertyName("machineId")]
    public Guid? MachineId { get; init; }

    /// <summary>
    /// Optional free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

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
    /// Optional serial number.
    /// </summary>
    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; init; }

    /// <summary>
    /// True when the device consumes power (participates in shared-load
    /// context).
    /// </summary>
    [JsonPropertyName("isPowerConsumer")]
    public required bool IsPowerConsumer { get; init; }
}
