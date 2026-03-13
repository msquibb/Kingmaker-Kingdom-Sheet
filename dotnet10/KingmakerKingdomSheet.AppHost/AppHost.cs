using System.IO;

var builder = DistributedApplication.CreateBuilder(args);

var databaseDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(databaseDirectory);

var kingmakerDatabase = builder.AddSqlite("kingmaker-dev", databaseDirectory, "kingmaker-dev.db");

var apiService = builder.AddProject<Projects.KingmakerKingdomSheet_ApiService>("apiservice")
    .WithReference(kingmakerDatabase)
    .WaitFor(kingmakerDatabase)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.KingmakerKingdomSheet_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
