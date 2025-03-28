using MassTransit;
using TekkenStats.CollectorService;
using TekkenStats.Core.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<CollectorService>();

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetRequiredSection(RabbitOptions.Section));

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

var host = builder.Build();
host.Run();