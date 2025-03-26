using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using TekkenStats.Application;
using TekkenStats.Core.Options;
using TekkenStats.DataAccess;
using TekkenStats.Seeder;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new Exception("Connection string not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));
builder.Services.AddApplication();

var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection") ??
                            throw new Exception("Redis connection string not found");

builder.Services.AddStackExchangeRedisCache(opts => opts.Configuration = redisConnectionString);

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHybridCache(opts =>
{
    opts.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };
});

builder.Services.AddMassTransit(configurator =>
{
    var rabbitMqOptions = builder.Configuration.GetRequiredSection(RabbitOptions.Section).Get<RabbitOptions>()
                          ?? throw new Exception("RabbitMQ setting not found");

    configurator.SetKebabCaseEndpointNameFormatter();
    configurator.AddConsumer<WavuWankConsumer>();
    configurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqOptions.Host, c =>
        {
            c.Username(rabbitMqOptions.Username);
            c.Password(rabbitMqOptions.Password);
        });

        cfg.ReceiveEndpoint("responseData_queue", endpointConfigurator =>
        {
            endpointConfigurator.UseTimeout(
                timeoutConfigurator => timeoutConfigurator.Timeout = TimeSpan.FromMinutes(2));
            endpointConfigurator.Consumer<WavuWankConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

host.Run();