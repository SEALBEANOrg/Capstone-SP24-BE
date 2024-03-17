using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utilities
{
    public class MomoUtils
    {
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (byte b in hashValue)
                {
                    hash.Append(b.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static string HmacSHA256(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (byte b in hashValue)
                {
                    hash.Append(b.ToString("x2"));
                }
            }

            return hash.ToString();
        }


    }
}
