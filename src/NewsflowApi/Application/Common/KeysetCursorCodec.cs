using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace NewsflowApi.Application.Common
{
    public static class KeysetCursorCodec
    {
        public static string Encode<T>(T cursor)
        {
            var json = JsonSerializer.Serialize(cursor);

            var bytes = Encoding.UTF8.GetBytes(json);

            return WebEncoders.Base64UrlEncode(bytes);
        }

        public static bool TryDecode<T>(string encodedCursor, out T? cursor)
        {
            cursor = default;

            try
            {
                var bytes = WebEncoders.Base64UrlDecode(encodedCursor);

                var json = Encoding.UTF8.GetString(bytes);

                cursor = JsonSerializer.Deserialize<T>(json);

                return cursor is not null;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
