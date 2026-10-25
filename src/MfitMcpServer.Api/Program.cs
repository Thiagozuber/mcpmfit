using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MfitMcpServer.Api;

var services = new ServiceCollection()
    .AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Information))
    .AddScoped<McpServer>()
    .AddScoped<MfitMcpServer.Application.IMFitService, MfitMcpServer.Infra.MockMfitService>();

var host = services.BuildServiceProvider();

var logger = host.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Iniciando MfitMcpServer...");

var server = host.GetRequiredService<McpServer>();
await server.RunAsync(CancellationToken.None);