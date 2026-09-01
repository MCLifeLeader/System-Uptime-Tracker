using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Auth;

/// <summary>
/// Request body for <c>POST /api/v1/auth/refresh</c> (TASK-0204).
/// </summary>
public sealed class RefreshTokenRequest
{
    /// <summary>
    /// The current refresh token. It is invalidated by this call and replaced
    /// by the token in the response.
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; init; }
}
