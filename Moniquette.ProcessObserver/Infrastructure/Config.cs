using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Moniquette.ProcessObserver.Infrastructure;

public sealed class Config
{
    [Required] public long[] Seeds { get; set; } = null!;

    [JsonIgnore] public static Config Instance { get; }

    static Config()
    {
        if (!Path.Exists(ConfigPath))
        {
            throw new FileNotFoundException("properties.json not found");
        }

        using StreamReader reader = new(ConfigPath);
        Instance = JsonSerializer.Deserialize<Config>(reader.BaseStream,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? throw new InvalidOperationException("Failed to get properties.json");
    }

    [JsonIgnore] private static readonly string ConfigPath =
        Path.Combine(Environment.CurrentDirectory, "properties.json");
}