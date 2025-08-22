using DevXpert.Academy.Conteudo.Domain.Cursos;
using DevXpert.Academy.Conteudo.Domain.Cursos.Events;
using DevXpert.Academy.Conteudo.Domain.Cursos.Interfaces;
using DevXpert.Academy.Conteudo.Domain.Cursos.Services;
using DevXpert.Academy.Conteudo.Domain.Cursos.ValuesObjects;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Moq;
using Moq.AutoMock;
using DevXpert.Academy.Core.Domain.Exceptions; // Adicionado para BusinessException

namespace DevXpert.Academy.Conteudo.Domain.Tests.Cursos
{
    public class CursoServiceTests
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<ICursoRepository> _cursoRepositoryMock;
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
        private readonly CursoService _cursoService;

        public CursoServiceTests()
        {
            _mocker = new AutoMocker();

            _cursoRepositoryMock = _mocker.GetMock<ICursoRepository>();
            _mediatorHandlerMock = _mocker.GetMock<IMediatorHandler>();
            _mocker.Use<INotificationHandler<DomainNotification>>(new DomainNotificationHandler());

            _cursoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(true);

            _cursoService = _mocker.CreateInstance<CursoService>();
        }

        private Curso CriarCursoValido(Guid? id = null, List<Aula>? aulas = null)
        {
            var cursoId = id ?? Guid.NewGuid();
            return new Curso(
                cursoId,
                "Titulo Valido " + Guid.NewGuid().ToString().Substring(0, 4),
                new ConteudoProgramatico("Descricao Conteudo Programatico", 10),
                100m,
                aulas ?? new List<Aula>() { new Aula(Guid.NewGuid(), cursoId, "Aula Padrao", "http://video.com/padrao") }
            );
        }

        private Aula CriarAulaValida(Guid cursoId, Guid? aulaId = null)
        {
            return new Aula(aulaId ?? Guid.NewGuid(), cursoId, "Titulo Aula Valida", "http://video.com/aula");
        }


        [Fact(DisplayName = "Cadastrar Curso deve retornar true e chamar repositório e commit quando válido e apto")]
        [Trait("Category", "CursoService - Cadastrar")]
        public async Task Cadastrar_CursoValidoEApto_DeveRetornarTrueChamarRepositorioECommit()
        {
            // Arrange
            var curso = CriarCursoValido();

            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false);

            // Act
            var result = await _cursoService.Cadastrar(curso);

