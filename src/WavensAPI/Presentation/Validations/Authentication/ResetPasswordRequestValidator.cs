using FluentValidation;
using WavensApi.Presentation.Dtos.Requests.Authentication;
using WavensApi.Presentation.Validations.Extensions;

namespace WavensApi.Presentation.Validations.Authentication
{
    public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .EmailAddress().WithMessage("Invalid {PropertyName} format.");

            RuleFor(token => token.Token)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(user => user.NewPassword).StrongPassword();
        }
    }
}
