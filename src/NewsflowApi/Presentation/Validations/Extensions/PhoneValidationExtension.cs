using FluentValidation;

namespace NewsflowApi.Presentation.Validations.Extensions
{
    public static class PhoneValidationExtension
    {
        public static IRuleBuilderOptions<T, string> ValidContactPhone<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .Matches(@"^\d{10,15}$")
                .WithMessage("{PropertyName} must contain only digits and be between 10 and 15 characters long.");
        }
    }
}
