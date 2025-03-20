using System.Text.Json.Serialization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var conditions = new[]
{
    "Sunny", "Partly Cloudy", "Cloudy", "Rainy", "Snowy", "Windy", "Foggy"
};

var locations = new[]
{
    "London", "Paris", "New York", "Tokyo", "Sydney", "Moscow", "Berlin", "Rome", "Madrid", "Beijing"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            {
                var random = Random.Shared;
                var date = DateOnly.FromDateTime(DateTime.Now.AddDays(index));
                var dailyHighC = random.Next(-20, 55);
                var dailyLowC = random.Next(-20, dailyHighC);
                return new WeatherForecast
                (
                    date,
                    dailyHighC,
                    dailyLowC,
                    summaries[random.Next(summaries.Length)],
                    conditions[random.Next(conditions.Length)],
                    locations[random.Next(locations.Length)],
                    random.Next(0, 100), // Humidity (0-100%)
                    random.Next(0, 100), // Precipitation (0-100%)
                    new Wind(random.Next(0, 50), random.Next(0, 360))
                );
            })
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

public record WeatherForecast(
    DateOnly Date,
    int DailyHighC,
    int DailyLowC,
    string? Summary,
    string? Condition,
    string? Location,
    int Humidity,
    int PrecipitationChance,
    Wind? Wind)
{
    [JsonIgnore] public int DailyHighF => 32 + (int)(DailyHighC / 0.5556);
    [JsonIgnore] public int DailyLowF => 32 + (int)(DailyLowC / 0.5556);
}

public record Wind(int Speed, int Direction);