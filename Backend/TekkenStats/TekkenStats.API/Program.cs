using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TekkenStats.API;
using TekkenStats.Application;
using TekkenStats.Application.Services;
using TekkenStats.Core.Options;
using TekkenStats.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetRequiredSection(RabbitOptions.Section));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new Exception("Default connection string not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddHostedService<WavuWankProducer>();

builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(policyBuilder => policyBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()));

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

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

app.Run();