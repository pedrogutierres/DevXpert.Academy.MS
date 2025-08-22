using DevXpert.Academy.Conteudo.Domain.Cursos.Events;
using System.Linq;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Adapters
{
    internal static class CursoAdapter
    {
        public static CursoCadastradoEvent ToCursoCadastradoEvent(Curso curso)
        {
            return new CursoCadastradoEvent(curso.Id, curso.Titulo, curso.ConteudoProgramatico?.Descricao, curso.ConteudoProgramatico?.CargaHoraria ?? 0, curso.Valor, curso.Aulas?.Select(a => new AulaCadastradaEvent(curso.Id, a.Id, a.Titulo, a.VideoUrl)).ToList());
        }
    }
}
