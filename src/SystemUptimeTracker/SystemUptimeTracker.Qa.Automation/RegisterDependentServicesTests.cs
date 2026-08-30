using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SystemUptimeTracker.Qa.Automation.Configuration;
using SystemUptimeTracker.Qa.Automation.Support;

namespace SystemUptimeTracker.Qa.Automation;

[TestFixture]
public sealed class RegisterDependentServicesTests
{
    private const string DEFAULT_DATABASE_CONNECTION_STRING =
        "Server=127.0.0.1,10433;Database=SystemUptimeTracker;User Id=sa;Password=P@ssword123!;Encrypt=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    [Test]
    public void ResolveRuntimeAutomationDatabaseConnectionString_WhenDefaultConnectionIsPlaceholder_Throws()
    {
        ConnectionStringsOptions connectionStrings = new()
        {
            DefaultConnection = "Replace-Key-From-Secrets.json",
        };

        Assert.That(
            () => RegisterDependentServices.ResolveRuntimeAutomationDatabaseConnectionString(
                connectionStrings,
                new QaAutomationExecutionOptions()),
            Throws.InvalidOperationException.With.Message.Contain("ConnectionStrings:DefaultConnection"));
    }

    [Test]
    public void ResolveRuntimeAutomationDatabaseConnectionString_WhenDefaultConnectionIsMissing_Throws()
    {
        Assert.That(
            () => RegisterDependentServices.ResolveRuntimeAutomationDatabaseConnectionString(
                new ConnectionStringsOptions(),
                new QaAutomationExecutionOptions()),
            Throws.InvalidOperationException.With.Message.Contain("ConnectionStrings:DefaultConnection"));
    }

    [Test]
    public void ResolveRuntimeAutomationDatabaseConnectionString_WhenMainDatabaseIsExplicitlyAllowed_UsesDefaultConnection()
    {
        ConnectionStringsOptions connectionStrings = new()
        {
            DefaultConnection = DEFAULT_DATABASE_CONNECTION_STRING,
        };
        QaAutomationExecutionOptions qaAutomation = new() { AllowMainDatabase = true };

        string resolved = RegisterDependentServices.ResolveRuntimeAutomationDatabaseConnectionString(
            connectionStrings,
            qaAutomation);

        Assert.That(resolved, Is.EqualTo(DEFAULT_DATABASE_CONNECTION_STRING));
    }

    [Test]
    public void ResolveRuntimeAutomationDatabaseConnectionString_WhenDefaultConnectionTargetsMainDatabase_Throws()
    {
        ConnectionStringsOptions connectionStrings = new()
        {
            DefaultConnection = DEFAULT_DATABASE_CONNECTION_STRING,
        };

        Assert.That(
            () => RegisterDependentServices.ResolveRuntimeAutomationDatabaseConnectionString(
                connectionStrings,
                new QaAutomationExecutionOptions()),
            Throws.InvalidOperationException.With.Message.Contain("must not run against the main SystemUptimeTracker database"));
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

    [Test]
    public void RegisterOptions_WhenConfigurationSectionsAreProvided_BindsTypedOptions()
    {
        IConfiguration configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = DEFAULT_DATABASE_CONNECTION_STRING,
                    ["QaAutomation:AllowMainDatabase"] = "true",
                })
                .Build();
        ServiceCollection services = new();
        RegisterDependentServices.RegisterOptions(services, configuration);
        using ServiceProvider serviceProvider = services.BuildServiceProvider(validateScopes: true);

        ConnectionStringsOptions connectionStrings =
            serviceProvider.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;
        QaAutomationExecutionOptions qaAutomation =
            serviceProvider.GetRequiredService<IOptions<QaAutomationExecutionOptions>>().Value;

        Assert.Multiple(() =>
        {
            Assert.That(connectionStrings.DefaultConnection, Is.EqualTo(DEFAULT_DATABASE_CONNECTION_STRING));
            Assert.That(qaAutomation.AllowMainDatabase, Is.True);
        });
    }
}
