using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MfitMcpServer.Application;

namespace MfitMcpServer.Api;

public class McpServer
{
    private readonly IMFitService _mfitService;
    private readonly ILogger<McpServer> _logger;

    public McpServer(IMFitService mfitService, ILogger<McpServer> logger)
    {
        _mfitService = mfitService;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Servidor MCP iniciado.");
        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await Console.In.ReadLineAsync(cancellationToken);
            if (line is null) break;
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                var response = await HandleAsync(line, cancellationToken);
                if (response is not null)
                {
                    await WriteAsync(response, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem MCP: {Line}", line);
            }
        }
    }

    private async Task<string?> HandleAsync(string rawJson, CancellationToken ct)
    {
        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(rawJson);
        }
        catch (JsonException)
        {
            // Tenta limpar espaços ou caracteres invisíveis se houver
            doc = JsonDocument.Parse(rawJson.Trim());
        }

        using (doc)
        {
            var root = doc.RootElement;
            var method = root.TryGetProperty("method", out var m) ? m.GetString() : null;

            // Tenta extrair o ID (pode ser número ou string no JSON-RPC)
            string idStr = "null";
            if (root.TryGetProperty("id", out var idProp) && idProp.ValueKind != JsonValueKind.Null)
            {
                idStr = idProp.GetRawText();
            }

            if (method == "initialize")
            {
                return BuildResponse(idStr, BuildInitialize());
            }
            if (method == "notifications/initialized")
            {
                return null; // Notificação não envia resposta
            }
            if (method == "tools/list")
            {
                return BuildResponse(idStr, BuildToolsList());
            }
            if (method == "tools/call")
            {
                return await HandleToolCallAsync(root, idStr, ct);
            }

            // Método desconhecido
            return BuildErrorResponse(idStr, -32601, $"Método não encontrado: {method}");
        }
    }

    private JsonElement BuildInitialize()
    {
        return JsonDocument.Parse("""
        {
          "protocolVersion": "2024-11-05",
          "capabilities": {
            "tools": {}
          },
          "serverInfo": {
            "name": "MfitMcpServer",
            "version": "1.0.0"
          }
        }
        """).RootElement.Clone();
    }

    private JsonElement BuildToolsList()
    {
        return JsonDocument.Parse("""
        {
          "tools": [
            {
              "name": "listar_alunos",
              "description": "Lista os alunos da plataforma MFIT Personal com id, nome e plano.",
              "inputSchema": {
                "type": "object",
                "properties": {},
                "required": []
              }
            }
          ]
        }
        """).RootElement.Clone();
    }

    private async Task<string> HandleToolCallAsync(JsonElement root, string id, CancellationToken ct)
    {
        try
        {
            var paramsObj = root.GetProperty("params");
            var toolName = paramsObj.GetProperty("name").GetString();

            if (toolName == "listar_alunos")
            {
                var alunos = await _mfitService.ListarAlunosAsync();
                var textContent = JsonSerializer.Serialize(alunos);

                var resultObj = new
                {
                    content = new[]
                    {
                        new { type = "text", text = textContent }
                    },
                    isError = false
                };

                return BuildResponse(id, JsonDocument.Parse(JsonSerializer.Serialize(resultObj)).RootElement.Clone());
            }

            return BuildErrorResponse(id, -32602, $"Ferramenta desconhecida: {toolName}");
        }
        catch (Exception ex)
        {
            var errorObj = new
            {
                content = new[]
                {
                    new { type = "text", text = $"Erro ao executar ferramenta: {ex.Message}" }
                },
                isError = true
            };
            return BuildResponse(id, JsonDocument.Parse(JsonSerializer.Serialize(errorObj)).RootElement.Clone());
        }
    }

    private static string BuildResponse(string id, JsonElement result)
    {
        var resultJson = result.GetRawText();
        return $"{{\"jsonrpc\":\"2.0\",\"id\":{id},\"result\":{resultJson}}}";
    }

    private static string BuildErrorResponse(string id, int code, string message)
    {
        return $"{{\"jsonrpc\":\"2.0\",\"id\":{id},\"error\":{{\"code\":{code},\"message\":\"{message}\"}}}}";
    }

    private async Task WriteAsync(string json, CancellationToken ct)
    {
        await Console.Out.WriteLineAsync(json.AsMemory(), ct);
        await Console.Out.FlushAsync(ct);
    }
}