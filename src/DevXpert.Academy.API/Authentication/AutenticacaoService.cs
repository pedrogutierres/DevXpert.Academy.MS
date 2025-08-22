using DevXpert.Academy.Alunos.Domain.Alunos;
using DevXpert.Academy.Alunos.Domain.Alunos.Interfaces;
using DevXpert.Academy.Alunos.Domain.Alunos.Services;
using DevXpert.Academy.Core.Domain.Exceptions;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevXpert.Academy.API.Authentication
{
    public sealed class AutenticacaoService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAlunoService _alunoService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<AutenticacaoService> _logger;
        private readonly DomainNotificationHandler _notifications;

        public AutenticacaoService(
            IAlunoService alunoService,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<AutenticacaoService> logger,
            INotificationHandler<DomainNotification> notifications)
        {
            _alunoService = alunoService;
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

                    var aluno = new Aluno(Guid.Parse(user.Id), nome);

                    if (!await _alunoService.Cadastrar(aluno))
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

                    await _alunoService.ExcluirNovoCadastro(Guid.Parse(user.Id));
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
