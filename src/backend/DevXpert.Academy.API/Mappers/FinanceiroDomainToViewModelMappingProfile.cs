using AutoMapper;
using DevXpert.Academy.API.ViewModels.Pagamentos;
using DevXpert.Academy.Financeiro.Domain.Alunos;
using DevXpert.Academy.Financeiro.Domain.Pagamentos;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.ValuesObejcts;

namespace DevXpert.Academy.API.Mappers
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

