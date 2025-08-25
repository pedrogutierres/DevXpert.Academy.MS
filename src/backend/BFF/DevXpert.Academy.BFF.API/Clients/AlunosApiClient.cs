using DevXpert.Academy.BFF.API.ViewModels.Alunos;
using DevXpert.Academy.Core.APIModel.ResponseType;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using System.Text;
using System.Text.Json;

namespace DevXpert.Academy.BFF.API.Clients
{
    public class AlunosApiClient : BaseApiClient
    {
        public AlunosApiClient(HttpClient httpClient, IMediatorHandler mediator) : base(httpClient, mediator)
        { }

        public async Task<MeuPerfilViewModel> ObterMeuPerfilAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("/api/alunos/meu-perfil");

            return await ProcessarRetorno<MeuPerfilViewModel>(response);
        }

        public async Task<List<AlunoViewModel>> ObterAlunosAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("/api/alunos");

            return await ProcessarRetorno<List<AlunoViewModel>>(response);
        }

        public async Task<ResponseSuccess?> AlunoSeMatricularAsync(string accessToken, Guid cursoId, object viewModel)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/matriculas/cursos/{cursoId}/se-matricular", content);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> RealizarPagamentoMatriculaAsync(string accessToken, Guid matriculaId, object viewModel)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/matriculas/{matriculaId}/realizar-pagamento", content);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> CancelarMatriculaAsync(string accessToken, Guid matriculaId, object viewModel)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/matriculas/{matriculaId}") { Content = content };

            var response = await _httpClient.SendAsync(request);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> AdministradorMatricularAlunoAsync(string accessToken, Guid cursoId, Guid alunoId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsync($"/api/matriculas/cursos/{cursoId}/matricular-aluno/{alunoId}", null);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> AlunoRegistrarAulaConcluidaAsync(string accessToken, Guid matriculaId, Guid aulaId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsync($"/api/matriculas/{matriculaId}/registrar-aula-concluida/{aulaId}", null);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> AlunoObterCertificadoAsync(string accessToken, Guid matriculaId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsync($"/api/matriculas/{matriculaId}/certificado", null);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }
    }
}