using DevXpert.Academy.API.Configurations;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace DevXpert.Academy.API.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationWebTestsFixtureCollection))]
    public class IntegrationWebTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture<Program>> { }

    [CollectionDefinition(nameof(IntegrationApiTestsFixtureCollection))]
    public class IntegrationApiTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture<Program>> { }

    public class IntegrationTestsFixture<TProgram> : IDisposable where TProgram : class
    {
        public string Token;

        public Guid UsuarioId;

        private readonly AcademyAppFactory<TProgram> _factory;

        public readonly HttpClient Client;

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
                BaseAddress = new Uri("https://localhost"),
                HandleCookies = true,
                MaxAutomaticRedirections = 7
            };

            _factory = new AcademyAppFactory<TProgram>();

            Client = _factory.CreateClient(clientOptions);
        }

        public async Task RealizarLoginDeAdministrador()
        {
            var userData = new
            {
                Email = "admin@academy.com",
                Senha = "Academy@123456"
            };

            //_client = _factory.CreateClient();

            var response = await Client.PostAsJsonAsync("/api/usuarios/login", userData);

            response.EnsureSuccessStatusCode();

            var contentString = await response.Content.ReadAsStringAsync();
            var authToken = JsonConvert.DeserializeObject<AuthToken>(contentString) ?? throw new Exception($"Não foi possível realizar o login do usuário {userData.Email}");

            Token = authToken.Result.Access_token;
            UsuarioId = authToken.Result.User.Id;

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
        }
        public async Task RealizarLoginDeAluno()
        {
            var userData = new
            {
                Email = "pedro@gmail.com",
                Senha = "Pedro@123456"
            };

            //_client = _factory.CreateClient();

            var response = await Client.PostAsJsonAsync("/api/usuarios/login", userData);

            response.EnsureSuccessStatusCode();

            var contentString = await response.Content.ReadAsStringAsync();
            var authToken = JsonConvert.DeserializeObject<AuthToken>(contentString) ?? throw new Exception($"Não foi possível realizar o login do usuário {userData.Email}");

            Token = authToken.Result.Access_token;
            UsuarioId = authToken.Result.User.Id;

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
        }

        public void Dispose()
        {
            Client.Dispose();
            _factory.Dispose();
        }
    }
}
