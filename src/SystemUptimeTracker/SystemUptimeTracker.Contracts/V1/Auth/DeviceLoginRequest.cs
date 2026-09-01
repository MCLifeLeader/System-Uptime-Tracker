using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Auth;

/// <summary>
/// Request body for <c>POST /api/v1/auth/device/login</c> (TASK-0204). The
/// presented password is the device account's single-use bootstrap
/// credential: it is invalidated by the first successful login (TASK-0004).
/// </summary>
public sealed class DeviceLoginRequest
{
    /// <summary>
    /// The device account name issued by the owner.
    /// </summary>
    [JsonPropertyName("deviceAccountName")]
    public required string DeviceAccountName { get; init; }

    /// <summary>
    /// The single-use bootstrap credential provisioned out-of-band. Never
    /// logged or echoed back.
    /// </summary>
    [JsonPropertyName("password")]
    public required string Password { get; init; }
}
