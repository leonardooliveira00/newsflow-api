using FluentValidation;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;
using NewsflowApi.Presentation.Validations.Extensions;

namespace NewsflowApi.Presentation.Validations.Authentication
{
    public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(user => user.Email).ValidEmail();

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }
}
