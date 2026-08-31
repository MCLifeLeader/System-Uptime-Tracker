using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.DeviceAccounts;

/// <summary>
/// Which authentication schemes a device account may use (TASK-0205; see the
/// DeviceAccount entity in docs/domain-model.md). Serialized as a string.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<AllowedAuthenticationMethods>))]
public enum AllowedAuthenticationMethods
{
    /// <summary>
    /// JWT bearer tokens only (single-use bootstrap credential plus refresh
    /// rotation).
    /// </summary>
    Jwt,

    /// <summary>
    /// HTTP Basic Auth with a hashed API key only (constrained devices).
    /// </summary>
    ApiKey,

    /// <summary>
    /// Both schemes are permitted.
    /// </summary>
    Both,
}
