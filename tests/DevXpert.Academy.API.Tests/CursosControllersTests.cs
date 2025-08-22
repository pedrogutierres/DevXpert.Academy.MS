using DevXpert.Academy.API.ResponseType;
using DevXpert.Academy.API.Tests.Config;
using DevXpert.Academy.API.ViewModels.Cursos;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace DevXpert.Academy.API.Tests
{
    [TestCaseOrderer("DevXpert.Academy.API.Tests.Config.PriorityOrderer", "DevXpert.Academy.API.Tests")]
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class CursosControllersTests
    {
        private readonly IntegrationTestsFixture<Program> _testsFixture;
        private static Guid _cursoId = Guid.Empty;
        private static Guid _aulaId = Guid.Empty;

        public CursosControllersTests(IntegrationTestsFixture<Program> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Obter cursos como anônimo deve retornar sucesso (Sem cursos cadastrados)")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(10)]
        public async Task Cursos_ObterCursosAnonimo_DeveRetornarSucesso()
        {
            // Arrange & Act
            var response = await _testsFixture.Client.GetAsync("/api/cursos");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cursos = await response.Content.ReadFromJsonAsync<List<CursoViewModel>>();
            Assert.NotNull(cursos);
        }

        [Fact(DisplayName = "Cadastrar curso como Administrador com dados válidos deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(20)]
        public async Task Cursos_CadastrarCurso_ComoAdministrador_ComDadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            var novoCurso = new
            {
                Titulo = $"Curso Teste {Guid.NewGuid()}",
                Valor = 199.99m,
                ConteudoProgramatico = new
                {
                    Descricao = "Descrição do curso de teste.",
                    CargaHoraria = 40
                },
                Aulas = new List<object>
                {
                    new { Titulo = "Aula Inicial 1", VideoUrl = "http://video.com/aula-inicial-1" },
                    new { Titulo = "Aula Inicial 2", VideoUrl = "http://video.com/aula-inicial-2" }
                }
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync("/api/cursos", novoCurso);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
            Assert.NotNull(responseContent.Id);
            Assert.NotEqual(Guid.Empty, responseContent.Id);
            _cursoId = responseContent.Id.Value;
        }

        [Fact(DisplayName = "Obter curso por ID como anônimo com ID válido (criado) deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(25)]
        public async Task Cursos_ObterCursoPorIdAnonimo_ComIdValido_DeveRetornarSucesso()
        {
            // Arrange
            Assert.NotEqual(Guid.Empty, _cursoId);

            // Act
            var response = await _testsFixture.Client.GetAsync($"/api/cursos/{_cursoId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var curso = await response.Content.ReadFromJsonAsync<CursoViewModel>();
            Assert.NotNull(curso);
            Assert.Equal(_cursoId, curso.Id);
        }

        [Fact(DisplayName = "Obter curso por ID para Admin como Administrador com ID válido (criado) deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(30)]
        public async Task Cursos_ObterCursoPorIdParaAdmin_ComoAdministrador_ComIdValido_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _cursoId);

            // Act
            var response = await _testsFixture.Client.GetAsync($"/api/cursos/{_cursoId}/admin");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var curso = await response.Content.ReadFromJsonAsync<CursoAdmViewModel>();
            Assert.NotNull(curso);
            Assert.Equal(_cursoId, curso.Id);
        }

        [Fact(DisplayName = "Alterar curso como Administrador com dados válidos deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(35)]
        public async Task Cursos_AlterarCurso_ComoAdministrador_ComDadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _cursoId);

            var cursoAlterado = new
            {
                Titulo = $"Curso Alterado {Guid.NewGuid()}",
                Valor = 250.00m,
                ConteudoProgramatico = new
                {
                    Descricao = "Nova descrição do curso alterado.",
                    CargaHoraria = 60
                }
            };

            // Act
            var response = await _testsFixture.Client.PutAsJsonAsync($"/api/cursos/{_cursoId}", cursoAlterado);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
            Assert.NotNull(responseContent.Id);
            Assert.Equal(_cursoId, responseContent.Id);
        }

        [Fact(DisplayName = "Adicionar aula a curso como Administrador com dados válidos deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(40)]
        public async Task Cursos_AdicionarAula_ComoAdministrador_ComDadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _cursoId);

            var novaAula = new
            {
                Titulo = "Nova Aula Teste",
                VideoUrl = "http://video.com/nova-aula"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync($"/api/cursos/{_cursoId}/aulas", novaAula);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
            Assert.NotNull(responseContent.Id);
            Assert.NotEqual(Guid.Empty, responseContent.Id);
            _aulaId = responseContent.Id.Value;
        }

        [Fact(DisplayName = "Ativar curso como Administrador com ID válido (criado) deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(50)]
        public async Task Cursos_AtivarCurso_ComoAdministrador_ComIdValido_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _cursoId);

            var getCursoResponse = await _testsFixture.Client.GetAsync($"/api/cursos/{_cursoId}/admin");
            getCursoResponse.EnsureSuccessStatusCode();
            var cursoAtual = await getCursoResponse.Content.ReadFromJsonAsync<CursoAdmViewModel>();
            if (cursoAtual.Ativo)
            {
                await _testsFixture.Client.DeleteAsync($"/api/cursos/{_cursoId}/inativar");
            }

            // Act
            var response = await _testsFixture.Client.PatchAsync($"/api/cursos/{_cursoId}/ativar", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
            Assert.NotNull(responseContent.Id);
            Assert.Equal(_cursoId, responseContent.Id);
        }

        [Fact(DisplayName = "Inativar curso como Administrador com ID válido (criado) deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(60)]
        public async Task Cursos_InativarCurso_ComoAdministrador_ComIdValido_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _cursoId);

            var getCursoResponse = await _testsFixture.Client.GetAsync($"/api/cursos/{_cursoId}/admin");
            getCursoResponse.EnsureSuccessStatusCode();
            var cursoAtual = await getCursoResponse.Content.ReadFromJsonAsync<CursoAdmViewModel>();
            if (!cursoAtual.Ativo)
            {
                await _testsFixture.Client.PatchAsync($"/api/cursos/{_cursoId}/ativar", null);
            }

            // Act
            var response = await _testsFixture.Client.DeleteAsync($"/api/cursos/{_cursoId}/inativar");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
            Assert.NotNull(responseContent.Id);
            Assert.Equal(_cursoId, responseContent.Id);
        }

        [Fact(DisplayName = "Remover aula de curso como Administrador com IDs válidos (criados) deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(70)]
        public async Task Cursos_RemoverAula_ComoAdministrador_ComIdsValidos_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _cursoId);
            Assert.NotEqual(Guid.Empty, _aulaId);

            // Act
            var response = await _testsFixture.Client.DeleteAsync($"/api/cursos/{_cursoId}/aulas/{_aulaId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
        }

        [Fact(DisplayName = "Cadastrar curso como Administrador sem título deve retornar BadRequest")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(80)]
        public async Task Cursos_CadastrarCurso_ComoAdministrador_SemTitulo_DeveRetornarBadRequest()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            var novoCurso = new
            {
                Valor = 199.99m,
                ConteudoProgramatico = new
                {
                    Descricao = "Descrição do curso de teste.",
                    CargaHoraria = 40
                }
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync("/api/cursos", novoCurso);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
            var responseContent = JsonConvert.DeserializeObject<ResponseError>(await response.Content.ReadAsStringAsync());
            Assert.Contains("O título do curso deve ser informado.", responseContent.Errors.SelectMany(v => v.Value));
        }

        [Fact(DisplayName = "Cadastrar curso como Administrador sem conteúdo programático deve retornar BadRequest")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(81)]
        public async Task Cursos_CadastrarCurso_ComoAdministrador_SemConteudoProgramatico_DeveRetornarBadRequest()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            var novoCurso = new
            {
                Titulo = $"Curso Teste Validação {Guid.NewGuid()}",
                Valor = 199.99m,
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync("/api/cursos", novoCurso);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
            var responseContent = JsonConvert.DeserializeObject<ResponseError>(await response.Content.ReadAsStringAsync());
            Assert.Contains("O conteúdo programático do curso deve ser informado.", responseContent.Errors.SelectMany(v => v.Value));
        }

        [Fact(DisplayName = "Obter curso por ID como anônimo com ID inválido deve retornar NotFound")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(90)]
        public async Task Cursos_ObterCursoPorIdAnonimo_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            var cursoIdInvalido = Guid.NewGuid();

            // Act
            var response = await _testsFixture.Client.GetAsync($"/api/cursos/{cursoIdInvalido}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Alterar curso como Administrador com ID inválido deve retornar NotFound")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(91)]
        public async Task Cursos_AlterarCurso_ComoAdministrador_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            var cursoIdInvalido = Guid.NewGuid();
            var cursoAlterado = new
            {
                Titulo = $"Curso Inexistente Alterado {Guid.NewGuid()}",
                Valor = 250.00m,
                ConteudoProgramatico = new
                {
                    Descricao = "Descrição de curso inexistente.",
                    CargaHoraria = 60
                }
            };

            // Act
            var response = await _testsFixture.Client.PutAsJsonAsync($"/api/cursos/{cursoIdInvalido}", cursoAlterado);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Obter cursos para Admin como Administrador deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(100)]
        public async Task Cursos_ObterCursosParaAdmin_ComoAdministrador_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            // Act
            var response = await _testsFixture.Client.GetAsync("/api/cursos/admin");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cursos = await response.Content.ReadFromJsonAsync<List<CursoAdmViewModel>>();
            Assert.NotNull(cursos);
        }

        [Fact(DisplayName = "Obter cursos apenas ativos para Admin como Administrador deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(100)]
        public async Task Cursos_ObterCursosApenasAtivosParaAdmin_ComoAdministrador_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            // Act
            var response = await _testsFixture.Client.GetAsync("/api/cursos/admin?ativo=true");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cursos = await response.Content.ReadFromJsonAsync<List<CursoAdmViewModel>>();
            Assert.NotNull(cursos);
        }

        [Fact(DisplayName = "Obter curso por ID para Admin como Administrador com ID inválido deve retornar NotFound")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(101)]
        public async Task Cursos_ObterCursoPorIdParaAdmin_ComoAdministrador_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            var cursoIdInvalido = Guid.NewGuid();

            // Act
            var response = await _testsFixture.Client.GetAsync($"/api/cursos/{cursoIdInvalido}/admin");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}
