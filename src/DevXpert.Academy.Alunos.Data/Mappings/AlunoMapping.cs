using DevXpert.Academy.Alunos.Domain.Alunos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevXpert.Academy.Alunos.Data.Mappings
{
     public class AlunoMapping : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasMany(p => p.Matriculas)
                .WithOne(p => p.Aluno)
                .HasForeignKey(p => p.AlunoId);

            builder.HasMany(p => p.AulasConcluidas)
                .WithOne(p => p.Aluno)
                .HasForeignKey(p => p.AlunoId);

            builder.Navigation(p => p.Matriculas).AutoInclude();
            builder.Navigation(p => p.AulasConcluidas).AutoInclude();

            builder.ToTable("Alunos");
        }
    }
}
