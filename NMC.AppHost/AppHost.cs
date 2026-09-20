var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("sql-password", "NMC-Database-Password123", secret: true);

var sqlServer = builder.AddSqlServer("sqlserver", password)
                       .WithImage("mssql/server", "2019-latest")
                       .WithHostPort(1433)
                       .WithDataVolume();

var database = sqlServer.AddDatabase("NmcDb");

builder.AddProject<Projects.NMC_ApiService>("nmc-api-service")
       .WithReference(database)
       .WaitFor(database)
       .WithHttpEndpoint(port: 5000, name: "http");

builder.AddProject<Projects.NMC_Mobile>("nmc-mobile");

builder.Build().Run();