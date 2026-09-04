using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Common;

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
                ApplicationErrorType.Internal => StatusCodes.Status500InternalServerError,

                _ => StatusCodes.Status500InternalServerError
            };

            return new ObjectResult(body)
            {
                StatusCode = statusCode
            };
        }
    }
}
