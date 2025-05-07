using Antifraud.Worker.Persistence;
using Antifraud.Worker.Services;
using Antifraud.Worker;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddScoped<IFraudDetectionService, FraudDetectionService>();
        services.AddScoped<IDailyLimitRepository, DailyLimitRepository>();
        services.AddDbContext<AntifraudDbContext>(options =>
        {
            options.UseNpgsql(hostContext.Configuration.GetConnectionString("Default"));
        });
        services.AddHttpClient();
    })
    .Build();

await InfrastructureSetup.InitializeAsync(host);

host.Run();
