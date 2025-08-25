using DevXpert.Academy.BFF.API.Clients;
using DevXpert.Academy.BFF.API.ViewModels.Usuarios;
using DevXpert.Academy.Core.APIModel.Controllers;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Exceptions;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevXpert.Academy.BFF.API.Controllers
{
    [Route("api/usuarios")]
    public class UsuariosController : MainController
    {
        private readonly AuthApiClient _authApiClient;

        public UsuariosController(
            AuthApiClient authApiClient,
            INotificationHandler<DomainNotification> notifications,
            IUser user,
            IMediatorHandler mediator)
            : base(notifications, user, mediator)
        {
            _authApiClient = authApiClient;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginViewModel viewModel)
        {
            try
            {
                var user = await _authApiClient.LogarUsuarioAsync(viewModel);
                if (user != null)
                    return Ok(user);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Falha",
                    Detail = ex.Message,
                });
            }
            catch
            {
                throw;
            }

            return BadRequest();
        }

        [HttpPost("novo-aluno")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> NovoAluno([FromBody] NovoAlunoViewModel viewModel)
        {
            try
            {
                var user = await _authApiClient.CadastrarUsuarioAsync(viewModel);
                if (user != null)
                    return Ok(user);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Falha",
                    Detail = ex.Message,
                });
            }
            catch
            {
                throw;
            }

            return BadRequest();
        }
    }
}
