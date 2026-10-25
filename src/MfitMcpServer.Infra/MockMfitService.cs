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

    public async Task<DetalheTreino?> ObterTreinoPorIdAsync(long idPrescription)
{
    // Retorna um treino simulado para testes ou null se não encontrado
    return await Task.FromResult<DetalheTreino?>(new DetalheTreino
    {
        Id = idPrescription,
        Nome = "Treino Mock de Teste",
        Obs = "Gerado pelo MockMfitService"
    });

    
}

public async Task<List<ExercicioCatalogo>> PesquisarExerciciosAsync(string termoOuGrupo)
{
    var exercicios = new List<ExercicioCatalogo>
    {
        new ExercicioCatalogo { Id = 101, Nome = "Supino Reto com Barra", GrupoMuscular = "Peitoral", Equipamento = "Barra" },
        new ExercicioCatalogo { Id = 102, Nome = "Agachamento Livre", GrupoMuscular = "Quadríceps", Equipamento = "Barra" },
        new ExercicioCatalogo { Id = 103, Nome = "Puxada Alta", GrupoMuscular = "Dorsal", Equipamento = "Polia" }
    };

    var resultado = exercicios
        .Where(e => e.Nome.Contains(termoOuGrupo, StringComparison.OrdinalIgnoreCase) ||
                    e.GrupoMuscular.Contains(termoOuGrupo, StringComparison.OrdinalIgnoreCase))
        .ToList();

    return await Task.FromResult(resultado);
}

public async Task<bool> CriarTreinoAsync(NovoTreino novoTreino)
{
    return await Task.FromResult(true);
}

}