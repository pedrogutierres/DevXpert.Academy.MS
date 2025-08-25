using DevXpert.Academy.BFF.API.Clients;
using DevXpert.Academy.BFF.API.ViewModels.Matriculas;
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
    [Authorize]
    [Route("api/matriculas")]
    public class MatriculasController : MainController
    {
        private readonly ConteudoApiClient _conteudoApiClient;

        public MatriculasController(
            ConteudoApiClient conteudoApiClient,
            INotificationHandler<DomainNotification> notifications,
            IUser user,
            IMediatorHandler mediator)
            : base(notifications, user, mediator)
        {
            _conteudoApiClient = conteudoApiClient;
        }

        [Authorize(Roles = "Aluno")]
        [HttpPost("cursos/{cursoId:guid}/se-matricular")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlunoSeMatricular([FromRoute] Guid cursoId, [FromBody] RealizarPagamentoViewModel viewModel)
        {
            var result = await _conteudoApiClient.AlunoSeMatricularAsync(_user.AccessToken, cursoId, viewModel);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Roles = "Aluno")]
        [HttpPost("{matriculaId:guid}/realizar-pagamento")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RealizarPagamentoMatricula([FromRoute] Guid matriculaId, [FromBody] RealizarPagamentoViewModel viewModel)
        {
            var result = await _conteudoApiClient.RealizarPagamentoMatriculaAsync(_user.AccessToken, matriculaId, viewModel);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{matriculaId:guid}")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelarMatricula([FromRoute] Guid matriculaId, [FromBody] CancelarMatriculaViewModel viewModel)
        {
            var result = await _conteudoApiClient.CancelarMatriculaAsync(_user.AccessToken, matriculaId, viewModel);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("cursos/{cursoId:guid}/matricular-aluno/{alunoId:guid}")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdministradorMatricularAluno([FromRoute] Guid cursoId, [FromRoute] Guid alunoId)
        {
            var result = await _conteudoApiClient.AdministradorMatricularAlunoAsync(_user.AccessToken, cursoId, alunoId);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Roles = "Aluno")]
        [HttpPost("{matriculaId:guid}/registrar-aula-concluida/{aulaId:guid}")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlunoRegistrarAulaConcluida([FromRoute] Guid matriculaId, [FromRoute] Guid aulaId)
        {
            var result = await _conteudoApiClient.AlunoRegistrarAulaConcluidaAsync(_user.AccessToken, matriculaId, aulaId);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Roles = "Aluno")]
        [HttpPost("{matriculaId:guid}/certificado")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlunoObterCertificado([FromRoute] Guid matriculaId)
        {
            var result = await _conteudoApiClient.AlunoObterCertificadoAsync(_user.AccessToken, matriculaId);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }
    }
}