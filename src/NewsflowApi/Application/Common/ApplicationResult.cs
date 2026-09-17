namespace NewsflowApi.Application.Common
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

        public static ApplicationResult Failure(
            string errorCode,
            string errorMessage,
            ApplicationErrorType errorType)
        {
            return new ApplicationResult
            {
                Succeeded = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                ErrorType = errorType
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

        public new static ApplicationResult<T> Failure(
            string errorCode,
            string errorMessage,
            ApplicationErrorType errorType)
        {
            return new ApplicationResult<T>
            {
                Succeeded = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                ErrorType = errorType
            };
        }
    }
}
