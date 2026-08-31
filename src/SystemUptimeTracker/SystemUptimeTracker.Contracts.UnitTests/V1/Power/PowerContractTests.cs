using System.Text.Json;
using System.Text.Json.Nodes;
using SystemUptimeTracker.Contracts.V1;
using SystemUptimeTracker.Contracts.V1.Associations;
using SystemUptimeTracker.Contracts.V1.Power;

namespace SystemUptimeTracker.Contracts.UnitTests.V1.Power;

[TestFixture(Category = "Unit")]
public class PowerContractTests
{
    private const string GOLDEN_READING_REQUEST_JSON = """
        {
          "payloadVersion": 1,
          "vendor": "Shelly",
          "externalDeviceId": "shellyplugus4-a8032ab12345",
          "messageId": "b6a1f2c3-d4e5-4f60-8a9b-0c1d2e3f4a5b",
          "measuredAtUtc": "2026-08-30T12:00:00+00:00",
          "activePowerWatts": 87.4,
          "voltage": 119.8,
          "currentAmps": 0.74,
          "apparentPowerVoltAmps": 89.1,
          "powerFactor": 0.98,
          "frequencyHz": 60.0,
          "totalEnergyWattHours": 15234.6,
          "returnedEnergyWattHours": 0,
          "outputIsOn": true,
          "deviceTemperatureCelsius": 41.2,
          "rawPayload": null
        }
        """;

    [Test]
    public void PowerReadingRequest_GoldenJson_DeserializesEveryField()
    {
        PowerReadingRequest reading =
            ContractJson.Deserialize<PowerReadingRequest>(GOLDEN_READING_REQUEST_JSON);

        Assert.Multiple(() =>
        {
            Assert.That(reading.PayloadVersion, Is.EqualTo(PayloadVersions.V1));
            Assert.That(reading.Vendor, Is.EqualTo("Shelly"));
            Assert.That(reading.ExternalDeviceId, Is.EqualTo("shellyplugus4-a8032ab12345"));
            Assert.That(reading.MessageId, Is.EqualTo(Guid.Parse("b6a1f2c3-d4e5-4f60-8a9b-0c1d2e3f4a5b")));
            Assert.That(reading.MeasuredAtUtc, Is.EqualTo(DateTimeOffset.Parse("2026-08-30T12:00:00Z")));
            Assert.That(reading.ActivePowerWatts, Is.EqualTo(87.4));
            Assert.That(reading.Voltage, Is.EqualTo(119.8));
            Assert.That(reading.CurrentAmps, Is.EqualTo(0.74));
            Assert.That(reading.PowerFactor, Is.EqualTo(0.98));
            Assert.That(reading.FrequencyHz, Is.EqualTo(60.0));
            Assert.That(reading.TotalEnergyWattHours, Is.EqualTo(15234.6));
            Assert.That(reading.OutputIsOn, Is.True);
            Assert.That(reading.DeviceTemperatureCelsius, Is.EqualTo(41.2));
            Assert.That(reading.RawPayload, Is.Null);
        });
    }

    [Test]
    public void PowerReadingRequest_CarriesNoMachineOrDeviceFields()
    {
        // Measured power belongs to the meter (TASK-0007). The reading wire
        // shape must never grow machine or monitored-device attribution.
        JsonNode golden = JsonNode.Parse(GOLDEN_READING_REQUEST_JSON)!;

        foreach (string forbiddenField in new[] { "machineId", "agentId", "monitoredDeviceId" })
        {
            Assert.That(golden.AsObject().ContainsKey(forbiddenField), Is.False,
                $"Golden reading payload must not contain '{forbiddenField}'.");
        }

        var properties = typeof(PowerReadingRequest).GetProperties().Select(property => property.Name);
        Assert.That(properties, Has.None.AnyOf("MachineId", "AgentId", "MonitoredDeviceId"));
    }

    [TestCase("payloadVersion")]
    [TestCase("vendor")]
    [TestCase("externalDeviceId")]
    [TestCase("messageId")]
    [TestCase("measuredAtUtc")]
    [TestCase("activePowerWatts")]
    public void PowerReadingRequest_MissingRequiredField_IsRejected(string requiredField)
    {
        JsonNode golden = JsonNode.Parse(GOLDEN_READING_REQUEST_JSON)!;
        golden.AsObject().Remove(requiredField);

        Assert.Throws<JsonException>(
            () => ContractJson.Deserialize<PowerReadingRequest>(golden.ToJsonString()));
    }

