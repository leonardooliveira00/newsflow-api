using FluentValidation;
using NewsflowApi.Presentation.Dtos.Requests.Staffs;
using NewsflowApi.Presentation.Validations.Extensions;

namespace NewsflowApi.Presentation.Validations.Staffs
{
    public sealed class UpdateStaffRequestValidator : AbstractValidator<UpdateStaffRequest>
    {
        public UpdateStaffRequestValidator()
        {
            RuleFor(staff => staff.FirstName)
                .MinimumLength(2).WithMessage("{PropertyName} must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("The field {PropertyName} must be at most 50 characters long.");

            RuleFor(staff => staff.LastName)
                .MinimumLength(2).WithMessage("{PropertyName} must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("{PropertyName} must be at most 50 characters long.");

            When(staff => staff.Email != null, () =>
            {
                RuleFor(staff => staff.Email!).ValidEmail();
            });

            When(staff => staff.ContactPhone != null, () =>
            {
                RuleFor(staff => staff.ContactPhone!).ValidContactPhone();
            });

            RuleFor(staff => staff.Bio)
                .MaximumLength(255).WithMessage("{PropertyName} must be at most 255 characters long.");
        }
    }
}
