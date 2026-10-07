using WavensApi.Application.Common.Application;

namespace WavensApi.Application.Common.Errors
{
    public static class KeysetPaginationErrors
    {
        public static readonly ApplicationError InvalidPageSize = new(
            "invalid_page_size",
            "Invalid page size.",
            ApplicationErrorType.Validation
            );
    }
}
