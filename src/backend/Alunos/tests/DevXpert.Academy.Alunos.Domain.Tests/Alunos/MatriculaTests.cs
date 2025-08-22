using DevXpert.Academy.Alunos.Domain.Alunos;
using DevXpert.Academy.Alunos.Domain.Alunos.Events;

namespace DevXpert.Academy.Alunos.Domain.Tests.Alunos
{
    public class MatriculaTests
    {
        private Matricula CriarMatriculaValida(Guid? id = null, Guid? alunoId = null, Guid? cursoId = null, bool concluido = false)
        {
            var mat = new Matricula(id ?? Guid.NewGuid(), alunoId ?? Guid.NewGuid(), cursoId ?? Guid.NewGuid());
            if (concluido)
            {
                mat.EmitirCertificado();
            }
            return mat;
        }

        [Fact(DisplayName = "Validar matrícula com dados consistentes")]
        [Trait("Domain", "Matricula - Construtor")]
        public void Matricula_ValidarMatricula_DeveEstarValida()
        {
            // Arrange
            var matricula = CriarMatriculaValida();

            // Act
            var result = matricula.EhValido();

            // Assert
            Assert.True(result);
            Assert.Empty(matricula.ValidationResult.Errors);
        }

        [Fact(DisplayName = "Validar matrícula com informações esperadas no construtor")]
        [Trait("Domain", "Matricula - Construtor")]
        public void Matricula_ValidarMatricula_DeveTerInformacoesEsperadas()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Guid alunoId = Guid.NewGuid();
            Guid cursoId = Guid.NewGuid();

            // Act
            var matricula = new Matricula(id, alunoId, cursoId);

            // Assert
            Assert.Equal(id, matricula.Id);
            Assert.Equal(alunoId, matricula.AlunoId);
            Assert.Equal(cursoId, matricula.CursoId);
            Assert.False(matricula.Ativa);
            Assert.False(matricula.Concluido);
            Assert.Null(matricula.DataHoraConclusaoDoCurso);
            Assert.Null(matricula.Certificado);
            Assert.NotEqual(default(DateTime), matricula.DataHoraCriacao);
        }

        [Fact(DisplayName = "Construtor de matrícula deve adicionar MatriculaCadastradaEvent")]
        [Trait("Domain", "Matricula - Eventos")]
        public void Matricula_Construtor_DeveAdicionarMatriculaCadastradaEvent()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Guid alunoId = Guid.NewGuid();
            Guid cursoId = Guid.NewGuid();

            // Act
            var matricula = new Matricula(id, alunoId, cursoId);

