using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM2E15044.Extensions
{
    public static class PhotoHelper
    {
        public static byte[] GetImageArrayFromBase64(string base64String)
        {
            if (!string.IsNullOrEmpty(base64String))
            {
                try
                {
                    byte[] data = Convert.FromBase64String(base64String);
                    return data;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting base64 string to byte array: {ex.Message}");
                }
            }
            return null;
        }
    }
}
