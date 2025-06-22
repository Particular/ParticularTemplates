var builder = DistributedApplication.CreateBuilder(args);
// Add services .
// builder.AddProject<Projects.MyServiceA>("servicea");
// builder.AddProject<Projects.MyServiceB>("serviceb");
builder.Build().Run();
