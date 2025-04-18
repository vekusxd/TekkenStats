using MassTransit;
using Microsoft.EntityFrameworkCore;
using TekkenStats.Core.Options;
using TekkenStats.DataAccess;
using TekkenStats.DataAccess.Extensions;
using TekkenStats.ProcessingService;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new Exception("Connection string not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));
builder.Services.AddScoped<ResponseProcessor>();

builder.Services.Configure<MongoOptions>(builder.Configuration.GetRequiredSection(MongoOptions.Section));

builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddElasticSearch(builder.Configuration);

builder.Services.AddMassTransit(configurator =>
{
    var rabbitMqOptions = builder.Configuration.GetRequiredSection(RabbitOptions.Section).Get<RabbitOptions>()
                          ?? throw new Exception("RabbitMQ setting not found");

    configurator.SetKebabCaseEndpointNameFormatter();
    configurator.AddConsumer<DataProcessor>();

    configurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseConcurrencyLimit(1);
        cfg.UseTimeout(timeoutConfigurator => timeoutConfigurator.Timeout = TimeSpan.FromMinutes(2));
        cfg.Host(rabbitMqOptions.Host, c =>
        {
            c.Username(rabbitMqOptions.Username);
            c.Password(rabbitMqOptions.Password);
        });

        cfg.ReceiveEndpoint("responseData_queue",
            endpointConfigurator => { endpointConfigurator.Consumer<DataProcessor>(context); });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();


using (var scope = host.Services.CreateScope())
{
    var mongoDb = scope.ServiceProvider.GetRequiredService<MongoDatabase>();
    await mongoDb.InitIndexes();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var elasticSearch = scope.ServiceProvider.GetRequiredService<ElasticSearch>();
    await elasticSearch.InitIndexes();
    dbContext.Database.Migrate();
}

host.Run();