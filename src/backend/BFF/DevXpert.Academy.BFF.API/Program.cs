using DevXpert.Academy.BFF.API.Clients;
using DevXpert.Academy.BFF.API.Configurations;
using DevXpert.Academy.Core.APIModel.BackgroundServices;
using DevXpert.Academy.Core.APIModel.Configurations;
using DevXpert.Academy.Core.APIModel.Middlewares;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfig();
builder.Services.AddCorsConfig(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddApiSecurity(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSwaggerConfig();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddDIConfigurationDefault(builder.Configuration, builder.Environment);

builder.Services.Configure<MicroservicesSettings>(
    builder.Configuration.GetSection("Microservices"));

builder.Services.AddHttpClient<AuthApiClient>("AuthApiClient", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<MicroservicesSettings>>().Value;
    client.BaseAddress = new Uri(settings.AuthApi.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.AuthApi.TimeoutSeconds);
});

builder.Services.AddHttpClient<AlunosApiClient>("AlunosApi", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<MicroservicesSettings>>().Value;
    client.BaseAddress = new Uri(settings.AlunosApi.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.AlunosApi.TimeoutSeconds);
});

builder.Services.AddHttpClient<ConteudoApiClient>("ConteudoApi", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<MicroservicesSettings>>().Value;
    client.BaseAddress = new Uri(settings.ConteudoApi.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.ConteudoApi.TimeoutSeconds);
});

builder.Services.AddHttpClient<FinanceiroApiClient>("FinanceiroApi", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<MicroservicesSettings>>().Value;
    client.BaseAddress = new Uri(settings.FinanceiroApi.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.FinanceiroApi.TimeoutSeconds);
});

builder.Services.AddQueue(builder.Configuration, () =>
{
    return new RabbitMQOptions
    {
        BaseQueueName = "bff",
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

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
