using MfitMcpServer.Application;
using MfitMcpServer.Domain;

namespace MfitMcpServer.Infra;

public sealed class MfitWebService : IMFitService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly MfitApiClient _apiClient;
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private CacheEntry? _cache;

    public MfitWebService(MfitApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<Aluno>> ListarAlunosAsync()
    {
        var cache = Volatile.Read(ref _cache);
        if (cache is not null && cache.ExpiresAt > DateTimeOffset.UtcNow)
        {
            return Copiar(cache.Alunos);
        }

        await _cacheSemaphore.WaitAsync();
        try
        {
            cache = Volatile.Read(ref _cache);
            if (cache is not null && cache.ExpiresAt > DateTimeOffset.UtcNow)
            {
                return Copiar(cache.Alunos);
            }

            var alunos = await _apiClient.ListarAlunosAsync();
            if (alunos.Count > 0)
            {
                Volatile.Write(ref _cache, new CacheEntry(
                    Copiar(alunos),
                    DateTimeOffset.UtcNow.Add(CacheDuration)));
            }

            return Copiar(alunos);
        }
        finally
        {
            _cacheSemaphore.Release();
        }
    }

    private static List<Aluno> Copiar(IEnumerable<Aluno> alunos) =>
        alunos.Select(aluno => new Aluno
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Plano = aluno.Plano
        }).ToList();

    private sealed record CacheEntry(List<Aluno> Alunos, DateTimeOffset ExpiresAt);
}
