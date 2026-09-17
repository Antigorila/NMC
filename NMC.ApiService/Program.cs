using Microsoft.EntityFrameworkCore;
using NMC.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add database context
builder.Services.AddDbContext<NmcDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NmcDb")));

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Map controllers
app.MapControllers();

//Database initialization
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NmcDbContext>();
    DbInitializer.Initialize(dbContext);
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();