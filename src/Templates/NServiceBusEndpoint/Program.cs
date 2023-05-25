#if (persistence == "CosmosDB")
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
#endif
#if UsesSQL
using Microsoft.Data.SqlClient;
#endif
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
#if (persistence == "CosmosDB")
using NServiceBus.Persistence.CosmosDB;
#endif
#if (persistence == "RavenDB")
using Raven.Client.Documents;
#endif
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBusWindowsService
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
#if (hosting == "WindowsService")
                .UseWindowsService()
                .ConfigureLogging(logging =>
                {
                    logging.AddEventLog();
                })
#else
                .UseConsoleLifetime()
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
#endif
                .UseNServiceBus(ctx =>
                {
                    // TODO: consider moving common endpoint configuration into a shared project
                    // for use by all endpoints in the system

                    // TODO: give the endpoint an appropriate name
                    var endpointConfiguration = new EndpointConfiguration("ProjectName");

                    // Message transport: https://docs.particular.net/transports/
#if (transport == "LearningTransport")
                    var transportExtensions = endpointConfiguration.UseTransport(new LearningTransport());
#elseif (transport == "AzureServiceBus")
                    // TODO: Provide Azure Service Bus connection string
                    var transport = new AzureServiceBusTransport("CONNECTION_STRING");
                    var transportExtensions = endpointConfiguration.UseTransport(transport);
#elseif (transport == "SQS")
                    var transport = new SqsTransport();
                    endpointConfiguration.UseTransport(transport);
#elseif (transport == "RabbitMQ")
                    // TODO: Provide Azure Service Bus connection string
                    var rabbitMqConnectionString = "CONNECTION_STRING";
                    var transport = new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), rabbitMqConnectionString);
                    var transportExtensions = endpointConfiguration.UseTransport(transport);
#elseif (transport == "SQL")
                    // TODO: Provide Azure Service Bus connection string
                    var transport = new SqlServerTransport("CONNECTION_STRING");
                    var transportExtensions = endpointConfiguration.UseTransport(transport);
#elseif (transport == "MSMQ")
                    var transportExtensions = endpointConfiguration.UseTransport(new MsmqTransport());
#endif

                    // Persistence: https://docs.particular.net/persistence/
#if (persistence == "LearningPersistence")
                    endpointConfiguration.UsePersistence<LearningPersistence>();
#elseif (persistence == "SQL")
                    var dbConnectionString = "CONNECTION_STRING";
                    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                    persistence.SqlDialect<SqlDialect.MsSqlServer>();
                    persistence.ConnectionBuilder(() => new SqlConnection(dbConnectionString));
#elseif (persistence == "CosmosDB")
                    var persistence = endpointConfiguration.UsePersistence<CosmosPersistence>();
                    persistence.CosmosClient(new CosmosClient("CONNECTION_STRING"));
                    persistence.DatabaseName("DATABASE_NAME");
#elseif (persistence == "AzureTable")
                    var persistence = endpointConfiguration.UsePersistence<AzureTablePersistence>();
                    persistence.ConnectionString("DefaultEndpointsProtocol=https;AccountName=[ACCOUNT];AccountKey=[KEY];");
#elseif (persistence == "RavenDB")
                    DocumentStore documentStore;
                    var persistence = endpointConfiguration.UsePersistence<RavenDBPersistence>();
                    persistence.SetDefaultDocumentStore(readOnlySettings =>
                    {
                        documentStore = new DocumentStore
                        {
                            Urls = new[] { "http://localhost:8080" },
                            Database = readOnlySettings.EndpointName()
                        };
                        return documentStore;
                    });
#elseif (persistence == "MongoDB")
                    var persistence = endpointConfiguration.UsePersistence<MongoPersistence>();
                    persistence.DatabaseName("DATABASE_NAME");
#elseif (persistence == "DynamoDB")
                    var persistence = endpointConfiguration.UsePersistence<DynamoPersistence>();
#elseif (persistence == "NonDurable")
                    var persistence = endpointConfiguration.UsePersistence<NonDurablePersistence>();
#endif

                    // Message serialization
                    endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

                    endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);

                    // TODO: remove this condition after choosing a transport, persistence and deployment method suitable for production
                    if (Environment.UserInteractive && Debugger.IsAttached)
                    {
                        // TODO: create a script for deployment to production
                        endpointConfiguration.EnableInstallers();
                    }

                    return endpointConfiguration;
                });
        }

        static async Task OnCriticalError(ICriticalErrorContext context, CancellationToken cancellationToken)
        {
            // TODO: decide if stopping the endpoint and exiting the process is the best response to a critical error
            // https://docs.particular.net/nservicebus/hosting/critical-errors
            // and consider setting up service recovery
            // https://docs.particular.net/nservicebus/hosting/windows-service#installation-restart-recovery
            try
            {
                await context.Stop(cancellationToken);
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        static void FailFast(string message, Exception exception)
        {
            try
            {
                // TODO: decide what kind of last resort logging is necessary
                // TODO: when using an external logging framework it is important to flush any pending entries prior to calling FailFast
                // https://docs.particular.net/nservicebus/hosting/critical-errors#when-to-override-the-default-critical-error-action
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }
    }
}
