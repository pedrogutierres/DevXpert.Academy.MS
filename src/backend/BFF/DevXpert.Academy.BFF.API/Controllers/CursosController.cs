using DevXpert.Academy.BFF.API.Clients;
using DevXpert.Academy.BFF.API.ViewModels.Cursos;
using DevXpert.Academy.Core.APIModel.Controllers;
using DevXpert.Academy.Core.APIModel.ResponseType;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevXpert.Academy.BFF.API.Controllers
{
    [Route("api/cursos")]
    public class CursosController : MainController
    {
        private readonly ConteudoApiClient _conteudoApiClient;

        public CursosController(
            ConteudoApiClient conteudoApiClient,
            INotificationHandler<DomainNotification> notifications,
            IUser user,
            IMediatorHandler mediator)
            : base(notifications, user, mediator)
        {
            _conteudoApiClient = conteudoApiClient;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<CursoViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterCursos()
        {
            var cursos = await _conteudoApiClient.ObterCursosAsync();
            if (cursos == null)
                return BadRequest();

            return Ok(cursos);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CursoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            var curso = await _conteudoApiClient.ObterCursoPorIdAsync(id);
            if (curso == null)
                return NotFound();

            return Ok(curso);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(List<CursoAdmViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterCursosParaAdmin([FromQuery] bool? ativo = null)
        {
            var cursos = await _conteudoApiClient.ObterCursosParaAdminAsync(_user.AccessToken, ativo);
            if (cursos == null)
                return BadRequest();

            return Ok(cursos);
        }

        [HttpGet("{id:guid}/admin")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(CursoAdmViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPorIdParaAdmin([FromRoute] Guid id)
        {
            var curso = await _conteudoApiClient.ObterCursoPorIdParaAdminAsync(_user.AccessToken, id);
            if (curso == null)
                return NotFound();

            return Ok(curso);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CadastrarCurso([FromBody] CadastrarCursoViewModel viewModel)
        {
            var result = await _conteudoApiClient.CadastrarCursoAsync(_user.AccessToken, viewModel);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpPut("{cursoId:guid}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AlterarCurso([FromRoute] Guid cursoId, [FromBody] AlterarCursoViewModel viewModel)
        {
            var result = await _conteudoApiClient.AlterarCursoAsync(_user.AccessToken, cursoId, viewModel);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPatch("{cursoId:guid}/ativar")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtivarCurso([FromRoute] Guid cursoId)
        {
            var result = await _conteudoApiClient.AtivarCursoAsync(_user.AccessToken, cursoId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{cursoId:guid}/inativar")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InativarCurso([FromRoute] Guid cursoId)
        {
            var result = await _conteudoApiClient.InativarCursoAsync(_user.AccessToken, cursoId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("{cursoId:guid}/aulas")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CadastrarAula([FromRoute] Guid cursoId, [FromBody] CadastrarAulaViewModel viewModel)
        {
            var result = await _conteudoApiClient.CadastrarAulaAsync(_user.AccessToken, cursoId, viewModel);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpDelete("{cursoId:guid}/aulas/{aulaId:guid}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverAula([FromRoute] Guid cursoId, [FromRoute] Guid aulaId)
        {
            var result = await _conteudoApiClient.RemoverAulaAsync(_user.AccessToken, cursoId, aulaId);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }
    }
}