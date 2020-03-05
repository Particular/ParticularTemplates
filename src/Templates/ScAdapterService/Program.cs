using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus.Logging;

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
                .ConfigureServices(services => services.AddHostedService<AdapterHostedService>());
        }

        // TODO: optionally choose a custom logging library
        // https://docs.particular.net/nservicebus/logging/#custom-logging
        // LogManager.Use<TheLoggingFactory>();
        static readonly ILog log = LogManager.GetLogger(typeof(Program));
    }
}