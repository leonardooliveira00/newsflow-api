namespace NewsflowApi.Application.Common.Application
{
    public class ApplicationResult
    {
        public bool Succeeded { get; init; }

        public string? ErrorCode { get; init; }

        public string? ErrorMessage { get; init; }

        public ApplicationErrorType? ErrorType { get; init; }

        public static ApplicationResult Success()
        {
            return new ApplicationResult
            {
                Succeeded = true
            };
        }

        public static ApplicationResult Failure(ApplicationError error)
        {
            return new ApplicationResult
            {
                Succeeded = false,
                ErrorCode = error.Code,
                ErrorMessage = error.Message,
                ErrorType = error.Type
            };
        }
    }

    public class ApplicationResult<T> : ApplicationResult
    {
        public T? Data { get; init; }

        public static ApplicationResult<T> Success(T data)
        {
            return new ApplicationResult<T>
            {
                Succeeded = true,
                Data = data
            };
        }

        public new static ApplicationResult<T> Failure(ApplicationError error)
        {
            return new ApplicationResult<T>
            {
                Succeeded = false,
                ErrorCode = error.Code,
                ErrorMessage = error.Message,
                ErrorType = error.Type
            };
        }
    }
}
