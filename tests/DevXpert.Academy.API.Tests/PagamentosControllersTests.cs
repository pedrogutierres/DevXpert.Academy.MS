using DevXpert.Academy.API.ResponseType;
using DevXpert.Academy.API.Tests.Config;
using DevXpert.Academy.API.ViewModels.Pagamentos;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace DevXpert.Academy.API.Tests
{
    [TestCaseOrderer("DevXpert.Academy.API.Tests.Config.PriorityOrderer", "DevXpert.Academy.API.Tests")]
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class PagamentosControllersTests
    {
        private readonly IntegrationTestsFixture<Program> _testsFixture;
        private static Guid _alunoComPagamentoId = Guid.Empty;
        private static Guid _pagamentoExistenteId = Guid.Empty;

        public PagamentosControllersTests(IntegrationTestsFixture<Program> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Obter Pagamentos como Administrador deve retornar sucesso e lista de pagamentos")]
        [Trait("Pagamentos", "Integração API - Pagamentos")]
        [TestPriority(210)]
        public async Task Pagamentos_ObterPagamentos_ComoAdministrador_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            // Act
            var response = await _testsFixture.Client.GetAsync("/api/pagamentos");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pagamentos = await response.Content.ReadFromJsonAsync<List<PagamentoViewModel>>();
            Assert.NotNull(pagamentos);
            Assert.NotEmpty(pagamentos);

            if (pagamentos.Any())
            {
                _pagamentoExistenteId = pagamentos.First().Id;
                _alunoComPagamentoId = pagamentos.First().Matricula.Aluno.Id;
            }
        }

        [Fact(DisplayName = "Obter Pagamentos como Aluno deve retornar Forbidden")]
        [Trait("Pagamentos", "Integração API - Pagamentos")]
        [TestPriority(211)]
        public async Task Pagamentos_ObterPagamentos_ComoAluno_DeveRetornarForbidden()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();

            // Act
            var response = await _testsFixture.Client.GetAsync("/api/pagamentos");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = "Obter Pagamento por ID como Administrador com ID válido deve retornar sucesso")]
        [Trait("Pagamentos", "Integração API - Pagamentos")]
        [TestPriority(220)]
        public async Task Pagamentos_ObterPagamentoPorId_ComoAdministrador_ComIdValido_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _pagamentoExistenteId);

            // Act
            var response = await _testsFixture.Client.GetAsync($"/api/pagamentos/{_pagamentoExistenteId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pagamento = await response.Content.ReadFromJsonAsync<PagamentoViewModel>();
            Assert.NotNull(pagamento);
            Assert.Equal(_pagamentoExistenteId, pagamento.Id);
        }

        [Fact(DisplayName = "Obter Pagamento por ID como Administrador com ID inválido deve retornar NotFound")]
        [Trait("Pagamentos", "Integração API - Pagamentos")]
        [TestPriority(221)]
        public async Task Pagamentos_ObterPagamentoPorId_ComoAdministrador_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            var pagamentoIdInvalido = Guid.NewGuid();

            // Act
            var response = await _testsFixture.Client.GetAsync($"/api/pagamentos/{pagamentoIdInvalido}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact(DisplayName = "Obter Pagamentos por Aluno como Administrador com AlunoId válido deve retornar sucesso")]
        [Trait("Pagamentos", "Integração API - Pagamentos")]
        [TestPriority(230)]
        public async Task Pagamentos_ObterPagamentosPorAluno_ComoAdministrador_ComAlunoIdValido_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _alunoComPagamentoId);

            // Act
            var response = await _testsFixture.Client.GetAsync($"/api/pagamentos/alunos/{_alunoComPagamentoId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pagamentos = await response.Content.ReadFromJsonAsync<List<PagamentoViewModel>>();
            Assert.NotNull(pagamentos);
            Assert.True(pagamentos.Any(p => p.Matricula.Aluno.Id == _alunoComPagamentoId));
        }

        [Fact(DisplayName = "Obter Pagamentos por Aluno como Administrador com AlunoId inválido deve retornar lista vazia")]
        [Trait("Pagamentos", "Integração API - Pagamentos")]
        [TestPriority(231)]
        public async Task Pagamentos_ObterPagamentosPorAluno_ComoAdministrador_ComAlunoIdInvalido_DeveRetornarListaVazia()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            var alunoIdInvalido = Guid.NewGuid();

            // Act
            var response = await _testsFixture.Client.GetAsync($"/api/pagamentos/alunos/{alunoIdInvalido}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pagamentos = await response.Content.ReadFromJsonAsync<List<PagamentoViewModel>>();
            Assert.NotNull(pagamentos);
            Assert.Empty(pagamentos);
        }

        [Fact(DisplayName = "Obter Pagamentos por Aluno como Aluno deve retornar Forbidden")]
        [Trait("Pagamentos", "Integração API - Pagamentos")]
        [TestPriority(232)]
        public async Task Pagamentos_ObterPagamentosPorAluno_ComoAluno_DeveRetornarForbidden()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();
            var alunoIdQualquer = Guid.NewGuid();

            // Act
            var response = await _testsFixture.Client.GetAsync($"/api/pagamentos/alunos/{alunoIdQualquer}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}