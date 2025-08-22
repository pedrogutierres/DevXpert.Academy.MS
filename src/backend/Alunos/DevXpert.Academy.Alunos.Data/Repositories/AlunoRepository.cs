using DevXpert.Academy.Alunos.Domain.Alunos;
using DevXpert.Academy.Alunos.Domain.Alunos.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevXpert.Academy.Alunos.Data.Repositories
{
    public class AlunoRepository : Repository<Aluno>, IAlunoRepository
    {
        public AlunoRepository(AlunosContext context) : base(context)
        { }

        public async Task CadastrarMatricula(Matricula matricula)
        {
            await Db.Set<Matricula>().AddAsync(matricula);
        }

        public Task<Aluno> ObterAtravesDaMatricula(Guid matriculaId, bool tracking = false)
        {
            if (tracking)
                return DbSet.FirstOrDefaultAsync(p => p.Matriculas.Any(m => m.Id == matriculaId));
            return DbSet.AsNoTracking().FirstOrDefaultAsync(p => p.Matriculas.Any(m => m.Id == matriculaId));
        }

        public Task<Matricula> ObterMatricula(Guid matriculaId)
        {
            return Db.Set<Matricula>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == matriculaId);
        }
    }
}
