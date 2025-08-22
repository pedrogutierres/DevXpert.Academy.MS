using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Validations;
using System;

namespace FluentValidation
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> IsValidAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, DomainSpecification<T> predicate) where T : Entity
        {
            return ruleBuilder.MustAsync(async (p, c) => await predicate.IsValidAsync());
        }

        public static IRuleBuilderOptions<T, TProperty> IsValidAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, DomainSpecification<T>> predicate) where T : Entity
        {
            return ruleBuilder.MustAsync(async (e, p, c) => await predicate(e).IsValidAsync());
        }

        public static IRuleBuilderOptions<T, string> Date<T>(this IRuleBuilder<T, string> ruleBuilder, string expression)
        {
            return ruleBuilder.SetValidator(new DateValidator<T, string>(expression));
        }

        public static IRuleBuilderOptions<T, string> Time<T>(this IRuleBuilder<T, string> ruleBuilder, string expression)
        {
            return ruleBuilder.SetValidator(new TimeValidator<T, string>(expression));
        }

        public static IRuleBuilderOptions<T, string> Number<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new NumberValidator<T, string>());
        }

        public static IRuleBuilderOptions<T, string> Number<T>(this IRuleBuilder<T, string> ruleBuilder, long min, long max)
        {
            return ruleBuilder.SetValidator(new NumberValidator<T, string>(min, max));
        }

        public static IRuleBuilderOptions<T, string> Decimal<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DecimalValidator<T, string>());
        }

        public static IRuleBuilderOptions<T, string> Decimal<T>(this IRuleBuilder<T, string> ruleBuilder, decimal min, decimal max)
        {
            return ruleBuilder.SetValidator(new DecimalValidator<T, string>(min, max));
        }
    }
}
