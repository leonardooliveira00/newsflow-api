namespace NewsflowApi.Application.Common.Application
{
    public sealed record ApplicationError(
        string Code,
        string Message,
        ApplicationErrorType Type
        );
}
