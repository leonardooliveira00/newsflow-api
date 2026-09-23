using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NewsflowApi.Application.Authentication
{
    public static class InvitationTokenCodec
    {
        public static string Encode(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);

            return WebEncoders.Base64UrlEncode(bytes);
        }

        public static bool TryDecode(string encodedToken, [NotNullWhen(true)] out string? decodedToken)
        {
            decodedToken = null;

            if (string.IsNullOrWhiteSpace(encodedToken)) return false;

            try
            {

                var bytes = WebEncoders.Base64UrlDecode(encodedToken);

                decodedToken = Encoding.UTF8.GetString(bytes);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
