namespace NewsflowApi.Application.Common
{
    public sealed class KeysetPagedResponse<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public string? NextCursor { get; init; }
        public bool HasMore { get; init; }
    }
}
