using FluentValidation;
using WavensApi.Presentation.Dtos.Requests.Authentication;

namespace WavensApi.Presentation.Validations.Authentication
{
    public sealed class RequestPasswordResetRequestValidator : AbstractValidator<RequestPasswordResetRequest>
    {
        public RequestPasswordResetRequestValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .EmailAddress().WithMessage("Invalid {PropertyName} format.");
        }
    }
}
