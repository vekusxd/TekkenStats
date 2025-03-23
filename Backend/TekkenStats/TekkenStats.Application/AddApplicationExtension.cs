using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TekkenStats.Application.Services;
using TekkenStats.Core.Options;

namespace TekkenStats.Application;

public static class AddApplicationExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddScoped<WavuWankResponseProcessor>();
        services.AddHttpClient("WavuWankClient", client =>
            client.BaseAddress = new Uri("https://wank.wavu.wiki/"));
        
        services.AddMassTransit(configurator =>
        {
            var rabbitMqOptions = configuration.GetRequiredSection(RabbitOptions.Section).Get<RabbitOptions>()
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
        return services;
    }
}