using DevXpert.Academy.Auth.API.Configurations;
using DevXpert.Academy.Auth.API.Services;
using DevXpert.Academy.Core.Domain.Exceptions;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevXpert.Academy.Auth.API.Authentication
{
    public sealed class AutenticacaoService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AlunoApiClient _alunoApiClient;
        private readonly JwtTokenGenerate _jwtTokenGenerate;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<AutenticacaoService> _logger;
        private readonly DomainNotificationHandler _notifications;

        public AutenticacaoService(
            AlunoApiClient alunoApiClient,
            JwtTokenGenerate jwtTokenGenerate,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<AutenticacaoService> logger,
            INotificationHandler<DomainNotification> notifications)
        {
            _alunoApiClient = alunoApiClient;
            _jwtTokenGenerate = jwtTokenGenerate;
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _notifications = (DomainNotificationHandler)notifications;
        }

        public async Task<bool> ValidarLoginUsuarioAsync(string email, string senha)
        {
            var result = await _signInManager.PasswordSignInAsync(email, senha, false, lockoutOnFailure: false);
            return result.Succeeded;
        }
        public async Task<IdentityUser> RegistrarUsuarioAsync(string email, string senha, string nome)
        {
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);

            try
            {
                var result = await _userManager.CreateAsync(user, senha);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Aluno");

                    // TODO: Este processo abaixo, será movido para o orquestrador BFF
                    AuthToken token;
                    try
                    {
                        token = await _jwtTokenGenerate.GerarToken(user.Email);
                    }
                    catch (Exception ex)
                    {
                        await _userManager.DeleteAsync(user);

                        throw new BusinessException($"Não foi possível obter os dados de autorização do usuário, tente novamente mais tarde. {ex.Message}");
                    }

                    var retornoCadastroAluno = await _alunoApiClient.CadastrarAlunoAsync(new
                    {
                        Id = Guid.Parse(user.Id),
                        Nome = nome
                    }, token.Result.Access_token);

                    if (!retornoCadastroAluno)
                    {
                        await _userManager.DeleteAsync(user);

                        throw new BusinessException(string.Join('.', _notifications.GetNotifications().Select(p => p.Value)));
                    }

                    _logger.LogInformation("Usuário criado com senha.");

                    return user;
                }
                else
                {
                    _logger.LogError("Falha ao criar usuário: {Errors}", string.Join(" ", result.Errors.Select(e => e.Description)));

                    throw new BusinessException($"Não foi possível criar seu usuário: {string.Join(" ", result.Errors.Select(e => e.Description))}");
                }
            }
            catch (DbException)
            {
                // caso houver erro na criacao de usuario aluno, excluir o usuario e aluno criado caso tenha sido registrado
                try
                {
                    await _userManager.DeleteAsync(user);

                    //TODO: implementar
                    //await _alunoApiClient.ExcluirNovoCadastro(Guid.Parse(user.Id));
                }
                catch { }

                throw;
            }
            catch
            {
                throw;
            }
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
