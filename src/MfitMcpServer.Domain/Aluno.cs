namespace MfitMcpServer.Domain;

/// <summary>
/// Representa um aluno da plataforma MFIT Personal.
/// </summary>
public class Aluno
{
    /// <summary>
    /// Identificador único do aluno.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome completo do aluno.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Plano de assinatura atual do aluno (ex.: "Básico", "Premium", "Elite").
    /// </summary>
    public string Plano { get; set; } = string.Empty;
}