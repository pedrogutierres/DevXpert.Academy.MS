namespace DevXpert.Academy.BFF.API.Configurations
{
    public class MicroserviceConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class MicroservicesSettings
    {
        public MicroserviceConfig CustomerApi { get; set; } = new();
        public MicroserviceConfig OrderApi { get; set; } = new();
        public MicroserviceConfig PaymentApi { get; set; } = new();
    }
}
