using DevXpert.Academy.API.Extensions;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DevXpert.Academy.API.Models
{
    internal class AspNetUser : IUser
    {
        private readonly IHttpContextAccessor _accessor;

        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Nome => _accessor.HttpContext?.User?.Identity?.Name;

        Guid IUser.UsuarioId => UsuarioIdNullValue() ?? throw new BusinessException("O usuário deve estar logado para realizar esta ação");

        public IEnumerable<Claim> RetornarClaims() => _accessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();

        public Guid? UsuarioIdNullValue()
        {
            return Autenticado() ? Guid.Parse(_accessor.HttpContext.User.GetUserId()) : null;
        }

        public bool Autenticado()
        {
            return _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public bool EhUmAluno()
        {
            return _accessor.HttpContext?.User?.IsInRole("Aluno") ?? false;
        }

        public bool EhUmAdministrador()
        {
            return _accessor.HttpContext?.User?.IsInRole("Administrador") ?? false;
        }
    }
}
