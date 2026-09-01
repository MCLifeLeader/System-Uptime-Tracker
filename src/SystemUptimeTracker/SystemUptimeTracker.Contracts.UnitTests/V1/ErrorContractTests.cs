using SystemUptimeTracker.Contracts.V1;

namespace SystemUptimeTracker.Contracts.UnitTests.V1;

/// <summary>
/// Pins the error/correlation conventions decided by TASK-0208. The values
/// must match the API's RequestTraceContext behavior, which is covered by
/// the API-side middleware and handler tests.
/// </summary>
[TestFixture(Category = "Unit")]
public class ErrorContractTests
{
    [Test]
    public void ErrorContract_PinsTraceAndProblemDetailsConventions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ErrorContract.TraceIdHeaderName, Is.EqualTo("X-Trace-Id"));
            Assert.That(ErrorContract.TraceIdExtensionKey, Is.EqualTo("traceId"));
            Assert.That(ErrorContract.RequestIdExtensionKey, Is.EqualTo("requestId"));
            Assert.That(ErrorContract.ProblemContentType, Is.EqualTo("application/problem+json"));
            Assert.That(ErrorContract.UnsupportedPayloadVersionType,
                Is.EqualTo("urn:systemuptimetracker:error:unsupported-payload-version"));
        });
    }
}
