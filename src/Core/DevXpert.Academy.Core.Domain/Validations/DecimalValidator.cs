using FluentValidation;
using FluentValidation.Validators;
using System;

namespace DevXpert.Academy.Core.Domain.Validations
{
    public class DecimalValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        public override string Name => "DecimalValidator";

        private readonly bool _informouMinimoEMaximo = false;
        private readonly decimal _min;
        private readonly decimal _max;

        public DecimalValidator()
        { }
        public DecimalValidator(decimal min, decimal max) : this()
        {
            _informouMinimoEMaximo = true;
            _min = min;
            _max = max;
        }

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            if (value == null) return false;

            if (!decimal.TryParse(Convert.ToString(value), out var valor)) return false;

            if (_informouMinimoEMaximo)
                return valor >= _min && valor <= _max;

            return true;
        }
    }
}