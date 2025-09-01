using NodaTime;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts
{
    // Трябва да се изтрие от генерирания клиент
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.0.7.0 (NJsonSchema v11.0.0.0 (Newtonsoft.Json v13.0.0.0))")]
    internal class DateFormatConverter : System.Text.Json.Serialization.JsonConverter<System.DateTimeOffset?>
    {
        public override System.DateTimeOffset? Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            var dateTime = reader.GetString();
            if (dateTime == null)
            {
                return null;
            }

            return System.DateTimeOffset.Parse(dateTime);
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, System.DateTimeOffset? value, System.Text.Json.JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                // var date = ConvertToUtcIfUnspecified(value.Value.DateTime);
                // date = ConvertUtcToBGTime(date);
                //writer.WriteStringValue(date.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
        public static DateTime ConvertToUtcIfUnspecified(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Unspecified)
            {
                DateTimeZone zone = DateTimeZoneProviders.Tzdb["Europe/Sofia"];
                var localtime = LocalDateTime.FromDateTime(dt);
                var zonedtime = localtime.InZoneLeniently(zone);
                dt = zonedtime.ToInstant().InZone(zone).ToDateTimeUtc();
            }

            return dt;
        }

        public static DateTime ConvertUtcToBGTime(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Utc || dt.Kind == DateTimeKind.Local)
            {
                dt = dt.ToUniversalTime();
                var bgTimeZone = DateTimeZoneProviders.Tzdb["Europe/Sofia"];
                dt = Instant.FromDateTimeUtc(dt)
                            .InZone(bgTimeZone)
                            .ToDateTimeUnspecified();
            }

            return dt;
        }

        public static DateTime? ConvertUtcToBGTime(DateTime? model)
        {
            if (model != null)
            {
                DateTime dt = model ?? DateTime.UtcNow;
                if (dt.Kind == DateTimeKind.Utc || dt.Kind == DateTimeKind.Local)
                {
                    dt = dt.ToUniversalTime();
                    var bgTimeZone = DateTimeZoneProviders.Tzdb["Europe/Sofia"];
                    dt = Instant.FromDateTimeUtc(dt)
                                .InZone(bgTimeZone)
                                .ToDateTimeUnspecified();
                }

                return dt;
            }
            else
                return model;
        }
    }
}
