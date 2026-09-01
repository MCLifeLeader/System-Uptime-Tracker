using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using SystemUptimeTracker.Contracts.V1.Auth;

namespace SystemUptimeTracker.Contracts.UnitTests.V1.Auth;

[TestFixture(Category = "Unit")]
public class AuthContractTests
{
    private const string GOLDEN_TOKEN_RESPONSE_JSON = """
        {
          "tokenType": "Bearer",
          "accessToken": "example.access.token",
          "expiresInSeconds": 900,
          "refreshToken": "example-refresh-token",
          "refreshTokenExpiresAtUtc": "2026-09-13T15:30:00+00:00"
        }
        """;

    private const string GOLDEN_DEVICE_CREDENTIAL_JSON = """
        {
          "deviceAccountId": "5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01",
          "deviceAccountName": "DEV-WORKSTATION-01",
          "bootstrapPassword": "example-one-time-bootstrap",
          "issuedAtUtc": "2026-08-30T12:00:00+00:00"
        }
        """;

    private const string GOLDEN_API_KEY_JSON = """
        {
          "deviceAccountId": "5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01",
          "deviceAccountName": "SHELLY-PLUG-KITCHEN",
          "apiKey": "example-one-time-api-key",
          "issuedAtUtc": "2026-08-30T12:00:00+00:00"
        }
        """;

    [Test]
    public void TokenResponse_GoldenJson_RoundTripsWithLifetimeMetadata()
    {
        TokenResponse response = ContractJson.Deserialize<TokenResponse>(GOLDEN_TOKEN_RESPONSE_JSON);

        Assert.Multiple(() =>
        {
            Assert.That(response.TokenType, Is.EqualTo("Bearer"));
            Assert.That(response.AccessToken, Is.EqualTo("example.access.token"));
            Assert.That(response.ExpiresInSeconds, Is.EqualTo(900));
            Assert.That(response.RefreshToken, Is.EqualTo("example-refresh-token"));
            Assert.That(response.RefreshTokenExpiresAtUtc, Is.EqualTo(DateTimeOffset.Parse("2026-09-13T15:30:00Z", CultureInfo.InvariantCulture)));
        });

        ContractJson.AssertMatchesGolden(response, GOLDEN_TOKEN_RESPONSE_JSON);
    }

    [Test]
    public void TokenResponse_NeverCarriesStoredSecretFields()
    {
        JsonNode actual = JsonNode.Parse(ContractJson.Serialize(new TokenResponse
        {
            TokenType = "Bearer",
            AccessToken = "a",
            ExpiresInSeconds = 900,
            RefreshToken = "r",
            RefreshTokenExpiresAtUtc = DateTimeOffset.Parse("2026-09-13T15:30:00Z", CultureInfo.InvariantCulture),
        }))!;

        // The wire shape must stay limited to issued tokens and lifetimes:
        // no password, hash, or key material fields may ever appear.
        Assert.That(actual.AsObject().Select(pair => pair.Key), Is.EquivalentTo(new[]
        {
            "tokenType",
            "accessToken",
            "expiresInSeconds",
            "refreshToken",
            "refreshTokenExpiresAtUtc",
        }));
    }

    [Test]
    public void OwnerLoginRequest_GoldenJson_Deserializes()
    {
        OwnerLoginRequest request = ContractJson.Deserialize<OwnerLoginRequest>("""
            { "email": "owner@example.test", "password": "example-password" }
            """);

        Assert.Multiple(() =>
        {
            Assert.That(request.Email, Is.EqualTo("owner@example.test"));
            Assert.That(request.Password, Is.EqualTo("example-password"));
        });
    }

    [TestCase("email")]
    [TestCase("password")]
    public void OwnerLoginRequest_MissingRequiredField_IsRejected(string requiredField)
    {
        ContractJson.AssertMissingRequiredFieldRejected<OwnerLoginRequest>("""
            { "email": "owner@example.test", "password": "example-password" }
            """,
            requiredField);
    }

