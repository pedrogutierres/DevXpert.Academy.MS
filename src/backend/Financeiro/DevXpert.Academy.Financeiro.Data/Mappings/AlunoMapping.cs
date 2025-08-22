using DevXpert.Academy.Financeiro.Domain.Alunos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevXpert.Academy.Financeiro.Data.Mappings
{
    public class AlunoMapping : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasMany(p => p.Matriculas)
                .WithOne(p => p.Aluno)
                .HasForeignKey(p => p.AlunoId);

            builder.Navigation(p => p.Matriculas).AutoInclude();

            builder.ToTable("Alunos");
        }
    }
}
