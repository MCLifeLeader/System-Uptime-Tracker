namespace SystemUptimeTracker.Contracts.V1;

/// <summary>
/// Accepted payload versions for the /api/v1 surface. A request carrying an
/// unsupported <c>payloadVersion</c> is rejected with 422
/// (unsupported-payload-version); see docs/api-contracts.md.
/// </summary>
public static class PayloadVersions
{
    /// <summary>
    /// The current (and only) accepted payload version for v1 contracts.
    /// </summary>
    public const int V1 = 1;
}
