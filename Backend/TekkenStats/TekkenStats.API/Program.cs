using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Scalar.AspNetCore;
using TekkenStats.API;
using TekkenStats.API.Extensions;
using TekkenStats.Application;
using TekkenStats.Application.Services;
using TekkenStats.Core.Options;
using TekkenStats.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetRequiredSection(RabbitOptions.Section));

var postgresConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new Exception("Default connection string not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(postgresConnectionString));


var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection") ??
                            throw new Exception("Redis connection string not found");
builder.Services.AddStackExchangeRedisCache(opts => opts.Configuration = redisConnectionString);

builder.Services.AddHybridCache(opts =>
{
    opts.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };
});


builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddScoped<DbSeeder>();
builder.Services.AddApplication();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddHostedService<WavuWankProducer>();

builder.Services.AddHttpClient("WavuWankClient", client =>
    client.BaseAddress = new Uri("https://wank.wavu.wiki/"));

builder.Services.AddMassTransit(configurator =>
{
    var rabbitMqOptions = builder.Configuration.GetRequiredSection(RabbitOptions.Section).Get<RabbitOptions>()
                          ?? throw new Exception("RabbitMQ setting not found");

    configurator.SetKebabCaseEndpointNameFormatter();
    configurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqOptions.Host, c =>
        {
            c.Username(rabbitMqOptions.Username);
            c.Password(rabbitMqOptions.Password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(policyBuilder => policyBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()));

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedDb();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

app.MapEndpoints();

app.Run();