using FluentValidation;

namespace NewsflowApi.Presentation.Validations.Extensions
{
    public static class EmailValidationExtensions
    {
        public static IRuleBuilder<T, string> ValidEmail<T> (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .EmailAddress().WithMessage("{PropertyName} must be a valid email address.")
                .MaximumLength(255).WithMessage("{PropertyName} must be at most 255 characters long.");
        }
    }
}
