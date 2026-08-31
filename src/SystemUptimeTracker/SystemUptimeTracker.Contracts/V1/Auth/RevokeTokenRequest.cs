using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Auth;

/// <summary>
/// Request body for <c>POST /api/v1/auth/revoke</c> (TASK-0204). Exactly one
/// of the two options must be used: a specific refresh token, or
/// <see cref="RevokeAll"/> for every refresh token belonging to the calling
/// principal. Revocation is idempotent.
/// </summary>
public sealed class RevokeTokenRequest
{
    /// <summary>
    /// The specific refresh token to revoke. Omit when
    /// <see cref="RevokeAll"/> is true.
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; init; }

    /// <summary>
    /// True to revoke every outstanding refresh token for the calling
    /// principal.
    /// </summary>
    [JsonPropertyName("revokeAll")]
    public bool RevokeAll { get; init; }
}
