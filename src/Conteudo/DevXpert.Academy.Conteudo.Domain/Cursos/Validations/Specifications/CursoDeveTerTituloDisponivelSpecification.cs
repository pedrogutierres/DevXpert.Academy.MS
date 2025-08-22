using DevXpert.Academy.Conteudo.Domain.Cursos.Interfaces;
using DevXpert.Academy.Core.Domain.Validations;
using System.Threading.Tasks;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Validations.Specifications
{
    public class CursoDeveTerTituloDisponivelSpecification : DomainSpecification<Curso>
    {
        private readonly ICursoRepository _cursoRepository;

        public CursoDeveTerTituloDisponivelSpecification(Curso entidade, ICursoRepository cursoRepository) : base(entidade)
        {
            _cursoRepository = cursoRepository;
        }

        public override async Task<bool> IsValidAsync()
        {
            return !await _cursoRepository.ExistePorTitulo(_entidade.Titulo, _entidade.Id);
        }
    }
}
