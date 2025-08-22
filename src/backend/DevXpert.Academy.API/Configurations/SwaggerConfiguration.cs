using DevXpert.Academy.API.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace DevXpert.Academy.API.Configurations
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DevXpert.Academy API",
                    Description = "API do projeto DevXpert.Academy",
                    TermsOfService = new Uri("https://devxpert.academy/"),
                    Contact = new OpenApiContact
                    {
                        Name = "DevXpert Academy",
                        Email = "contato@devxpertacademy.com.br",
                        Url = new Uri("https://devxpert.academy/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "CC BY-NC-ND",
                        Url = new Uri("https://creativecommons.org/licenses/by-nc-nd/4.0/legalcode")
                    }
                });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Scheme = "Bearer",
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Type = SecuritySchemeType.ApiKey
                });

                s.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });
        }

        public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(s =>
                {
                    s.SwaggerEndpoint("/swagger/v1/swagger.json", "DevXpert.Academy API v1.0");
                });
            }

            return app;
        }
    }
}
