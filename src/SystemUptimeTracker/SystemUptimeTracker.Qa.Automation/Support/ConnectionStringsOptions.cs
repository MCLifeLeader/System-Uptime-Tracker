namespace SystemUptimeTracker.Qa.Automation.Support;

public sealed class ConnectionStringsOptions
{
    public const string SECTION_NAME = "ConnectionStrings";

    public string DefaultConnection { get; set; } = string.Empty;
}