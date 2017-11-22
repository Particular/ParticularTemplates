using System;
using System.Diagnostics;
using NServiceBus;

namespace NServiceBusWindowsService
{
    // TODO: move this into a shared project for use in all endpoints
    static class EndpointConfigurationExtensions
    {
        public static void ApplySettingsForAllEndpoints(
            this EndpointConfiguration endpointConfiguration,
            Action<TransportExtensions<LearningTransport>> applyEndpointSpecificSettings)
        {
            // TODO: ensure the most appropriate serializer is chosen
            // https://docs.particular.net/nservicebus/serialization/
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

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

                applyEndpointSpecificSettings?.Invoke(transportExtensions);
            }
        }
    }
}
