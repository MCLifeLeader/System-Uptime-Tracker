namespace SystemUptimeTracker.Contracts.V1;

/// <summary>
/// Wire-level error and correlation conventions for the /api/v1 surface
/// (TASK-0208). Every non-2xx response is an RFC 9457 Problem Details
/// payload (application/problem+json) enriched with the extension keys
/// below, and every response carries the trace header.
/// </summary>
public static class ErrorContract
{
    /// <summary>
    /// Response header carrying the W3C trace identifier on every response,
    /// success or failure. Callers propagate context inbound with the
    /// standard <c>traceparent</c> header.
    /// </summary>
    public const string TraceIdHeaderName = "X-Trace-Id";

    /// <summary>
    /// Problem Details extension key holding the W3C trace identifier.
    /// </summary>
    public const string TraceIdExtensionKey = "traceId";

    /// <summary>
    /// Problem Details extension key holding the server request identifier.
    /// </summary>
    public const string RequestIdExtensionKey = "requestId";

    /// <summary>
    /// Problem Details <c>type</c> for a request whose <c>payloadVersion</c>
    /// is not supported; returned with HTTP 422.
    /// </summary>
    public const string UnsupportedPayloadVersionType =
        "urn:systemuptimetracker:error:unsupported-payload-version";

    /// <summary>
    /// The media type of every v1 error response.
    /// </summary>
    public const string ProblemContentType = "application/problem+json";
}
