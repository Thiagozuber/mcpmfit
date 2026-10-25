
using System.Text;
using System.Text.Json;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using MfitMcpServer.Application;
using MfitMcpServer.Domain;

namespace MfitMcpServer.Api;

public class McpServer
{
    private readonly IMFitService _mfitService;
    private readonly ILogger<McpServer> _logger;

    public McpServer(
        IMFitService mfitService,
        ILogger<McpServer> logger)
    {
        _mfitService = mfitService;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.OutputEncoding = new UTF8Encoding(false);
        Console.InputEncoding = new UTF8Encoding(false);

        _logger.LogInformation("Servidor MCP iniciado.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await Console.In.ReadLineAsync(cancellationToken);

            if (line is null)
                break;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                var response = await HandleAsync(line, cancellationToken);

                if (response is not null)
                    await WriteAsync(response, cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem MCP.");
            }
        }
    }

    private async Task<string?> HandleAsync(
        string rawJson,
        CancellationToken ct)
    {
        using var doc = JsonDocument.Parse(rawJson);

        var root = doc.RootElement;

        var method = root.TryGetProperty("method", out var methodElement)
            ? methodElement.GetString()
            : null;

        string idStr = "null";

        if (root.TryGetProperty("id", out var idElement) &&
            idElement.ValueKind != JsonValueKind.Null)
        {
            idStr = idElement.GetRawText();
        }

        // Notificações não recebem resposta.
        if (method is not null &&
            method.StartsWith("notifications/", StringComparison.Ordinal))
        {
            return null;
        }

        if (method == "initialize")
        {
            return BuildResponse(idStr, BuildInitialize());
        }

        if (method == "ping")
        {
            return BuildResponse(
                idStr,
                JsonDocument.Parse("{}").RootElement.Clone());
        }

        if (method == "tools/list")
        {
            return BuildResponse(idStr, BuildToolsList());
        }

        if (method == "tools/call")
        {
            return await HandleToolCallAsync(root, idStr, ct);
        }

        // Mensagem sem ID é uma notificação.
        if (idStr == "null")
            return null;

        return BuildErrorResponse(
            idStr,
            -32601,
            $"Método não encontrado: {method}");
    }

