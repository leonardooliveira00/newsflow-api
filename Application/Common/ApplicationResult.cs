namespace NewsflowApi.Application.Common
{
    public class ApplicationResult<T>
    {
        public bool Succeeded { get; init; }

        public T? Data { get; init; }

        public string? ErrorCode { get; init; }

        public string? ErrorMessage { get; init; }

        public static ApplicationResult<T> Success(T data)
        {
            return new ApplicationResult<T>
            {
                Succeeded = true,
                Data = data
            };
        }

        public static ApplicationResult<T> Failure(string errorCode, string errorMessage)
        {
            return new ApplicationResult<T>
            {
                Succeeded = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }

    public class ApplicationResult
    {
        public bool Succeeded { get; init; }

        public string? ErrorCode { get; init; }

        public string? ErrorMessage { get; init; }

        public static ApplicationResult Success()
        {
            return new ApplicationResult
            {
                Succeeded = true
            };
        }

        public static ApplicationResult Failure(
            string errorCode,
            string errorMessage)
        {
            return new ApplicationResult
            {
                Succeeded = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }
}
