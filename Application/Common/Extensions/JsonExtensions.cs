using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CaseConverter;
using FluentResults;

namespace Application.Common.Extensions;

public static class JsonExtensions
{
    public static Result<T> ConvertJsonElementToRecord<T>(this JsonElement jsonElement)
    {
        // Obtener las propiedades válidas del record
        HashSet<string> validKeys = typeof(T)
            .GetProperties()
            .Select(p => p.Name.ToCamelCase())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Verificar que no haya keys desconocidas
        foreach (JsonProperty property in jsonElement.EnumerateObject())
        {
            if (!validKeys.Contains(property.Name))
                return Result.Fail(
                    $"Opción desconocida '{property.Name}' para {typeof(T).Name}. " +
                    $"Opciones válidas: {string.Join(", ", validKeys)}");
        }

        IEnumerable<string> requiredKeys = typeof(T)
            .GetProperties()
            .Where(p => p.GetCustomAttribute<JsonRequiredAttribute>() != null)
            .Select(p => p.Name.ToCamelCase());

        foreach (string key in requiredKeys)
            if (!jsonElement.TryGetProperty(key, out _))
                return Result.Fail($"La opción '{key}' es requerida para {typeof(T).Name}.");

        try
        {
            T? jsonObject = JsonSerializer.Deserialize<T>(jsonElement.GetRawText(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper) }
            });
            return jsonObject != null ? Result.Ok(jsonObject) : Result.Fail($"No se pudo deserializar las opciones para {typeof(T).Name}");
        }
        catch (JsonException ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public static string? GetString(this Dictionary<string, JsonElement> json, string key)
        => json.TryGetValue(key, out var val) ? val.GetString() : null;

    public static bool GetBool(this Dictionary<string, JsonElement> json, string key)
        => json.TryGetValue(key, out var val) && val.GetBoolean();

    public static int? GetInt(this Dictionary<string, JsonElement> json, string key)
        => json.TryGetValue(key, out var val) ? val.GetInt32() : null;
}
