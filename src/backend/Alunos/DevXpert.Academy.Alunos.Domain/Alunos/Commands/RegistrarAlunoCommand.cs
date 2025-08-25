using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Commands
{
    public class RegistrarAlunoCommand : Command<bool>
    {
        public string Nome { get; private set; }
        public string Email { get; private set; }

        public RegistrarAlunoCommand(Guid id, string nome, string email)
        {
            AggregateId = id;
            Nome = nome;
            Email = email;
        }
    }
}
