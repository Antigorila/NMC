using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("sql-password", "nmc-db-psw", secret: true);

var sqlServer = builder.AddSqlServer("sqlserver", password)
                       .WithHostPort(1433)
                       .WithDataVolume();

var database = sqlServer.AddDatabase("NmcDb");

builder.AddProject<Projects.NMC_ApiService>("nmc-apiservice")
       .WithReference(database)
       .WaitFor(database);

builder.Build().Run();