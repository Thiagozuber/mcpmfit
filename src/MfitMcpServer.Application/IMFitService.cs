using MfitMcpServer.Domain;

namespace MfitMcpServer.Application;

public interface IMFitService
{
    Task<List<Aluno>> ListarAlunosAsync();
}