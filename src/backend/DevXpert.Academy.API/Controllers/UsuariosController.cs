using DevXpert.Academy.API.Authentication;
using DevXpert.Academy.API.Configurations;
using DevXpert.Academy.API.ViewModels.Usuarios;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Exceptions;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevXpert.Academy.API.Controllers
{
    [Route("api/usuarios")]
    public class UsuariosController : MainController
    {
        private readonly AutenticacaoService _autenticacaoService;
        private readonly JwtTokenGenerate _jwtTokenGenerate;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UsuariosController(
            AutenticacaoService autenticacaoService,
            JwtTokenGenerate jwtTokenGenerate,
            SignInManager<IdentityUser> signInManager,
            INotificationHandler<DomainNotification> notifications,
            IUser user,
            IMediatorHandler mediator)
            : base(notifications, user, mediator)
        {
            _autenticacaoService = autenticacaoService;
            _jwtTokenGenerate = jwtTokenGenerate;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Senha, false, lockoutOnFailure: false);

            if (result.Succeeded)
                return Ok(await _jwtTokenGenerate.GerarToken(login.Email));

            return BadRequest(new ProblemDetails
            {
                Title = "Falha na autenticação",
                Detail = "E-mail e/ou senha inválidos."
            });
        }

        [HttpPost("novo-aluno")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> NovoAluno([FromBody] NovoAlunoViewModel viewModel)
        {
            try
            {
                var user = await _autenticacaoService.RegistrarUsuarioAsync(viewModel.Email, viewModel.Senha, nome: viewModel.Nome);
                if (user != null)
                    return Ok(await _jwtTokenGenerate.GerarToken(user.Email));
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Falha no registro",
                    Detail = ex.Message,
                });
            }
            catch
            {
                throw;
            }

            return BadRequest(new ProblemDetails
            {
                Title = "Falha no registro",
                Detail = "Não foi possível criar seu usuário, entre em contato com os administradores da plataforma."
            });
        }
    }
}
