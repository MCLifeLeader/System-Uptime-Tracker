using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Auth;

/// <summary>
/// Response body for successful owner login, device login, and refresh calls
/// (TASK-0204). Contains issued tokens and lifetime metadata only — never
/// stored secrets, hashes, or the credential that was presented.
/// </summary>
public sealed class TokenResponse
{
    /// <summary>
    /// Token type for the Authorization header; always "Bearer" in v1.
    /// </summary>
    [JsonPropertyName("tokenType")]
    public required string TokenType { get; init; }

    /// <summary>
    /// The short-lived access token.
    /// </summary>
    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; init; }

    /// <summary>
    /// Access-token lifetime in seconds from issuance.
    /// </summary>
    [JsonPropertyName("expiresInSeconds")]
    public required int ExpiresInSeconds { get; init; }

    /// <summary>
    /// The refresh token. Single-use: presenting it at the refresh endpoint
    /// rotates it, and replaying a rotated token is rejected (TASK-0404).
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; init; }

    /// <summary>
    /// Absolute UTC expiry of the refresh token.
    /// </summary>
    [JsonPropertyName("refreshTokenExpiresAtUtc")]
    public required DateTimeOffset RefreshTokenExpiresAtUtc { get; init; }
}
