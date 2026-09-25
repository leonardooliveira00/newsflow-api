using FluentValidation;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;
using NewsflowApi.Presentation.Validations.Extensions;

namespace NewsflowApi.Presentation.Validations.Authentication
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
