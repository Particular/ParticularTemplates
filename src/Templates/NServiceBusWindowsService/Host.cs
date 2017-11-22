using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace NServiceBusWindowsService
{
    class Host
    {
        // TODO: optionally choose a custom logging library
        // https://docs.particular.net/nservicebus/logging/#custom-logging
        // LogManager.Use<TheLoggingFactory>();
        static readonly ILog log = LogManager.GetLogger<Host>();

        IEndpointInstance endpoint;

        // TODO: give the endpoint an appropriate name
        public string EndpointName => "MyNServiceBusWindowsService";

        public async Task Start()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration(EndpointName);
                endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);

                endpointConfiguration.ApplySettingsForAllEndpoints(transport =>
                {
                    // TODO: apply endpoint-specific transport settings
                });

                // TODO: perform any futher start up operations before or after starting the endpoint
                endpoint = await Endpoint.Start(endpointConfiguration);
            }
            catch (Exception ex)
            {
                await LogFatalAndFailFast("Failed to start.", ex);
            }
        }

        public async Task Stop()
        {
            try
            {
                // TODO: perform any futher shutdown operations before or after stopping the endpoint
                await endpoint?.Stop();
            }
            catch (Exception ex)
            {
                await LogFatalAndFailFast("Failed to stop correctly.", ex);
            }
        }

        async Task OnCriticalError(ICriticalErrorContext context)
        {
            // TODO: decide if stopping the endpoint and exiting the process is the best response to a critical error
            // https://docs.particular.net/nservicebus/hosting/critical-errors
            // and consider setting up service recovery
            // https://docs.particular.net/nservicebus/hosting/windows-service?version=core_7#installation-restart-recovery
            try
            {
                await context.Stop();
            }
            finally
            {
                await LogFatalAndFailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        async Task LogFatalAndFailFast(string message, Exception exception)
        {
            log.Fatal(message, exception);

            // TODO: when using an external logging framework it is important to flush any pending entries prior to calling FailFast
            // https://docs.particular.net/nservicebus/hosting/critical-errors#when-to-override-the-default-critical-error-action
            await Task.CompletedTask;

            // TODO: https://docs.particular.net/nservicebus/hosting/windows-service#installation-restart-recovery
            Environment.FailFast(message, exception);
        }
    }
}
