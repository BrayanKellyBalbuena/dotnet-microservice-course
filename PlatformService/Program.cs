using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// Add services to the container.

if (env.IsProduction())
{
    Console.WriteLine("Using sqlserver database");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"))
);
}
else
{
    Console.WriteLine("Using in memory database");
    builder.Services.AddDbContext<AppDbContext>(opt =>
    {
        opt.UseInMemoryDatabase("InMen");
    });
}


builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
PrepDb.PrepPopulation(app, env.IsProduction());

Console.WriteLine($"CommandService endpoint: {builder.Configuration["CommandService"]}");

app.Run();

