using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Interfaces
{
    public interface IAlunoService
    {
        Task<bool> Cadastrar(Aluno aluno);
        Task<bool> ExcluirNovoCadastro(Guid id);

        Task<Matricula> Matricular(Guid alunoId, Guid cursoId);
        Task<bool> AprovarMatricula(Guid matriculaId);
        Task<bool> BloquearMatricula(Guid matriculaId);

        Task<bool> RegistrarAulaConcluida(Guid matriculaId, Guid aulaId);
        Task<string> EmitirCertificado(Guid matriculaId);
    }
}
