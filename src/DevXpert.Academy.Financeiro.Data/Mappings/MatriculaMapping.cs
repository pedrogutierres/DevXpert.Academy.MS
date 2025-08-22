using DevXpert.Academy.Financeiro.Domain.Alunos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevXpert.Academy.Financeiro.Data.Mappings
{
    public class MatriculaMapping : IEntityTypeConfiguration<Matricula>
    {
        public void Configure(EntityTypeBuilder<Matricula> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Aluno)
                .WithMany(p => p.Matriculas)
                .HasForeignKey(p => p.AlunoId);

            builder.HasMany(p => p.Pagamentos)
                .WithOne(p => p.Matricula)
                .HasForeignKey(p => p.MatriculaId);

            builder.Navigation(p => p.Aluno).AutoInclude();

            builder.ToTable("Matriculas");
        }
    }
}
