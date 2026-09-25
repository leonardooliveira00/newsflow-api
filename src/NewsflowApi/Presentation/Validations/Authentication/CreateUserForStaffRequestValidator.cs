using FluentValidation;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;

namespace NewsflowApi.Presentation.Validations.Authentication
{
    public sealed class CreateUserForStaffRequestValidator : AbstractValidator<CreateUserForStaffRequest>
    {
       public CreateUserForStaffRequestValidator()
        {
            RuleFor(staff => staff.StaffId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(255).WithMessage("{PropertyName} must be at most 255 characters long.")
                .EmailAddress().WithMessage("Invalid {PropertyName} format.");
        }
    }
}
