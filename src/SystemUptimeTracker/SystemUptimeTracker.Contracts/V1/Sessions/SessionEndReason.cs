using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Sessions;

/// <summary>
/// Why a runtime session ended, serialized as a string (TASK-0205; values
/// from the RuntimeSession entity in docs/domain-model.md).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<SessionEndReason>))]
public enum SessionEndReason
{
    /// <summary>
    /// The session is still open.
    /// </summary>
    Running,

    /// <summary>
    /// The machine reported a graceful shutdown.
    /// </summary>
    GracefulShutdown,

    /// <summary>
    /// The agent service was stopped.
    /// </summary>
    ServiceStopped,

    /// <summary>
    /// The machine suspended or hibernated.
    /// </summary>
    SleepOrHibernate,

    /// <summary>
    /// No heartbeat arrived within the session-break threshold.
    /// </summary>
    HeartbeatTimeout,

    /// <summary>
    /// The agent restarted without a reboot.
    /// </summary>
    AgentRestart,

    /// <summary>
    /// The machine rebooted (boot-time evidence changed).
    /// </summary>
    MachineReboot,

    /// <summary>
    /// The end cause could not be classified.
    /// </summary>
    Unknown,
}
