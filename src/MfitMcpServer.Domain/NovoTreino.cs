using System.Text.Json.Serialization;

namespace MfitMcpServer.Domain;

public class NovoTreino
{
    [JsonPropertyName("idAluno")]
    public long IdAluno { get; set; }

    [JsonPropertyName("nomeTreino")]
    public string NomeTreino { get; set; } = string.Empty;

    [JsonPropertyName("observacoes")]
    public string Observacoes { get; set; } = string.Empty;

    [JsonPropertyName("exercicios")]
    public List<ItemTreino> Exercicios { get; set; } = new();
}

public class ItemTreino
{
    [JsonPropertyName("idExercicio")]
    public long IdExercicio { get; set; }

    [JsonPropertyName("series")]
    public int Series { get; set; }

    [JsonPropertyName("repeticoes")]
    public string Repeticoes { get; set; } = string.Empty;

    [JsonPropertyName("carga")]
    public string Carga { get; set; } = string.Empty;
}