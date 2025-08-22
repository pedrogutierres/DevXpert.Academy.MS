using AutoMapper;
using DevXpert.Academy.Alunos.Domain.Alunos;
using DevXpert.Academy.Alunos.Domain.Alunos.ValuesObjects;
using DevXpert.Academy.Alunos.Domain.Cursos;
using DevXpert.Academy.API.ViewModels.Alunos;

namespace DevXpert.Academy.API.Mappers
{
    public class AlunosDomainToViewModelMappingProfile : Profile
    {
        public AlunosDomainToViewModelMappingProfile()
        {
            CreateMap<Aluno, AlunoViewModel>();
            CreateMap<Matricula, MatriculaViewModel>();
            CreateMap<Certificado, CertificadoViewModel>();
            CreateMap<Curso, CursoViewModel>();

            CreateMap<Aluno, MeuPerfilViewModel>();
            CreateMap<Matricula, MeuPerfilMatriculaViewModel>();
            CreateMap<Certificado, MeuPerfilCertificadoViewModel>();
        }
    }
}

