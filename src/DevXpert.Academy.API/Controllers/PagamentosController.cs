using AutoMapper;
using DevXpert.Academy.API.ViewModels.Pagamentos;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevXpert.Academy.API.Controllers
{
    [Authorize]
    [Route("api/pagamentos")]
    public class PagamentosController : MainController
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMapper _mapper;

        public PagamentosController(
            IPagamentoRepository pagamentoRepository,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications,
            IUser user,
            IMediatorHandler mediator)
            : base(notifications, user, mediator)
        {
            _pagamentoRepository = pagamentoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<PagamentoViewModel>> ObterPagamentos()
        {
            return _mapper.Map<List<PagamentoViewModel>>(await _pagamentoRepository.Buscar(p => true).OrderBy(p => p.DataHoraCriacao).ToListAsync());
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PagamentoViewModel> ObterPagamentoPorId([FromRoute] Guid id)
        {
            return _mapper.Map<PagamentoViewModel>(await _pagamentoRepository.ObterPorId(id));
        }

        [HttpGet("alunos/{alunoId:guid}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<PagamentoViewModel>> ObterPagamentosPorAluno([FromRoute] Guid alunoId)
        {
            return _mapper.Map<List<PagamentoViewModel>>(await _pagamentoRepository.Buscar(p => p.Matricula.AlunoId == alunoId).OrderBy(p => p.DataHoraCriacao).ToListAsync());
        }
    }
}
