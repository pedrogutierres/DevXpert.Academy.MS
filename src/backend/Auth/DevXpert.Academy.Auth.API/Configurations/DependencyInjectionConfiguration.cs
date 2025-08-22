using DevXpert.Academy.Auth.API.Authentication;
using DevXpert.Academy.Auth.API.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevXpert.Academy.Auth.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            services.AddScoped<AutenticacaoService>();

            services.AddScoped<AlunoApiClient>();
        }
    }
}
