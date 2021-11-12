using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventulaEntranceClient.Converter
{
    /// <summary>
    /// Handles converting JSON string values into a C# boolean data type.
    /// </summary>
    public class BooleanJsonConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value;
            switch (reader.TokenType)
            {
                case JsonTokenType.None:
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.Number:
                    value = reader.GetInt64().ToString();
                    break;
                case JsonTokenType.String:
                    value = reader.GetString();
                    break;
                default:
                    throw new NotSupportedException($"TokenType {reader.TokenType} is not yet supported!");
            }

            if (bool.TryParse(value, out var result))
            {
                return result;
            }

            switch (value.ToLower().Trim())
            {
                case "true":
                case "yes":
                case "y":
                case "1":
                    return true;
                case "false":
                case "no":
                case "n":
                case "0":
                    return false;
                default:
                    // Do nothing
                    break;
            }

            // If we reach here, we're pretty much going to throw an error so let's let Json.NET throw it's pretty-fied error message.
            return JsonSerializer.Deserialize(reader.GetString(), typeToConvert) as bool? ?? false;
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}
