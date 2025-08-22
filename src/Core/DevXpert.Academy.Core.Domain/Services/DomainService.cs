using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DataModels;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Exceptions;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using DevXpert.Academy.Core.Domain.Validations;
using FluentValidation.Results;
using MediatR;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.Services
{
    public abstract class DomainService
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IMediatorHandler _mediator;
        protected readonly DomainNotificationHandler _notifications;

        protected DomainService(IUnitOfWork uow, IMediatorHandler mediator, INotificationHandler<DomainNotification> notifications)
        {
            _uow = uow;
            _mediator = mediator;
            _notifications = (DomainNotificationHandler)notifications;
        }

        protected void NotificarValidacoesErro(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                _mediator.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));
            }
        }

        protected Task NotificarErro(string nome, string mensagem)
        {
            return _mediator.RaiseEvent(new DomainNotification(nome, mensagem));
        }

        protected Task NotificarAdvertencia(string nome, string mensagem)
        {
            return _mediator.RaiseEvent(new DomainNotification(nome, mensagem, false));
        }

        protected bool HasNotifications()
        {
            return _notifications.HasNotifications();
        }

        protected async Task<bool> CommitAsync(bool ignoreNotifications = false, bool ignoreNoChangeUpdated = false)
        {
            if (HasNotifications() && !ignoreNotifications) return false;
            try
            {
                if (await _uow.CommitAsync())
                    return true;
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Não foi possível salvar as informação na base de dados, por favor tente novamente ou contate o suporte.", ex);
            }

            if (ignoreNoChangeUpdated)
                return true;

            await NotificarErro("CommitSemAlteracoes", "Não foi identificada nenhuma alteração nos dados.");
            return false;
        }

        protected async Task<T> ObterEntidade<T>(IRepository<T> repository, Guid id, string messageType, string message, bool tracking = false) where T : Entity, IAggregateRoot
        {
            var entidade = await repository.ObterPorId(id, tracking);

            if (entidade != null) return entidade;

            await _mediator.RaiseEvent(new DomainNotification(messageType, message));
            return null;
        }
        protected bool EntidadeValida<T>(T entidade) where T : Entity
        {
            if (entidade == null)
            {
                NotificarErro("EntidadeInvalida", "Entidade não pode ser nula.");
                return false;
            }

            if (entidade.EhValido()) return true;

            NotificarValidacoesErro(entidade.ValidationResult);
            return false;
        }
        protected async Task<bool> EntidadeAptaParaTransacionar<T>(T entidade, DomainValidator<T> validator) where T : Entity
        {
            if (entidade == null)
            {
                await NotificarErro("EntidadeInvalida", "Entidade não pode ser nula.");
                return false;
            }

            var aptoParaTransacionarResult = await validator.ValidateAsync(entidade);
            if (!aptoParaTransacionarResult.IsValid)
            {
                NotificarValidacoesErro(aptoParaTransacionarResult);
                return false;
            }

            return true;
        }
    }
}