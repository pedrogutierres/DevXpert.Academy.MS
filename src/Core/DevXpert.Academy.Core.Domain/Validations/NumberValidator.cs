using FluentValidation;
using FluentValidation.Validators;
using System;

namespace DevXpert.Academy.Core.Domain.Validations
{
    public class NumberValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        public override string Name => "NumberValidator";

        private readonly bool _informouMinimoEMaximo = false;
        private readonly long _min;
        private readonly long _max;

        public NumberValidator() { }
        public NumberValidator(long min, long max) : this()
        {
            _informouMinimoEMaximo = true;
            _min = min;
            _max = max;
        }

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            if (value == null) return false;

            if (!long.TryParse(Convert.ToString(value), out var number)) return false;

            if (_informouMinimoEMaximo)
                return number >= _min && number <= _max;

            return true;
        }
    }
}