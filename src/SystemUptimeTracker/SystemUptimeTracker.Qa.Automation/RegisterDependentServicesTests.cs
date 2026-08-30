using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SystemUptimeTracker.Qa.Automation.Configuration;
using SystemUptimeTracker.Qa.Automation.Support;

namespace SystemUptimeTracker.Qa.Automation;

[TestFixture]
public sealed class RegisterDependentServicesTests
{
    private const string QA_DATABASE_CONNECTION_STRING =
        "Server=127.0.0.1,10433;Database=SystemUptimeTracker_QaAutomation;User Id=sa;Password=P@ssword123!;Encrypt=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    private const string MAIN_DATABASE_CONNECTION_STRING =
        "Server=127.0.0.1,10433;Database=SystemUptimeTracker;User Id=sa;Password=P@ssword123!;Encrypt=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    [Test]
    public void ResolveRuntimeAutomationDatabaseConnectionString_WhenDefaultConnectionIsPlaceholder_FallsBackToQaConnectionString()
    {
        IConfiguration configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Replace-Key-From-Secrets.json",
            ["ConnectionStrings:SharedQaDatabase"] = "__SET_IN_USER_SECRETS_OR_ENV__",
        });

        string resolved = RegisterDependentServices.ResolveRuntimeAutomationDatabaseConnectionString(configuration);

        Assert.That(resolved, Does.Contain("Database=SystemUptimeTracker_QaAutomation"));
    }

    [Test]
    public void ResolveRuntimeAutomationDatabaseConnectionString_WhenDefaultConnectionIsMissing_FallsBackToQaConnectionString()
    {
        IConfiguration configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:SharedQaDatabase"] = QA_DATABASE_CONNECTION_STRING,
        });

        string resolved = RegisterDependentServices.ResolveRuntimeAutomationDatabaseConnectionString(configuration);

        Assert.That(resolved, Is.EqualTo(QA_DATABASE_CONNECTION_STRING));
    }

    [Test]
    public void ResolveRuntimeAutomationDatabaseConnectionString_WhenDefaultConnectionIsConfigured_UsesItVerbatim()
    {
        IConfiguration configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = QA_DATABASE_CONNECTION_STRING,
        });

        string resolved = RegisterDependentServices.ResolveRuntimeAutomationDatabaseConnectionString(configuration);

        Assert.That(resolved, Is.EqualTo(QA_DATABASE_CONNECTION_STRING));
    }

    [Test]
    public void ResolveRuntimeAutomationDatabaseConnectionString_WhenDefaultConnectionTargetsMainDatabase_Throws()
    {
        IConfiguration configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = MAIN_DATABASE_CONNECTION_STRING,
        });

        Assert.That(
            () => RegisterDependentServices.ResolveRuntimeAutomationDatabaseConnectionString(configuration),
            Throws.InvalidOperationException.With.Message.Contain("must not run against the main SystemUptimeTracker database"));
    }

    [Test]
    public void ResolveRuntimeAutomationDatabaseConnectionString_WhenMainDatabaseIsExplicitlyAllowed_UsesDefaultConnection()
    {
        IConfiguration configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = MAIN_DATABASE_CONNECTION_STRING,
            ["QaAutomation:AllowMainDatabase"] = "true",
        });

        string resolved = RegisterDependentServices.ResolveRuntimeAutomationDatabaseConnectionString(configuration);

        Assert.That(resolved, Is.EqualTo(MAIN_DATABASE_CONNECTION_STRING));
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    [Test]
    public void RegisterQaAutomationServices_WhenUsingDefaultConfiguration_BindsStableLocalBaseUrls()
    {
        ServiceProvider services = new ServiceCollection()
            .RegisterQaAutomationServices(environmentName: "Development")
            .BuildServiceProvider(validateScopes: true);

        try
        {
            AutomationAppSettings appSettings = services.GetRequiredService<IOptions<AutomationAppSettings>>().Value;
            SystemUptimeTrackerWebValidationOptions webValidation =
                services.GetRequiredService<IOptions<SystemUptimeTrackerWebValidationOptions>>().Value;

            Assert.Multiple(() =>
            {
                Assert.That(appSettings.BaseUrl, Is.EqualTo("https://localhost:7060/"));
                Assert.That(webValidation.BaseUrl, Is.EqualTo("https://localhost:3001"));
            });
        }
        finally
        {
            services.Dispose();
        }
    }
}
