using DevXpert.Academy.Conteudo.Domain.Cursos;
using DevXpert.Academy.Conteudo.Domain.Cursos.ValuesObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevXpert.Academy.Conteudo.Data.Mappings
{
    public class CursoMapping : IEntityTypeConfiguration<Curso>
    {
        public void Configure(EntityTypeBuilder<Curso> builder)
        {
            builder.HasQueryFilter(p => p.Ativo == true);

            builder.OwnsOne(p => p.ConteudoProgramatico, conteudo =>
            {
                conteudo.Property(p => p.Descricao)
                    .HasColumnName(nameof(ConteudoProgramatico.Descricao));

                conteudo.Property(p => p.CargaHoraria)
                    .HasColumnName(nameof(ConteudoProgramatico.CargaHoraria));
            });

            builder.OwnsMany(p => p.Aulas, aula =>
            {
                aula.WithOwner().HasForeignKey(p => p.CursoId).HasPrincipalKey(p => p.Id);

                aula.HasKey(p => p.Id);
                aula.Property(p => p.Id).ValueGeneratedNever();

                aula.ToTable("CursosAulas");
            });

            builder.HasKey(p => p.Id);

            builder.ToTable("Cursos");
        }
    }
}
