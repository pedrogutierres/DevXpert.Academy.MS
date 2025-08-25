using DevXpert.Academy.BFF.API.Clients;
using DevXpert.Academy.BFF.API.ViewModels.Pagamentos;
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
    [Route("api/pagamentos")]
    public class PagamentosController : MainController
    {
        private readonly FinanceiroApiClient _pagamentosApiClient;

        public PagamentosController(
            FinanceiroApiClient pagamentosApiClient,
            INotificationHandler<DomainNotification> notifications,
            IUser user,
            IMediatorHandler mediator)
            : base(notifications, user, mediator)
        {
            _pagamentosApiClient = pagamentosApiClient;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(List<PagamentoViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPagamentos()
        {
            var pagamentos = await _pagamentosApiClient.ObterPagamentosAsync(_user.AccessToken);
            if (pagamentos == null)
                return BadRequest();

            return Ok(pagamentos);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(PagamentoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPagamentoPorId([FromRoute] Guid id)
        {
            var pagamento = await _pagamentosApiClient.ObterPagamentoPorIdAsync(_user.AccessToken, id);
            if (pagamento == null)
                return BadRequest();

            return Ok(pagamento);
        }

        [HttpGet("alunos/{alunoId:guid}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(List<PagamentoViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPagamentosPorAluno([FromRoute] Guid alunoId)
        {
            var pagamentos = await _pagamentosApiClient.ObterPagamentosPorAlunoAsync(_user.AccessToken, alunoId);
            if (pagamentos == null)
                return BadRequest();

            return Ok(pagamentos);
        }
    }
}