using DevXpert.Academy.API.ResponseType;
using DevXpert.Academy.API.Tests.Config;
using DevXpert.Academy.API.ViewModels.Alunos;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace DevXpert.Academy.API.Tests
{
    [TestCaseOrderer("DevXpert.Academy.API.Tests.Config.PriorityOrderer", "DevXpert.Academy.API.Tests")]
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class MatriculasControllersTests
    {
        private readonly IntegrationTestsFixture<Program> _testsFixture;
        private static Guid _curso1Id = Guid.Empty;
        private static Guid _curso2Id = Guid.Empty;
        private static Guid _matriculaId = Guid.Empty;

        public MatriculasControllersTests(IntegrationTestsFixture<Program> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Cadastrar curso como Administrador com dados válidos deve retornar sucesso")]
        [Trait("Cursos", "Integração API - Cursos")]
        [TestPriority(1)]
        public async Task Cursos_CadastrarCurso_ComoAdministrador_ComDadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();

            var novoCurso1 = new
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

            var novoCurso2 = new
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
                    new { Titulo = "Aula Inicial 1", VideoUrl = "http://video.com/aula-inicial-1" }
                }
            };

            // Act
            var response1 = await _testsFixture.Client.PostAsJsonAsync("/api/cursos", novoCurso1);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            var responseContent1 = await response1.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent1);
            Assert.NotNull(responseContent1.Id);
            Assert.NotEqual(Guid.Empty, responseContent1.Id);
            _curso1Id = responseContent1.Id.Value;

            // Act
            var response2 = await _testsFixture.Client.PostAsJsonAsync("/api/cursos", novoCurso2);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            var responseContent2 = await response2.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent2);
            Assert.NotNull(responseContent2.Id);
            Assert.NotEqual(Guid.Empty, responseContent2.Id);
            _curso2Id = responseContent2.Id.Value;
        }

        [Fact(DisplayName = "Aluno se matricular em curso deve retornar sucesso e registrar pagamento")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(10)]
        public async Task Matriculas_AlunoSeMatricular_DeveRetornarSucessoERegistrarPagamento()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();

            Assert.NotEqual(Guid.Empty, _testsFixture.UsuarioId);
            Assert.NotEqual(Guid.Empty, _curso1Id);

            var pagamento = new
            {
                DadosCartao_Nome = "Aluno Teste",
                DadosCartao_Numero = "1111222233334444",
                DadosCartao_Vencimento = "12/26",
                DadosCartao_CcvCvc = "123"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync($"/api/matriculas/cursos/{_curso1Id}/se-matricular", pagamento);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
            Assert.NotNull(responseContent.Id);
            Assert.NotEqual(Guid.Empty, responseContent.Id);
            _matriculaId = responseContent.Id.Value;
        }

        [Fact(DisplayName = "Aluno se matricular em curso deve retornar sucesso e pagamento ser recusado")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(11)]
        public async Task Matriculas_AlunoSeMatricular_DeveRetornarSucessoERecusarPagamento()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();

            Assert.NotEqual(Guid.Empty, _testsFixture.UsuarioId);
            Assert.NotEqual(Guid.Empty, _curso2Id);

            var pagamento = new
            {
                DadosCartao_Nome = "Recusado",
                DadosCartao_Numero = "1111222233334444",
                DadosCartao_Vencimento = "12/26",
                DadosCartao_CcvCvc = "123"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync($"/api/matriculas/cursos/{_curso2Id}/se-matricular", pagamento);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "Aluno se matricular em curso inexistente deve retornar BadRequest")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(12)]
        public async Task Matriculas_AlunoSeMatricular_CursoInexistente_DeveRetornarBadRequest()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();
            var cursoIdInexistente = Guid.NewGuid();

            var pagamento = new
            {
                DadosCartao_Nome = "Aluno Teste",
                DadosCartao_Numero = "1111222233334444",
                DadosCartao_Vencimento = "12/26",
                DadosCartao_CcvCvc = "123"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync($"/api/matriculas/cursos/{cursoIdInexistente}/se-matricular", pagamento);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorContent = JsonConvert.DeserializeObject<ResponseError>(await response.Content.ReadAsStringAsync());
            Assert.Contains("Curso não encontrado.", errorContent.Errors.SelectMany(v => v.Value));
        }

        [Fact(DisplayName = "Aluno se matricular em curso que já está matriculado (ativo) deve retornar sucesso sem novo pagamento")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(13)]
        public async Task Matriculas_AlunoSeMatricular_JaMatriculadoAtivo_DeveRetornarSucessoSemNovoPagamento()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();
            Assert.NotEqual(Guid.Empty, _testsFixture.UsuarioId);
            Assert.NotEqual(Guid.Empty, _curso1Id);
            Assert.NotEqual(Guid.Empty, _matriculaId);

            var pagamento = new
            {
                DadosCartao_Nome = "Aluno Teste",
                DadosCartao_Numero = "1111222233334444",
                DadosCartao_Vencimento = "12/26",
                DadosCartao_CcvCvc = "123"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync($"/api/matriculas/cursos/{_curso1Id}/se-matricular", pagamento);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
            Assert.Equal(_matriculaId, responseContent.Id);
        }

        [Fact(DisplayName = "Realizar pagamento de matrícula não ativa deve retornar Sucesso")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(20)]
        public async Task Matriculas_RealizarPagamento_MatriculaInativa_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();

            var responseMeuPerfil = await _testsFixture.Client.GetAsync("/api/alunos/meu-perfil");

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMeuPerfil.StatusCode);
            var meuPerfil = await responseMeuPerfil.Content.ReadFromJsonAsync<MeuPerfilViewModel>();
            Assert.NotNull(meuPerfil);

            var matriculaId = meuPerfil.Matriculas.FirstOrDefault(p => !p.Ativa).Id;

            var pagamento = new
            {
                DadosCartao_Nome = "Aluno Teste Pagamento",
                DadosCartao_Numero = "5555666677778888",
                DadosCartao_Vencimento = "12/28",
                DadosCartao_CcvCvc = "456"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync($"/api/matriculas/{matriculaId}/realizar-pagamento", pagamento);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Realizar pagamento de matrícula já ativa deve retornar BadRequest")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(21)]
        public async Task Matriculas_RealizarPagamento_MatriculaAtiva_DeveRetornarBadRequest()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();
            Assert.NotEqual(Guid.Empty, _matriculaId);

            var pagamento = new
            {
                DadosCartao_Nome = "Aluno Teste Pagamento",
                DadosCartao_Numero = "5555666677778888",
                DadosCartao_Vencimento = "12/28",
                DadosCartao_CcvCvc = "456"
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync($"/api/matriculas/{_matriculaId}/realizar-pagamento", pagamento);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorContent = JsonConvert.DeserializeObject<ResponseError>(await response.Content.ReadAsStringAsync());
            Assert.Contains("Matrícula já está ativa, não é necessário realizar um novo pagamento.", errorContent.Errors.SelectMany(v => v.Value));
        }

        [Fact(DisplayName = "Administrador matricular aluno em curso inexistente deve retornar BadRequest")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(31)]
        public async Task Matriculas_AdministradorMatricularAluno_CursoInexistente_DeveRetornarBadRequest()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _testsFixture.UsuarioId);
            var cursoIdInexistente = Guid.NewGuid();

            // Act
            var response = await _testsFixture.Client.PostAsync($"/api/matriculas/cursos/{cursoIdInexistente}/matricular-aluno/{_testsFixture.UsuarioId}", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorContent = JsonConvert.DeserializeObject<ResponseError>(await response.Content.ReadAsStringAsync());
            Assert.Contains("Curso não encontrado.", errorContent.Errors.SelectMany(v => v.Value));
        }

        [Fact(DisplayName = "Administrador matricular aluno inexistente deve retornar BadRequest")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(32)]
        public async Task Matriculas_AdministradorMatricularAluno_AlunoInexistente_DeveRetornarBadRequest()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAdministrador();
            Assert.NotEqual(Guid.Empty, _curso1Id);
            var alunoIdInexistente = Guid.NewGuid();

            // Act
            var response = await _testsFixture.Client.PostAsync($"/api/matriculas/cursos/{_curso1Id}/matricular-aluno/{alunoIdInexistente}", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorContent = JsonConvert.DeserializeObject<ResponseError>(await response.Content.ReadAsStringAsync());
            Assert.Contains("Aluno não encontrado.", errorContent.Errors.SelectMany(v => v.Value)); // Mensagem da BusinessException
        }

        [Fact(DisplayName = "Aluno registrar aula concluida em matricula ativa deve retornar sucesso")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(35)]
        public async Task Matriculas_AlunoRegistrarAulaConcluida_EmMatriculaAtiva_DeveRetornarSucesso()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();

            Assert.NotEqual(Guid.Empty, _curso1Id);

            var responseGetCourse = await _testsFixture.Client.GetAsync($"/api/cursos/{_curso1Id}");
            responseGetCourse.EnsureSuccessStatusCode(); // Assert success for fetching course
            var courseContent = await responseGetCourse.Content.ReadAsStringAsync();
            dynamic cursoDetails = JsonConvert.DeserializeObject(courseContent);
            Assert.NotNull(cursoDetails);
            Assert.NotNull(cursoDetails.aulas);
            Assert.True(cursoDetails.aulas.Count > 0);

            var aulaId = Guid.Parse(cursoDetails.aulas[0].id.ToString());
            Assert.NotEqual(Guid.Empty, aulaId); 

            // Act
            var response = await _testsFixture.Client.PostAsync($"/api/matriculas/{_matriculaId}/registrar-aula-concluida/{aulaId}", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
            Assert.Equal(aulaId, responseContent.Id);
        }

        [Fact(DisplayName = "Aluno obter certificado de matricula não concluida deve retornar BadRequest")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(36)]
        public async Task Matriculas_AlunoObterCertificado_MatriculaNaoConcluida_DeveRetornarBadRequest()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();
            Assert.NotEqual(Guid.Empty, _matriculaId);

            // Act
            var response = await _testsFixture.Client.PostAsync($"/api/matriculas/{_matriculaId}/certificado", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "Aluno obter certificado de matricula ativa e concluida deve retornar sucesso e URL do certificado")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(37)]
        public async Task Matriculas_AlunoObterCertificado_MatriculaConcluida_DeveRetornarSucessoEUrl()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();

            Assert.NotEqual(Guid.Empty, _matriculaId);
            Assert.NotEqual(Guid.Empty, _curso1Id);

            var responseGetCourse = await _testsFixture.Client.GetAsync($"/api/cursos/{_curso1Id}");
            responseGetCourse.EnsureSuccessStatusCode(); // Assert success for fetching course
            var courseContent = await responseGetCourse.Content.ReadAsStringAsync();
            dynamic cursoDetails = JsonConvert.DeserializeObject(courseContent);
            Assert.NotNull(cursoDetails);
            Assert.NotNull(cursoDetails.aulas);
            Assert.True(cursoDetails.aulas.Count > 0);

            foreach (var aula in cursoDetails.aulas)
            {
                var aulaId = Guid.Parse(aula.id.ToString());

                var registrarAulaResponse = await _testsFixture.Client.PostAsync($"/api/matriculas/{_matriculaId}/registrar-aula-concluida/{aulaId}", null);
                registrarAulaResponse.EnsureSuccessStatusCode();
            }

            // Act
            var response = await _testsFixture.Client.PostAsync($"/api/matriculas/{_matriculaId}/certificado", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Aluno registrar aula concluida em matricula inexistente deve retornar BadRequest")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(38)]
        public async Task Matriculas_AlunoRegistrarAulaConcluida_MatriculaInexistente_DeveRetornarBadRequest()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();
            var matriculaIdInexistente = Guid.NewGuid();
            var aulaId = Guid.NewGuid();

            // Act
            var response = await _testsFixture.Client.PostAsync($"/api/matriculas/{matriculaIdInexistente}/registrar-aula-concluida/{aulaId}", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorContent = JsonConvert.DeserializeObject<ResponseError>(await response.Content.ReadAsStringAsync());
            Assert.Contains("Matrícula não encontrada.", errorContent.Errors.SelectMany(v => v.Value));
        }

        [Fact(DisplayName = "Cancelar matricula por Aluno deve retornar sucesso e solicitar estorno")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(40)]
        public async Task Matriculas_CancelarMatricula_PorAluno_DeveRetornarSucessoESolicitarEstorno()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();
            Assert.NotEqual(Guid.Empty, _matriculaId); // A matrícula deve existir

            var cancelamento = new
            {
                Motivo = "Desistência do curso pelo aluno."
            };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/matriculas/{_matriculaId}")
            {
                Content = JsonContent.Create(cancelamento)
            };
            var response = await _testsFixture.Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<ResponseSuccess>();
            Assert.NotNull(responseContent);
            Assert.Equal(_matriculaId, responseContent.Id);
        }

        [Fact(DisplayName = "Aluno obter certificado de matricula inexistente deve retornar BadRequest")]
        [Trait("Matriculas", "Integração API - Matrículas")]
        [TestPriority(60)]
        public async Task Matriculas_AlunoObterCertificado_MatriculaInexistente_DeveRetornarBadRequest()
        {
            // Arrange
            await _testsFixture.RealizarLoginDeAluno();
            var matriculaIdInexistente = Guid.NewGuid();

            // Act
            var response = await _testsFixture.Client.PostAsync($"/api/matriculas/{matriculaIdInexistente}/certificado", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorContent = JsonConvert.DeserializeObject<ResponseError>(await response.Content.ReadAsStringAsync());
            Assert.Contains("Matrícula não encontrada.", errorContent.Errors.SelectMany(v => v.Value));
        }
    }
}