using System.Text.Json;
using System.Text.Json.Nodes;
using SystemUptimeTracker.Contracts.V1;
using SystemUptimeTracker.Contracts.V1.Heartbeats;

namespace SystemUptimeTracker.Contracts.UnitTests.V1.Heartbeats;

[TestFixture(Category = "Unit")]
public class HeartbeatContractTests
{
    private const string GOLDEN_REQUEST_JSON = """
        {
          "payloadVersion": 1,
          "agentId": "3a812c1a-9dfd-42e7-97f4-8a47d68971e4",
          "sequenceNumber": 4211,
          "sentAtUtc": "2026-07-25T15:30:00+00:00",
          "agentStartedAtUtc": "2026-07-25T12:10:34+00:00",
          "systemBootTimeUtc": "2026-07-24T18:42:11+00:00",
          "machineName": "BUILD-SERVER-01",
          "operatingSystem": "Ubuntu 24.04.3 LTS",
          "operatingSystemVersion": "24.04.3",
          "architecture": "X64",
          "agentVersion": "1.0.0",
          "processor": {
            "logicalProcessorCount": 16,
            "usagePercent": 14.7
          },
          "memory": {
            "totalBytes": 34359738368,
            "availableBytes": 18253611008
          },
          "storage": [
            {
              "volumeName": "/",
              "fileSystem": "ext4",
              "totalBytes": 1073741824000,
              "availableBytes": 584115552256
            }
          ]
        }
        """;

    private const string GOLDEN_RESPONSE_JSON = """
        {
          "heartbeatId": "9c0a95f2-63d4-4b7e-8a3a-52f27cf7f5a1",
          "machineId": "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
          "sequenceNumber": 4211,
          "receivedAtUtc": "2026-07-25T15:30:02+00:00",
          "duplicate": false
        }
        """;

    [Test]
    public void HeartbeatRequest_GoldenJson_DeserializesEveryField()
    {
        HeartbeatRequest request = ContractJson.Deserialize<HeartbeatRequest>(GOLDEN_REQUEST_JSON);

        Assert.Multiple(() =>
        {
            Assert.That(request.PayloadVersion, Is.EqualTo(PayloadVersions.V1));
            Assert.That(request.AgentId, Is.EqualTo(Guid.Parse("3a812c1a-9dfd-42e7-97f4-8a47d68971e4")));
            Assert.That(request.SequenceNumber, Is.EqualTo(4211));
            Assert.That(request.SentAtUtc, Is.EqualTo(DateTimeOffset.Parse("2026-07-25T15:30:00Z")));
            Assert.That(request.AgentStartedAtUtc, Is.EqualTo(DateTimeOffset.Parse("2026-07-25T12:10:34Z")));
            Assert.That(request.SystemBootTimeUtc, Is.EqualTo(DateTimeOffset.Parse("2026-07-24T18:42:11Z")));
            Assert.That(request.MachineName, Is.EqualTo("BUILD-SERVER-01"));
            Assert.That(request.OperatingSystem, Is.EqualTo("Ubuntu 24.04.3 LTS"));
            Assert.That(request.OperatingSystemVersion, Is.EqualTo("24.04.3"));
            Assert.That(request.Architecture, Is.EqualTo("X64"));
            Assert.That(request.AgentVersion, Is.EqualTo("1.0.0"));
            Assert.That(request.Processor.LogicalProcessorCount, Is.EqualTo(16));
            Assert.That(request.Processor.UsagePercent, Is.EqualTo(14.7));
            Assert.That(request.Memory.TotalBytes, Is.EqualTo(34359738368));
            Assert.That(request.Memory.AvailableBytes, Is.EqualTo(18253611008));
            Assert.That(request.Storage, Has.Count.EqualTo(1));
            Assert.That(request.Storage![0].VolumeName, Is.EqualTo("/"));
            Assert.That(request.Storage[0].FileSystem, Is.EqualTo("ext4"));
            Assert.That(request.Storage[0].TotalBytes, Is.EqualTo(1073741824000));
            Assert.That(request.Storage[0].AvailableBytes, Is.EqualTo(584115552256));
        });
    }

