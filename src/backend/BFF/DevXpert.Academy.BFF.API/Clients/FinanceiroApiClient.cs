using DevXpert.Academy.BFF.API.ViewModels.Pagamentos;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;

namespace DevXpert.Academy.BFF.API.Clients
{
    public class FinanceiroApiClient : BaseApiClient
    {
        public FinanceiroApiClient(HttpClient httpClient, IMediatorHandler mediator) : base(httpClient, mediator)
        { }

        public async Task<List<PagamentoViewModel>> ObterPagamentosAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("/api/pagamentos");

            return await ProcessarRetorno<List<PagamentoViewModel>>(response);
        }

        public async Task<PagamentoViewModel> ObterPagamentoPorIdAsync(string accessToken, Guid id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"/api/pagamentos/{id}");

            return await ProcessarRetorno<PagamentoViewModel>(response);
        }

        public async Task<List<PagamentoViewModel>> ObterPagamentosPorAlunoAsync(string accessToken, Guid alunoId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"/api/pagamentos/alunos/{alunoId}");

            return await ProcessarRetorno<List<PagamentoViewModel>>(response);
        }
    }
}