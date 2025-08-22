using DevXpert.Academy.Core.Domain.DataModels;
using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Interfaces
{
    public interface IAlunoRepository : IRepository<Aluno>
    {
        Task<Aluno> ObterAtravesDaMatricula(Guid matriculaId, bool tracking = false);
        Task CadastrarMatricula(Matricula matricula);

        Task<Matricula> ObterMatricula(Guid matriculaId);
    }
}
