using DevXpert.Academy.Conteudo.Domain.Cursos;
using DevXpert.Academy.Conteudo.Domain.Cursos.Events;
using DevXpert.Academy.Conteudo.Domain.Cursos.ValuesObjects;

namespace DevXpert.Academy.Conteudo.Domain.Tests.Cursos
{
    public class CursoTests
    {
        [Fact(DisplayName = "Validar curso com dados consistentes")]
        [Trait("Domain", "Cursos - Construtor")]
        public void Cursos_ValidarCurso_DeveEstarValido()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var aulas = new List<Aula>
            {
                new(Guid.NewGuid(), id, "Aula 1", "https://www.youtube.com/watch?v=1"),
                new(Guid.NewGuid(), id, "Aula 2", "https://www.youtube.com/watch?v=2")
            };

            var curso = new Curso(id, "Curso de ASP.NET", new ConteudoProgramatico("CURSO PARA ASP.NET CORE", 20), 100, aulas);

            // Act
            var result = curso.EhValido();

            // Assert
            Assert.True(result);
            Assert.Equal(aulas.Count, curso.Aulas?.Count ?? 0);
            Assert.Empty(curso.ValidationResult.Errors);
        }

        [Fact(DisplayName = "Validar curso com informações esperadas")]
        [Trait("Domain", "Cursos - Construtor")]
        public void Cursos_ValidarCurso_DeveTerInformacoesEsperadas()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string titulo = "Curso de ASP.NET";
            string descricao = "CURSO PARA ASP.NET CORE";
            int cargaHoraria = 20;

            // Act
            var curso = new Curso(id, titulo, new ConteudoProgramatico(descricao, cargaHoraria), 100, null);
            curso.Ativar();

