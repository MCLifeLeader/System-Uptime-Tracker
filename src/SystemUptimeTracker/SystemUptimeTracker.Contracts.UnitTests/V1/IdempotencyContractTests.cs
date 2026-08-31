using System.Text.Json.Nodes;
using SystemUptimeTracker.Contracts.V1.Heartbeats;
using SystemUptimeTracker.Contracts.V1.Power;

namespace SystemUptimeTracker.Contracts.UnitTests.V1;

/// <summary>
/// Pins the duplicate-delivery behavior decided by TASK-0207: a duplicate
/// heartbeat (AgentId + SequenceNumber) or reading (meter identity +
/// MessageId) returns the originally persisted identifiers with the
/// duplicate flag set, and never a second persisted record.
/// </summary>
[TestFixture(Category = "Unit")]
public class IdempotencyContractTests
{
    [Test]
    public void HeartbeatDuplicateResponse_GoldenJson_ReturnsOriginalIdentifiers()
    {
        const string goldenDuplicateJson = """
            {
              "heartbeatId": "9c0a95f2-63d4-4b7e-8a3a-52f27cf7f5a1",
              "machineId": "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
              "sequenceNumber": 4211,
              "receivedAtUtc": "2026-07-25T15:30:02+00:00",
              "duplicate": true
            }
            """;

        HeartbeatResponse response =
            ContractJson.Deserialize<HeartbeatResponse>(goldenDuplicateJson);

        Assert.Multiple(() =>
        {
            // Same identifiers as the original delivery: the only wire
            // difference for a retried heartbeat is the duplicate flag.
            Assert.That(response.HeartbeatId, Is.EqualTo(Guid.Parse("9c0a95f2-63d4-4b7e-8a3a-52f27cf7f5a1")));
            Assert.That(response.SequenceNumber, Is.EqualTo(4211));
            Assert.That(response.Duplicate, Is.True);
        });

        JsonNode? actual = JsonNode.Parse(ContractJson.Serialize(response));
        JsonNode? expected = JsonNode.Parse(goldenDuplicateJson);

        Assert.That(JsonNode.DeepEquals(actual, expected), Is.True,
            $"Serialized contract drifted from the pinned golden shape. Actual: {actual}");
    }

    [Test]
    public void PowerReadingDuplicateResponse_GoldenJson_ReturnsOriginalIdentifiers()
    {
        const string goldenDuplicateJson = """
            {
              "powerReadingId": "4d3c2b1a-0f9e-4d8c-b7a6-5e4f3a2b1c0d",
              "powerMeterId": "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
              "messageId": "b6a1f2c3-d4e5-4f60-8a9b-0c1d2e3f4a5b",
              "receivedAtUtc": "2026-08-30T12:00:03+00:00",
              "duplicate": true
            }
            """;

        PowerReadingResponse response =
            ContractJson.Deserialize<PowerReadingResponse>(goldenDuplicateJson);

        Assert.Multiple(() =>
        {
            Assert.That(response.PowerReadingId, Is.EqualTo(Guid.Parse("4d3c2b1a-0f9e-4d8c-b7a6-5e4f3a2b1c0d")));
            Assert.That(response.MessageId, Is.EqualTo(Guid.Parse("b6a1f2c3-d4e5-4f60-8a9b-0c1d2e3f4a5b")));
            Assert.That(response.Duplicate, Is.True);
        });

        JsonNode? actual = JsonNode.Parse(ContractJson.Serialize(response));
        JsonNode? expected = JsonNode.Parse(goldenDuplicateJson);

        Assert.That(JsonNode.DeepEquals(actual, expected), Is.True,
            $"Serialized contract drifted from the pinned golden shape. Actual: {actual}");
    }
}
