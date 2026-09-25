using FluentValidation;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;
using NewsflowApi.Presentation.Validations.Extensions;

namespace NewsflowApi.Presentation.Validations.Authentication
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
