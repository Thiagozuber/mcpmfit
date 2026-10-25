using MfitMcpServer.Application;
using MfitMcpServer.Domain;

namespace MfitMcpServer.Infra;

/// <summary>
/// Implementação de teste (mock) de <see cref="IMFitService"/> que devolve dados fictícios.
/// </summary>
public class MockMfitService : IMFitService
{
    /// <summary>
    /// Lista fixa de três alunos fictícios para prototipagem.
    /// </summary>
    private static readonly List<Aluno> AlunosMock = new()
    {
        new Aluno { Id = 1, Nome = "Ana Silva", Plano = "Premium" },
        new Aluno { Id = 2, Nome = "Bruno Costa", Plano = "Básico" },
        new Aluno { Id = 3, Nome = "Carla Mendes", Plano = "Elite" },
    };

    public Task<List<Aluno>> ListarAlunosAsync()
    {
        return Task.FromResult(new List<Aluno>(AlunosMock));
    }
}