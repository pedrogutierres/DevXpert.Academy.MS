using DevXpert.Academy.Core.APIModel.ResponseType;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using System.Text.Json;

namespace DevXpert.Academy.BFF.API.Services
{
    public abstract class BaseApiClient
    {
        protected readonly string _keyNotification;

        protected readonly HttpClient _httpClient;
        protected readonly IMediatorHandler _mediator;

        public BaseApiClient(HttpClient httpClient, IMediatorHandler mediator)
        {
            _keyNotification = GetType().Name;

            _httpClient = httpClient;
            _mediator = mediator;
        }

        protected async Task<T?> ProcessarRetorno<T>(HttpResponseMessage response) where T : class
        {
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
                                await _mediator.RaiseEvent(new DomainNotification(_keyNotification, e));
                        }
                        else if (!string.IsNullOrEmpty(error.Detail))
                        {
                            await _mediator.RaiseEvent(new DomainNotification(_keyNotification, error.Detail));
                        }
                        else
                        {
                            await _mediator.RaiseEvent(new DomainNotification(_keyNotification, "Erro não identificado ao comunicar com servidor."));
                        }

                        return default;
                    }

                    await _mediator.RaiseEvent(new DomainNotification(_keyNotification, "Erro desconhecido ao comunicar com servidor."));
                }
                catch (Exception ex)
                {
                    await _mediator.RaiseEvent(new DomainNotification(_keyNotification, $"Erro ao processar a resposta do servidor: {ex.Message}"));
                }

                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
