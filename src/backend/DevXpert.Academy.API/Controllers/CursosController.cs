using AutoMapper;
using DevXpert.Academy.API.ResponseType;
using DevXpert.Academy.API.ViewModels.Cursos;
using DevXpert.Academy.Conteudo.Domain.Cursos;
using DevXpert.Academy.Conteudo.Domain.Cursos.Interfaces;
using DevXpert.Academy.Conteudo.Domain.Cursos.ValuesObjects;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
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
    [Authorize(Roles = "Administrador")]
    [Route("api/cursos")]
    public class CursosController : MainController
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly ICursoService _cursoService;
        private readonly IMapper _mapper;

        public CursosController(
            ICursoRepository cursoRepository,
            ICursoService cursoService,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications, 
            IUser user,
            IMediatorHandler mediator) 
            : base(notifications, user, mediator)
        {
            _cursoRepository = cursoRepository;
            _cursoService = cursoService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<CursoViewModel>> ObterCursos()
        {
            return await _mapper.ProjectTo<CursoViewModel>(_cursoRepository.Buscar(p => true)).OrderBy(p => p.Titulo).ToListAsync();
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CursoViewModel>> ObterPorId([FromRoute] Guid id)
        {
            var curso = await _cursoRepository.ObterPorId(id, false);
            if (curso == null || !curso.Ativo)
                return NotFound();

            return _mapper.Map<CursoViewModel>(curso);
        }

        [HttpGet("admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<CursoAdmViewModel>> ObterCursosParaAdmin([FromQuery] bool? ativo = null)
        {
            var query = _cursoRepository.Buscar(p => true).IgnoreQueryFilters();

            if (ativo.HasValue)
                query = query.Where(p => p.Ativo == ativo.Value);

            return _mapper.Map<List<CursoAdmViewModel>>(await query.OrderBy(p => p.Titulo).ToListAsync());
        }

        [HttpGet("{id:guid}/admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CursoAdmViewModel>> ObterPorIdParaAdmin([FromRoute] Guid id)
        {
            var curso = await _cursoRepository.ObterPorId(id, false);
            if (curso == null) 
                return NotFound();

            return _mapper.Map<CursoAdmViewModel>(curso);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CadastrarCurso([FromBody] CadastrarCursoViewModel viewModel)
        {
            var curso = _mapper.Map<Curso>(viewModel);

            await _cursoService.Cadastrar(curso);

            return Response(curso.Id);
        }

        [HttpPut("{cursoId:guid}")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AlterarCurso([FromRoute] Guid cursoId, [FromBody] AlterarCursoViewModel viewModel)
        {
            var curso = await _cursoRepository.ObterPorId(cursoId, true);
            if (curso == null)
                return NotFound();

            if (curso.Titulo != viewModel.Titulo)
                curso.AlterarTitulo(viewModel.Titulo);

            if (curso.Valor != viewModel.Valor)
                curso.AlterarValor(viewModel.Valor);
            
            var novoConteudoProgramativo = new ConteudoProgramatico(viewModel.ConteudoProgramatico.Descricao, viewModel.ConteudoProgramatico.CargaHoraria);
            if (curso.ConteudoProgramatico != novoConteudoProgramativo)
                curso.AlterarConteudoProgramatico(novoConteudoProgramativo);

            await _cursoService.Alterar(curso);

            return Response(curso.Id);
        }

        [HttpPatch("{cursoId:guid}/ativar")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtivarCurso([FromRoute] Guid cursoId)
        {
            await _cursoService.Ativar(cursoId);

            return Response(cursoId);
        }

        [HttpDelete("{cursoId:guid}/inativar")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InativarCurso([FromRoute] Guid cursoId)
        {
            await _cursoService.Inativar(cursoId);

            return Response(cursoId);
        }

        [HttpPost("{cursoId:guid}/aulas")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CadastrarAula([FromRoute] Guid cursoId, [FromBody] CadastrarAulaViewModel viewModel)
        {
            var aula = new Aula(viewModel.Id, cursoId, viewModel.Titulo, viewModel.VideoUrl);

            await _cursoService.AdicionarAula(cursoId, aula);

            return Response(aula.Id);
        }

        [HttpDelete("{cursoId:guid}/aulas/{aulaId:guid}")]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverAula([FromRoute] Guid cursoId, [FromRoute] Guid aulaId)
        {
            await _cursoService.RemoverAula(cursoId, aulaId);

            return Response();
        }

    }
}
