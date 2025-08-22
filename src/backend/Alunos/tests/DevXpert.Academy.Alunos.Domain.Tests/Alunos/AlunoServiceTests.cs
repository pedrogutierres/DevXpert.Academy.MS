using DevXpert.Academy.Alunos.Domain.Alunos;
using DevXpert.Academy.Alunos.Domain.Alunos.Events;
using DevXpert.Academy.Alunos.Domain.Alunos.Interfaces;
using DevXpert.Academy.Alunos.Domain.Alunos.Services;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.Exceptions;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DevXpert.Academy.Alunos.Domain.Tests.Alunos
{
    public class AlunoServiceTests
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
        private readonly AlunoService _alunoService;

        public AlunoServiceTests()
        {
            _mocker = new AutoMocker();

            _alunoRepositoryMock = _mocker.GetMock<IAlunoRepository>();
            _mediatorHandlerMock = _mocker.GetMock<IMediatorHandler>();
            _mocker.Use<INotificationHandler<DomainNotification>>(new DomainNotificationHandler());

            _alunoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(true);

            _alunoService = _mocker.CreateInstance<AlunoService>();
        }

        private Aluno CriarAlunoValido(Guid? id = null, string nome = null, List<Matricula> matriculas = null)
        {
            var aluno = new Aluno(id ?? Guid.NewGuid(), nome ?? $"Aluno Teste {Guid.NewGuid().ToString().Substring(0, 4)}");
            return aluno;
        }

        private Matricula CriarMatriculaValida(Guid alunoId, Guid cursoId, Guid? id = null, bool ativa = false, bool concluido = false)
        {
            var matricula = new Matricula(id ?? Guid.NewGuid(), alunoId, cursoId);
            if (ativa)
            {
                matricula.Ativar();
            }
            if (concluido)
            {
                matricula.EmitirCertificado();
            }
            return matricula;
        }

        [Fact(DisplayName = "Cadastrar Aluno deve retornar true e chamar repositório e commit quando válido")]
        [Trait("Category", "AlunoService - Cadastrar")]
        public async Task Cadastrar_AlunoValido_DeveRetornarTrueChamarRepositorioECommit()
        {
            // Arrange
            var aluno = CriarAlunoValido();

            // Act
            var result = await _alunoService.Cadastrar(aluno);

            // Assert
            Assert.True(result);
            _alunoRepositoryMock.Verify(r => r.Cadastrar(aluno), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Cadastrar Aluno deve retornar false se a entidade for inválida")]
        [Trait("Category", "AlunoService - Cadastrar")]
        public async Task Cadastrar_EntidadeInvalida_DeveRetornarFalse()
        {
            // Arrange
            var alunoInvalido = new Aluno(Guid.NewGuid(), string.Empty);

            // Act
            var result = await _alunoService.Cadastrar(alunoInvalido);

            // Assert
            Assert.False(result);
            _alunoRepositoryMock.Verify(r => r.Cadastrar(It.IsAny<Aluno>()), Times.Never);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "O nome do aluno deve ser informado."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cadastrar Aluno deve retornar false se o commit falhar")]
        [Trait("Category", "AlunoService - Cadastrar")]
        public async Task Cadastrar_CommitFalhar_DeveRetornarFalse()
        {
            // Arrange
            var aluno = CriarAlunoValido();
            _alunoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(false);

            // Act
            var result = await _alunoService.Cadastrar(aluno);

            // Assert
            Assert.False(result);
            _alunoRepositoryMock.Verify(r => r.Cadastrar(aluno), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Não foi identificada nenhuma alteração nos dados."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "ExcluirNovoCadastro deve retornar true e chamar repositório e commit")]
        [Trait("Category", "AlunoService - Excluir")]
        public async Task ExcluirNovoCadastro_DeveRetornarTrueChamarRepositorioECommit()
        {
            // Arrange
            var alunoId = Guid.NewGuid();

            // Act
            var result = await _alunoService.ExcluirNovoCadastro(alunoId);

            // Assert
            Assert.True(result);
            _alunoRepositoryMock.Verify(r => r.Excluir(alunoId), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Matricular deve retornar nova matrícula e chamar commit quando aluno e curso são válidos e não há matrícula existente")]
        [Trait("Category", "AlunoService - Matricular")]
        public async Task Matricular_AlunoECursoValidosSemMatriculaExistente_DeveRetornarMatriculaEChamarCommit()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var aluno = CriarAlunoValido(alunoId, matriculas: new List<Matricula>()); 
            _alunoRepositoryMock.Setup(r => r.ObterPorId(alunoId, true)).ReturnsAsync(aluno);

            // Act
            var result = await _alunoService.Matricular(alunoId, cursoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(alunoId, result.AlunoId);
            Assert.Equal(cursoId, result.CursoId);
            Assert.Single(aluno.Matriculas); 
            _alunoRepositoryMock.Verify(r => r.ObterPorId(alunoId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Matricular deve retornar matrícula existente se já matriculado e ativa")]
        [Trait("Category", "AlunoService - Matricular")]
        public async Task Matricular_AlunoJaMatriculadoEAtiva_DeveRetornarMatriculaExistente()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaExistente = CriarMatriculaValida(alunoId, cursoId, ativa: true);
            var aluno = CriarAlunoValido(alunoId);
            // Use reflection or a test-specific method to add the matricula directly for setup
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matriculaExistente });
            _alunoRepositoryMock.Setup(r => r.ObterPorId(alunoId, true)).ReturnsAsync(aluno);

            // Act
            var result = await _alunoService.Matricular(alunoId, cursoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(matriculaExistente.Id, result.Id);
            _alunoRepositoryMock.Verify(r => r.ObterPorId(alunoId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never); 
        }

        [Fact(DisplayName = "Matricular deve lançar BusinessException se aluno não for encontrado")]
        [Trait("Category", "AlunoService - Matricular")]
        public async Task Matricular_AlunoNaoEncontrado_DeveLancarBusinessException()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            _alunoRepositoryMock.Setup(r => r.ObterPorId(alunoId, true)).ReturnsAsync((Aluno)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _alunoService.Matricular(alunoId, cursoId));
            Assert.Equal("Aluno não encontrado.", exception.Message);
            _alunoRepositoryMock.Verify(r => r.ObterPorId(alunoId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "Matricular deve lançar BusinessException se já matriculado mas inativa")]
        [Trait("Category", "AlunoService - Matricular")]
        public async Task Matricular_AlunoJaMatriculadoMasInativa_DeveLancarBusinessException()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaExistente = CriarMatriculaValida(alunoId, cursoId, ativa: false);
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matriculaExistente });
            _alunoRepositoryMock.Setup(r => r.ObterPorId(alunoId, true)).ReturnsAsync(aluno);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _alunoService.Matricular(alunoId, cursoId));
            Assert.Equal("O aluno já está matriculado neste curso, favor realizar o pagamento.", exception.Message);
            _alunoRepositoryMock.Verify(r => r.ObterPorId(alunoId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "Matricular deve retornar null e notificar erros se a entidade Aluno se tornar inválida após matricular")]
        [Trait("Category", "AlunoService - Matricular")]
        public async Task Matricular_AlunoInvalidoAposMatricular_DeveRetornarNullENotificarErros()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            
            var aluno = CriarAlunoValido(alunoId, nome: "Nome Valido");
            _alunoRepositoryMock.Setup(r => r.ObterPorId(alunoId, true)).ReturnsAsync(aluno);
            _alunoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(false);


            // Act
            var result = await _alunoService.Matricular(alunoId, cursoId);

            // Assert
            Assert.Null(result);
            _alunoRepositoryMock.Verify(r => r.ObterPorId(alunoId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Não foi identificada nenhuma alteração nos dados."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "AprovarMatricula deve retornar true e ativar matrícula e chamar commit")]
        [Trait("Category", "AlunoService - AprovarMatricula")]
        public async Task AprovarMatricula_DeveRetornarTrueAtivarMatriculaEChamarCommit()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();
            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: false);
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });

            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);

            // Act
            var result = await _alunoService.AprovarMatricula(matriculaId);

            // Assert
            Assert.True(result);
            Assert.True(matricula.Ativa);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "AprovarMatricula deve retornar true se matrícula já estiver ativa")]
        [Trait("Category", "AlunoService - AprovarMatricula")]
        public async Task AprovarMatricula_MatriculaJaAtiva_DeveRetornarTrue()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();
            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: true);
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });

            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);

            // Act
            var result = await _alunoService.AprovarMatricula(matriculaId);

            // Assert
            Assert.True(result);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "AprovarMatricula deve lançar BusinessException se aluno não for encontrado")]
        [Trait("Category", "AlunoService - AprovarMatricula")]
        public async Task AprovarMatricula_AlunoNaoEncontrado_DeveLancarBusinessException()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync((Aluno)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _alunoService.AprovarMatricula(matriculaId));
            Assert.Equal("Aluno não encontrado.", exception.Message);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "BloquearMatricula deve retornar true e bloquear matrícula e chamar commit")]
        [Trait("Category", "AlunoService - BloquearMatricula")]
        public async Task BloquearMatricula_DeveRetornarTrueBloquearMatriculaEChamarCommit()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();
            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: true);
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });

            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);

            // Act
            var result = await _alunoService.BloquearMatricula(matriculaId);

            // Assert
            Assert.True(result);
            Assert.False(matricula.Ativa);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "BloquearMatricula deve retornar true se matrícula já estiver inativa")]
        [Trait("Category", "AlunoService - BloquearMatricula")]
        public async Task BloquearMatricula_MatriculaJaInativa_DeveRetornarTrue()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();
            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: false);
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });

            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);

            // Act
            var result = await _alunoService.BloquearMatricula(matriculaId);

            // Assert
            Assert.True(result);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never); // No commit expected
        }

        [Fact(DisplayName = "BloquearMatricula deve lançar BusinessException se aluno não for encontrado")]
        [Trait("Category", "AlunoService - BloquearMatricula")]
        public async Task BloquearMatricula_AlunoNaoEncontrado_DeveLancarBusinessException()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync((Aluno?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _alunoService.BloquearMatricula(matriculaId));
            Assert.Equal("Aluno não encontrado.", exception.Message);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "RegistrarAulaConcluida deve retornar true e registrar aula e chamar commit")]
        [Trait("Category", "AlunoService - RegistrarAulaConcluida")]
        public async Task RegistrarAulaConcluida_DeveRetornarTrueRegistrarAulaEChamarCommit()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();
            var aulaId = Guid.NewGuid();

            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: true);
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });
            typeof(Aluno).GetProperty(nameof(Aluno.AulasConcluidas))?.SetValue(aluno, new List<AulaConcluida>()); 

            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);

            // Act
            var result = await _alunoService.RegistrarAulaConcluida(matriculaId, aulaId);

            // Assert
            Assert.True(result);
            Assert.Single(aluno.AulasConcluidas);
            Assert.Equal(aulaId, aluno.AulasConcluidas.First().AulaId);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "RegistrarAulaConcluida não deve registrar aula se já concluída e retornar true")]
        [Trait("Category", "AlunoService - RegistrarAulaConcluida")]
        public async Task RegistrarAulaConcluida_AulaJaConcluida_NaoDeveRegistrarEContinuarTrue()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();
            var aulaId = Guid.NewGuid();

            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: true);
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });
            typeof(Aluno).GetProperty(nameof(Aluno.AulasConcluidas))?.SetValue(aluno, new List<AulaConcluida> { new AulaConcluida(alunoId, cursoId, aulaId, DateTime.Now) });

            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);

            // Act
            var result = await _alunoService.RegistrarAulaConcluida(matriculaId, aulaId);

            // Assert
            Assert.True(result);
            Assert.Single(aluno.AulasConcluidas);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once); 
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.IsAny<AulaConcluidaEvent>(), It.IsAny<CancellationToken>()), Times.Never); // No new event
        }

        [Fact(DisplayName = "RegistrarAulaConcluida deve lançar BusinessException se aluno não for encontrado")]
        [Trait("Category", "AlunoService - RegistrarAulaConcluida")]
        public async Task RegistrarAulaConcluida_AlunoNaoEncontrado_DeveLancarBusinessException()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var aulaId = Guid.NewGuid();
            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync((Aluno)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _alunoService.RegistrarAulaConcluida(matriculaId, aulaId));
            Assert.Equal("Aluno não encontrado.", exception.Message);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "RegistrarAulaConcluida deve lançar BusinessException se matrícula não estiver ativa")]
        [Trait("Category", "AlunoService - RegistrarAulaConcluida")]
        public async Task RegistrarAulaConcluida_MatriculaNaoAtiva_DeveLancarBusinessException()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();
            var aulaId = Guid.NewGuid();

            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: false); // Not active
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });

            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _alunoService.RegistrarAulaConcluida(matriculaId, aulaId));
            Assert.Equal("A matrícula não está ativa, não é possível registrar a conclusão da aula.", exception.Message);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "EmitirCertificado deve retornar URL do certificado e chamar commit")]
        [Trait("Category", "AlunoService - EmitirCertificado")]
        public async Task EmitirCertificado_DeveRetornarUrlCertificadoEChamarCommit()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();

            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: true, concluido: false);
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });

            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);
            _alunoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(true);

            // Act
            var result = await _alunoService.EmitirCertificado(matriculaId);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("gerar-link", result);
            Assert.True(matricula.Concluido);
            Assert.NotNull(matricula.Certificado);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "EmitirCertificado deve lançar BusinessException se aluno não for encontrado")]
        [Trait("Category", "AlunoService - EmitirCertificado")]
        public async Task EmitirCertificado_AlunoNaoEncontrado_DeveLancarBusinessException()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync((Aluno)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _alunoService.EmitirCertificado(matriculaId));
            Assert.Equal("Aluno não encontrado.", exception.Message);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "EmitirCertificado deve lançar BusinessException se matrícula não estiver ativa")]
        [Trait("Category", "AlunoService - EmitirCertificado")]
        public async Task EmitirCertificado_MatriculaNaoAtiva_DeveLancarBusinessException()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();

            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: false); // Not active
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });

            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _alunoService.EmitirCertificado(matriculaId));
            Assert.Equal("A matrícula não está ativa, não é possível registrar a conclusão da aula.", exception.Message);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "EmitirCertificado deve lançar BusinessException se a entidade Aluno se tornar inválida após emitir certificado")]
        [Trait("Category", "AlunoService - EmitirCertificado")]
        public async Task EmitirCertificado_AlunoInvalidoAposEmitirCertificado_DeveLancarBusinessException()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();

            var matricula = CriarMatriculaValida(alunoId, cursoId, matriculaId, ativa: true);
            var aluno = CriarAlunoValido(alunoId);
            typeof(Aluno).GetProperty(nameof(Aluno.Matriculas))?.SetValue(aluno, new List<Matricula> { matricula });
            _alunoRepositoryMock.Setup(r => r.ObterAtravesDaMatricula(matriculaId, true)).ReturnsAsync(aluno);

            _alunoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(false); 

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _alunoService.EmitirCertificado(matriculaId));
            Assert.Equal("Não foi possível emitir o certificado, tente novamente mais tarde ou contate os administradores da plataforma.", exception.Message);
            _alunoRepositoryMock.Verify(r => r.ObterAtravesDaMatricula(matriculaId, true), Times.Once);
            _alunoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }
    }
}