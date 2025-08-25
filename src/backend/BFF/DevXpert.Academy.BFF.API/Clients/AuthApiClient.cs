using DevXpert.Academy.BFF.API.ViewModels.Usuarios;
using DevXpert.Academy.Core.APIModel.ResponseType;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using System.Text;
using System.Text.Json;

namespace DevXpert.Academy.BFF.API.Services
{
    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IMediatorHandler _mediator;

        public AuthApiClient(HttpClient httpClient, IMediatorHandler mediator)
        {
            _httpClient = httpClient;
            _mediator = mediator;
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

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var error = JsonSerializer.Deserialize<ResponseError>(responseContent);
                    if (error != null)
                    {
                        if (error.Errors?.Count > 0)
                        {
                            foreach (var e in error.Errors.SelectMany(p => p.Value))
                                await _mediator.RaiseEvent(new DomainNotification("AuthApiClient", e));
                        }
                        else if (!string.IsNullOrEmpty(error.Detail))
                        {
                            await _mediator.RaiseEvent(new DomainNotification("AuthApiClient", error.Detail));
                        }
                        else 
                        {
                            await _mediator.RaiseEvent(new DomainNotification("AuthApiClient", "Erro não identificado ao cadastrar usuário no servidor de Autorização."));
                        }

                        return null;
                    }

                    await _mediator.RaiseEvent(new DomainNotification("AuthApiClient", "Erro desconhecido ao cadastrar usuário no servidor de Autorização."));
                }
                catch (Exception ex)
                {
                    await _mediator.RaiseEvent(new DomainNotification("AuthApiClient", $"Erro ao processar a resposta do servidor de Autorização: {ex.Message}"));
                }

                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthToken>();
        }
    }

}
