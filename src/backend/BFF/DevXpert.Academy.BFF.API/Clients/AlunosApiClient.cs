using DevXpert.Academy.BFF.API.ViewModels.Alunos;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;

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

    }
}