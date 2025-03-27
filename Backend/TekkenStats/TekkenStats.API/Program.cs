using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TekkenStats.API.Extensions;
using TekkenStats.API.Features.DataFetcher;
using TekkenStats.API.Features.SeedDb;
using TekkenStats.Core.Options;
using TekkenStats.DataAccess;
using TekkenStats.DataAccess.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetRequiredSection(RabbitOptions.Section));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new Exception("Default connection string not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddMongoDb(builder.Configuration);

builder.Services.AddHostedService<DataFetcher>();

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

builder.Services.Configure<MongoOptions>(builder.Configuration.GetRequiredSection(MongoOptions.Section));

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.InitIndexes();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedDb();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

app.MapEndpoints();

app.Run();