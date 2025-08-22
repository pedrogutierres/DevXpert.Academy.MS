using AutoMapper;
using DevXpert.Academy.API.ViewModels.Cursos;
using DevXpert.Academy.Conteudo.Domain.Cursos;
using DevXpert.Academy.Conteudo.Domain.Cursos.ValuesObjects;
using System;
using System.Linq;

namespace DevXpert.Academy.API.Mappers
{
    public class ConteudoViewModelToDomainMappingProfile : Profile
    {
        public ConteudoViewModelToDomainMappingProfile()
        {
            CreateMap<CadastrarCursoViewModel, Curso>()
                .ConstructUsing(c => new Curso(c.Id, c.Titulo, c.ConteudoProgramatico != null ? new ConteudoProgramatico(c.ConteudoProgramatico.Descricao, c.ConteudoProgramatico.CargaHoraria) : null, c.Valor, c.Aulas.Select(a => new Aula(a.Id, c.Id, a.Titulo, a.VideoUrl)).ToList()));
            CreateMap<CadastrarConteudoProgramaticoViewModel, ConteudoProgramatico>()
                .ConstructUsing(c => new ConteudoProgramatico(c.Descricao, c.CargaHoraria));
            CreateMap<CadastrarAulaViewModel, Aula>()
                .ConstructUsing(a => new Aula(a.Id, Guid.Empty, a.VideoUrl, a.VideoUrl)); 
        }
    }
}
