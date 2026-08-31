using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Power;

/// <summary>
/// Request body for <c>PUT /api/v1/power-meters/{id}</c> (TASK-0206). The
/// vendor/external identity is immutable; lifecycle transitions use their
/// dedicated routes.
/// </summary>
public sealed class UpdatePowerMeterRequest
{
    /// <summary>
    /// Owner-facing display name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Optional vendor model.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; init; }

    /// <summary>
    /// Optional MAC address; unique when present.
    /// </summary>
    [JsonPropertyName("macAddress")]
    public string? MacAddress { get; init; }

    /// <summary>
    /// Current IP address.
    /// </summary>
    [JsonPropertyName("ipAddress")]
    public string? IpAddress { get; init; }

    /// <summary>
    /// How readings reach the platform.
    /// </summary>
    [JsonPropertyName("connectionType")]
    public required MeterConnectionType ConnectionType { get; init; }

    /// <summary>
    /// Optional secret-store reference for the local polling credential;
    /// never the credential itself.
    /// </summary>
    [JsonPropertyName("authenticationReference")]
    public string? AuthenticationReference { get; init; }
}
