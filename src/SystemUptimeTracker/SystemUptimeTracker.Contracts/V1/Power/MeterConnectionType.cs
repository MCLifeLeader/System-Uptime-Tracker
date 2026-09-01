using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Power;

/// <summary>
/// How a power meter's readings reach the platform (TASK-0206). Only
/// AgentPolling is active in the first release (TASK-0007); the remaining
/// values are accepted for registration metadata but have no ingestion path
/// until the EPIC-15 evaluation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<MeterConnectionType>))]
public enum MeterConnectionType
{
    /// <summary>
    /// A reporting machine's agent polls the meter locally.
    /// </summary>
    AgentPolling,

    /// <summary>
    /// MQTT broker delivery (deferred; TASK-1505).
    /// </summary>
    Mqtt,

    /// <summary>
    /// WebSocket delivery (deferred; TASK-1505).
    /// </summary>
    WebSocket,

    /// <summary>
    /// Direct webhook delivery (deferred; TASK-1505).
    /// </summary>
    Webhook,

    /// <summary>
    /// Vendor cloud integration (deferred; TASK-1505).
    /// </summary>
    ShellyCloud,
}
