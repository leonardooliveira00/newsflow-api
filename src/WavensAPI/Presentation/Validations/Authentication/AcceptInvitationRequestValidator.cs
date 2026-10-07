using FluentValidation;
using WavensApi.Presentation.Dtos.Requests.Authentication;
using WavensApi.Presentation.Validations.Extensions;

namespace WavensApi.Presentation.Validations.Authentication
{
    public sealed class AcceptInvitationRequestValidator : AbstractValidator<AcceptInvitationRequest>
    {
        public AcceptInvitationRequestValidator()
        {
            RuleFor(invitation => invitation.UserId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(invitation => invitation.Token)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(user => user.Password).StrongPassword();
        }
    }
}
