using DevXpert.Academy.Alunos.Domain.Cursos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevXpert.Academy.Alunos.Data.Mappings
{
    public class CursoMapping : IEntityTypeConfiguration<Curso>
    {
        public void Configure(EntityTypeBuilder<Curso> builder)
        {
            builder.HasKey(p => p.Id);

            builder.OwnsMany(p => p.Aulas, aula =>
            {
                aula.WithOwner().HasForeignKey("CursoId").HasPrincipalKey(p => p.Id);

                aula.HasKey(p => p.Id);
                aula.Property(p => p.Id).ValueGeneratedNever();

                aula.ToTable("CursosAulas");
            });

            builder.HasMany(p => p.Matriculas)
                .WithOne(p => p.Curso)
                .HasForeignKey(p => p.CursoId);

            builder.Navigation(p => p.Aulas).AutoInclude();

            builder.ToTable("Cursos");
        }
    }
}
