using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlueCardPortal.Infrastructure.Converters
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
                DateOnly.ParseExact(reader.GetString()!,
                    "dd-MM-yyyy", CultureInfo.InvariantCulture);

        public override void Write(
            Utf8JsonWriter writer,
            DateOnly dateValue,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(dateValue.ToString(
                    "dd-MM-yyyy", CultureInfo.InvariantCulture));
    }
}
