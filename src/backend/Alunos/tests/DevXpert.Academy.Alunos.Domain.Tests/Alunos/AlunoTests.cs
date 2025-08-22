using DevXpert.Academy.Alunos.Domain.Alunos;
using DevXpert.Academy.Alunos.Domain.Alunos.Events;
using DevXpert.Academy.Alunos.Domain.Alunos.Validations;
using DevXpert.Academy.Core.Domain.Exceptions;

namespace DevXpert.Academy.Alunos.Domain.Tests.Alunos
{
    public class AlunoTests
    {
        private Aluno CriarAlunoValido(Guid? id = null, string nome = null)
        {
            return new Aluno(id ?? Guid.NewGuid(), nome ?? $"Aluno Teste {Guid.NewGuid().ToString().Substring(0, 4)}");
        }

        private Matricula CriarMatriculaValida(Guid alunoId, Guid cursoId)
        {
            return new Matricula(Guid.NewGuid(), alunoId, cursoId);
        }

        [Fact(DisplayName = "Validar aluno com dados consistentes")]
        [Trait("Domain", "Alunos - Construtor")]
        public void Alunos_ValidarAluno_DeveEstarValido()
        {
            // Arrange
            var aluno = CriarAlunoValido();

            // Act
            var result = aluno.EhValido();

            // Assert
            Assert.True(result);
            Assert.Empty(aluno.ValidationResult.Errors);
        }

        [Fact(DisplayName = "Validar aluno com informações esperadas no construtor")]
        [Trait("Domain", "Alunos - Construtor")]
        public void Alunos_ValidarAluno_DeveTerInformacoesEsperadas()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string nome = "Maria da Silva";

            // Act
            var aluno = new Aluno(id, nome);

            // Assert
            Assert.Equal(id, aluno.Id);
            Assert.Equal(nome, aluno.Nome);
        }

        [Fact(DisplayName = "Construtor de aluno deve adicionar AlunoCadastradoEvent")]
        [Trait("Domain", "Alunos - Eventos")]
        public void Aluno_Construtor_DeveAdicionarAlunoCadastradoEvent()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string nome = "João Pedro";

            // Act
            var aluno = new Aluno(id, nome);

