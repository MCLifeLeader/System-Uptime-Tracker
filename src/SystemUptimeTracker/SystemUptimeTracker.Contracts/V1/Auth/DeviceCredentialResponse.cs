using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Auth;

/// <summary>
/// Response body when a device account is created or its credentials are
/// rotated (TASK-0204). The bootstrap password is returned exactly once, is
/// single-use (invalidated by the first successful device login, TASK-0004),
/// and is never persisted or logged in recoverable form. A rotation also
/// revokes every outstanding refresh token for the account.
/// </summary>
public sealed class DeviceCredentialResponse
{
    /// <summary>
    /// The device account the credential belongs to.
    /// </summary>
    [JsonPropertyName("deviceAccountId")]
    public required Guid DeviceAccountId { get; init; }

    /// <summary>
    /// The device account name the device presents at login.
    /// </summary>
    [JsonPropertyName("deviceAccountName")]
    public required string DeviceAccountName { get; init; }

    /// <summary>
    /// The one-time-displayed, single-use bootstrap password.
    /// </summary>
    [JsonPropertyName("bootstrapPassword")]
    public required string BootstrapPassword { get; init; }

    /// <summary>
    /// When the credential was issued (UTC).
    /// </summary>
    [JsonPropertyName("issuedAtUtc")]
    public required DateTimeOffset IssuedAtUtc { get; init; }
}
