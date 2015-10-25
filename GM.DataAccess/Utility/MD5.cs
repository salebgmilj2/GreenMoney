using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace GM.Utility
{
    public static class MD5
    {
        public static string Hash(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var provider = new MD5CryptoServiceProvider();
            var bytes = provider.ComputeHash(data);

            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2").ToLowerInvariant());
            }
            return sb.ToString();
        }

        public static string Hash(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            return Hash(Encoding.UTF8.GetBytes(data));
        }
    }
}