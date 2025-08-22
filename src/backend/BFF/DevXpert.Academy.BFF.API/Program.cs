using DevXpert.Academy.BFF.API.Configurations;
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
builder.Services.AddQueue(builder.Configuration);
builder.Services.AddDIConfigurationDefault(builder.Configuration, builder.Environment);

builder.Services.Configure<MicroservicesSettings>(
    builder.Configuration.GetSection("Microservices"));

builder.Services.AddHttpClient("AlunosApi", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<MicroservicesSettings>>().Value;
    client.BaseAddress = new Uri(settings.CustomerApi.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.CustomerApi.TimeoutSeconds);
});

builder.Services.AddHttpClient("ConteudoApi", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<MicroservicesSettings>>().Value;
    client.BaseAddress = new Uri(settings.OrderApi.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.OrderApi.TimeoutSeconds);
});

builder.Services.AddHttpClient("FinanceiroApi", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<MicroservicesSettings>>().Value;
    client.BaseAddress = new Uri(settings.OrderApi.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.OrderApi.TimeoutSeconds);
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
