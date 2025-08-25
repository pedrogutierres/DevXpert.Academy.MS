namespace DevXpert.Academy.BFF.API.Configurations
{
    public class MicroserviceConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class MicroservicesSettings
    {
        public MicroserviceConfig AuthApi { get; set; } = new();
        public MicroserviceConfig AlunosApi { get; set; } = new();
        public MicroserviceConfig ConteudoApi { get; set; } = new();
        public MicroserviceConfig FinanceiroApi { get; set; } = new();
    }
}
