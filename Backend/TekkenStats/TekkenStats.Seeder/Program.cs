using Microsoft.EntityFrameworkCore;
using TekkenStats.DataAccess;
using TekkenStats.Seeder;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new Exception("Connection string not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));
builder.Services.AddHttpClient("WavuWankClient", client =>
    client.BaseAddress = new Uri("https://wank.wavu.wiki/"));

builder.Services.AddHostedService<Seeder>();

var host = builder.Build();
host.Run();