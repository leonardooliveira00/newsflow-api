using FluentValidation;
using WavensApi.Presentation.Dtos.Requests.Authentication;
using WavensApi.Presentation.Validations.Extensions;

namespace WavensApi.Presentation.Validations.Authentication
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
