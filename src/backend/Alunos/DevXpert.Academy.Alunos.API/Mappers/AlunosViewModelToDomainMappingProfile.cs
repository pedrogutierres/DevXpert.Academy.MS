using AutoMapper;
using DevXpert.Academy.Alunos.API.ViewModels.Alunos;
using DevXpert.Academy.Alunos.Domain.Alunos;

namespace DevXpert.Academy.Financeiro.API.Mappers
{
    public class AlunosViewModelToDomainMappingProfile : Profile
    {
        public AlunosViewModelToDomainMappingProfile()
        {
            CreateMap<CadastrarAlunoViewModel, Aluno>()
                .ConstructUsing(a => new Aluno(a.Id, a.Nome));
        }
    }
}
