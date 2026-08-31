namespace SystemUptimeTracker.Contracts.V1;

/// <summary>
/// Bounded pagination rules for every v1 list endpoint (TASK-0205). Requests
/// beyond <see cref="MaxPageSize"/> are rejected with 400 rather than
/// silently clamped.
/// </summary>
public static class PaginationDefaults
{
    /// <summary>
    /// Page size applied when the caller does not specify one.
    /// </summary>
    public const int DefaultPageSize = 50;

    /// <summary>
    /// Maximum accepted page size.
    /// </summary>
    public const int MaxPageSize = 200;

    /// <summary>
    /// Pages are 1-based.
    /// </summary>
    public const int FirstPage = 1;
}
