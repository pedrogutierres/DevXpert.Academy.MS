using DevXpert.Academy.Auth.API.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace DevXpert.Academy.Auth.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            services.AddScoped<AutenticacaoService>();
        }
    }
}
