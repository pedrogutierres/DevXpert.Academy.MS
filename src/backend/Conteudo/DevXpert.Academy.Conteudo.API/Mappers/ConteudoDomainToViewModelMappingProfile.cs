using AutoMapper;
using DevXpert.Academy.Conteudo.API.ViewModels.Cursos;
using DevXpert.Academy.Conteudo.Domain.Cursos;
using DevXpert.Academy.Conteudo.Domain.Cursos.ValuesObjects;

namespace DevXpert.Academy.Financeiro.API.Mappers
{
    public class ConteudoDomainToViewModelMappingProfile : Profile
    {
        public ConteudoDomainToViewModelMappingProfile()
        {
            CreateMap<Curso, CursoAdmViewModel>();
            CreateMap<ConteudoProgramatico, ConteudoProgramaticoAdmViewModel>();
            CreateMap<Aula, AulaAdmViewModel>();

            CreateMap<Curso, CursoViewModel>();
            CreateMap<ConteudoProgramatico, ConteudoProgramaticoViewModel>();
            CreateMap<Aula, AulaViewModel>();
        }
    }
}

