using System.Text.Json.Serialization;
namespace MfitMcpServer.Domain;

public class ExercicioCatalogo
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("grupoMuscular")]
    public string GrupoMuscular { get; set; } = string.Empty;

    [JsonPropertyName("equipamento")]
    public string Equipamento { get; set; } = string.Empty;
}