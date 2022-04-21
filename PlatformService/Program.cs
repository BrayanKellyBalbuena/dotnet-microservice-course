using Microsoft.EntityFrameworkCore;
using PlatformService.Data.Cache;
using PlatformService.Data.Db;
using PlatformService.ISyncDataServices;
using PlatformService.Models;
using PlatformService.SyncDataServices.Grpc;
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


builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddSingleton<ICacheRepository<Platform>, CacheRepository<Platform>>();
builder.Services.AddSingleton<ICacheService<Platform>, PlatformCacheService>();
builder.Services.AddStackExchangeRedisCache(opt => 
        {
            opt.Configuration = builder.Configuration.GetConnectionString("Redis");
            opt.InstanceName = "SimpleInstance";
        }
);

builder.Services.AddGrpc();
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
    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.pro", async context => {
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

PrepDb.PrepPopulation(app, env.IsProduction());

Console.WriteLine($"CommandService endpoint: {builder.Configuration["CommandService"]}");

app.Run();

