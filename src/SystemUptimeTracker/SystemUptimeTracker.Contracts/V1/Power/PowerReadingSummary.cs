using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Power;

/// <summary>
/// Owner-facing view of a persisted reading for
/// <c>GET /api/v1/power-meters/{id}/readings</c> (TASK-0206).
/// </summary>
public sealed class PowerReadingSummary
{
    /// <summary>
    /// The persisted reading identifier.
    /// </summary>
    [JsonPropertyName("powerReadingId")]
    public required Guid PowerReadingId { get; init; }

    /// <summary>
    /// The meter the reading belongs to.
    /// </summary>
    [JsonPropertyName("powerMeterId")]
    public required Guid PowerMeterId { get; init; }

    /// <summary>
    /// Producer-assigned message identifier.
    /// </summary>
    [JsonPropertyName("messageId")]
    public required Guid MessageId { get; init; }

    /// <summary>
    /// When the meter measured the value (UTC).
    /// </summary>
    [JsonPropertyName("measuredAtUtc")]
    public required DateTimeOffset MeasuredAtUtc { get; init; }

    /// <summary>
    /// Server-authoritative receipt time (UTC).
    /// </summary>
    [JsonPropertyName("receivedAtUtc")]
    public required DateTimeOffset ReceivedAtUtc { get; init; }

    /// <summary>
    /// Active (real) power in watts.
    /// </summary>
    [JsonPropertyName("activePowerWatts")]
    public required double ActivePowerWatts { get; init; }

    /// <summary>
    /// RMS voltage in volts.
    /// </summary>
    [JsonPropertyName("voltage")]
    public double? Voltage { get; init; }

    /// <summary>
    /// RMS current in amps.
    /// </summary>
    [JsonPropertyName("currentAmps")]
    public double? CurrentAmps { get; init; }

    /// <summary>
    /// Power factor in the range -1 to 1.
    /// </summary>
    [JsonPropertyName("powerFactor")]
    public double? PowerFactor { get; init; }

    /// <summary>
    /// Line frequency in hertz.
    /// </summary>
    [JsonPropertyName("frequencyHz")]
    public double? FrequencyHz { get; init; }

    /// <summary>
    /// Cumulative consumed energy in watt-hours.
    /// </summary>
    [JsonPropertyName("totalEnergyWattHours")]
    public double? TotalEnergyWattHours { get; init; }

    /// <summary>
    /// Whether the meter's switched output was on.
    /// </summary>
    [JsonPropertyName("outputIsOn")]
    public bool? OutputIsOn { get; init; }

    /// <summary>
    /// Meter-reported device temperature in Celsius.
    /// </summary>
    [JsonPropertyName("deviceTemperatureCelsius")]
    public double? DeviceTemperatureCelsius { get; init; }
}
