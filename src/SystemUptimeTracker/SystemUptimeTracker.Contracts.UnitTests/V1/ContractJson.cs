using System.Text.Json;
using System.Text.Json.Nodes;

namespace SystemUptimeTracker.Contracts.UnitTests.V1;

/// <summary>
/// Shared serializer configuration for contract tests, matching the web
/// defaults the API and QA client use. The golden-JSON deep-equality tests
/// are what pin the wire names; the [JsonPropertyName] attributes make each
/// name survive a C# property rename, but removing an attribute is not
/// detectable while the property name still camel-cases to the same value.
/// </summary>
internal static class ContractJson
{
    // Web defaults include NumberHandling.AllowReadingFromString: the server
    // is a tolerant reader (accepts "4211" for a number), while the canonical
    // wire form pinned by the serialize-side golden tests is always unquoted.
    // The portal's Zod schemas validate server *responses*, which only ever
    // carry the canonical form.
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

    /// <summary>
    /// Pins the serialized wire shape of <paramref name="value"/> to the
    /// golden JSON by structural deep equality.
    /// </summary>
    internal static void AssertMatchesGolden<T>(T value, string goldenJson)
    {
        JsonNode? actual = JsonNode.Parse(Serialize(value));
        JsonNode? expected = JsonNode.Parse(goldenJson);

        Assert.That(JsonNode.DeepEquals(actual, expected), Is.True,
            $"Serialized contract drifted from the pinned golden shape. Actual: {actual}");
    }

    /// <summary>
    /// Asserts that removing <paramref name="requiredField"/> from the golden
    /// payload makes deserialization fail.
    /// </summary>
    internal static void AssertMissingRequiredFieldRejected<T>(string goldenJson, string requiredField)
    {
        JsonNode golden = JsonNode.Parse(goldenJson)!;
        golden.AsObject().Remove(requiredField);

        Assert.Throws<JsonException>(() => Deserialize<T>(golden.ToJsonString()));
    }
}
