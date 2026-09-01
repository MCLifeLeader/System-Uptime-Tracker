using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.DeviceAccounts;

/// <summary>
/// Request body for <c>PUT /api/v1/device-accounts/{id}</c> (TASK-0205).
/// Enable/disable and credential operations use their dedicated routes rather
/// than this update.
/// </summary>
public sealed class UpdateDeviceAccountRequest
{
    /// <summary>
    /// New unique operator-facing account name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Which authentication schemes the account may use. Removing a scheme
    /// revokes that scheme's credential material.
    /// </summary>
    [JsonPropertyName("allowedAuthenticationMethods")]
    public required AllowedAuthenticationMethods AllowedAuthenticationMethods { get; init; }
}
