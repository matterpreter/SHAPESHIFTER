using System;
using System.Linq;
using System.Net;


namespace SHAPESHIFTER
{
    class Helpers
    {
        public static bool ValidateIP(string ip)
        {
            // Check if the string has 4 octets with Linq
            if (ip.Count(d => d == '.') != 3) return false;
            // Validate the IP
            return IPAddress.TryParse(ip, out IPAddress addr);
        }
        
        public static string GenerateRandomFileName()
        {
            Random random = new Random();
            const string charset = "abcdefghijklmnopqrstuvwxyz0123456789";
            char[] name = Enumerable.Repeat(charset, 12).Select(s => s[random.Next(s.Length)]).ToArray();
            return new string(name) + ".exe";
        }
    }
}
