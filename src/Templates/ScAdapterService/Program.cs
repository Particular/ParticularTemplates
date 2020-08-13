using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScAdapterService
{
    static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                // TODO: NServiceBus.Extensions.Hosting comes with built-in logging support for Microsoft.Extensions.Logging
                // optionally you can use the logging integrations for your logger of choice with the microsoft logging or
                // choose a custom logging library if you favour the NServiceBus logger
                // https://docs.particular.net/nservicebus/logging/#custom-logging
                // LogManager.Use<TheLoggingFactory>();
                .ConfigureLogging(logging =>
                {
                    logging.AddEventLog();
                })
                .ConfigureServices(services => services.AddHostedService<AdapterHostedService>());
        }
    }
}