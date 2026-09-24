using NewsflowApi.Application.Common.Application;

namespace NewsflowApi.Application.Common.Errors
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
