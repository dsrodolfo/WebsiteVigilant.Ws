using Serilog;
using WebsiteVigilant.Ws;

try
{
    IHost host = Host.CreateDefaultBuilder(args)
        .UseWindowsService()
        .UseSerilog()
        .ConfigureServices((hostContext, services) =>
        {
            Log.Logger = new LoggerConfiguration()
                               .ReadFrom
                               .Configuration(hostContext.Configuration)
                               .CreateLogger();

            services.AddHttpClient();
            services.AddHostedService<Worker>();

            Log.Information("Worker service starting up.");
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker service failed to start correctly.");
}
finally
{
    Log.CloseAndFlush();
}