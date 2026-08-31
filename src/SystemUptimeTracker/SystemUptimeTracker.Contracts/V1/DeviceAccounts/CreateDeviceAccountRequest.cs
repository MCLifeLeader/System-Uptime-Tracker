using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.DeviceAccounts;

/// <summary>
/// Request body for <c>POST /api/v1/device-accounts</c> (TASK-0205). The
/// response carries the account's one-time credential material
/// (DeviceCredentialResponse or ApiKeyResponse depending on the allowed
/// methods).
/// </summary>
public sealed class CreateDeviceAccountRequest
{
    /// <summary>
    /// Unique operator-facing account name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Which authentication schemes the account may use.
    /// </summary>
    [JsonPropertyName("allowedAuthenticationMethods")]
    public required AllowedAuthenticationMethods AllowedAuthenticationMethods { get; init; }
}
