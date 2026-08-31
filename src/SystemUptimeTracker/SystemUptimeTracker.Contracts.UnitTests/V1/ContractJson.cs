using System.Text.Json;

namespace SystemUptimeTracker.Contracts.UnitTests.V1;

/// <summary>
/// Shared serializer configuration for contract tests, matching the web
/// defaults the API and QA client use. Wire names are pinned by
/// [JsonPropertyName] attributes, so tests fail if an attribute is removed
/// even though the naming policy would produce the same name.
/// </summary>
internal static class ContractJson
{
    internal static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    internal static T Deserialize<T>(string json)
    {
        T? value = JsonSerializer.Deserialize<T>(json, Options);
        Assert.That(value, Is.Not.Null);
        return value;
    }

    internal static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, Options);
    }
}
