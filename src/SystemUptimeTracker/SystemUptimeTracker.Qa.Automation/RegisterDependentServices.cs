using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SystemUptimeTracker.Api.Services.Identity;
using SystemUptimeTracker.Data.Identity;
using SystemUptimeTracker.Qa.Automation.Configuration;
using SystemUptimeTracker.Qa.Automation.Pages;
using SystemUptimeTracker.Qa.Automation.Services;
using SystemUptimeTracker.Qa.Automation.Support;

namespace SystemUptimeTracker.Qa.Automation;

public static class RegisterDependentServices
{
    private const string DEFAULT_CONNECTION_PLACEHOLDER = "Replace-Key-From-Secrets.json";
    private const string MAIN_APPLICATION_DATABASE_NAME = "SystemUptimeTracker";

    public static IServiceCollection RegisterQaAutomationServices(
        this IServiceCollection services,
        string environmentName)
    {
        ArgumentNullException.ThrowIfNull(services);

        IConfiguration configuration = RegisterConfiguration(environmentName);

        services.AddSingleton(configuration);
        services.AddSingleton<IHostEnvironment>(_ => new QaAutomationHostEnvironment(environmentName));
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.AddConsole();
        });

        RegisterOptions(services, configuration);
        RegisterFrameworkServices(services);
        RegisterIdentityAutomationServices(services);
        RegisterApplicationServices(services);
        RegisterPageObjects(services);

        return services;
    }

    private static IConfiguration RegisterConfiguration(string environmentName)
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
            .AddUserSecrets(typeof(SystemUptimeTracker.Api.RegisterDependentServices).Assembly, optional: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    internal static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AutomationAppSettings>()
            .Bind(configuration.GetSection("AppSettings"))
            .ValidateOnStart();

        services.AddOptions<QaAutomationExecutionOptions>()
            .Bind(configuration.GetSection(QaAutomationExecutionOptions.SECTION_NAME))
            .Validate(
                options => !options.UseExternalHost || !string.IsNullOrWhiteSpace(options.WebBaseUrl),
                "QaAutomation:WebBaseUrl is required when QaAutomation:UseExternalHost is true.")
            .ValidateOnStart();

        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SECTION_NAME))
            .ValidateOnStart();

        services.AddOptions<SystemUptimeTrackerWebValidationOptions>()
            .Bind(configuration.GetSection(SystemUptimeTrackerWebValidationOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.BaseUrl),
                "TestConfiguration:WebValidation:BaseUrl is required.")
            .ValidateOnStart();
    }

    private static void RegisterFrameworkServices(IServiceCollection services)
    {
        services.AddSingleton<IApiClientFactory, ApiClientFactory>();
        services.AddSingleton<IPlaywrightBrowserEnvironment, PlaywrightBrowserEnvironment>();
        services.AddSingleton<IPlaywrightBrowserFactory, PlaywrightBrowserFactory>();
        services.AddSingleton<IPlaywrightPageSessionFactory, PlaywrightPageSessionFactory>();
        services.AddSingleton<IPageObjectFactory, PageObjectFactory>();
    }

    private static void RegisterIdentityAutomationServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            ConnectionStringsOptions connectionStrings =
                serviceProvider.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;
            QaAutomationExecutionOptions qaAutomation =
                serviceProvider.GetRequiredService<IOptions<QaAutomationExecutionOptions>>().Value;
            string connectionString = ResolveRuntimeAutomationDatabaseConnectionString(
                connectionStrings,
                qaAutomation);

            options.UseSqlServer(connectionString);
        });

        services.AddHttpContextAccessor();
        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager<ApplicationSignInManager>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityRoleSeeder, IdentityRoleSeeder>();
        services.AddScoped<UserManager<ApplicationUser>, ApplicationUserManager>();
        services.AddScoped<SignInManager<ApplicationUser>, ApplicationSignInManager>();
    }

    internal static string ResolveRuntimeAutomationDatabaseConnectionString(
        ConnectionStringsOptions connectionStrings,
        QaAutomationExecutionOptions qaAutomation)
    {
        string applicationConnectionString = connectionStrings.DefaultConnection;
        if (string.IsNullOrWhiteSpace(applicationConnectionString) ||
            string.Equals(applicationConnectionString, DEFAULT_CONNECTION_PLACEHOLDER, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection must be configured for QA automation using user secrets or an environment variable.");
        }

        ValidateQaDatabaseIsolation(qaAutomation, applicationConnectionString);
        return applicationConnectionString;
    }

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        services.AddSingleton<ISystemUptimeTrackerPageCatalog, SystemUptimeTrackerPageCatalog>();
        services.AddSingleton<ISystemUptimeTrackerApiClient, SystemUptimeTrackerApiClient>();
        services.AddScoped<ITestDatabaseCleanupService, TestDatabaseCleanupService>();
        services.AddScoped<ITestIdentityAccountCleanupService, TestIdentityAccountCleanupService>();
        services.AddScoped<ITestIdentityAccountProvisioningService, TestIdentityAccountProvisioningService>();
    }

    private static void ValidateQaDatabaseIsolation(
        QaAutomationExecutionOptions qaAutomation,
        string connectionString)
    {
        if (qaAutomation.AllowMainDatabase)
        {
            return;
        }

        SqlConnectionStringBuilder connectionStringBuilder = new(connectionString);

        if (!string.Equals(
                connectionStringBuilder.InitialCatalog,
                MAIN_APPLICATION_DATABASE_NAME,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new InvalidOperationException(
            "QA automation must not run against the main SystemUptimeTracker database. " +
            "Set ConnectionStrings:DefaultConnection to a dedicated QA database, or " +
            "set QaAutomation:AllowMainDatabase=true for deliberate local cleanup or debugging.");
    }

    private static void RegisterPageObjects(IServiceCollection services)
    {
        services.AddTransient<SystemUptimeTrackerHomePage>();
        services.AddTransient<SystemUptimeTrackerAdminUsersPage>();
        services.AddTransient<LoginPage>();
    }

    private sealed class QaAutomationHostEnvironment : IHostEnvironment
    {
        public QaAutomationHostEnvironment(string environmentName)
        {
            EnvironmentName = string.IsNullOrWhiteSpace(environmentName) ? Environments.Development : environmentName;
            ApplicationName = typeof(RegisterDependentServices).Assembly.GetName().Name ?? "SystemUptimeTracker.Qa.Automation";
            ContentRootPath = AppContext.BaseDirectory;
            ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
        }

        public string EnvironmentName { get; set; }

        public string ApplicationName { get; set; }

        public string ContentRootPath { get; set; }

        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
