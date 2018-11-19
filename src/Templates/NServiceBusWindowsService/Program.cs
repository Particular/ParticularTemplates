using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace NServiceBusWindowsService
{
    static class Program
    {
        // TODO: consider using C# 7.1 or later, which will allow
        // removal of this method, and renaming of MainAsync to Main
        public static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        public async static Task MainAsync(string[] args)
        {
            var host = new Host();

            // pass this command line option to run as a windows service
            if (args.Contains("--run-as-service"))
            {
                using (var windowsService = new WindowsService(host))
                {
                    ServiceBase.Run(windowsService);
                    return;
                }
            }

            Console.Title = host.EndpointName;

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => { e.Cancel = true; tcs.SetResult(null); };

            await host.Start();
            await Console.Out.WriteLineAsync("Press Ctrl+C to exit...");

            await tcs.Task;
            await host.Stop();
        }
    }
}
