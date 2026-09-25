using FluentValidation;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;
using NewsflowApi.Presentation.Validations.Extensions;

namespace NewsflowApi.Presentation.Validations.Authentication
{
    public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(user => user.CurrentPassword)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(user => user.NewPassword)
                .StrongPassword()
                .NotEqual(user => user.CurrentPassword).WithMessage("The new password must be different from the current password.");
        }
    }
}
