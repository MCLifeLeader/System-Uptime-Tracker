using System.Text.Json;
using System.Text.Json.Nodes;
using SystemUptimeTracker.Contracts.V1;
using SystemUptimeTracker.Contracts.V1.Machines;

namespace SystemUptimeTracker.Contracts.UnitTests.V1.Machines;

[TestFixture(Category = "Unit")]
public class MachineRegistrationContractTests
{
    private const string GOLDEN_REQUEST_JSON = """
        {
          "payloadVersion": 1,
          "agentId": "3a812c1a-9dfd-42e7-97f4-8a47d68971e4",
          "machineName": "BUILD-SERVER-01",
          "operatingSystem": "Ubuntu 24.04.3 LTS",
          "operatingSystemVersion": "24.04.3",
          "architecture": "X64",
          "agentVersion": "1.0.0"
        }
        """;

    private const string GOLDEN_RESPONSE_JSON = """
        {
          "machineId": "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
          "agentId": "3a812c1a-9dfd-42e7-97f4-8a47d68971e4",
          "registrationStatus": "Active",
          "wasCreated": true
        }
        """;

    [Test]
    public void MachineRegistrationRequest_GoldenJson_DeserializesEveryField()
    {
        MachineRegistrationRequest request =
            ContractJson.Deserialize<MachineRegistrationRequest>(GOLDEN_REQUEST_JSON);

        Assert.Multiple(() =>
        {
            Assert.That(request.PayloadVersion, Is.EqualTo(PayloadVersions.V1));
            Assert.That(request.AgentId, Is.EqualTo(Guid.Parse("3a812c1a-9dfd-42e7-97f4-8a47d68971e4")));
            Assert.That(request.MachineName, Is.EqualTo("BUILD-SERVER-01"));
            Assert.That(request.OperatingSystem, Is.EqualTo("Ubuntu 24.04.3 LTS"));
            Assert.That(request.OperatingSystemVersion, Is.EqualTo("24.04.3"));
            Assert.That(request.Architecture, Is.EqualTo("X64"));
            Assert.That(request.AgentVersion, Is.EqualTo("1.0.0"));
        });
    }

    [Test]
    public void MachineRegistrationRequest_SerializedShape_MatchesPinnedFieldNames()
    {
        var request = new MachineRegistrationRequest
        {
            PayloadVersion = PayloadVersions.V1,
            AgentId = Guid.Parse("3a812c1a-9dfd-42e7-97f4-8a47d68971e4"),
            MachineName = "BUILD-SERVER-01",
            OperatingSystem = "Ubuntu 24.04.3 LTS",
            OperatingSystemVersion = "24.04.3",
            Architecture = "X64",
            AgentVersion = "1.0.0",
        };

        ContractJson.AssertMatchesGolden(request, GOLDEN_REQUEST_JSON);
    }

    [Test]
    public void MachineRegistrationRequest_WithoutOptionalOsVersion_Deserializes()
    {
        const string json = """
            {
              "payloadVersion": 1,
              "agentId": "3a812c1a-9dfd-42e7-97f4-8a47d68971e4",
              "machineName": "BUILD-SERVER-01",
              "operatingSystem": "Ubuntu 24.04.3 LTS",
              "architecture": "X64",
              "agentVersion": "1.0.0"
            }
            """;

        MachineRegistrationRequest request =
            ContractJson.Deserialize<MachineRegistrationRequest>(json);

        Assert.That(request.OperatingSystemVersion, Is.Null);
    }

    [TestCase("payloadVersion")]
    [TestCase("agentId")]
    [TestCase("machineName")]
    [TestCase("operatingSystem")]
    [TestCase("architecture")]
    [TestCase("agentVersion")]
    public void MachineRegistrationRequest_MissingRequiredField_IsRejected(string requiredField)
    {
        ContractJson.AssertMissingRequiredFieldRejected<MachineRegistrationRequest>(
            GOLDEN_REQUEST_JSON,
            requiredField);
    }

    [Test]
    public void MachineRegistrationRequest_InvalidAgentId_IsRejected()
    {
        JsonNode golden = JsonNode.Parse(GOLDEN_REQUEST_JSON)!;
        golden.AsObject()["agentId"] = "not-a-guid";

        Assert.Throws<JsonException>(
            () => ContractJson.Deserialize<MachineRegistrationRequest>(golden.ToJsonString()));
    }

    [Test]
    public void MachineRegistrationResponse_GoldenJson_RoundTrips()
    {
        MachineRegistrationResponse response =
            ContractJson.Deserialize<MachineRegistrationResponse>(GOLDEN_RESPONSE_JSON);

        Assert.Multiple(() =>
        {
            Assert.That(response.MachineId, Is.EqualTo(Guid.Parse("7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d")));
            Assert.That(response.AgentId, Is.EqualTo(Guid.Parse("3a812c1a-9dfd-42e7-97f4-8a47d68971e4")));
            Assert.That(response.RegistrationStatus, Is.EqualTo(RegistrationStatus.Active));
            Assert.That(response.WasCreated, Is.True);
        });

        ContractJson.AssertMatchesGolden(response, GOLDEN_RESPONSE_JSON);
    }

    [Test]
    public void RegistrationStatus_SerializesAsStableStrings()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ContractJson.Serialize(RegistrationStatus.Active), Is.EqualTo("\"Active\""));
            Assert.That(ContractJson.Serialize(RegistrationStatus.Disabled), Is.EqualTo("\"Disabled\""));
            Assert.That(ContractJson.Serialize(RegistrationStatus.Retired), Is.EqualTo("\"Retired\""));
            Assert.That(ContractJson.Serialize(RegistrationStatus.Discovered), Is.EqualTo("\"Discovered\""));
            Assert.That(ContractJson.Serialize(RegistrationStatus.PendingApproval), Is.EqualTo("\"PendingApproval\""));
        });
    }

    [Test]
    public void RegistrationStatus_UnknownWireValue_IsRejected()
    {
        Assert.Throws<JsonException>(
            () => ContractJson.Deserialize<RegistrationStatus>("\"Approved\""));
    }
}
