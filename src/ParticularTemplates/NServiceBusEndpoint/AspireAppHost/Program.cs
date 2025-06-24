var builder = DistributedApplication.CreateBuilder(args);
// Add services .
// builder.AddProject<Projects.MyServiceA>("servicea");
// builder.AddProject<Projects.MyServiceB>("serviceb");

#if (transport == "RabbitMQ")
var transport = builder.AddRabbitMQ("transport")
    .WithManagementPlugin(15672)
    .WithUrlForEndpoint("management", url => url.DisplayText = "RabbitMQ Management");
#endif

#if (persistence == "PostgreSQL")
var database = builder.AddPostgres("database");

database.WithPgAdmin(resource =>
{
    resource.WithParentRelationship(database);
    resource.WithUrlForEndpoint("http", url => url.DisplayText = "pgAdmin");
});
#endif

builder.Build().Run();
