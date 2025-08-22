using DevXpert.Academy.API.Tests.Config;
using System.Net;
using System.Net.Http.Json;

namespace DevXpert.Academy.API.Tests
{
    [TestCaseOrderer("DevXpert.Academy.API.Tests.Config.PriorityOrderer", "DevXpert.Academy.API.Tests")]
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class UsuariosControllersTests
    {
        private readonly IntegrationTestsFixture<Program> _testsFixture;

        public UsuariosControllersTests(IntegrationTestsFixture<Program> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Realizar login de usuario administrador cadastrado")]
        [Trait("Usuarios", "Integração API - Usuario")]
        public async Task Usuarios_ValidarLogin_DeveLogarComSucesso()
        {
            // Arrange
            var dados = new // LoginViewModel - Não estou utilizando a view model default, para o teste falhar se houver mudanças não previstas
            {
                Email = "pedro@gmail.com",
                Senha = "Pedro@123456"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync("/api/usuarios/login", dados);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Fact(DisplayName = "Realizar login de usuario inexistente invalido")]
        [Trait("Usuarios", "Integração API - Usuario")]
        public async Task Usuarios_ValidarLogin_NaoDeveLogar()
        {
            // Arrange
            var dados = new // LoginViewModel - Não estou utilizando a view model default, para o teste falhar se houver mudanças não previstas
            {
                Email = "xxx@gmail.com",
                Senha = "Pedro@123456"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync("/api/usuarios/login", dados);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "Registrar novo aluno com sucesso")]
        [Trait("Usuarios", "Integração API - Usuario")]
        public async Task Usuarios_NovoAluno_DeveRegistrarComSucesso()
        {
            // Arrange
            var novoAlunoData = new
            {
                Email = "novo.aluno@teste.com",
                Senha = "Senha@123",
                Nome = "Novo Aluno Teste"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync("/api/usuarios/novo-aluno", novoAlunoData);

            // Assert
            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Tentar registrar aluno com email já existente")]
        [Trait("Usuarios", "Integração API - Usuario")]
        public async Task Usuarios_NovoAluno_DeveRetornarBadRequestParaEmailExistente()
        {
            // Arrange
            var novoAlunoData = new
            {
                Email = "pedro@gmail.com",
                Senha = "Pedro@123456",
                Nome = "Pedro"
            };

            await _testsFixture.Client.PostAsJsonAsync("/api/usuarios/novo-aluno", novoAlunoData);

            var response = await _testsFixture.Client.PostAsJsonAsync("/api/usuarios/novo-aluno", novoAlunoData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
