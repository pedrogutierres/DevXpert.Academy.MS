using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DevXpert.Academy.API.Tests.Config
{
    public class AcademyAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            ExcluitarBaseDeDados();

            builder.UseEnvironment("Test");

            return base.CreateHost(builder);
        }

        private void ExcluitarBaseDeDados()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Test.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnectionSqLite") ?? "DevXpertAcademyTest.db";

            var dbPath = connectionString.Replace("Data Source=", "").Trim();

            if (File.Exists(Path.Combine(AppContext.BaseDirectory, dbPath)))
                File.Delete(Path.Combine(AppContext.BaseDirectory, dbPath));
        }

    }
}
