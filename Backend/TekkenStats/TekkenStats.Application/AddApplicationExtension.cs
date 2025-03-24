using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TekkenStats.Application.Services;
using TekkenStats.Core.Options;

namespace TekkenStats.Application;

public static class AddApplicationExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<WavuWankResponseProcessor>();

        return services;
    }
}