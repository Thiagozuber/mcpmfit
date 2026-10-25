using System.Text.Json.Serialization;

namespace MfitMcpServer.Domain;

public class DetalheTreino
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("idAluno")]
    public long IdAluno { get; set; }

    [JsonPropertyName("idRoutine")]
    public long IdRoutine { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("obs")]
    public string Obs { get; set; } = string.Empty;

    [JsonPropertyName("exerciciosMontados")]
    public Dictionary<string, List<ExercicioMontado>> ExerciciosMontados { get; set; } = new();
}

public class ExercicioMontado
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("series")]
    public List<SerieExercicio> Series { get; set; } = new();

    [JsonPropertyName("urlMedia")]
    public string UrlMedia { get; set; } = string.Empty;
}

public class SerieExercicio
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("repeticao")]
    public string Repeticao { get; set; } = string.Empty;

    [JsonPropertyName("carga")]
    public string Carga { get; set; } = string.Empty;

    [JsonPropertyName("tempo")]
    public string Tempo { get; set; } = string.Empty;

    [JsonPropertyName("intervalo")]
    public double Intervalo { get; set; }
}