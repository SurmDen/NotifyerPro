using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace IdentityManager.Models
{
    public class PasswordHesher
    {
        public static string HeshPassword(string password)
        {
            SHA256 heshAlg = SHA256.Create();

            byte[] heshedBytes = heshAlg.ComputeHash(Encoding.ASCII.GetBytes(password));

            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in heshedBytes)
            {
                stringBuilder.Append(b.ToString("X2"));
            }

            return stringBuilder.ToString();
        }
    }
}