            // Assert
            var domainEvent = aluno.Notifications.OfType<AlunoCadastradoEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(id, domainEvent.AggregateId);
            Assert.Equal(nome, domainEvent.Nome);
        }

        [Fact(DisplayName = "AlterarNome deve atualizar o nome do aluno")]
        [Trait("Domain", "Alunos - Métodos")]
        public void Aluno_AlterarNome_DeveAtualizarNome()
        {
            // Arrange
            var aluno = CriarAlunoValido(nome: "Nome Antigo");
            var novoNome = "Nome Novo do Aluno";

            // Act
            aluno.AlterarNome(novoNome);

            // Assert
            Assert.Equal(novoNome, aluno.Nome);
        }

        [Fact(DisplayName = "AlterarNome deve adicionar AlunoNomeAlteradoEvent")]
        [Trait("Domain", "Alunos - Eventos")]
        public void Aluno_AlterarNome_DeveAdicionarAlunoNomeAlteradoEvent()
        {
            // Arrange
            var aluno = CriarAlunoValido(nome: "Nome Original");
            var novoNome = "Nome Atualizado";

            // Act
            aluno.AlterarNome(novoNome);

            // Assert
            var domainEvent = aluno.Notifications.OfType<AlunoNomeAlteradoEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(aluno.Id, domainEvent.AggregateId);
            Assert.Equal(novoNome, domainEvent.Nome);
        }

        [Fact(DisplayName = "Matricular deve adicionar matrícula à lista e evento")]
        [Trait("Domain", "Alunos - Métodos")]
        public void Aluno_Matricular_DeveAdicionarMatriculaAListaEEvento()
        {
            // Arrange
            var aluno = CriarAlunoValido();
            var cursoId = Guid.NewGuid();
            var matricula = CriarMatriculaValida(aluno.Id, cursoId);

            // Act
            aluno.Matricular(matricula);

            // Assert
            Assert.Contains(aluno.Matriculas, m => m.Id == matricula.Id);
            Assert.Single(aluno.Matriculas);
            var domainEvent = aluno.Notifications.OfType<MatriculaVinculadaAoAlunoEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(matricula.Id, domainEvent.MatriculaId);
            Assert.Equal(aluno.Id, domainEvent.AggregateId);
            Assert.Equal(cursoId, domainEvent.CursoId);
        }

        [Fact(DisplayName = "Matricular deve lançar BusinessException se já matriculado no curso")]
        [Trait("Domain", "Alunos - Métodos")]
        public void Aluno_Matricular_DeveLancarBusinessExceptionSeJaMatriculado()
        {
            // Arrange
            var aluno = CriarAlunoValido();
            var cursoId = Guid.NewGuid();
            var matriculaExistente = CriarMatriculaValida(aluno.Id, cursoId);
            aluno.Matricular(matriculaExistente);

            var novaMatriculaMesmoCurso = CriarMatriculaValida(aluno.Id, cursoId);

            // Act & Assert
            var exception = Assert.Throws<BusinessException>(() => aluno.Matricular(novaMatriculaMesmoCurso));
            Assert.Equal("Você já está matriculado neste curso.", exception.Message);
            Assert.Single(aluno.Matriculas);
        }

        [Fact(DisplayName = "EstaMatriculado deve retornar true se o aluno estiver matriculado no curso")]
        [Trait("Domain", "Alunos - Métodos")]
        public void Aluno_EstaMatriculado_DeveRetornarTrueSeMatriculado()
        {
            // Arrange
            var aluno = CriarAlunoValido();
            var cursoId = Guid.NewGuid();
            var matricula = CriarMatriculaValida(aluno.Id, cursoId);
            aluno.Matricular(matricula);

            // Act
            var estaMatriculado = aluno.EstaMatriculado(cursoId);

            // Assert
            Assert.True(estaMatriculado);
        }

        [Fact(DisplayName = "EstaMatriculado deve retornar false se o aluno não estiver matriculado no curso")]
        [Trait("Domain", "Alunos - Métodos")]
        public void Aluno_EstaMatriculado_DeveRetornarFalseSeNaoMatriculado()
        {
            // Arrange
            var aluno = CriarAlunoValido();
            var cursoId = Guid.NewGuid();

            // Act
            var estaMatriculado = aluno.EstaMatriculado(cursoId);

            // Assert
            Assert.False(estaMatriculado);
        }

        [Fact(DisplayName = "Aluno deve ser inválido quando o nome é nulo ou vazio")]
        [Trait("Domain", "Alunos - Validação")]
        public void Aluno_NomeInvalido_DeveSerInvalido()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), string.Empty);

            // Act
            var valido = aluno.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(aluno.ValidationResult.Errors, e => e.ErrorMessage == "O nome do aluno deve ser informado.");
        }

        [Fact(DisplayName = "Aluno deve ser inválido quando o nome excede o limite de caracteres")]
        [Trait("Domain", "Alunos - Validação")]
        public void Aluno_NomeMuitoLongo_DeveSerInvalido()
        {
            // Arrange
            var nomeMuitoLongo = new string('X', 201);
            var aluno = new Aluno(Guid.NewGuid(), nomeMuitoLongo);

            // Act
            var valido = aluno.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(aluno.ValidationResult.Errors, e => e.ErrorMessage == "O nome do aluno deve conter no máximo 200 caracteres.");
        }

        [Fact(DisplayName = "EhValido deve retornar false se alguma matrícula for inválida")]
        [Trait("Domain", "Alunos - Validação Aninhada")]
        public void Aluno_EhValido_DeveRetornarFalseSeMatriculaInvalida()
        {
            // Arrange
            var aluno = CriarAlunoValido();
            var matriculaInvalida = new Matricula(Guid.NewGuid(), Guid.Empty, Guid.Empty);
            aluno.Matricular(matriculaInvalida);

            // Act
            var resultado = aluno.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(aluno.ValidationResult.Errors, e => e.ErrorMessage == "O aluno deve ser informado.");
            Assert.Contains(aluno.ValidationResult.Errors, e => e.ErrorMessage == "O curso deve ser informado.");
        }
    }
}