            // Assert
            Assert.True(result);
            _cursoRepositoryMock.Verify(r => r.Cadastrar(curso), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Cadastrar Curso deve retornar false se a entidade for inválida")]
        [Trait("Category", "CursoService - Cadastrar")]
        public async Task Cadastrar_EntidadeInvalida_DeveRetornarFalse()
        {
            // Arrange
            var cursoInvalido = new Curso(Guid.NewGuid(), string.Empty, null, 100m, null);

            // Act
            var result = await _cursoService.Cadastrar(cursoInvalido);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.Cadastrar(It.IsAny<Curso>()), Times.Never);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "O título do curso deve ser informado."), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value.Contains("O conteúdo programático do curso")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cadastrar Curso deve retornar false se o título já existir (Validação de Aptidão)")]
        [Trait("Category", "CursoService - Cadastrar")]
        public async Task Cadastrar_CursoComTituloExistente_DeveRetornarFalse()
        {
            // Arrange
            var curso = CriarCursoValido();

            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(true);

            // Act
            var result = await _cursoService.Cadastrar(curso);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.Cadastrar(It.IsAny<Curso>()), Times.Never);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Já existe um outro curso com o título informado."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cadastrar Curso deve retornar false se o commit falhar")]
        [Trait("Category", "CursoService - Cadastrar")]
        public async Task Cadastrar_CommitFalhar_DeveRetornarFalse()
        {
            // Arrange
            var curso = CriarCursoValido();
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false);
            _cursoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(false);

            // Act
            var result = await _cursoService.Cadastrar(curso);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.Cadastrar(curso), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Não foi identificada nenhuma alteração nos dados."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Alterar Curso deve retornar true e chamar commit quando válido e apto")]
        [Trait("Category", "CursoService - Alterar")]
        public async Task Alterar_CursoValidoEApto_DeveRetornarTrueChamarCommit()
        {
            // Arrange
            var cursoExistente = CriarCursoValido();
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoExistente.Id, It.IsAny<bool>())).ReturnsAsync(cursoExistente);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(cursoExistente.Titulo, cursoExistente.Id)).ReturnsAsync(false);

            // Act
            var result = await _cursoService.Alterar(cursoExistente);

            // Assert
            Assert.True(result);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
            _cursoRepositoryMock.Verify(r => r.Alterar(It.IsAny<Curso>()), Times.Never); // Alterar não é chamado no service, mas no repository
        }

        [Fact(DisplayName = "Alterar Curso deve retornar false se a entidade for inválida")]
        [Trait("Category", "CursoService - Alterar")]
        public async Task Alterar_EntidadeInvalida_DeveRetornarFalse()
        {
            // Arrange
            var cursoInvalido = new Curso(Guid.NewGuid(), string.Empty, null, 100m, null);

            // Act
            var result = await _cursoService.Alterar(cursoInvalido);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "O título do curso deve ser informado."), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value.Contains("O conteúdo programático do curso")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Alterar Curso deve retornar false se o título já existir em outro curso (Validação de Aptidão)")]
        [Trait("Category", "CursoService - Alterar")]
        public async Task Alterar_TituloExistenteEmOutroCurso_DeveRetornarFalse()
        {
            // Arrange
            var cursoParaAlterar = CriarCursoValido();
            cursoParaAlterar.AlterarTitulo("Novo Titulo");
            // Não precisamos de "outroCursoComMesmoTitulo" aqui, apenas simular a existência no mock
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoParaAlterar.Id, It.IsAny<bool>())).ReturnsAsync(cursoParaAlterar);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(cursoParaAlterar.Titulo, cursoParaAlterar.Id)).ReturnsAsync(true); // Simulando que o título já existe em outro curso

            // Act
            var result = await _cursoService.Alterar(cursoParaAlterar);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Já existe um outro curso com o título informado."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Alterar Curso deve retornar false se o commit falhar")]
        [Trait("Category", "CursoService - Alterar")]
        public async Task Alterar_CommitFalhar_DeveRetornarFalse()
        {
            // Arrange
            var curso = CriarCursoValido();
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, It.IsAny<bool>())).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false);
            _cursoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(false);

            // Act
            var result = await _cursoService.Alterar(curso);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Não foi identificada nenhuma alteração nos dados."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Ativar Curso deve retornar true e ativar o curso e chamar commit")]
        [Trait("Category", "CursoService - Ativar")]
        public async Task Ativar_CursoEncontrado_DeveRetornarTrueAtivarCursoEChamarCommit()
        {
            // Arrange
            var curso = CriarCursoValido();
            curso.Inativar(); // Certifica que está inativo para ativar
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, true)).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false); // Para passar a validação de aptidão

            // Act
            var result = await _cursoService.Ativar(curso.Id);

            // Assert
            Assert.True(result);
            Assert.True(curso.Ativo);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(curso.Id, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Ativar Curso deve retornar false se o curso não for encontrado")]
        [Trait("Category", "CursoService - Ativar")]
        public async Task Ativar_CursoNaoEncontrado_DeveRetornarFalse()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoId, true)).ReturnsAsync((Curso?)null);

            // Act
            var result = await _cursoService.Ativar(cursoId);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(cursoId, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Curso não encontrado"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Ativar Curso deve retornar false se o curso não tiver aulas e estiver no estado de ativação")]
        [Trait("Category", "CursoService - Ativar")]
        public async Task Ativar_CursoSemAulas_DeveRetornarFalse()
        {
            // Arrange
            var curso = CriarCursoValido(aulas: new List<Aula>()); // Curso sem aulas
            curso.Inativar(); // Inativa para tentar ativar
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, true)).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false); // Para passar a validação de aptidão

            // Act
            var result = await _cursoService.Ativar(curso.Id);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(curso.Id, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value.Contains("O curso") && n.Value.Contains("deve ter aulas para ser ativado.")), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact(DisplayName = "Inativar Curso deve retornar true e inativar o curso e chamar commit")]
        [Trait("Category", "CursoService - Inativar")]
        public async Task Inativar_CursoEncontrado_DeveRetornarTrueInativarCursoEChamarCommit()
        {
            // Arrange
            var curso = CriarCursoValido();
            curso.Ativar(); // Certifica que está ativo para inativar
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, true)).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false); // Para passar a validação de aptidão

            // Act
            var result = await _cursoService.Inativar(curso.Id);

            // Assert
            Assert.True(result);
            Assert.False(curso.Ativo);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(curso.Id, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Inativar Curso deve retornar false se o curso não for encontrado")]
        [Trait("Category", "CursoService - Inativar")]
        public async Task Inativar_CursoNaoEncontrado_DeveRetornarFalse()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoId, true)).ReturnsAsync((Curso?)null);

            // Act
            var result = await _cursoService.Inativar(cursoId);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(cursoId, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Curso não encontrado"), It.IsAny<CancellationToken>()), Times.Once);
        }

        // Novos testes para AdicionarAula
        [Fact(DisplayName = "AdicionarAula deve retornar true e chamar commit quando curso e aula são válidos")]
        [Trait("Category", "CursoService - AdicionarAula")]
        public async Task AdicionarAula_CursoEAulaValidos_DeveRetornarTrueEChamarCommit()
        {
            // Arrange
            var curso = CriarCursoValido();
            var aula = CriarAulaValida(curso.Id);
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, true)).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false); // Para aptidão

            // Act
            var result = await _cursoService.AdicionarAula(curso.Id, aula);

            // Assert
            Assert.True(result);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(curso.Id, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
            Assert.Contains(aula, curso.Aulas); // Verifica se a aula foi adicionada ao curso
        }

        [Fact(DisplayName = "AdicionarAula deve retornar false se o curso não for encontrado")]
        [Trait("Category", "CursoService - AdicionarAula")]
        public async Task AdicionarAula_CursoNaoEncontrado_DeveRetornarFalse()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            var aula = CriarAulaValida(cursoId);
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoId, true)).ReturnsAsync((Curso?)null);

            // Act
            var result = await _cursoService.AdicionarAula(cursoId, aula);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(cursoId, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Curso não encontrado"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "AdicionarAula deve retornar false e notificar se a aula for inválida (Título vazio)")]
        [Trait("Category", "CursoService - AdicionarAula")]
        public async Task AdicionarAula_AulaInvalidaTituloVazio_DeveRetornarFalse()
        {
            // Arrange
            var curso = CriarCursoValido();
            var aulaInvalida = new Aula(Guid.NewGuid(), curso.Id, string.Empty, "http://video.com/valido"); // Título vazio
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, true)).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false); // Para aptidão

            // Act
            var result = await _cursoService.AdicionarAula(curso.Id, aulaInvalida);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "O título da aula deve ser informado."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "AdicionarAula deve retornar false e notificar se a aula for inválida (URL de vídeo vazia)")]
        [Trait("Category", "CursoService - AdicionarAula")]
        public async Task AdicionarAula_AulaInvalidaUrlVazia_DeveRetornarFalse()
        {
            // Arrange
            var curso = CriarCursoValido();
            var aulaInvalida = new Aula(Guid.NewGuid(), curso.Id, "Titulo Valido", string.Empty); // URL vazia
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, true)).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false); // Para aptidão

            // Act
            var result = await _cursoService.AdicionarAula(curso.Id, aulaInvalida);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value.Contains("A URL da vídeo aula") && n.Value.Contains("deve ser informada.")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "AdicionarAula deve retornar false se o commit falhar")]
        [Trait("Category", "CursoService - AdicionarAula")]
        public async Task AdicionarAula_CommitFalhar_DeveRetornarFalse()
        {
            // Arrange
            var curso = CriarCursoValido();
            var aula = CriarAulaValida(curso.Id);
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, true)).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false); // Para aptidão
            _cursoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(false);

            // Act
            var result = await _cursoService.AdicionarAula(curso.Id, aula);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(curso.Id, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Não foi identificada nenhuma alteração nos dados."), It.IsAny<CancellationToken>()), Times.Once);
        }

        // Novos testes para RemoverAula
        [Fact(DisplayName = "RemoverAula deve retornar true e chamar commit quando curso e aula são válidos para remoção")]
        [Trait("Category", "CursoService - RemoverAula")]
        public async Task RemoverAula_CursoEAulaValidos_DeveRetornarTrueEChamarCommit()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            var aulaExistente = CriarAulaValida(cursoId);
            var curso = CriarCursoValido(cursoId, new List<Aula> { aulaExistente }); // Curso com a aula a ser removida
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoId, true)).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false); // Para aptidão

            // Act
            var result = await _cursoService.RemoverAula(cursoId, aulaExistente.Id);

            // Assert
            Assert.True(result);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(cursoId, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
            Assert.DoesNotContain(aulaExistente, curso.Aulas); // Verifica se a aula foi removida do curso
        }

        [Fact(DisplayName = "RemoverAula deve retornar false se o curso não for encontrado")]
        [Trait("Category", "CursoService - RemoverAula")]
        public async Task RemoverAula_CursoNaoEncontrado_DeveRetornarFalse()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            var aulaId = Guid.NewGuid();
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoId, true)).ReturnsAsync((Curso?)null);

            // Act
            var result = await _cursoService.RemoverAula(cursoId, aulaId);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(cursoId, true), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Curso não encontrado"), It.IsAny<CancellationToken>()), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "RemoverAula deve lançar BusinessException se a aula não for encontrada no curso")]
        [Trait("Category", "CursoService - RemoverAula")]
        public async Task RemoverAula_AulaNaoEncontradaNoCurso_DeveLancarBusinessException()
        {
            // Arrange
            var curso = CriarCursoValido(); // Curso com aulas padrão ou nenhuma aula
            var aulaIdInexistente = Guid.NewGuid();
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, true)).ReturnsAsync(curso);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _cursoService.RemoverAula(curso.Id, aulaIdInexistente));
            Assert.Equal("Aula não encontrada no curso", exception.Message);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(curso.Id, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "RemoverAula deve retornar false se o commit falhar")]
        [Trait("Category", "CursoService - RemoverAula")]
        public async Task RemoverAula_CommitFalhar_DeveRetornarFalse()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            var aulaExistente = CriarAulaValida(cursoId);
            var curso = CriarCursoValido(cursoId, new List<Aula> { aulaExistente });
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoId, true)).ReturnsAsync(curso);
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false); // Para aptidão
            _cursoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(false);

            // Act
            var result = await _cursoService.RemoverAula(cursoId, aulaExistente.Id);

            // Assert
            Assert.False(result);
            _cursoRepositoryMock.Verify(r => r.ObterPorId(cursoId, true), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Não foi identificada nenhuma alteração nos dados."), It.IsAny<CancellationToken>()), Times.Once);
        }


        // Testes de validação de nível de DomainService (já existentes, mantidos)
        [Fact(DisplayName = "EntidadeValida deve notificar erros de validação da entidade")]
        [Trait("Category", "CursoService - DomainService")]
        public async Task EntidadeValida_ComErros_DeveNotificarErros()
        {
            // Arrange
            var cursoInvalido = new Curso(Guid.NewGuid(), "", null, 0, null);

            // Act
            await _cursoService.Cadastrar(cursoInvalido);

            // Assert
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "O título do curso deve ser informado."), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value.Contains("O conteúdo programático do curso")), It.IsAny<CancellationToken>()), Times.Once);
            _cursoRepositoryMock.Verify(r => r.UnitOfWork.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "CommitAsync deve notificar erro se não houver alterações e ignoreNoChangeUpdated for false")]
        [Trait("Category", "CursoService - DomainService")]
        public async Task CommitAsync_SemAlteracoesEIgnorarFalso_DeveNotificarErro()
        {
            var curso = CriarCursoValido();
            _cursoRepositoryMock.Setup(r => r.ExistePorTitulo(curso.Titulo, curso.Id)).ReturnsAsync(false);
            _cursoRepositoryMock.Setup(r => r.UnitOfWork.CommitAsync()).ReturnsAsync(false); // Simula falha no commit

            // Act
            var result = await _cursoService.Cadastrar(curso);

            // Assert
            Assert.False(result);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Não foi identificada nenhuma alteração nos dados."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "ObterEntidade deve publicar notificação se entidade não for encontrada")]
        [Trait("Category", "CursoService - DomainService")]
        public async Task ObterEntidade_NaoEncontrado_DevePublicarNotificacao()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoId, true)).ReturnsAsync((Curso?)null);

            // Act
            var result = await _cursoService.Ativar(cursoId);

            // Assert
            Assert.False(result);
            _mediatorHandlerMock.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(n => n.Value == "Curso não encontrado"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}