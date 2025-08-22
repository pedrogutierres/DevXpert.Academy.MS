using DevXpert.Academy.Alunos.Data;
using DevXpert.Academy.API.Authentication;
using DevXpert.Academy.Conteudo.Data;
using DevXpert.Academy.Core.EventSourcing.EventStore.Context;
using DevXpert.Academy.Financeiro.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DevXpert.Academy.API.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddDbContext<ApplicationDbContext>(o =>
            {
                if (env.IsDevelopment() || env.EnvironmentName == "Test")
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnectionLite") ?? throw new InvalidOperationException("String de conexão 'DefaultConnectionLite' não encontrada para banco SQLite em ambiente de desenvolvimento.");
                    o.UseSqlite(connectionString);
                    
                }
                else
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
                    o.UseSqlServer(connectionString);
                }
            });

            services.AddDbContext<EventStoreSQLContext>(o =>
            {
                if (env.IsDevelopment() || env.EnvironmentName == "Test")
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnectionLite") ?? throw new InvalidOperationException("String de conexão 'DefaultConnectionLite' não encontrada para banco SQLite em ambiente de desenvolvimento.");
                    o.UseSqlite(connectionString);

                }
                else
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
                    o.UseSqlServer(connectionString);
                }
            });

            services.AddDbContext<ConteudoContext>();
            services.AddDbContext<AlunosContext>();
            services.AddDbContext<FinanceiroContext>();

            /*services.AddDbContext<ConteudoContext>(options =>
            {
                if (env.IsDevelopment())
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnectionLite") ?? throw new InvalidOperationException("String de conexão 'DefaultConnectionLite' não encontrada para banco SQLite em ambiente de desenvolvimento.");
                    options.UseSqlite(connectionString);

                }
                else
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
                    options.UseSqlServer(connectionString);
                }
            });

            services.AddDbContext<AlunosContext>(options =>
            {
                if (env.IsDevelopment())
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnectionLite") ?? throw new InvalidOperationException("String de conexão 'DefaultConnectionLite' não encontrada para banco SQLite em ambiente de desenvolvimento.");
                    options.UseSqlite(connectionString);

                }
                else
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
                    options.UseSqlServer(connectionString);
                }
            });

            services.AddDbContext<FinanceiroContext>(options =>
            {
                if (env.IsDevelopment())
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnectionLite") ?? throw new InvalidOperationException("String de conexão 'DefaultConnectionLite' não encontrada para banco SQLite em ambiente de desenvolvimento.");
                    options.UseSqlite(connectionString);

                }
                else
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
                    options.UseSqlServer(connectionString);
                }
            });*/

            return services;
        }
    }
}
