using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Chat
{
    public class AuthToken
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        public const string SampleKey = "hWwqCfipzCPXOY5avytwjhdv8PN3u/RThzd9hNEtP6I=";
        readonly static byte[] keyBytes = DefaultEncoding.GetBytes(SampleKey);

        public static string GenerateAuthToken(long dbId, string accountId, string ip)
        {
            return $"{dbId}{accountId}{ip}";
        }

        public static string EncryptAuthToken(long dbId, string accountId, string ip)
        {
            return EncryptAuthToken(GenerateAuthToken(dbId, accountId, ip));
        }

        public static string EncryptAuthToken(string token)
        {
            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] bytes = hmac.ComputeHash(DefaultEncoding.GetBytes(token));
                return Convert.ToBase64String(bytes);
            }
        }

        public static bool Verify(string original, string encrypted)
        {
            return string.Equals(EncryptAuthToken(original), encrypted);
        }
    }
}
