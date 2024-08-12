using Microsoft.Extensions.Configuration;

namespace Service
{
    public class ConfigHelper
    {
        private readonly IConfigurationRoot _root;
        public ConfigHelper()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            _root = builder.Build();
            var defaultConnection = _root.GetConnectionString("DefaultConnection");
        }

        public string SupportAddress
        { 
            get => _root.GetSection("AppSettings:SupportAddress").Value;
        }

        public string SupportPassword
        {
            get => _root.GetSection("AppSettings:SupportPassword").Value;
        }

        public string SalesAddress
        {
            get => _root.GetSection("AppSettings:SalesAddress").Value;
        }

        public string SalesPassword
        {
            get => _root.GetSection("AppSettings:SalesPassword").Value;
        }

        public string SalesAddressPartsCo
        {
            get => _root.GetSection("AppSettings:SalesAddressPartsCo").Value;
        }

        public string SalesPasswordPartsCo
        {
            get => _root.GetSection("AppSettings:SalesPasswordPartsCo").Value;
        }

        public string SMTPHost
        {
            get => _root.GetSection("AppSettings:SMTPHost").Value;
        }





        public static IConfigurationRoot Load(Env? env = null)
        {
            var configurationBuilder = new ConfigurationBuilder();
            AddJsonFiles(configurationBuilder, env);
            return configurationBuilder.Build();
        }
        public static void AddJsonFiles(IConfigurationBuilder configurationBuilder, Env? env = null)
        {
            if (!env.HasValue)
                env = EnvHelper.GetEnvironment();
            configurationBuilder
                  .AddJsonFile($"Config/appsettings.json")
                  .AddJsonFile($"Config/appsettings.{env}.json");
        }

        public enum Env
        {
            Development,
            IntegrationTests,
            QA,
            Staging,
            Production
        }

        public static class EnvHelper
        {
            public static Env GetEnvironment()
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                ArgumentNullException.ThrowIfNull(environmentName);
                return (Env)Enum.Parse(typeof(Env), environmentName);
            }
        }
    }
}
