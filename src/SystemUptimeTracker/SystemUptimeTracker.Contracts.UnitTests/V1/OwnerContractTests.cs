using SystemUptimeTracker.Contracts.V1;
using SystemUptimeTracker.Contracts.V1.DeviceAccounts;
using SystemUptimeTracker.Contracts.V1.Machines;
using SystemUptimeTracker.Contracts.V1.Sessions;

namespace SystemUptimeTracker.Contracts.UnitTests.V1;

[TestFixture(Category = "Unit")]
public class OwnerContractTests
{
    private const string GOLDEN_PAGED_DEVICE_ACCOUNTS_JSON = """
        {
          "items": [
            {
              "deviceAccountId": "5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01",
              "name": "DEV-WORKSTATION-01",
              "allowedAuthenticationMethods": "Jwt",
              "isActive": true,
              "hasApiKey": false,
              "apiKeyCreatedAtUtc": null,
              "apiKeyLastUsedAtUtc": null,
              "machineCount": 1,
              "createdAtUtc": "2026-08-30T12:00:00+00:00"
            }
          ],
          "page": 1,
          "pageSize": 50,
          "totalItemCount": 1
        }
        """;

    [Test]
    public void PaginationDefaults_AreBoundedAsDecided()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PaginationDefaults.DefaultPageSize, Is.EqualTo(50));
            Assert.That(PaginationDefaults.MaxPageSize, Is.EqualTo(200));
            Assert.That(PaginationDefaults.FirstPage, Is.EqualTo(1));
            Assert.That(PaginationDefaults.DefaultPageSize, Is.LessThanOrEqualTo(PaginationDefaults.MaxPageSize));
        });
    }

    [Test]
    public void PagedDeviceAccounts_GoldenJson_RoundTrips()
    {
        PagedResponse<DeviceAccountSummary> page =
            ContractJson.Deserialize<PagedResponse<DeviceAccountSummary>>(GOLDEN_PAGED_DEVICE_ACCOUNTS_JSON);

        Assert.Multiple(() =>
        {
            Assert.That(page.Items, Has.Count.EqualTo(1));
            Assert.That(page.Page, Is.EqualTo(PaginationDefaults.FirstPage));
            Assert.That(page.PageSize, Is.EqualTo(PaginationDefaults.DefaultPageSize));
            Assert.That(page.TotalItemCount, Is.EqualTo(1));
            Assert.That(page.Items[0].DeviceAccountId, Is.EqualTo(Guid.Parse("5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01")));
            Assert.That(page.Items[0].Name, Is.EqualTo("DEV-WORKSTATION-01"));
            Assert.That(page.Items[0].AllowedAuthenticationMethods, Is.EqualTo(AllowedAuthenticationMethods.Jwt));
            Assert.That(page.Items[0].IsActive, Is.True);
            Assert.That(page.Items[0].HasApiKey, Is.False);
            Assert.That(page.Items[0].ApiKeyCreatedAtUtc, Is.Null);
            Assert.That(page.Items[0].MachineCount, Is.EqualTo(1));
        });
    }

    [Test]
    public void AllowedAuthenticationMethods_SerializeAsStableStrings()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ContractJson.Serialize(AllowedAuthenticationMethods.Jwt), Is.EqualTo("\"Jwt\""));
            Assert.That(ContractJson.Serialize(AllowedAuthenticationMethods.ApiKey), Is.EqualTo("\"ApiKey\""));
            Assert.That(ContractJson.Serialize(AllowedAuthenticationMethods.Both), Is.EqualTo("\"Both\""));
        });
    }

    [Test]
    public void MachineSummary_PreCreatedMachine_AllowsNullAgentFields()
    {
        const string json = """
            {
              "machineId": "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
              "agentId": null,
              "machineName": "PLANNED-SERVER-02",
              "operatingSystem": null,
              "operatingSystemVersion": null,
              "architecture": null,
              "agentVersion": null,
              "registrationStatus": "Active",
              "firstSeenAtUtc": null,
              "lastSeenAtUtc": null,
              "deviceAccountId": "5f0c4c1f-8f52-4f0f-a6b7-3b1de111aa01"
            }
            """;

        MachineSummary machine = ContractJson.Deserialize<MachineSummary>(json);

        Assert.Multiple(() =>
        {
            Assert.That(machine.AgentId, Is.Null);
            Assert.That(machine.OperatingSystem, Is.Null);
            Assert.That(machine.FirstSeenAtUtc, Is.Null);
            Assert.That(machine.RegistrationStatus, Is.EqualTo(RegistrationStatus.Active));
            Assert.That(machine.DeviceAccountId, Is.Not.Null);
        });
    }

    [Test]
    public void RuntimeSessionSummary_RunningSession_RoundTrips()
    {
        const string goldenJson = """
            {
              "runtimeSessionId": "0d9e6a11-1b8e-4d3c-9f7b-6a5d4c3b2a10",
              "machineId": "7f1d2f7e-4a83-4a5f-9f6a-2f4f0f1b2c3d",
              "startedAtUtc": "2026-08-30T06:00:00+00:00",
              "lastHeartbeatAtUtc": "2026-08-30T12:34:00+00:00",
              "endedAtUtc": null,
              "endReason": "Running",
              "heartbeatCount": 394,
              "calculatedUptimeSeconds": 23640
            }
            """;

        RuntimeSessionSummary session =
            ContractJson.Deserialize<RuntimeSessionSummary>(goldenJson);

        Assert.Multiple(() =>
        {
            Assert.That(session.EndedAtUtc, Is.Null);
            Assert.That(session.EndReason, Is.EqualTo(SessionEndReason.Running));
            Assert.That(session.HeartbeatCount, Is.EqualTo(394));
            Assert.That(session.CalculatedUptimeSeconds, Is.EqualTo(23640));
        });

        ContractJson.AssertMatchesGolden(session, goldenJson);
    }

    [Test]
    public void SessionEndReason_SerializesAsStableStrings()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ContractJson.Serialize(SessionEndReason.Running), Is.EqualTo("\"Running\""));
            Assert.That(ContractJson.Serialize(SessionEndReason.GracefulShutdown), Is.EqualTo("\"GracefulShutdown\""));
            Assert.That(ContractJson.Serialize(SessionEndReason.ServiceStopped), Is.EqualTo("\"ServiceStopped\""));
            Assert.That(ContractJson.Serialize(SessionEndReason.SleepOrHibernate), Is.EqualTo("\"SleepOrHibernate\""));
            Assert.That(ContractJson.Serialize(SessionEndReason.HeartbeatTimeout), Is.EqualTo("\"HeartbeatTimeout\""));
            Assert.That(ContractJson.Serialize(SessionEndReason.AgentRestart), Is.EqualTo("\"AgentRestart\""));
            Assert.That(ContractJson.Serialize(SessionEndReason.MachineReboot), Is.EqualTo("\"MachineReboot\""));
            Assert.That(ContractJson.Serialize(SessionEndReason.Unknown), Is.EqualTo("\"Unknown\""));
        });
    }
}
