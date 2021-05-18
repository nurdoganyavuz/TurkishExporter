using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;

namespace Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value.TrimAll());
        public static string TrimAll(this string value) => value == null ? string.Empty : value.Trim();

        public static string ToJson(this object value, bool camelCase = false, string dateTimeFormatConverter = null) => JsonConvert.SerializeObject(value, GetJsonSettings(camelCase, dateTimeFormatConverter));

        public static T FromJson<T>(this string value, bool camelCase = false, string dateTimeFormatConverter = null) => JsonConvert.DeserializeObject<T>(value, GetJsonSettings(camelCase, dateTimeFormatConverter));

        public static JsonSerializerSettings GetJsonSettings(bool camelCase, string dateTimeFormatConverter = null)
        {
            var settings = new JsonSerializerSettings
            {
                Culture = CultureInfo.CurrentCulture,
                Error = (sender, args) =>
                {
                    args.ErrorContext.Handled = true;
                },
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            if (!dateTimeFormatConverter.IsNullOrEmpty())
                settings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = dateTimeFormatConverter });
            if (camelCase)
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return settings;
        }
    }
}
