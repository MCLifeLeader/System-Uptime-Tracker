using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Auth;

/// <summary>
/// Response body for API-key issue/rotate calls (TASK-0204). The plaintext
/// key is returned exactly once, at issue or rotation time; only a salted
/// hash is stored, and rotation invalidates the previous key (TASK-0405).
/// </summary>
public sealed class ApiKeyResponse
{
    /// <summary>
    /// The device account the key belongs to.
    /// </summary>
    [JsonPropertyName("deviceAccountId")]
    public required Guid DeviceAccountId { get; init; }

    /// <summary>
    /// The device account name used as the Basic Auth user name.
    /// </summary>
    [JsonPropertyName("deviceAccountName")]
    public required string DeviceAccountName { get; init; }

    /// <summary>
    /// The one-time-displayed plaintext API key used as the Basic Auth
    /// password.
    /// </summary>
    [JsonPropertyName("apiKey")]
    public required string ApiKey { get; init; }

    /// <summary>
    /// When the key was issued (UTC).
    /// </summary>
    [JsonPropertyName("issuedAtUtc")]
    public required DateTimeOffset IssuedAtUtc { get; init; }
}
