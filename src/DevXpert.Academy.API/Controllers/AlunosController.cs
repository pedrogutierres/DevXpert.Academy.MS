using AutoMapper;
using DevXpert.Academy.Alunos.Domain.Alunos.Interfaces;
using DevXpert.Academy.API.ViewModels.Alunos;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevXpert.Academy.API.Controllers
{
    [Authorize]
    [Route("api/alunos")]
    public class AlunosController : MainController
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IMapper _mapper;

        public AlunosController(
            IAlunoRepository alunoRepository,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications, 
            IUser user,
            IMediatorHandler mediator) 
            : base(notifications, user, mediator)
        {
            _alunoRepository = alunoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<AlunoViewModel>> ObterAlunos()
        {
            return _mapper.Map<List<AlunoViewModel>>(await _alunoRepository.Buscar(p => true).OrderBy(p => p.Nome).ToListAsync());
        }

        [HttpGet("meu-perfil")]
        [Authorize(Roles = "Aluno")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MeuPerfilViewModel>> MeuPerfil()
        {
            return await _mapper.ProjectTo<MeuPerfilViewModel>(_alunoRepository.Buscar(p => p.Id == _user.UsuarioId)).FirstOrDefaultAsync();
        }
    }
}
