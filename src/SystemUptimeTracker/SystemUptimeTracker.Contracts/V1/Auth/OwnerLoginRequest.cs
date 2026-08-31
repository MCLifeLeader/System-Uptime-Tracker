using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Auth;

/// <summary>
/// Request body for <c>POST /api/v1/auth/owner/login</c> (TASK-0204).
/// </summary>
public sealed class OwnerLoginRequest
{
    /// <summary>
    /// The owner account's email address (owner accounts require a unique
    /// email).
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    /// <summary>
    /// The owner account password. Never logged or echoed back.
    /// </summary>
    [JsonPropertyName("password")]
    public required string Password { get; init; }
}
