using DevXpert.Academy.Financeiro.Domain.Pagamentos;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.ValuesObejcts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevXpert.Academy.Financeiro.Data.Mappings
{
    public class PagamentoMapping : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.OwnsOne(p => p.DadosCartao, conteudo =>
            {
                conteudo.Property(p => p.Token)
                    .HasColumnName("DadosCartaoToken");
            });

            builder.OwnsOne(p => p.Situacao, situacao =>
            {
                situacao.Property(p => p.Situacao)
                    .HasColumnName(nameof(PagamentoSituacao.Situacao));

                situacao.Property(p => p.DataHoraProcessamento)
                    .HasColumnName("DataHoraUltimoProcessamento");

                situacao.Property(p => p.Mensagem)
                    .HasColumnName(nameof(PagamentoSituacao.Mensagem));
            });

            //builder.Ignore(p => p.HistoricoTransacoes);

            /*builder.OwnsMany(p => p.HistoricoTransacoes, transacao =>
            {
                transacao.WithOwner().HasForeignKey("PagamentoId").HasPrincipalKey(p => p.Id);

                transacao.Property("Id").HasColumnType("TEXT").ValueGeneratedNever();
                transacao.HasKey("Id");

                transacao.ToTable("PagamentosTransacoes");
            });*/

            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Matricula)
                .WithMany(p => p.Pagamentos)
                .HasForeignKey(p => p.MatriculaId);

            builder.Navigation(p => p.Matricula).AutoInclude();

            builder.ToTable("Pagamentos");
        }
    }
}
