#if (persistence == "CosmosDB")
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
#endif
#if UsesMSSQL
using Microsoft.Data.SqlClient;
#endif
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
#if (persistence == "MySQL")
using MySql.Data.MySqlClient;
#endif
#if (persistence == "PostgreSQL")
using Npgsql;
using NpgsqlTypes;
#endif
#if (persistence == "CosmosDB")
using NServiceBus.Persistence.CosmosDB;
#endif
#if (persistence == "Oracle")
using Oracle.ManagedDataAccess.Client;
#endif
#if (persistence == "RavenDB")
using Raven.Client.Documents;
#endif

namespace ProjectName
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

                    var endpointConfiguration = new EndpointConfiguration("ProjectName");

#if (transport == "LearningTransport")
                    // Learning Transport: https://docs.particular.net/transports/learning/
                    var routing = endpointConfiguration.UseTransport(new LearningTransport());
#elseif (transport == "AzureServiceBus")
                    // Azure Service Bus Transport: https://docs.particular.net/transports/azure-service-bus/
                    var transport = new AzureServiceBusTransport("CONNECTION_STRING");
                    var routing = endpointConfiguration.UseTransport(transport);
#elseif (transport == "AzureStorageQueues")
                    // Azure Storage Queues Transport: https://docs.particular.net/transports/azure-storage-queues/
                    var transport = new AzureStorageQueueTransport("DefaultEndpointsProtocol=https;AccountName=[ACCOUNT];AccountKey=[KEY];");
                    var routing = endpointConfiguration.UseTransport(transport);
#elseif (transport == "SQS")
                    // Amazon SQS Transport: https://docs.particular.net/transports/sqs/
                    var transport = new SqsTransport();
                    var routing = endpointConfiguration.UseTransport(transport);
#elseif (transport == "RabbitMQ")
                    // RabbitMQ Transport: https://docs.particular.net/transports/rabbitmq/
                    var rabbitMqConnectionString = "CONNECTION_STRING";
                    var transport = new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), rabbitMqConnectionString);
                    var routing = endpointConfiguration.UseTransport(transport);
#elseif (transport == "SQL")
                    // SQL Server Transport: https://docs.particular.net/transports/sql/
                    var transport = new SqlServerTransport("Data Source=.\\SqlExpress;Initial Catalog=dbname;Integrated Security=True");
                    var routing = endpointConfiguration.UseTransport(transport);
#endif

                    // Define routing for commands: https://docs.particular.net/nservicebus/messaging/routing#command-routing
                    // routing.RouteToEndpoint(typeof(MessageType), "DestinationEndpointForType");
                    // routing.RouteToEndpoint(typeof(MessageType).Assembly, "DestinationForAllCommandsInAssembly");

#if (persistence == "LearningPersistence")
                    // Learning Persistence: https://docs.particular.net/persistence/learning/
                    endpointConfiguration.UsePersistence<LearningPersistence>();
#elseif (persistence == "MSSQL")
                    // SQL Persistence: https://docs.particular.net/persistence/sql/
                    // Microsoft SQL Server dialect: https://docs.particular.net/persistence/sql/dialect-mssql
                    var dbConnectionString = "Data Source=.\\SqlExpress;Initial Catalog=dbname;Integrated Security=True;";
                    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                    persistence.SqlDialect<SqlDialect.MsSqlServer>();
                    persistence.ConnectionBuilder(() => new SqlConnection(dbConnectionString));
#elseif (persistence == "MySQL")
                    // SQL Persistence: https://docs.particular.net/persistence/sql/
                    // MySQL dialect: https://docs.particular.net/persistence/sql/dialect-mysql
                    var dbConnectionString = "server=localhost;user=root;database=dbname;port=3306;password=pass;AllowUserVariables=True;AutoEnlist=false";
                    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                    persistence.SqlDialect<SqlDialect.MySql>();
                    persistence.ConnectionBuilder(() => new MySqlConnection(dbConnectionString));
#elseif (persistence == "PostgreSQL")
                    // SQL Persistence: https://docs.particular.net/persistence/sql/
                    // PostgreSQL dialect: https://docs.particular.net/persistence/sql/dialect-postgresql
                    var dbConnectionString = "Server=localhost;Port=5432;Database=dbname;User Id=user;Password=pass;";
                    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                    var dialect = persistence.SqlDialect<SqlDialect.PostgreSql>();
                    dialect.JsonBParameterModifier(parameter =>
                    {
                        var npgsqlParameter = (NpgsqlParameter)parameter;
                        npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
                    });
                    persistence.ConnectionBuilder(() => new NpgsqlConnection(dbConnectionString));
#elseif (persistence == "Oracle")
                    // SQL Persistence: https://docs.particular.net/persistence/sql/
                    // Oracle dialect: https://docs.particular.net/persistence/sql/dialect-oracle
                    var dbConnectionString = "Data Source=localhost;User Id=username;Password=pass;Enlist=false;";
                    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                    persistence.SqlDialect<SqlDialect.Oracle>();
                    persistence.ConnectionBuilder(() => new OracleConnection(dbConnectionString));
#elseif (persistence == "CosmosDB")
                    // Cosmos DB Persistence: https://docs.particular.net/persistence/cosmosdb/
                    var persistence = endpointConfiguration.UsePersistence<CosmosPersistence>();
                    persistence.CosmosClient(new CosmosClient("CONNECTION_STRING"));
                    persistence.DatabaseName("DATABASE_NAME");
#elseif (persistence == "AzureTable")
                    // Azure Table Persistence: https://docs.particular.net/persistence/azure-table/
                    var persistence = endpointConfiguration.UsePersistence<AzureTablePersistence>();
                    persistence.ConnectionString("DefaultEndpointsProtocol=https;AccountName=[ACCOUNT];AccountKey=[KEY];");
#elseif (persistence == "RavenDB")
                    // RavenDB Persistence: https://docs.particular.net/persistence/ravendb/
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
                    // MongoDB Persistence: https://docs.particular.net/persistence/mongodb/
                    var persistence = endpointConfiguration.UsePersistence<MongoPersistence>();
                    persistence.DatabaseName("DATABASE_NAME");
#elseif (persistence == "DynamoDB")
                    // Amazon DynamoDB Persistence: https://docs.particular.net/persistence/dynamodb/
                    var persistence = endpointConfiguration.UsePersistence<DynamoPersistence>();
#endif

                    // Message serialization
                    endpointConfiguration.UseSerialization<SystemJsonSerializer>();

                    endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);

                    // Installers are useful in development. Consider disabling in production.
                    // https://docs.particular.net/nservicebus/operations/installers
                    endpointConfiguration.EnableInstallers();

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
