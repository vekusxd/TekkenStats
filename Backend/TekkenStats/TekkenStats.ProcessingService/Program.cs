using MassTransit;
using Microsoft.EntityFrameworkCore;
using TekkenStats.Core.Options;
using TekkenStats.DataAccess;
using TekkenStats.DataAccess.Extensions;
using TekkenStats.Seeder;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new Exception("Connection string not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));
builder.Services.AddScoped<ResponseProcessor>();

builder.Services.Configure<MongoOptions>(builder.Configuration.GetRequiredSection(MongoOptions.Section));

builder.Services.AddMongoDb(builder.Configuration);

builder.Services.AddMassTransit(configurator =>
{
    var rabbitMqOptions = builder.Configuration.GetRequiredSection(RabbitOptions.Section).Get<RabbitOptions>()
                          ?? throw new Exception("RabbitMQ setting not found");

    configurator.SetKebabCaseEndpointNameFormatter();
    configurator.AddConsumer<DataProcessor>();
    configurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqOptions.Host, c =>
        {
            c.Username(rabbitMqOptions.Username);
            c.Password(rabbitMqOptions.Password);
        });

        cfg.ReceiveEndpoint("responseData_queue", endpointConfigurator =>
            endpointConfigurator.Consumer<DataProcessor>(context));

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    await scope.ServiceProvider.InitIndexes();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

host.Run();