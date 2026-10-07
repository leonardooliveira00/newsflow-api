using FluentValidation;
using WavensApi.Presentation.Dtos.Requests.Authentication;
using WavensApi.Presentation.Validations.Extensions;

namespace WavensApi.Presentation.Validations.Authentication
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
