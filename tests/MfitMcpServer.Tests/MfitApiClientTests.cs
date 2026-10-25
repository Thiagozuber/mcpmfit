using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using MfitMcpServer.Infra;

namespace MfitMcpServer.Tests;

public sealed class MfitApiClientTests
{
    [Fact]
    public async Task ListarAlunosAsync_MapeiaSomenteOsCamposPermitidos()
    {
        var handler = new FakeHttpMessageHandler(_ => JsonResponse("""
            {
              "items": [
                {
                  "id": 42,
                  "nome": "Aluna Ficticia de Teste",
                  "status": "ativo",
                  "telefone": "FICTICIO-NAO-REAL",
                  "profileUrl": "https://example.invalid/foto",
                  "grupo": { "nome": "Plano Ficticio" }
                },
                {
                  "id": 43,
                  "nome": "Aluno Ficticio sem Grupo",
                  "grupo": null
                }
              ]
            }
            """));
        var service = CriarService(handler, out _);

        var alunos = await service.ListarAlunosAsync();

        Assert.Collection(
            alunos,
            aluna =>
            {
                Assert.Equal(42, aluna.Id);
                Assert.Equal("Aluna Ficticia de Teste", aluna.Nome);
                Assert.Equal("Plano Ficticio", aluna.Plano);
            },
            aluno =>
            {
                Assert.Equal(43, aluno.Id);
                Assert.Equal("Aluno Ficticio sem Grupo", aluno.Nome);
                Assert.Equal("Padrão", aluno.Plano);
            });

        var resultado = JsonSerializer.Serialize(alunos);
        Assert.DoesNotContain("telefone", resultado, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("profileUrl", resultado, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("status", resultado, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListarAlunosAsync_PaginaDeVinteEmVinte()
    {
        var handler = new FakeHttpMessageHandler(request =>
        {
            var pagina = request.RequestUri!.Query.Contains("page=1", StringComparison.Ordinal)
                ? 1
                : 2;
            var quantidade = pagina == 1 ? 20 : 1;
            var itens = Enumerable.Range(1, quantidade)
                .Select(indice => new
                {
                    id = (pagina - 1) * 20 + indice,
                    nome = $"Aluno Ficticio {pagina}-{indice}",
                    grupo = new { nome = "Plano de Teste" }
                });
            return JsonResponse(JsonSerializer.Serialize(new { items = itens }));
        });
        var service = CriarService(handler, out var chamadasToken);

        var alunos = await service.ListarAlunosAsync();

        Assert.Equal(21, alunos.Count);
        Assert.Equal(2, handler.Requests.Count);
        Assert.Contains("page=1&pageSize=20", handler.Requests.ElementAt(0)!.Query);
        Assert.Contains("page=2&pageSize=20", handler.Requests.ElementAt(1)!.Query);
        Assert.Equal(1, chamadasToken());
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task ListarAlunosAsync_401Ou403SolicitaNovoLoginSemRetry(HttpStatusCode status)
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(status));
        var client = CriarClient(handler, out var chamadasToken);

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.ListarAlunosAsync());

        Assert.Contains("--login", erro.Message, StringComparison.Ordinal);
        Assert.Single(handler.Requests);
        Assert.Equal(1, chamadasToken());
    }

    [Fact]
    public async Task ListarAlunosAsync_StatusDiferenteDe200FalhaExplicitamente()
    {
        var handler = new FakeHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var client = CriarClient(handler, out _);

        var erro = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.ListarAlunosAsync());

        Assert.Contains("HTTP 500", erro.Message, StringComparison.Ordinal);
        Assert.Single(handler.Requests);
    }

    [Theory]
    [InlineData("{\"items\":")]
    [InlineData("{\"data\":[]}")]
    public async Task ListarAlunosAsync_RejeitaJsonInvalidoOuFormatoInesperado(string body)
    {
        var handler = new FakeHttpMessageHandler(_ => JsonResponse(body));
        var client = CriarClient(handler, out _);

        await Assert.ThrowsAsync<InvalidDataException>(() => client.ListarAlunosAsync());
    }

    [Fact]
    public async Task ListarAlunosAsync_NaoArmazenaListaVaziaEmCache()
    {
        var chamada = 0;
        var handler = new FakeHttpMessageHandler(_ =>
        {
            chamada++;
            return JsonResponse(chamada == 1
                ? """{"items":[]}"""
                : """{"items":[{"id":7,"nome":"Aluno Ficticio","grupo":{"nome":"Plano de Teste"}}]}""");
        });
        var service = CriarService(handler, out var chamadasToken);

        var primeiraConsulta = await service.ListarAlunosAsync();
        var segundaConsulta = await service.ListarAlunosAsync();

        Assert.Empty(primeiraConsulta);
        Assert.Single(segundaConsulta);
        Assert.Equal(2, handler.Requests.Count);
        Assert.Equal(1, chamadasToken());
    }

    private static MfitWebService CriarService(
        FakeHttpMessageHandler handler,
        out Func<int> chamadasToken)
    {
        var client = CriarClient(handler, out chamadasToken);
        return new MfitWebService(client);
    }

    private static MfitApiClient CriarClient(
        FakeHttpMessageHandler handler,
        out Func<int> chamadasToken)
    {
        var chamadas = 0;
        chamadasToken = () => Volatile.Read(ref chamadas);
        return new MfitApiClient(
            new HttpClient(handler),
            NullLogger<MfitApiClient>.Instance,
            () =>
            {
                Interlocked.Increment(ref chamadas);
                return Task.FromResult("fixture");
            });
    }

    private static HttpResponseMessage JsonResponse(string body) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(body)
        };

    private sealed class FakeHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        public ConcurrentQueue<Uri?> Requests { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Enqueue(request.RequestUri);
            return Task.FromResult(responder(request));
        }
    }
}
