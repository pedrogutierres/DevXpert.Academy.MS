using DevXpert.Academy.API.ResponseType;
using DevXpert.Academy.API.Tests.Config;
using DevXpert.Academy.API.ViewModels.Alunos;
using System.Net;
using System.Net.Http.Json;

namespace DevXpert.Academy.API.Tests
{
    [TestCaseOrderer("DevXpert.Academy.API.Tests.Config.PriorityOrderer", "DevXpert.Academy.API.Tests")]
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class AlunosControllersTests
    {
        private readonly IntegrationTestsFixture<Program> _testsFixture;
        private static Guid _alunoIdAdministrador = Guid.Empty; 
        private static Guid _alunoIdPadrao = Guid.Empty; 

        public AlunosControllersTests(IntegrationTestsFixture<Program> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Obter Alunos como Administrador deve retornar sucesso")]
        [Trait("Alunos", "Integração API - Alunos")]
        [TestPriority(10)]
        public async Task Alunos_ObterAlunos_ComoAdministrador_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            // Act
            var response = await _testsFixture.Client.GetAsync("/api/alunos");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var alunos = await response.Content.ReadFromJsonAsync<List<AlunoViewModel>>();
            Assert.NotNull(alunos);
            Assert.NotEmpty(alunos); 
        }

        [Fact(DisplayName = "Obter Alunos como Aluno deve retornar Forbidden")]
        [Trait("Alunos", "Integração API - Alunos")]
        [TestPriority(11)]
        public async Task Alunos_ObterAlunos_ComoAluno_DeveRetornarForbidden()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno(); 

            // Act
            var response = await _testsFixture.Client.GetAsync("/api/alunos");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = "Obter Meu Perfil como Aluno deve retornar sucesso")]
        [Trait("Alunos", "Integração API - Alunos")]
        [TestPriority(20)]
        public async Task Alunos_MeuPerfil_ComoAluno_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();

            // Act
            var response = await _testsFixture.Client.GetAsync("/api/alunos/meu-perfil");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var meuPerfil = await response.Content.ReadFromJsonAsync<MeuPerfilViewModel>();
            Assert.NotNull(meuPerfil);
            Assert.Equal(_testsFixture.UsuarioId, meuPerfil.Id);
        }
    }
}