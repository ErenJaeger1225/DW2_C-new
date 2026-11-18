namespace RestApi.Entidades;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Permite leer tanto valores de texto como numéricos (por ejemplo: "password": 12345678)
/// convirtiéndolos automáticamente a string para evitar errores de deserialización.
/// </summary>
public class StringOrNumberConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.TryGetInt64(out var l) ? l.ToString() : reader.GetDouble().ToString();
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        using var doc = JsonDocument.ParseValue(ref reader);
        return doc.RootElement.GetRawText();
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

// Lo que ENTRA al iniciar sesión
public record LoginRequest(
    [property: JsonConverter(typeof(StringOrNumberConverter))] string? Correo = null,
    [property: JsonConverter(typeof(StringOrNumberConverter))] string? Password = null
);

// Lo que SALE: el token y los datos públicos del usuario
public record LoginRespuesta(
    string Token,
    DateTime Expira,
    UsuarioDtos Usuario);
