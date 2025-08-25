using DevXpert.Academy.BFF.API.Clients;
using DevXpert.Academy.BFF.API.ViewModels.Alunos;
using DevXpert.Academy.Core.APIModel.Controllers;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevXpert.Academy.BFF.API.Controllers
{
    [Authorize]
    [Route("api/alunos")]
    public class AlunosController : MainController
    {
        private readonly AlunosApiClient _alunosApiClient;

        public AlunosController(
            AlunosApiClient alunosApiClient,
            INotificationHandler<DomainNotification> notifications,
            IUser user,
            IMediatorHandler mediator)
            : base(notifications, user, mediator)
        {
            _alunosApiClient = alunosApiClient;
        }

        [HttpGet("meu-perfil")]
        [Authorize(Roles = "Aluno")]
        [ProducesResponseType(typeof(MeuPerfilViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MeuPerfil()
        {
            var perfil = await _alunosApiClient.ObterMeuPerfilAsync(_user.AccessToken);
            if (perfil == null)
                return BadRequest();

            return Ok(perfil);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(List<AlunoViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterAlunos()
        {
            var alunos = await _alunosApiClient.ObterAlunosAsync(_user.AccessToken);
            if (alunos == null)
                return BadRequest();

            return Ok(alunos);
        }
    }
}