using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.DataAccess.Utility
{
    public static class ZBase32
    {
        public const string ValidDataPattern = @"^[ABCDEFGHIJKMNOPQRSTUWXYabcdefghijkmnopqrstuwxyz13456789]+$";

        public static string Encode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var zbase = new ZBase32Encoder();
            return zbase.Encode(data);
        }

        public static byte[] Decode(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var zbase = new ZBase32Encoder();
            return zbase.Decode(data);
        }
    }
}
