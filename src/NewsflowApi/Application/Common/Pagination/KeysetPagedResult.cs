namespace NewsflowApi.Application.Common.Pagination
{
    public sealed class KeysetPagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public string? NextCursor { get; init; }
        public bool HasMore { get; init; }
    }

    public sealed record StaffKeysetCursor(DateTime CreatedAt, Guid Id);
}
