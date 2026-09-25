using FluentValidation;
using NewsflowApi.Presentation.Dtos.Requests.Staffs;
using NewsflowApi.Presentation.Validations.Extensions;

namespace NewsflowApi.Presentation.Validations.Staffs
{
    public sealed class RegisterStaffRequestValidator : AbstractValidator<RegisterStaffRequest>
    {
        public RegisterStaffRequestValidator()
        {
            RuleFor(staff => staff.FirstName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MinimumLength(2).WithMessage("{PropertyName} must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("The field {PropertyName} must be at most 50 characters long.");

            RuleFor(staff => staff.LastName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MinimumLength(2).WithMessage("{PropertyName} must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("{PropertyName} must be at most 50 characters long.");

            RuleFor(staff => staff.Email).ValidEmail();

            RuleFor(staff => staff.ContactPhone)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(20).WithMessage("{PropertyName} must be at most 20 characters long.");

            RuleFor(staff => staff.Bio)
                .MaximumLength(255).WithMessage("{PropertyName} must be at most 255 characters long.");
        }
    }
}
