using DevXpert.Academy.Core.Domain.DomainObjects;
using FluentValidation;

namespace DevXpert.Academy.Core.Domain.Validations
{
    public abstract class DomainValidator<TEntity> : AbstractValidator<TEntity> where TEntity : Entity
    {
        protected void ValidarId()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("O Id deve ser gerado.");
        }
    }
}