            // Assert
            var domainEvent = matricula.Notifications.OfType<MatriculaCadastradaEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(id, domainEvent.MatriculaId);
            Assert.Equal(alunoId, domainEvent.AggregateId);
            Assert.Equal(cursoId, domainEvent.CursoId);
        }

        [Fact(DisplayName = "Ativar deve definir Ativa como true e adicionar MatriculaAtivadaEvent")]
        [Trait("Domain", "Matricula - Métodos")]
        public void Matricula_Ativar_DeveDefinirAtivaComoTrueEAdicionarEvento()
        {
            // Arrange
            var matricula = CriarMatriculaValida();
            Assert.False(matricula.Ativa);

            // Act
            matricula.Ativar();

            // Assert
            Assert.True(matricula.Ativa);
            var domainEvent = matricula.Notifications.OfType<MatriculaAtivadaEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(matricula.Id, domainEvent.MatriculaId);
        }

        [Fact(DisplayName = "Bloquear deve definir Ativa como false e adicionar MatriculaBloqueadaEvent")]
        [Trait("Domain", "Matricula - Métodos")]
        public void Matricula_Bloquear_DeveDefinirAtivaComoFalseEAdicionarEvento()
        {
            // Arrange
            var matricula = CriarMatriculaValida();
            matricula.Ativar();
            Assert.True(matricula.Ativa);

            // Act
            matricula.Bloquear();

            // Assert
            Assert.False(matricula.Ativa);
            var domainEvent = matricula.Notifications.OfType<MatriculaBloqueadaEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(matricula.Id, domainEvent.MatriculaId);
        }

        [Fact(DisplayName = "Concluir deve definir Concluido como true, DataHoraConclusao e Certificado e adicionar MatriculaConcluidaEvent")]
        [Trait("Domain", "Matricula - Métodos")]
        public void Matricula_Concluir_DeveDefinirConcluidoDataHoraConclusaoCertificadoEAdicionarEvento()
        {
            // Arrange
            var matricula = CriarMatriculaValida();
            Assert.False(matricula.Concluido);
            Assert.Null(matricula.DataHoraConclusaoDoCurso);
            Assert.Null(matricula.Certificado);

            // Act
            var dataHoraAntesConclusao = DateTime.Now;
            matricula.EmitirCertificado();

            // Assert
            Assert.True(matricula.Concluido);
            Assert.NotNull(matricula.DataHoraConclusaoDoCurso);
            Assert.True(matricula.DataHoraConclusaoDoCurso.Value >= dataHoraAntesConclusao);
            Assert.NotNull(matricula.Certificado);
            Assert.Equal(matricula.DataHoraConclusaoDoCurso.Value, matricula.Certificado.DataHoraEmissao);

            var domainEvent = matricula.Notifications.OfType<MatriculaCursoConcluidoEvent>().FirstOrDefault();
            Assert.NotNull(domainEvent);
            Assert.Equal(matricula.Id, domainEvent.MatriculaId);
            Assert.Equal(matricula.AlunoId, domainEvent.AggregateId);
            Assert.Equal(matricula.CursoId, domainEvent.CursoId);
            Assert.Equal(matricula.DataHoraConclusaoDoCurso.Value, domainEvent.DataHoraConclusao);
        }

        [Fact(DisplayName = "Matrícula deve ser inválida quando AlunoId é vazio")]
        [Trait("Domain", "Matricula - Validação")]
        public void Matricula_AlunoIdVazio_DeveSerInvalida()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            // Act
            var valido = matricula.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(matricula.ValidationResult.Errors, e => e.ErrorMessage == "O aluno deve ser informado.");
        }

        [Fact(DisplayName = "Matrícula deve ser inválida quando CursoId é vazio")]
        [Trait("Domain", "Matricula - Validação")]
        public void Matricula_CursoIdVazio_DeveSerInvalida()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

            // Act
            var valido = matricula.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(matricula.ValidationResult.Errors, e => e.ErrorMessage == "O curso deve ser informado.");
        }

        [Fact(DisplayName = "Matrícula concluída deve ser inválida sem DataHoraConclusao")]
        [Trait("Domain", "Matricula - Validação")]
        public void Matricula_ConcluidaSemDataConclusao_DeveSerInvalida()
        {
            // Arrange
            var matricula = CriarMatriculaValida();
            matricula.EmitirCertificado();
            var matriculaInvalida = new Matricula(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            typeof(Matricula).GetProperty(nameof(Matricula.Concluido))?.SetValue(matriculaInvalida, true);
            typeof(Matricula).GetProperty(nameof(Matricula.DataHoraConclusaoDoCurso))?.SetValue(matriculaInvalida, null);
            typeof(Matricula).GetProperty(nameof(Matricula.Certificado))?.SetValue(matriculaInvalida, null);

            // Act
            var valido = matriculaInvalida.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(matriculaInvalida.ValidationResult.Errors, e => e.ErrorMessage == "A data de conclusão deve ser informada oara cursos concluídos.");
            Assert.Contains(matriculaInvalida.ValidationResult.Errors, e => e.ErrorMessage == "O certificado deve ser gerado para cursos concluídos.");
        }

        [Fact(DisplayName = "Matrícula não concluída deve ser inválida se tiver DataHoraConclusao")]
        [Trait("Domain", "Matricula - Validação")]
        public void Matricula_NaoConcluidaComDataConclusao_DeveSerInvalida()
        {
            // Arrange
            var matriculaInvalida = CriarMatriculaValida();
            typeof(Matricula).GetProperty(nameof(Matricula.Concluido))?.SetValue(matriculaInvalida, false);
            typeof(Matricula).GetProperty(nameof(Matricula.DataHoraConclusaoDoCurso))?.SetValue(matriculaInvalida, DateTime.Now);

            // Act
            var valido = matriculaInvalida.EhValido();

            // Assert
            Assert.False(valido);
            Assert.Contains(matriculaInvalida.ValidationResult.Errors, e => e.ErrorMessage == "A data de conclusão não deve ser informada para cursos não concluídos.");
        }
    }
}