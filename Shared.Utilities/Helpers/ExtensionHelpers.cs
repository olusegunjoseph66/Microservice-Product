using MimeTypes;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Shared.Utilities.Helpers
{
    public static class ExtensionHelpers
    {
        public static string ToDescription(this Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T? Deserialize<T>(this string jsonString) where T : new()
        {
            return !string.IsNullOrWhiteSpace(jsonString) ? JsonConvert.DeserializeObject<T>(jsonString) : new T();
        }

        public static string Serialize(this object @object)
        {
            return @object != null ? JsonConvert.SerializeObject(@object) : string.Empty;
        }

        public static string GetExtension(this string base64FileString)
        {
            if (string.IsNullOrWhiteSpace(base64FileString))
                return string.Empty;

            string[] stringComponents = base64FileString.Remove(0, 5).Split(';');
            return MimeTypeMap.GetExtension(stringComponents[0]);
        }

        public static string GetBase64String(this string base64FileString)
        {
            if (string.IsNullOrWhiteSpace(base64FileString))
                return string.Empty;

            string[] stringComponents = base64FileString.Remove(0, 5).Split(',');
            return stringComponents[1].Replace("base64,","");
        }

        public static bool IsValidBase64String(this string base64FileString)
        {
            if (string.IsNullOrWhiteSpace(base64FileString))
                return false;

            string pattern = "^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{4})$";
            string base64String = base64FileString.Remove(0, 5).Split(',')[1];
            return Regex.IsMatch(base64String, pattern);
        }
        
    }
}
