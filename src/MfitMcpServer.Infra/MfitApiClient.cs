using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using MfitMcpServer.Domain;

namespace MfitMcpServer.Infra;

public sealed class MfitApiClient
{
    private const int PageSize = 20;
    private const int MaxPages = 50;
    private const string Endpoint = "https://api.mfitpersonal.com.br/v2/user/client/";

    private readonly HttpClient _httpClient;
    private readonly ILogger<MfitApiClient> _logger;
    private readonly Lazy<Task<string>> _authorizationToken;

    public MfitApiClient(HttpClient httpClient, ILogger<MfitApiClient> logger)
        : this(httpClient, logger, CapturarAuthorizationAsync)
    {
    }

    internal MfitApiClient(
        HttpClient httpClient,
        ILogger<MfitApiClient> logger,
        Func<Task<string>> authorizationTokenProvider)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authorizationToken = new Lazy<Task<string>>(
            authorizationTokenProvider,
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public async Task<List<Aluno>> ListarAlunosAsync(CancellationToken cancellationToken = default)
    {
        var token = await _authorizationToken.Value;
        var alunos = new List<Aluno>();

        for (var pagina = 1; pagina <= MaxPages; pagina++)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{Endpoint}?status=0&page={pagina}&pageSize={PageSize}&search=");
            request.Headers.TryAddWithoutValidation("Authorization", token);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var statusCode = (int)response.StatusCode;

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                _logger.LogWarning("MFIT API: página {Pagina}, HTTP {Status}.", pagina, statusCode);
                throw new InvalidOperationException(
                    "A autenticação MFIT expirou ou foi recusada. Execute novamente " +
                    "\"dotnet MfitMcpServer.Api.dll --login\" e reinicie o servidor.");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogWarning("MFIT API: página {Pagina}, HTTP {Status}.", pagina, statusCode);
                throw new HttpRequestException(
                    $"A API do MFIT retornou HTTP {statusCode} na página {pagina}.",
                    null,
                    response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var itensDaPagina = InterpretarResposta(content, pagina);
            _logger.LogInformation(
                "MFIT API: página {Pagina}, HTTP {Status}, {Quantidade} itens.",
                pagina,
                statusCode,
                itensDaPagina.Count);

            alunos.AddRange(itensDaPagina);
            if (itensDaPagina.Count < PageSize)
            {
                return alunos;
            }
        }

        throw new InvalidDataException(
            $"A paginação do MFIT excedeu o limite de segurança de {MaxPages} páginas.");
    }

    private static List<Aluno> InterpretarResposta(string content, int pagina)
    {
        JsonDocument documento;
        try
        {
            documento = JsonDocument.Parse(content);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException(
                $"A resposta da API do MFIT na página {pagina} não contém JSON válido.",
                ex);
        }

        using (documento)
        {
            var raiz = documento.RootElement;
            if (raiz.ValueKind != JsonValueKind.Object
                || !raiz.TryGetProperty("items", out var itens)
                || itens.ValueKind != JsonValueKind.Array
                || itens.GetArrayLength() > PageSize)
            {
                throw new InvalidDataException(
                    $"O formato da resposta da API do MFIT na página {pagina} é inesperado.");
            }

            var alunos = new List<Aluno>(itens.GetArrayLength());
            foreach (var item in itens.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object
                    || !item.TryGetProperty("id", out var id)
                    || !id.TryGetInt32(out var idAluno)
                    || !item.TryGetProperty("nome", out var nome)
                    || nome.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidDataException(
                        $"Um item da resposta da API do MFIT na página {pagina} tem formato inesperado.");
                }

                var plano = ObterPlano(item, pagina);
                alunos.Add(new Aluno
                {
                    Id = idAluno,
                    Nome = nome.GetString() ?? string.Empty,
                    Plano = plano
                });
            }

            return alunos;
        }
    }

    private static string ObterPlano(JsonElement item, int pagina)
    {
        if (!item.TryGetProperty("grupo", out var grupo) || grupo.ValueKind == JsonValueKind.Null)
        {
            return "Padrão";
        }

        if (grupo.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException(
                $"O grupo de um item da resposta da API do MFIT na página {pagina} tem formato inesperado.");
        }

        if (!grupo.TryGetProperty("nome", out var nomeGrupo))
        {
            return "Padrão";
        }

        if (nomeGrupo.ValueKind != JsonValueKind.String)
        {
            throw new InvalidDataException(
                $"O nome do grupo de um item da resposta da API do MFIT na página {pagina} tem formato inesperado.");
        }

        var plano = nomeGrupo.GetString();
        return string.IsNullOrWhiteSpace(plano) ? "Padrão" : plano;
    }

    private static async Task<string> CapturarAuthorizationAsync()
    {
        var pastaPerfil = MfitSessao.PastaPerfil();
        if (!Directory.Exists(pastaPerfil))
        {
            throw new InvalidOperationException(
                "Sessão do MFIT não encontrada. Execute \"dotnet MfitMcpServer.Api.dll --login\".");
        }

        using var playwright = await Playwright.CreateAsync();
        await using var contexto = await playwright.Chromium.LaunchPersistentContextAsync(
            pastaPerfil,
            new BrowserTypeLaunchPersistentContextOptions { Headless = true });

        var pagina = contexto.Pages.FirstOrDefault() ?? await contexto.NewPageAsync();
        pagina.SetDefaultTimeout(30000);

        var authorizationCapturado = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        pagina.Request += (_, request) =>
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri)
                || !uri.Host.Equals("api.mfitpersonal.com.br", StringComparison.OrdinalIgnoreCase)
                || !request.Headers.TryGetValue("authorization", out var authorization)
                || string.IsNullOrWhiteSpace(authorization))
            {
                return;
            }

            authorizationCapturado.TrySetResult(authorization);
        };

        await pagina.GotoAsync(
            MfitSessao.UrlLista,
            new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

        if (pagina.Url.Contains("/login", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "A sessão do MFIT expirou. Execute novamente \"dotnet MfitMcpServer.Api.dll --login\".");
        }

        var captura = await Task.WhenAny(
            authorizationCapturado.Task,
            Task.Delay(TimeSpan.FromSeconds(20)));
        if (captura != authorizationCapturado.Task)
        {
            throw new InvalidOperationException(
                "Não foi possível obter a autorização da sessão MFIT. Execute novamente " +
                "\"dotnet MfitMcpServer.Api.dll --login\".");
        }

        return await authorizationCapturado.Task;
    }

    public async Task<DetalheTreino?> ObterTreinoPorIdAsync(long idPrescription)
{
    using var playwright = await Playwright.CreateAsync();
    var pastaPerfil = MfitSessao.PastaPerfil();

    if (!Directory.Exists(pastaPerfil))
    {
        throw new InvalidOperationException("Sessão do MFIT não encontrada. Rode: dotnet MfitMcpServer.Api.dll --login");
    }

    await using var context = await playwright.Chromium.LaunchPersistentContextAsync(
        pastaPerfil,
        new BrowserTypeLaunchPersistentContextOptions { Headless = true });

    var page = context.Pages.FirstOrDefault() ?? await context.NewPageAsync();
    await page.GotoAsync(MfitSessao.UrlLista, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

    var urlApi = $"https://api.mfitpersonal.com.br/v2/user/workout/prescription/?id={idPrescription}";
    var responseJson = await page.EvaluateAsync<string>("async (url) => { const res = await fetch(url); return await res.text(); }", urlApi);

    if (string.IsNullOrWhiteSpace(responseJson))
    {
        return null;
    }

    return JsonSerializer.Deserialize<DetalheTreino>(responseJson);
}

}
