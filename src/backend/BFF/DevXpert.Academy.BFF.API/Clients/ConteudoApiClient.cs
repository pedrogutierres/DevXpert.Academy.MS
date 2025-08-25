using DevXpert.Academy.BFF.API.ViewModels.Cursos;
using DevXpert.Academy.Core.APIModel.ResponseType;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using System.Text;
using System.Text.Json;

namespace DevXpert.Academy.BFF.API.Clients
{
    public class ConteudoApiClient : BaseApiClient
    {
        public ConteudoApiClient(HttpClient httpClient, IMediatorHandler mediator) : base(httpClient, mediator)
        { }

        public async Task<List<CursoViewModel>> ObterCursosAsync()
        {
            var response = await _httpClient.GetAsync("/api/cursos");

            return await ProcessarRetorno<List<CursoViewModel>>(response);
        }

        public async Task<CursoViewModel> ObterCursoPorIdAsync(Guid cursoId)
        {
            var response = await _httpClient.GetAsync($"/api/cursos/{cursoId}");

            return await ProcessarRetorno<CursoViewModel>(response);
        }

        public async Task<List<CursoAdmViewModel>> ObterCursosParaAdminAsync(string accessToken, bool? ativo = null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var query = ativo.HasValue ? $"ativo={ativo.Value}" : string.Empty;
            var response = await _httpClient.GetAsync($"/api/cursos/admin{query}");

            return await ProcessarRetorno<List<CursoAdmViewModel>>(response);
        }

        public async Task<CursoAdmViewModel> ObterCursoPorIdParaAdminAsync(string accessToken, Guid cursoId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"/api/cursos/{cursoId}/admin");

            return await ProcessarRetorno<CursoAdmViewModel>(response);
        }

        public async Task<ResponseSuccess> CadastrarCursoAsync(string accessToken, object viewModel)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var content = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/cursos", content);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> AlterarCursoAsync(string accessToken, Guid cursoId, object viewModel)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var content = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/cursos/{cursoId}", content);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> AtivarCursoAsync(string accessToken, Guid cursoId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PatchAsync($"/api/cursos/{cursoId}/ativar", null);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> InativarCursoAsync(string accessToken, Guid cursoId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.DeleteAsync($"/api/cursos/{cursoId}/inativar");

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> CadastrarAulaAsync(string accessToken, Guid cursoId, object viewModel)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var content = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/cursos/{cursoId}/aulas", content);

            return await ProcessarRetorno<ResponseSuccess>(response);
        }

        public async Task<ResponseSuccess> RemoverAulaAsync(string accessToken, Guid cursoId, Guid aulaId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.DeleteAsync($"/api/cursos/{cursoId}/aulas/{aulaId}");

            return await ProcessarRetorno<ResponseSuccess>(response);
        }
    }
}