using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Saba.Application.Helpers
{
    internal static class CryptoHelper
    {
        const int SaltLength = 8;
        public static bool ComparePassword(string pwd, string pwdHash, string salt)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.ASCII.GetBytes(pwd + salt));
                var hash = BitConverter.ToString(bytes).Replace("-", null);
                return String.Compare(pwdHash, hash, true) == 0;
            }
        }

        public static (string passwordHash, string salt) ComputePassword(string pwd)
        {
            using (var sha = SHA256.Create())
            {
                var saltBytes = RandomNumberGenerator.GetBytes(SaltLength);
                string salt = Convert.ToBase64String(saltBytes);
                var bytes = sha.ComputeHash(Encoding.ASCII.GetBytes(pwd + salt));
                var hash = BitConverter.ToString(bytes).Replace("-", null);
                return (hash, salt);
            }
        }

    }
}