    [Test]
    public void DeviceLoginRequest_GoldenJson_Deserializes()
    {
        DeviceLoginRequest request = ContractJson.Deserialize<DeviceLoginRequest>("""
            { "deviceAccountName": "DEV-WORKSTATION-01", "password": "example-one-time-bootstrap" }
            """);

        Assert.Multiple(() =>
        {
            Assert.That(request.DeviceAccountName, Is.EqualTo("DEV-WORKSTATION-01"));
            Assert.That(request.Password, Is.EqualTo("example-one-time-bootstrap"));
        });
    }

    [Test]
    public void RefreshTokenRequest_MissingToken_IsRejected()
    {
        Assert.Throws<JsonException>(
            () => ContractJson.Deserialize<RefreshTokenRequest>("{}"));
    }

    [Test]
    public void RevokeTokenRequest_BothOptionsAreExpressible()
    {
        RevokeTokenRequest specific = ContractJson.Deserialize<RevokeTokenRequest>("""
            { "refreshToken": "example-refresh-token" }
            """);
        RevokeTokenRequest all = ContractJson.Deserialize<RevokeTokenRequest>("""
            { "revokeAll": true }
            """);

        Assert.Multiple(() =>
        {
            Assert.That(specific.RefreshToken, Is.EqualTo("example-refresh-token"));
            Assert.That(specific.RevokeAll, Is.False);
            Assert.That(all.RefreshToken, Is.Null);
            Assert.That(all.RevokeAll, Is.True);
        });
    }

    [Test]
    public void DeviceCredentialResponse_GoldenJson_RoundTripsOneTimeBootstrapCredential()
    {
        DeviceCredentialResponse response =
            ContractJson.Deserialize<DeviceCredentialResponse>(GOLDEN_DEVICE_CREDENTIAL_JSON);

        Assert.Multiple(() =>
        {
            Assert.That(response.DeviceAccountId, Is.EqualTo(Guid.Parse("5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01")));
            Assert.That(response.DeviceAccountName, Is.EqualTo("DEV-WORKSTATION-01"));
            Assert.That(response.BootstrapPassword, Is.EqualTo("example-one-time-bootstrap"));
            Assert.That(response.IssuedAtUtc, Is.EqualTo(DateTimeOffset.Parse("2026-08-30T12:00:00Z", CultureInfo.InvariantCulture)));
        });

        ContractJson.AssertMatchesGolden(response, GOLDEN_DEVICE_CREDENTIAL_JSON);
    }

    [Test]
    public void ApiKeyResponse_GoldenJson_RoundTripsOneTimePlaintextKey()
    {
        ApiKeyResponse response = ContractJson.Deserialize<ApiKeyResponse>(GOLDEN_API_KEY_JSON);

        Assert.Multiple(() =>
        {
            Assert.That(response.DeviceAccountId, Is.EqualTo(Guid.Parse("5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01")));
            Assert.That(response.DeviceAccountName, Is.EqualTo("SHELLY-PLUG-KITCHEN"));
            Assert.That(response.ApiKey, Is.EqualTo("example-one-time-api-key"));
            Assert.That(response.IssuedAtUtc, Is.EqualTo(DateTimeOffset.Parse("2026-08-30T12:00:00Z", CultureInfo.InvariantCulture)));
        });

        ContractJson.AssertMatchesGolden(response, GOLDEN_API_KEY_JSON);
    }

    [Test]
    public void ApiKeyResponse_NeverCarriesHashFields()
    {
        JsonNode actual = JsonNode.Parse(ContractJson.Serialize(new ApiKeyResponse
        {
            DeviceAccountId = Guid.Parse("5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01"),
            DeviceAccountName = "SHELLY-PLUG-KITCHEN",
            ApiKey = "example-one-time-api-key",
            IssuedAtUtc = DateTimeOffset.Parse("2026-08-30T12:00:00Z", CultureInfo.InvariantCulture),
        }))!;

        Assert.That(actual.AsObject().Select(pair => pair.Key), Is.EquivalentTo(new[]
        {
            "deviceAccountId",
            "deviceAccountName",
            "apiKey",
            "issuedAtUtc",
        }));
    }
}
