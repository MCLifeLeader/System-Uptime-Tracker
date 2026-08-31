using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.DeviceAccounts;

/// <summary>
/// Owner-facing view of a device account (TASK-0205). Never carries secret
/// material: API keys appear only as issuance metadata, never as values or
/// hashes.
/// </summary>
public sealed class DeviceAccountSummary
{
    /// <summary>
    /// The device account identifier.
    /// </summary>
    [JsonPropertyName("deviceAccountId")]
    public required Guid DeviceAccountId { get; init; }

    /// <summary>
    /// Operator-facing label, for example "DEV-WORKSTATION-01".
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Which authentication schemes the account may use.
    /// </summary>
    [JsonPropertyName("allowedAuthenticationMethods")]
    public required AllowedAuthenticationMethods AllowedAuthenticationMethods { get; init; }

    /// <summary>
    /// False when the account is disabled and its telemetry is rejected.
    /// </summary>
    [JsonPropertyName("isActive")]
    public required bool IsActive { get; init; }

    /// <summary>
    /// True when an API key is currently issued for the account.
    /// </summary>
    [JsonPropertyName("hasApiKey")]
    public required bool HasApiKey { get; init; }

    /// <summary>
    /// When the current API key was issued (UTC); null when none.
    /// </summary>
    [JsonPropertyName("apiKeyCreatedAtUtc")]
    public DateTimeOffset? ApiKeyCreatedAtUtc { get; init; }

    /// <summary>
    /// When the current API key last authenticated a request (UTC); null when
    /// never used or no key exists.
    /// </summary>
    [JsonPropertyName("apiKeyLastUsedAtUtc")]
    public DateTimeOffset? ApiKeyLastUsedAtUtc { get; init; }

    /// <summary>
    /// Number of machines currently authorized through this account.
    /// </summary>
    [JsonPropertyName("machineCount")]
    public required int MachineCount { get; init; }

    /// <summary>
    /// When the account was created (UTC).
    /// </summary>
    [JsonPropertyName("createdAtUtc")]
    public required DateTimeOffset CreatedAtUtc { get; init; }
}
