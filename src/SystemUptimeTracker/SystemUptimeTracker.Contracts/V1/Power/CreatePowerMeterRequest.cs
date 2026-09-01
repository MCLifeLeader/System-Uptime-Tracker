using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Power;

/// <summary>
/// Request body for <c>POST /api/v1/power-meters</c> (TASK-0206). Meter
/// identity is <see cref="Vendor"/> + <see cref="ExternalDeviceId"/> and must
/// be unique; a duplicate registration is rejected with 409.
/// </summary>
public sealed class CreatePowerMeterRequest
{
    /// <summary>
    /// Vendor name, for example "Shelly".
    /// </summary>
    [JsonPropertyName("vendor")]
    public required string Vendor { get; init; }

    /// <summary>
    /// Vendor-specific durable device identifier.
    /// </summary>
    [JsonPropertyName("externalDeviceId")]
    public required string ExternalDeviceId { get; init; }

    /// <summary>
    /// Owner-facing display name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Optional vendor model, for example "Plug US Gen4".
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; init; }

    /// <summary>
    /// Optional MAC address; unique when present.
    /// </summary>
    [JsonPropertyName("macAddress")]
    public string? MacAddress { get; init; }

    /// <summary>
    /// Current IP address. Runtime connectivity data, never identity.
    /// </summary>
    [JsonPropertyName("ipAddress")]
    public string? IpAddress { get; init; }

    /// <summary>
    /// How readings reach the platform.
    /// </summary>
    [JsonPropertyName("connectionType")]
    public required MeterConnectionType ConnectionType { get; init; }

    /// <summary>
    /// Optional reference (name/key) into a secret store holding the local
    /// polling credential. Never the credential itself — the API rejects
    /// values that look like secrets rather than references.
    /// </summary>
    [JsonPropertyName("authenticationReference")]
    public string? AuthenticationReference { get; init; }
}
