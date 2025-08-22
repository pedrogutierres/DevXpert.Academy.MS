using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Globalization;

namespace DevXpert.Academy.Core.Domain.Validations
{
    public class TimeValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        public override string Name => "TimeValidator";

        private readonly string _expression;

        public TimeValidator(string expression)
        {
            _expression = expression;
        }

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            if (value == null) return false;

            return DateTime.TryParseExact(Convert.ToString(value), _expression, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}