    [Test]
    public void CreatePowerMeterRequest_GoldenJson_Deserializes()
    {
        CreatePowerMeterRequest request = ContractJson.Deserialize<CreatePowerMeterRequest>("""
            {
              "vendor": "Shelly",
              "externalDeviceId": "shellyplugus4-a8032ab12345",
              "name": "Kitchen Plug",
              "model": "Plug US Gen4",
              "macAddress": "A8:03:2A:B1:23:45",
              "ipAddress": "192.168.1.57",
              "connectionType": "AgentPolling",
              "authenticationReference": "shelly/kitchen-plug"
            }
            """);

        Assert.Multiple(() =>
        {
            Assert.That(request.Vendor, Is.EqualTo("Shelly"));
            Assert.That(request.ExternalDeviceId, Is.EqualTo("shellyplugus4-a8032ab12345"));
            Assert.That(request.ConnectionType, Is.EqualTo(MeterConnectionType.AgentPolling));
            Assert.That(request.AuthenticationReference, Is.EqualTo("shelly/kitchen-plug"));
        });
    }

    [Test]
    public void MachineMeterRelationshipTypes_RepresentAllThreeDecidedKinds()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ContractJson.Serialize(MachineMeterRelationshipType.DedicatedLoad), Is.EqualTo("\"DedicatedLoad\""));
            Assert.That(ContractJson.Serialize(MachineMeterRelationshipType.SharedLoad), Is.EqualTo("\"SharedLoad\""));
            Assert.That(ContractJson.Serialize(MachineMeterRelationshipType.CollectorOnly), Is.EqualTo("\"CollectorOnly\""));
            Assert.That(ContractJson.Serialize(DeviceAssociationType.Dedicated), Is.EqualTo("\"Dedicated\""));
            Assert.That(ContractJson.Serialize(DeviceAssociationType.Shared), Is.EqualTo("\"Shared\""));
        });
    }

    [TestCase("DedicatedLoad")]
    [TestCase("SharedLoad")]
    [TestCase("CollectorOnly")]
    public void MachinePowerMeterAssociation_EachRelationshipKind_RoundTrips(string relationshipType)
    {
        string goldenJson = $$"""
            {
              "machineId": "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
              "powerMeterId": "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
              "relationshipType": "{{relationshipType}}",
              "effectiveFromUtc": "2026-08-30T00:00:00+00:00",
              "isPrimary": true
            }
            """;

        CreateMachinePowerMeterAssociationRequest request =
            ContractJson.Deserialize<CreateMachinePowerMeterAssociationRequest>(goldenJson);

        JsonNode? actual = JsonNode.Parse(ContractJson.Serialize(request));
        JsonNode? expected = JsonNode.Parse(goldenJson);

        Assert.That(JsonNode.DeepEquals(actual, expected), Is.True,
            $"Serialized contract drifted from the pinned golden shape. Actual: {actual}");
    }

    [Test]
    public void PowerMeterDeviceAssociation_SharedWithEstimate_IsLabeledEstimateOnly()
    {
        CreatePowerMeterDeviceAssociationRequest request =
            ContractJson.Deserialize<CreatePowerMeterDeviceAssociationRequest>("""
                {
                  "powerMeterId": "1a2b3c4d-5e6f-4a70-8b91-a2b3c4d5e6f7",
                  "monitoredDeviceId": "9e8d7c6b-5a49-4382-b716-0f1e2d3c4b5a",
                  "associationType": "Shared",
                  "estimatedSharePercent": 35.0,
                  "effectiveFromUtc": "2026-08-30T00:00:00+00:00",
                  "isPrimary": false,
                  "notes": "Monitor and dock share the plug"
                }
                """);

        Assert.Multiple(() =>
        {
            Assert.That(request.AssociationType, Is.EqualTo(DeviceAssociationType.Shared));
            Assert.That(request.EstimatedSharePercent, Is.EqualTo(35.0));
        });

        // The association carries only the estimated share; no measured-power
        // fields may exist on association contracts.
        var properties = typeof(CreatePowerMeterDeviceAssociationRequest)
            .GetProperties()
            .Select(property => property.Name);
        Assert.That(properties, Has.None.AnyOf("ActivePowerWatts", "TotalEnergyWattHours", "Voltage"));
    }
}
