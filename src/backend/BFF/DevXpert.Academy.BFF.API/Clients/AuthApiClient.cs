using DevXpert.Academy.BFF.API.ViewModels.Usuarios;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using System.Text;
using System.Text.Json;

namespace DevXpert.Academy.BFF.API.Clients
{
    public class AuthApiClient : BaseApiClient
    {
        public AuthApiClient(HttpClient httpClient, IMediatorHandler mediator) : base(httpClient, mediator)
        { }

        public async Task<AuthToken?> LogarUsuarioAsync(object viewModel)
        {
            //_httpClient.DefaultRequestHeaders.Authorization =
            //    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent(
                JsonSerializer.Serialize(viewModel),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("/api/usuarios/login", content);

            return await ProcessarRetorno<AuthToken>(response);
        }

        public async Task<AuthToken?> CadastrarUsuarioAsync(object viewModel)
        {
            //_httpClient.DefaultRequestHeaders.Authorization =
            //    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent(
                JsonSerializer.Serialize(viewModel),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("/api/usuarios/novo-aluno", content);

            return await ProcessarRetorno<AuthToken>(response);
        }
    }

}
