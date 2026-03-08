using System.Security.Cryptography;
using System.Text;

namespace Reservo.Services.Credentials
{
    public static class CryptoHelper
    {
        private static readonly byte[] Key = SHA256.HashData(Encoding.UTF8.GetBytes(CredentialsService.apiKey));

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor();
            var inputBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Convert.ToBase64String(aes.IV.Concat(cipherBytes).ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = Key;

            var iv = fullCipher[..16];
            var cipher = fullCipher[16..];

            aes.IV = iv;
            var decryptor = aes.CreateDecryptor();

            var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
