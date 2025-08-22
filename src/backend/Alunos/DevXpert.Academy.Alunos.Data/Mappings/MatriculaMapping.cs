using DevXpert.Academy.Alunos.Domain.Alunos;
using DevXpert.Academy.Alunos.Domain.Alunos.ValuesObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevXpert.Academy.Alunos.Data.Mappings
{
    public class MatriculaMapping : IEntityTypeConfiguration<Matricula>
    {
        public void Configure(EntityTypeBuilder<Matricula> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.OwnsOne(p => p.Certificado, certificado =>
            {
                certificado.Property(p => p.CertificadoUrl)
                    .HasColumnName(nameof(Certificado.CertificadoUrl));

                certificado.Property(p => p.DataHoraEmissao)
                    .HasColumnName(nameof(Certificado.DataHoraEmissao));
            });

            builder.HasOne(p => p.Aluno)
                .WithMany(p => p.Matriculas)
                .HasForeignKey(p => p.AlunoId);

            builder.HasOne(p => p.Curso)
                .WithMany(p => p.Matriculas)
                .HasForeignKey(p => p.CursoId);

            builder.Navigation(p => p.Aluno).AutoInclude();
            builder.Navigation(p => p.Curso).AutoInclude();

            builder.ToTable("Matriculas");
        }
    }
}
