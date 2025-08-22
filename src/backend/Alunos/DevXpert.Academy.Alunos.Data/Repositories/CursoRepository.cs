using DevXpert.Academy.Alunos.Domain.Cursos;
using DevXpert.Academy.Alunos.Domain.Cursos.Interfaces;

namespace DevXpert.Academy.Alunos.Data.Repositories
{
    public class CursoRepository : ReadOnlyRepository<Curso>, ICursoReadOnlyRepository
    {
        public CursoRepository(AlunosContext context) : base(context)
        { }
    }
}
