using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

namespace NsbDockerEndpoint
{
    class Program
    {
        static IEndpointInstance endpointInstance;

        // TODO: give the endpoint an appropriate name
        static string EndpointName => "AnEndpointName";

        static async Task Main()
        {
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;

            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            endpointConfiguration.UseTransport<LearningTransport>();

            endpointInstance = await Endpoint.Start(endpointConfiguration)
                        .ConfigureAwait(false);

            // Wait until the docker container is stopped
            closingEvent.WaitOne();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        static void ProcessExit(object sender, EventArgs e)
        {
            closingEvent.Set();
        }
        static AutoResetEvent closingEvent = new AutoResetEvent(false);
    }
}