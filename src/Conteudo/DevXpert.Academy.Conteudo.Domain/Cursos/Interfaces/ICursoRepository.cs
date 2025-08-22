using DevXpert.Academy.Core.Domain.DataModels;
using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Interfaces
{
    public interface ICursoRepository : IRepository<Curso>
    {
        Task<bool> ExistePorTitulo(string titulo, Guid? cursoId = null);
    }
}
