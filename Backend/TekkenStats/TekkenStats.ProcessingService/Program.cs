using MassTransit;
using Microsoft.EntityFrameworkCore;
using TekkenStats.Application;
using TekkenStats.Core.Options;
using TekkenStats.DataAccess;
using TekkenStats.Seeder;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new Exception("Connection string not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));
builder.Services.AddApplication();

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
            endpointConfigurator.Consumer<WavuWankConsumer>(context));

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