    [Test]
    public void HeartbeatRequest_SerializedShape_MatchesPinnedFieldNames()
    {
        var request = new HeartbeatRequest
        {
            PayloadVersion = PayloadVersions.V1,
            AgentId = Guid.Parse("3a812c1a-9dfd-42e7-97f4-8a47d68971e4"),
            SequenceNumber = 4211,
            SentAtUtc = DateTimeOffset.Parse("2026-07-25T15:30:00Z"),
            AgentStartedAtUtc = DateTimeOffset.Parse("2026-07-25T12:10:34Z"),
            SystemBootTimeUtc = DateTimeOffset.Parse("2026-07-24T18:42:11Z"),
            MachineName = "BUILD-SERVER-01",
            OperatingSystem = "Ubuntu 24.04.3 LTS",
            OperatingSystemVersion = "24.04.3",
            Architecture = "X64",
            AgentVersion = "1.0.0",
            Processor = new ProcessorTelemetry
            {
                LogicalProcessorCount = 16,
                UsagePercent = 14.7,
            },
            Memory = new MemoryTelemetry
            {
                TotalBytes = 34359738368,
                AvailableBytes = 18253611008,
            },
            Storage =
            [
                new StorageVolumeTelemetry
                {
                    VolumeName = "/",
                    FileSystem = "ext4",
                    TotalBytes = 1073741824000,
                    AvailableBytes = 584115552256,
                },
            ],
        };

        JsonNode? actual = JsonNode.Parse(ContractJson.Serialize(request));
        JsonNode? expected = JsonNode.Parse(GOLDEN_REQUEST_JSON);

        Assert.That(JsonNode.DeepEquals(actual, expected), Is.True,
            $"Serialized contract drifted from the pinned golden shape. Actual: {actual}");
    }

    [Test]
    public void HeartbeatRequest_WithoutStorageSnapshot_Deserializes()
    {
        JsonNode golden = JsonNode.Parse(GOLDEN_REQUEST_JSON)!;
        golden.AsObject().Remove("storage");

        HeartbeatRequest request =
            ContractJson.Deserialize<HeartbeatRequest>(golden.ToJsonString());

        Assert.That(request.Storage, Is.Null);
    }

    [TestCase("payloadVersion")]
    [TestCase("agentId")]
    [TestCase("sequenceNumber")]
    [TestCase("sentAtUtc")]
    [TestCase("agentStartedAtUtc")]
    [TestCase("systemBootTimeUtc")]
    [TestCase("machineName")]
    [TestCase("operatingSystem")]
    [TestCase("architecture")]
    [TestCase("agentVersion")]
    [TestCase("processor")]
    [TestCase("memory")]
    public void HeartbeatRequest_MissingRequiredField_IsRejected(string requiredField)
    {
        JsonNode golden = JsonNode.Parse(GOLDEN_REQUEST_JSON)!;
        golden.AsObject().Remove(requiredField);

        Assert.Throws<JsonException>(
            () => ContractJson.Deserialize<HeartbeatRequest>(golden.ToJsonString()));
    }

    [TestCase("sequenceNumber", "\"not-a-number\"")]
    [TestCase("sentAtUtc", "\"not-a-timestamp\"")]
    [TestCase("agentId", "\"not-a-guid\"")]
    public void HeartbeatRequest_InvalidFieldValue_IsRejected(string field, string invalidJsonValue)
    {
        JsonNode golden = JsonNode.Parse(GOLDEN_REQUEST_JSON)!;
        golden.AsObject()[field] = JsonNode.Parse(invalidJsonValue);

        Assert.Throws<JsonException>(
            () => ContractJson.Deserialize<HeartbeatRequest>(golden.ToJsonString()));
    }

    [Test]
    public void HeartbeatResponse_GoldenJson_RoundTrips()
    {
        HeartbeatResponse response =
            ContractJson.Deserialize<HeartbeatResponse>(GOLDEN_RESPONSE_JSON);

        Assert.Multiple(() =>
        {
            Assert.That(response.HeartbeatId, Is.EqualTo(Guid.Parse("9c0a95f2-63d4-4b7e-8a3a-52f27cf7f5a1")));
            Assert.That(response.MachineId, Is.EqualTo(Guid.Parse("7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d")));
            Assert.That(response.SequenceNumber, Is.EqualTo(4211));
            Assert.That(response.ReceivedAtUtc, Is.EqualTo(DateTimeOffset.Parse("2026-07-25T15:30:02Z")));
            Assert.That(response.Duplicate, Is.False);
        });

        JsonNode? actual = JsonNode.Parse(ContractJson.Serialize(response));
        JsonNode? expected = JsonNode.Parse(GOLDEN_RESPONSE_JSON);

        Assert.That(JsonNode.DeepEquals(actual, expected), Is.True,
            $"Serialized contract drifted from the pinned golden shape. Actual: {actual}");
    }
}
