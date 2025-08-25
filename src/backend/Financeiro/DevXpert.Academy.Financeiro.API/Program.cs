using DevXpert.Academy.Core.APIModel.BackgroundServices;
using DevXpert.Academy.Core.APIModel.Configurations;
using DevXpert.Academy.Core.APIModel.Middlewares;
using DevXpert.Academy.Financeiro.API.Configurations;
using DevXpert.Academy.Financeiro.API.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DevXpert.Academy.Financeiro.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApiConfig();
            builder.Services.AddCorsConfig(builder.Configuration);
            builder.Services.AddOpenApi();
            builder.Services.AddDbContextConfig(builder.Configuration, builder.Environment);
            builder.Services.AddApiSecurity(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddSwaggerConfig();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
            builder.Services.AddDIConfigurationDefault(builder.Configuration, builder.Environment);
            builder.Services.AddDIConfiguration();

            builder.Services.AddQueue(builder.Configuration, () =>
            {
                return new RabbitMQOptions
                {
                    BaseQueueName = "financeiro",
                    MessageTypes = new Dictionary<string, Type>
                    {
                        //{ "NomeDaMensagem", typeof(NomeDaMensagem) }
                    }
                };
            });

            var app = builder.Build();

            app.ConfigureExceptionHandler(app.Environment, app.Services.GetRequiredService<ILoggerFactory>());

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapOpenApi();
                app.UseCors("Development");
            }
            else
                app.UseCors("Production");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseDbMigrationHelper();

            app.Run();
        }
    }
}