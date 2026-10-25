using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MfitMcpServer.Api;
using MfitMcpServer.Application;
using MfitMcpServer.Infra;

// Modo de login manual: dotnet MfitMcpServer.Api.dll --login
if (args.Contains("--login"))
{
    await MfitSessao.LoginManualAsync();
    return;
}

var usarReal = Environment.GetEnvironmentVariable("MFIT_USAR_REAL") == "1";

var services = new ServiceCollection()
    .AddLogging(b => b
        .AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace)
        .SetMinimumLevel(LogLevel.Information))
    .AddScoped<McpServer>();

if (usarReal)
{
    services
        .AddSingleton<HttpClient>()
        .AddSingleton<MfitApiClient>()
        .AddSingleton<IMFitService, MfitWebService>();
}
else
{
    services.AddSingleton<IMFitService, MockMfitService>();
}

var host = services.BuildServiceProvider();

var logger = host.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Iniciando MfitMcpServer (Modo Real: {UsarReal})...", usarReal);

var server = host.GetRequiredService<McpServer>();
await server.RunAsync(CancellationToken.None);