    private JsonElement BuildInitialize()
    {
        return JsonDocument.Parse(
            """
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
        return JsonDocument.Parse(
            """
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
                },
                {
                  "name": "obter_treino",
                  "description": "Obtém os detalhes e exercícios de uma prescrição de treino pelo ID.",
                  "inputSchema": {
                    "type": "object",
                    "properties": {
                      "idPrescription": {
                        "type": "integer",
                        "description": "ID da prescrição do treino."
                      }
                    },
                    "required": ["idPrescription"]
                  }
                },
                {
                  "name": "pesquisar_exercicios",
                  "description": "Pesquisa exercícios no catálogo do MFIT por nome ou grupo muscular.",
                  "inputSchema": {
                    "type": "object",
                    "properties": {
                      "termo": {
                        "type": "string",
                        "description": "Termo de busca, como Peitoral, Supino ou Agachamento."
                      }
                    },
                    "required": ["termo"]
                  }
                },
                {
                  "name": "criar_treino",
                  "description": "Cria ou atribui uma nova prescrição de treino para um aluno no MFIT.",
                  "inputSchema": {
                    "type": "object",
                    "properties": {
                      "idAluno": {
                        "type": "integer",
                        "description": "ID do aluno."
                      },
                      "nomeTreino": {
                        "type": "string",
                        "description": "Nome do treino."
                      },
                      "observacoes": {
                        "type": "string",
                        "description": "Observações gerais do treino."
                      }
                    },
                    "required": ["idAluno", "nomeTreino", "observacoes"]
                  }
                }
              ]
            }
            """).RootElement.Clone();
    }

    private async Task<string> HandleToolCallAsync(
        JsonElement root,
        string id,
        CancellationToken ct)
    {
        try
        {
            var paramsObj = root.GetProperty("params");
            var toolName = paramsObj.GetProperty("name").GetString();

            JsonElement arguments;

            if (paramsObj.TryGetProperty("arguments", out var argsElement))
            {
                arguments = argsElement;
            }
            else
            {
                arguments = JsonDocument.Parse("{}").RootElement.Clone();
            }

            switch (toolName)
            {
                case "listar_alunos":
                {
                    var alunos = await _mfitService.ListarAlunosAsync();

                    return BuildToolResult(
                        id,
                        JsonSerializer.Serialize(alunos));
                }

                case "obter_treino":
                {
                    var idPrescription = arguments
                        .GetProperty("idPrescription")
                        .GetInt64();

                    var treino = await _mfitService
                        .ObterTreinoPorIdAsync(idPrescription);

                    if (treino is null)
                    {
                        return BuildToolResult(
                            id,
                            "Treino não encontrado ou erro ao consultar a API do MFIT.",
                            isError: true);
                    }

                    return BuildToolResult(
                        id,
                        JsonSerializer.Serialize(
                            treino,
                            new JsonSerializerOptions
                            {
                                WriteIndented = true
                            }));
                }

                case "pesquisar_exercicios":
                {
                    var termo = arguments
                        .GetProperty("termo")
                        .GetString();

                    if (string.IsNullOrWhiteSpace(termo))
                    {
                        return BuildToolResult(
                            id,
                            "O termo de pesquisa não pode estar vazio.",
                            isError: true);
                    }

                    var exercicios = await _mfitService
                        .PesquisarExerciciosAsync(termo);

                    if (exercicios is null || !exercicios.Any())
                    {
                        return BuildToolResult(
                            id,
                            $"Nenhum exercício encontrado para o termo '{termo}'.");
                    }

                    return BuildToolResult(
                        id,
                        JsonSerializer.Serialize(
                            exercicios,
                            new JsonSerializerOptions
                            {
                                WriteIndented = true
                            }));
                }

                case "criar_treino":
                {
                    var idAluno = arguments
                        .GetProperty("idAluno")
                        .GetInt64();

                    var nomeTreino = arguments
                        .GetProperty("nomeTreino")
                        .GetString();

                    var observacoes = arguments
                        .GetProperty("observacoes")
                        .GetString();

                    if (idAluno <= 0 ||
                        string.IsNullOrWhiteSpace(nomeTreino) ||
                        observacoes is null)
                    {
                        return BuildToolResult(
                            id,
                            "Informe um ID de aluno válido, o nome do treino e as observações.",
                            isError: true);
                    }

                    var treino = new NovoTreino
                    {
                        IdAluno = idAluno,
                        NomeTreino = nomeTreino,
                        Observacoes = observacoes
                    };

                    var sucesso = await _mfitService.CriarTreinoAsync(treino);

                    if (!sucesso)
                    {
                        return BuildToolResult(
                            id,
                            "Erro ao tentar criar o treino no MFIT.",
                            isError: true);
                    }

                    return BuildToolResult(
                        id,
                        $"Treino '{nomeTreino}' criado com sucesso para o aluno ID {idAluno}!");
                }

                default:
                    return BuildErrorResponse(
                        id,
                        -32602,
                        $"Ferramenta desconhecida: {toolName}");
            }
        }
        catch (OperationCanceledException)
            when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar ferramenta MCP.");

            return BuildToolResult(
                id,
                $"Erro ao executar ferramenta: {ex.Message}",
                isError: true);
        }
    }

    private static string BuildToolResult(
        string id,
        string text,
        bool isError = false)
    {
        var result = new
        {
            content = new[]
            {
                new
                {
                    type = "text",
                    text
                }
            },
            isError
        };

        var resultElement = JsonDocument
            .Parse(JsonSerializer.Serialize(result))
            .RootElement.Clone();

        return BuildResponse(id, resultElement);
    }

    private static string BuildResponse(
        string id,
        JsonElement result)
    {
        var resultJson = JsonSerializer.Serialize(result);

        return
            $"{{\"jsonrpc\":\"2.0\",\"id\":{id},\"result\":{resultJson}}}";
    }

    private static string BuildErrorResponse(
        string id,
        int code,
        string message)
    {
        var messageJson = JsonSerializer.Serialize(message);

        return
            $"{{\"jsonrpc\":\"2.0\",\"id\":{id},\"error\":{{\"code\":{code},\"message\":{messageJson}}}}}";
    }

    private async Task WriteAsync(
        string json,
        CancellationToken ct)
    {
        await Console.Out.WriteLineAsync(json.AsMemory(), ct);
        await Console.Out.FlushAsync();
    }
}
