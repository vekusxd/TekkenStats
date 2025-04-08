using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TekkenStats.API.Extensions;
using TekkenStats.API.Features.Seeder;
using TekkenStats.API.Features.Shared;
using TekkenStats.Core.Options;
using TekkenStats.DataAccess;
using TekkenStats.DataAccess.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new Exception("Default connection string not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));
builder.Services.AddScoped<Seeder>();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddScoped<CharacterStore>();
builder.Services.AddElasticSearch(builder.Configuration);

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
    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
    var mongoDb = scope.ServiceProvider.GetRequiredService<MongoDatabase>();
    var elasticSearch = scope.ServiceProvider.GetRequiredService<ElasticSearch>();
    await elasticSearch.InitIndexes();
    await mongoDb.InitIndexes();
    await seeder.Migrate();
    await seeder.SeedDb();
    await seeder.InitCache();
}


app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

var apiGroup = app.MapGroup("/api");

app.MapEndpoints(apiGroup);

app.Run();