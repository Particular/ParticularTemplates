using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace NsbDockerEndpoint
{
    class Program
    {
        static IEndpointInstance endpoint;
        static AutoResetEvent closingEvent = new AutoResetEvent(false);

        // TODO: replace the license.xml file with your license file

        // TODO: optionally choose a custom logging library
        // https://docs.particular.net/nservicebus/logging/#custom-logging
        // LogManager.Use<TheLoggingFactory>();
        static readonly ILog log = LogManager.GetLogger<Program>();

        // TODO: give the endpoint an appropriate name
        static string EndpointName => "NsbDockerEndpoint";

        // TODO: consider using C# 7.1 or later, which will allow
        // removal of this method, and renaming of MainAsync to Main
        public static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //required to identify when a "docker stop" command has been issued on a Windows container
                    //  and allow for graceful shutdown of the endpoint
                    SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
                }
                else
                {
                    AppDomain.CurrentDomain.ProcessExit += ProcessExit;
                }

                Console.Title = EndpointName;

                var endpointConfiguration = new EndpointConfiguration(EndpointName);

                // TODO: ensure the most appropriate serializer is chosen
                // https://docs.particular.net/nservicebus/serialization/
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

                endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);

                // TODO: remove this condition after choosing a transport, persistence and deployment method suitable for production
                if (Environment.UserInteractive && Debugger.IsAttached)
                {
                    // TODO: choose a durable transport for production
                    // https://docs.particular.net/transports/
                    var transportExtensions = endpointConfiguration.UseTransport<LearningTransport>();

                    // TODO: choose a durable persistence for production
                    // https://docs.particular.net/persistence/
                    endpointConfiguration.UsePersistence<LearningPersistence>();

                    // TODO: create a script for deployment to production
                    endpointConfiguration.EnableInstallers();
                }

                // TODO: perform any futher start up operations before or after starting the endpoint
                endpoint = await Endpoint.Start(endpointConfiguration);

                // Wait until notified that the process should exit
                closingEvent.WaitOne();

                // TODO: perform any futher shutdown operations before or after stopping the endpoint

                await endpoint.Stop();
            }
            catch (Exception ex)
            {
                FailFast("Failed to start.", ex);
            }
        }

        static Task OnCriticalError(ICriticalErrorContext context)
        {
            // TODO: decide if stopping the endpoint and exiting the process is the best response to a critical error
            // https://docs.particular.net/nservicebus/hosting/critical-errors
            try
            {
                //notify the MainAsync method to continue executing past the closingEvent.WaitOne
                closingEvent.Set();
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }

            return Task.CompletedTask;
        }

        static void ProcessExit(object sender, EventArgs e)
        {
            try
            {
                //notify the MainAsync method to continue executing past the closingEvent.WaitOne
                closingEvent.Set();
            }
            catch (Exception ex)
            {
                FailFast("Failed to stop correctly.", ex);
            }
        }

        static void FailFast(string message, Exception exception)
        {
            try
            {
                log.Fatal(message, exception);

                // TODO: when using an external logging framework it is important to flush any pending entries prior to calling FailFast
                // https://docs.particular.net/nservicebus/hosting/critical-errors#when-to-override-the-default-critical-error-action
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            if (ctrlType == CtrlTypes.CTRL_CLOSE_EVENT)
            {
                closingEvent.Set();
            }
            return true;
        }

        #region 
        //Imports required to successfully notice when "docker stop <containerid>" has been run
        //  and allow for a graceful shutdown of the endpoint
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public enum CtrlTypes
        {
            CTRL_CLOSE_EVENT = 2
        }
        #endregion
    }
}