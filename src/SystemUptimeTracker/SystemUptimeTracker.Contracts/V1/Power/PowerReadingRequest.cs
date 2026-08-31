using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1.Power;

/// <summary>
/// Request body for <c>POST /api/v1/power-readings</c> (TASK-0206) — the one
/// canonical power-reading command every ingestion path normalizes into
/// (TASK-0007). The idempotency key is the meter identity
/// (<see cref="Vendor"/> + <see cref="ExternalDeviceId"/>) plus
/// <see cref="MessageId"/> (TASK-0207). Measured power belongs to the meter:
/// this payload intentionally carries no machine or monitored-device fields.
/// </summary>
public sealed class PowerReadingRequest
{
    /// <summary>
    /// Contract payload version; see <see cref="PayloadVersions"/>.
    /// </summary>
    [JsonPropertyName("payloadVersion")]
    public required int PayloadVersion { get; init; }

    /// <summary>
    /// Vendor name of the registered meter.
    /// </summary>
    [JsonPropertyName("vendor")]
    public required string Vendor { get; init; }

    /// <summary>
    /// Vendor-specific durable device identifier of the registered meter.
    /// </summary>
    [JsonPropertyName("externalDeviceId")]
    public required string ExternalDeviceId { get; init; }

    /// <summary>
    /// Producer-assigned unique message identifier; with the meter identity
    /// it forms the reading idempotency key.
    /// </summary>
    [JsonPropertyName("messageId")]
    public required Guid MessageId { get; init; }

    /// <summary>
    /// When the meter measured the value (UTC). Preserved for delayed
    /// delivery; the server records its own receipt time separately.
    /// </summary>
    [JsonPropertyName("measuredAtUtc")]
    public required DateTimeOffset MeasuredAtUtc { get; init; }

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
    /// Apparent power in volt-amps.
    /// </summary>
    [JsonPropertyName("apparentPowerVoltAmps")]
    public double? ApparentPowerVoltAmps { get; init; }

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
    /// Cumulative returned energy in watt-hours.
    /// </summary>
    [JsonPropertyName("returnedEnergyWattHours")]
    public double? ReturnedEnergyWattHours { get; init; }

    /// <summary>
    /// Whether the meter's switched output is on.
    /// </summary>
    [JsonPropertyName("outputIsOn")]
    public bool? OutputIsOn { get; init; }

    /// <summary>
    /// Meter-reported device temperature in Celsius.
    /// </summary>
    [JsonPropertyName("deviceTemperatureCelsius")]
    public double? DeviceTemperatureCelsius { get; init; }

    /// <summary>
    /// Optional raw vendor payload. Accepted only under the explicit size,
    /// redaction, and retention policy owned by TASK-1206; oversized or
    /// credential-bearing payloads are rejected.
    /// </summary>
    [JsonPropertyName("rawPayload")]
    public string? RawPayload { get; init; }
}
