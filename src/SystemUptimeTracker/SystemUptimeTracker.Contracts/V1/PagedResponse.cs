using System.Text.Json.Serialization;

namespace SystemUptimeTracker.Contracts.V1;

/// <summary>
/// Bounded page envelope returned by every v1 list endpoint (TASK-0205).
/// Ordering is deterministic per endpoint and documented in
/// docs/api-contracts.md.
/// </summary>
public sealed class PagedResponse<TItem>
{
    /// <summary>
    /// The items on this page, in the endpoint's documented order.
    /// </summary>
    [JsonPropertyName("items")]
    public required IReadOnlyList<TItem> Items { get; init; }

    /// <summary>
    /// The 1-based page number that was returned.
    /// </summary>
    [JsonPropertyName("page")]
    public required int Page { get; init; }

    /// <summary>
    /// The page size that was applied.
    /// </summary>
    [JsonPropertyName("pageSize")]
    public required int PageSize { get; init; }

    /// <summary>
    /// Total matching items across all pages.
    /// </summary>
    [JsonPropertyName("totalItemCount")]
    public required long TotalItemCount { get; init; }
}