            // Assert
            Assert.Equal(id, curso.Id);
            Assert.Equal(titulo, curso.Titulo);
            Assert.Equal(descricao, curso.ConteudoProgramatico?.Descricao);
            Assert.Equal(cargaHoraria, curso.ConteudoProgramatico?.CargaHoraria ?? 0);
            Assert.True(curso.Ativo);
        }

        [Fact(DisplayName = "Validar curso deve entrar como inativado por padrão")]
        [Trait("Domain", "Cursos - Construtor")]
        public void Cursos_ValidarCurso_DeveEntrarComoInativado()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Curso de ASP.NET", new ConteudoProgramatico("CURSO PARA ASP.NET CORE", 20), 100, null);

            // Act

            // Assert
            Assert.False(curso.Ativo);
        }

        [Fact(DisplayName = "Validar aula com informações esperadas")]
        [Trait("Domain", "Aulas - Construtor")]
        public void Aulas_ValidarAula_DeveTerInformacoesEsperadas()
        {
            // Arrange
            Guid cursoId = Guid.NewGuid();
            Guid id = Guid.NewGuid();
            string titulo = "Aula 1";
            string videoUrl = "https://www.youtube.com/watch?v=1";

            // Act
            var aula = new Aula(id, cursoId, titulo, videoUrl);

            // Assert
            Assert.Equal(id, aula.Id);
            Assert.Equal(cursoId, aula.CursoId);
            Assert.Equal(titulo, aula.Titulo);
            Assert.Equal(videoUrl, aula.VideoUrl);
        }

        [Fact(DisplayName = "AlterarTitulo deve atualizar o título")]
        [Trait("Domain", "Cursos - Métodos")]
        public void Curso_AlterarTitulo_DeveAtualizarTitulo()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Titulo Antigo", new ConteudoProgramatico("Desc", 10), 100, null);
            var novoTitulo = "Novo Titulo do Curso";

            // Act
            curso.AlterarTitulo(novoTitulo);

            // Assert
            Assert.Equal(novoTitulo, curso.Titulo);
        }

        [Fact(DisplayName = "AlterarConteudoProgramatico deve atualizar o conteúdo programático")]
        [Trait("Domain", "Cursos - Métodos")]
        public void Curso_AlterarConteudoProgramatico_DeveAtualizarConteudo()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Titulo", new ConteudoProgramatico("Desc Antiga", 10), 100, null);
            var novoConteudo = new ConteudoProgramatico("Nova Descricao", 50);

            // Act
            curso.AlterarConteudoProgramatico(novoConteudo);

            // Assert
            Assert.Equal(novoConteudo, curso.ConteudoProgramatico);
        }

        [Fact(DisplayName = "Ativar deve definir Ativo como true")]
        [Trait("Domain", "Cursos - Métodos")]
        public void Curso_Ativar_DeveDefinirAtivoComoTrue()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Titulo", new ConteudoProgramatico("Desc", 10), 100, null);
            Assert.False(curso.Ativo);

            // Act
            curso.Ativar();

            // Assert
            Assert.True(curso.Ativo);
        }

        [Fact(DisplayName = "Inativar deve definir Ativo como false")]
        [Trait("Domain", "Cursos - Métodos")]
        public void Curso_Inativar_DeveDefinirAtivoComoFalse()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Titulo", new ConteudoProgramatico("Desc", 10), 100, null);
            curso.Ativar();
            Assert.True(curso.Ativo);

            // Act
            curso.Inativar();

            // Assert
            Assert.False(curso.Ativo);
        }

        [Fact(DisplayName = "AdicionarAula deve adicionar a aula à lista")]
        [Trait("Domain", "Cursos - Métodos")]
        public void Curso_AdicionarAula_DeveAdicionarAulaALista()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Curso de ASP.NET", new ConteudoProgramatico("CURSO PARA ASP.NET CORE", 20), 100, null);
            var aula = new Aula(Guid.NewGuid(), curso.Id, "Aula 1", "https://www.youtube.com/watch?v=1");

            // Act
            curso.AdicionarAula(aula);

            // Assert
            Assert.Contains(curso.Aulas, e => e.Id == aula.Id);
            Assert.Single(curso.Aulas);
        }

        [Fact(DisplayName = "RemoverAula deve remover a aula da lista")]
        [Trait("Domain", "Cursos - Métodos")]
        public void Curso_RemoverAula_DeveRemoverAulaDaLista()
        {
            // Arrange
            Guid cursoId = Guid.NewGuid();
            var aula = new Aula(Guid.NewGuid(), cursoId, "Aula 1", "https://www.youtube.com/watch?v=1");
            var curso = new Curso(cursoId, "Curso de ASP.NET", new ConteudoProgramatico("CURSO PARA ASP.NET CORE", 20), 100, [aula]);

            // Act
            curso.RemoverAula(aula);

            // Assert
            Assert.DoesNotContain(curso.Aulas, e => e.Id == aula.Id);
            Assert.Empty(curso.Aulas);
        }

        [Fact(DisplayName = "RemoverAula deve inativar o curso se a lista de aulas ficar vazia")]
        [Trait("Domain", "Cursos - Métodos")]
        public void Curso_RemoverAula_DeveInativarCursoSeListaFicarVazia()
        {
            // Arrange
            Guid cursoId = Guid.NewGuid();
            var aula = new Aula(Guid.NewGuid(), cursoId, "Aula 1", "https://www.youtube.com/watch?v=1");
            var curso = new Curso(cursoId, "Curso de ASP.NET", new ConteudoProgramatico("CURSO PARA ASP.NET CORE", 20), 100, [aula]);
            curso.Ativar();
            Assert.True(curso.Ativo);

            // Act
            curso.RemoverAula(aula);

            // Assert
            Assert.False(curso.Ativo); // Verifica se o curso foi inativado
        }

        [Fact(DisplayName = "Validar curso não pode ser ativado sem aulas")]
        [Trait("Domain", "Cursos - Validação")]
        public void Cursos_ValidarCurso_DeveNaoPodeSerAtivadoSemAulas()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Curso de ASP.NET", new ConteudoProgramatico("CURSO PARA ASP.NET CORE", 20), 100, null);

            // Act
            curso.Ativar();
            curso.EhValido();

            // Assert
            Assert.False(curso.ValidationResult.IsValid);
            Assert.Contains(curso.ValidationResult.Errors, e => e.ErrorMessage == $"O curso {curso.Titulo} deve ter aulas para ser ativado.");
        }

        [Fact(DisplayName = "Curso deve ser inválido quando o título é nulo ou vazio")]
        [Trait("Domain", "Cursos - Validação")]
        public void Curso_TituloInvalido_DeveSerInvalido()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), string.Empty, new ConteudoProgramatico("Descrição", 10), 100, null);

            // Act
            var valido = curso.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(curso.ValidationResult.Errors, e => e.ErrorMessage == "O título do curso deve ser informado.");
        }

        [Fact(DisplayName = "Curso deve ser inválido quando o título excede o limite de caracteres")]
        [Trait("Domain", "Cursos - Validação")]
        public void Curso_TituloMuitoLongo_DeveSerInvalido()
        {
            // Arrange
            var tituloMuitoLongo = new string('A', 101); // 101 caracteres
            var curso = new Curso(Guid.NewGuid(), tituloMuitoLongo, new ConteudoProgramatico("Descrição", 10), 100, null);

            // Act
            var valido = curso.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(curso.ValidationResult.Errors, e => e.ErrorMessage == "O título do curso deve conter no máximo 100 caracteres.");
        }

        [Fact(DisplayName = "Curso deve ser inválido quando o ConteudoProgramatico é nulo")]
        [Trait("Domain", "Cursos - Validação")]
        public void Curso_ConteudoProgramaticoNulo_DeveSerInvalido()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Título Válido", null, 100, null);

            // Act
            var valido = curso.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(curso.ValidationResult.Errors, e => e.ErrorMessage == $"O conteúdo programático do curso {curso.Titulo} deve ser informado.");
        }

        [Fact(DisplayName = "Curso deve ser inválido quando a descrição do ConteudoProgramatico é nula ou vazia")]
        [Trait("Domain", "Cursos - Validação")]
        public void Curso_ConteudoProgramaticoDescricaoInvalida_DeveSerInvalido()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Título Válido", new ConteudoProgramatico(string.Empty, 10), 100, null);

            // Act
            var valido = curso.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(curso.ValidationResult.Errors, e => e.ErrorMessage == $"A descrição do curso {curso.Titulo} deve ser informada.");
        }

        [Theory(DisplayName = "Curso deve ser inválido quando a CargaHoraria do ConteudoProgramatico está fora do intervalo")]
        [Trait("Domain", "Cursos - Validação")]
        [InlineData(0)]
        [InlineData(1001)]
        public void Curso_ConteudoProgramaticoCargaHorariaForaDoIntervalo_DeveSerInvalido(int cargaHorariaInvalida)
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Título Válido", new ConteudoProgramatico("Descrição Válida", cargaHorariaInvalida), 100, null);

            // Act
            var valido = curso.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(curso.ValidationResult.Errors, e => e.ErrorMessage == $"A carga horária do curso {curso.Titulo} deve ser estar entre 1h e 1000hs.");
        }

        [Fact(DisplayName = "EhValido deve retornar false se alguma aula não for válida")]
        [Trait("Domain", "Cursos - Validação Aninhada")]
        public void Curso_EhValido_DeveRetornarFalseSeAulaInvalida()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var aulaInvalida = new Aula(Guid.NewGuid(), id, "", "");
            var aulas = new List<Aula> { aulaInvalida };

            var curso = new Curso(id, "Curso de Teste", new ConteudoProgramatico("Desc", 20), 100, aulas);
            curso.Ativar();

            // Act
            var resultado = curso.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(curso.ValidationResult.Errors, e => e.ErrorMessage == "O título da aula deve ser informado.");
            Assert.Contains(curso.ValidationResult.Errors, e => e.ErrorMessage == "A URL da vídeo aula  deve ser informada.");
        }

        [Fact(DisplayName = "EhValido deve retornar false se o título da aula for muito grande")]
        [Trait("Domain", "Cursos - Validação Aninhada")]
        public void Curso_EhValido_DeveRetornarFalseSeTituloDaAulaForMuitoGrande()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var aulaInvalida = new Aula(Guid.NewGuid(), id, "Titulo grande - Titulo grande - Titulo grande - Titulo grande - Titulo grande - Titulo grande - Titulo grande", "");
            var aulas = new List<Aula> { aulaInvalida };

            var curso = new Curso(id, "Curso de Teste", new ConteudoProgramatico("Desc", 20), 100, aulas);
            curso.Ativar();

            // Act
            var resultado = curso.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(curso.ValidationResult.Errors, e => e.ErrorMessage == "O título da aula deve conter no máximo 100 caracteres.");
        }

        [Fact(DisplayName = "Construtor deve adicionar CursoCadastradoEvent")]
        [Trait("Domain", "Cursos - Eventos")]
        public void Curso_Construtor_DeveAdicionarCursoCadastradoEvent()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string titulo = "Curso Evento";
            ConteudoProgramatico conteudo = new("Descrição Evento", 10);
            List<Aula> aulas = new();

            // Act
            var curso = new Curso(id, titulo, conteudo, 100, aulas);

            // Assert
            var domainEvent = curso.Notifications.OfType<CursoCadastradoEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(id, domainEvent.AggregateId);
            Assert.Equal(titulo, domainEvent.Titulo);
            Assert.Equal(conteudo.Descricao, domainEvent.Descricao);
            Assert.Equal(conteudo.CargaHoraria, domainEvent.CargaHoraria);
            Assert.Equal(curso.Valor, domainEvent.Valor);
            Assert.Equal(curso.Aulas.Count, domainEvent.Aulas?.Count ?? 0);
        }

        [Fact(DisplayName = "AlterarTitulo deve adicionar CursoTituloAlteradoEvent")]
        [Trait("Domain", "Cursos - Eventos")]
        public void Curso_AlterarTitulo_DeveAdicionarCursoTituloAlteradoEvent()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Titulo Antigo", new ConteudoProgramatico("Desc", 10), 100, null);
            var novoTitulo = "Novo Titulo do Curso";

            // Act
            curso.AlterarTitulo(novoTitulo);

            // Assert
            var domainEvent = curso.Notifications.OfType<CursoTituloAlteradoEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(curso.Id, domainEvent.AggregateId);
            Assert.Equal(novoTitulo, domainEvent.Titulo);
        }

        [Fact(DisplayName = "AlterarConteudoProgramatico deve adicionar CursoConteudoProgramaticoAlteradoEvent")]
        [Trait("Domain", "Cursos - Eventos")]
        public void Curso_AlterarConteudoProgramatico_DeveAdicionarCursoConteudoProgramaticoAlteradoEvent()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Titulo", new ConteudoProgramatico("Desc Antiga", 10), 100, null);
            var novoConteudo = new ConteudoProgramatico("Nova Descricao", 50);

            // Act
            curso.AlterarConteudoProgramatico(novoConteudo);

            // Assert
            var domainEvent = curso.Notifications.OfType<CursoConteudoProgramaticoAlteradoEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(curso.Id, domainEvent.AggregateId);
            Assert.Equal(novoConteudo.Descricao, domainEvent.Descricao);
            Assert.Equal(novoConteudo.CargaHoraria, domainEvent.CargaHoraria);
        }

        [Fact(DisplayName = "Ativar deve adicionar CursoAtivadoEvent")]
        [Trait("Domain", "Cursos - Eventos")]
        public void Curso_Ativar_DeveAdicionarCursoAtivadoEvent()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Titulo", new ConteudoProgramatico("Desc", 10), 100, null);
            
            // Act
            curso.Ativar();

            // Assert
            var domainEvent = curso.Notifications.OfType<CursoAtivadoEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(curso.Id, domainEvent.AggregateId);
        }

        [Fact(DisplayName = "Inativar deve adicionar CursoInativadoEvent")]
        [Trait("Domain", "Cursos - Eventos")]
        public void Curso_Inativar_DeveAdicionarCursoInativadoEvent()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Titulo", new ConteudoProgramatico("Desc", 10), 100, null);
            curso.Ativar();
            Assert.True(curso.Ativo);

            // Act
            curso.Inativar();

            // Assert
            var domainEvent = curso.Notifications.OfType<CursoInativadoEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(curso.Id, domainEvent.AggregateId);
        }

        [Fact(DisplayName = "AdicionarAula deve adicionar AulaCadastradaEvent")]
        [Trait("Domain", "Cursos - Eventos")]
        public void Curso_AdicionarAula_DeveAdicionarAulaCadastradaEvent()
        {
            // Arrange
            var curso = new Curso(Guid.NewGuid(), "Curso de ASP.NET", new ConteudoProgramatico("CURSO PARA ASP.NET CORE", 20), 100, null);
            var aula = new Aula(Guid.NewGuid(), curso.Id, "Aula 1", "https://www.youtube.com/watch?v=1");

            // Act
            curso.AdicionarAula(aula);

            // Assert
            var domainEvent = curso.Notifications.OfType<AulaCadastradaEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(curso.Id, domainEvent.AggregateId);
            Assert.Equal(aula.Id, domainEvent.Id);
            Assert.Equal(aula.Titulo, domainEvent.Titulo);
            Assert.Equal(aula.VideoUrl, domainEvent.VideoUrl);
        }

        [Fact(DisplayName = "RemoverAula deve adicionar AulaExcluidaEvent")]
        [Trait("Domain", "Cursos - Eventos")]
        public void Curso_RemoverAula_DeveAdicionarAulaExcluidaEvent()
        {
            // Arrange
            Guid cursoId = Guid.NewGuid();
            var aula = new Aula(Guid.NewGuid(), cursoId, "Aula 1", "https://www.youtube.com/watch?v=1");
            var curso = new Curso(cursoId, "Curso de ASP.NET", new ConteudoProgramatico("CURSO PARA ASP.NET CORE", 20), 100, [aula]);

            // Act
            curso.RemoverAula(aula);

            // Assert
            var domainEvent = curso.Notifications.OfType<AulaExcluidaEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(curso.Id, domainEvent.AggregateId);
            Assert.Equal(aula.Id, domainEvent.Id);
        }

        [Fact(DisplayName = "RemoverAula deve adicionar CursoInativadoEvent se a lista de aulas ficar vazia")]
        [Trait("Domain", "Cursos - Eventos")]
        public void Curso_RemoverAula_DeveAdicionarCursoInativadoEventQuandoUltimaAulaRemovida()
        {
            // Arrange
            Guid cursoId = Guid.NewGuid();
            var aula = new Aula(Guid.NewGuid(), cursoId, "Aula 1", "https://www.youtube.com/watch?v=1");
            var curso = new Curso(cursoId, "Curso de ASP.NET", new ConteudoProgramatico("CURSO PARA ASP.NET CORE", 20), 100, [aula]);
            curso.Ativar(); // Ativa para garantir que o evento de inativação seja disparado
            Assert.True(curso.Ativo);

            // Act
            curso.RemoverAula(aula);

            // Assert
            var inativadoEvent = curso.Notifications.OfType<CursoInativadoEvent>().FirstOrDefault();
            Assert.NotNull(inativadoEvent);
            Assert.Equal(curso.Id, inativadoEvent.AggregateId);
        }
    }
}