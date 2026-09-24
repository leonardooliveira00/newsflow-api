using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Common.Application;

namespace NewsflowApi.Presentation.Extensions.Http
{
    public static class ApplicationResultExtensions
    {
        public static IActionResult ToErrorResult(this ApplicationResult result)
        {
            var body = new
            {
                code = result.ErrorCode,
                message = result.ErrorMessage
            };

            var statusCode = result.ErrorType switch
            {
                ApplicationErrorType.Validation => StatusCodes.Status400BadRequest,
                ApplicationErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ApplicationErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ApplicationErrorType.NotFound => StatusCodes.Status404NotFound,
                ApplicationErrorType.Conflict => StatusCodes.Status409Conflict,
                ApplicationErrorType.Locked => StatusCodes.Status423Locked,

                _ => throw new InvalidOperationException(
                    $"Unsupported application error type: {result.ErrorType}"
                    )
            };

            return new ObjectResult(body)
            {
                StatusCode = statusCode
            };
        }
    }
}
