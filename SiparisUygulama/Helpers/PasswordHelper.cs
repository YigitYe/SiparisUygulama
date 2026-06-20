using System.Security.Cryptography;
using System.Text;

namespace SiparisUygulama.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string sifre)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(sifre));
            return Convert.ToHexString(bytes).ToLower();
        }

        public static bool Dogrula(string sifre, string hash)
        {
            return Hash(sifre) == hash;
        }
    }
}
