using AutoMapper;
using DevXpert.Academy.Financeiro.API.ViewModels.Pagamentos;
using DevXpert.Academy.Financeiro.Domain.Alunos;
using DevXpert.Academy.Financeiro.Domain.Pagamentos;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.ValuesObejcts;

namespace DevXpert.Academy.Financeiro.API.Mappers
{
    public class FinanceiroDomainToViewModelMappingProfile : Profile
    {
        public FinanceiroDomainToViewModelMappingProfile()
        {
            CreateMap<Pagamento, PagamentoViewModel>();
            CreateMap<PagamentoSituacao, PagamentoSituacaoViewModel>();
            CreateMap<Matricula, PagamentoMatriculaViewModel>();
            CreateMap<Aluno, PagamentoAlunoViewModel>();
        }
    }
}

