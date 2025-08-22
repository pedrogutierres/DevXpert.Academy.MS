using DevXpert.Academy.Core.Domain.Messages;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace DevXpert.Academy.Core.Domain.DomainObjects
{
    public abstract class Entity : IEntity
    {
        protected Entity()
        {
            ValidationResult = new ValidationResult();
        }

        public virtual Guid Id { get; protected set; }

        public DateTime DataHoraCriacao { get; protected set; } = DateTime.Now;
        public DateTime? DataHoraAlteracao { get; protected set; }

        public abstract bool EhValido();
        public ValidationResult ValidationResult { get; protected set; }

        public static implicit operator bool(Entity @this) => @this != null && @this.EhValido();

        public void NovoValidationResult(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }

        public void AdicionarValidationResultErros(ValidationResult validationResult)
        {
            ValidationResult.Errors.AddRange(validationResult.Errors);
        }

        protected void AdicionarErroDeNotificacao(string propriedade, string erro)
        {
            if (ValidationResult == null)
                ValidationResult = new ValidationResult();

            ValidationResult.Errors.Add(new ValidationFailure(propriedade, erro));
        }

        #region Eventos
        private List<Event> _notifications;
        public IReadOnlyCollection<Event> Notifications => _notifications?.AsReadOnly();

        protected void AddEvent(Event @event)
        {
            _notifications ??= [];
            _notifications.Add(@event);
        }

        protected void RemoveEvent(Event @event)
        {
            _notifications?.Remove(@event);
        }

        public void ClearEvents()
        {
            _notifications?.Clear();
        }

        #endregion

        #region Comparações
        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity;   

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 760) + Id.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            return $"{GetType().Name} [Id = {Id}]";
        }
    }

    public abstract class ReadOnlyEntity : Entity, IEntity
    {
        public override bool EhValido() => true;
    }

    public interface IEntity
    {
        Guid Id { get; }

        IReadOnlyCollection<Event> Notifications { get; }
        void ClearEvents();
    }
}
