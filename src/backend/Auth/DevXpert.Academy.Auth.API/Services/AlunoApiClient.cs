using DevXpert.Academy.Core.APIModel.ResponseType;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevXpert.Academy.Auth.API.Services
{
    public class AlunoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IMediatorHandler _mediator;

        public AlunoApiClient(HttpClient httpClient, IMediatorHandler mediator)
        {
            _httpClient = httpClient;
            _mediator = mediator;
        }

        public async Task<bool> CadastrarAlunoAsync(object viewModel, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent(
                JsonSerializer.Serialize(viewModel),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("/api/alunos", content);

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
                                await _mediator.RaiseEvent(new DomainNotification("AlunoApiClient", e));
                        }
                        else if (!string.IsNullOrEmpty(error.Detail))
                        {
                            await _mediator.RaiseEvent(new DomainNotification("AlunoApiClient", error.Detail));
                        }
                        else 
                        {
                            await _mediator.RaiseEvent(new DomainNotification("AlunoApiClient", "Erro não identificado ao cadastrar aluno no servidor de Alunos."));
                        }

                        return false;
                    }

                    await _mediator.RaiseEvent(new DomainNotification("AlunoApiClient", "Erro desconhecido ao cadastrar aluno no servidor de Alunos."));
                }
                catch (Exception ex)
                {
                    await _mediator.RaiseEvent(new DomainNotification("AlunoApiClient", $"Erro ao processar a resposta do servidor de Alunos: {ex.Message}"));
                }

                return false;

            }

            return true;
        }
    }

}
