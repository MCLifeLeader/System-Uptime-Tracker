# SystemUptimeTracker.Contracts

Versioned wire contracts for the `/api/v1` surface documented in
[docs/api-contracts.md](../../../docs/api-contracts.md).

Rules (decided under TASK-0102):

- This project is the sole owner of API request/response DTOs. Do not
  duplicate DTOs into `Common`, `Api`, or `Data`.
- No ASP.NET Core, EF Core, or other runtime implementation dependencies —
  serialization attributes only, so agents and portal tooling can consume
  contracts without pulling server code.
- Wire field names are pinned with `[JsonPropertyName]` and covered by golden
  JSON tests in `SystemUptimeTracker.Contracts.UnitTests`. Changing a name,
  requiredness, or type is a versioned contract change.
