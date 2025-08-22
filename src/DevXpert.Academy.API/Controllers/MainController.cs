using DevXpert.Academy.API.ResponseType;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Mime;

namespace DevXpert.Academy.API.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public abstract class MainController : Controller
    {
        protected readonly DomainNotificationHandler _notifications;
        protected readonly IMediatorHandler _mediator;
        protected readonly IUser _user;

        protected Guid? UsuarioId { get; set; }

        protected MainController(INotificationHandler<DomainNotification> notifications, IUser user, IMediatorHandler mediator)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _mediator = mediator;
            _user = user;

            UsuarioId = user.UsuarioIdNullValue();
        }

        protected new IActionResult Response(Guid id) => Response<object>(id: id);
        protected new IActionResult Response(Guid id, long codigo) => Response<object>(id: id, codigo: codigo);
        protected new IActionResult Response(object result = null) => Response<object>(result: result);
        protected new IActionResult Response<T>(Guid? id = null, long? codigo = null, T result = default)
        {
            if (OperacaoValida())
            {
                return Ok(new ResponseSuccess<T>
                {
                    Data = result,
                    Id = id,
                    Codigo = codigo,
                    Warnings = _notifications.HasWarningNotifications() ? _notifications.GetWarningNotifications().Select(p => p.Value) : null
                });
            }

            return BadRequest("Erros nas regras de negócio encontradas", true);
        }

        protected new ActionResult BadRequest()
        {
            return BadRequest("Erros nas regras de negócio encontradas");
        }
        protected ActionResult BadRequest(string mensagem, bool jaExisteNotificacoes = false)
        {
            return new BadRequestObjectResult(
                new ResponseError(
                    "Erros de negócio",
                    mensagem,
                    StatusCodes.Status400BadRequest,
                    HttpContext.Request.Path,
                    (jaExisteNotificacoes || _notifications.HasNotifications()) ? _notifications.GetNotifications() : new[] { new DomainNotification("error", mensagem) }
                ))
            {
                ContentTypes = { "application/problem+json" }
            };
        }

        protected bool OperacaoValida()
        {
            return (!_notifications.HasNotifications());
        }

        protected void NotificarValidacoesErro(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
                NotificarErro(error.PropertyName, error.ErrorMessage);
        }

        protected void NotificarErro(string codigo, string mensagem)
        {
            _mediator.RaiseEvent(new DomainNotification(codigo, mensagem));
        }
        protected void NotificarAdvertencia(string codigo, string mensagem)
        {
            _mediator.RaiseEvent(new DomainNotification(codigo, mensagem, false));
        }
    }
}