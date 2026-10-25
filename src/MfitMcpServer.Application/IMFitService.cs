using MfitMcpServer.Domain;

namespace MfitMcpServer.Application;

public interface IMFitService
{
    Task<List<Aluno>> ListarAlunosAsync();
    Task<DetalheTreino?> ObterTreinoPorIdAsync(long idPrescription);
    Task<List<ExercicioCatalogo>> PesquisarExerciciosAsync(string termoOuGrupo);
    Task<bool> CriarTreinoAsync(NovoTreino novoTreino);
}