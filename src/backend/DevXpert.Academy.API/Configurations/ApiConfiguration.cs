using DevXpert.Academy.API.ResponseType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DevXpert.Academy.API.Configurations
{
    internal static class ApiConfiguration
    {
        public static IServiceCollection AddApiConfig(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddControllers()
                .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory =
                        context =>
                        {
                            return new BadRequestObjectResult(
                                new ResponseError(
                                    "Erros de validações encontrados",
                                    "Erros nas validações da model enviada",
                                    StatusCodes.Status400BadRequest,
                                    context.HttpContext.Request.Path,
                                    context.ModelState))
                            {
                                ContentTypes = { "application/problem+json" }
                            };
                        };

                });

            return services;
        }
    }
}
