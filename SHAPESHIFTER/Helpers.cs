using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

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

        public static IList<string> ResultsParser(byte[] results)
        {
            IList<string> hookedFunctions = new List<string>();
            string[] functions =
            {
                "NtClose",
                "NtAllocateVirtualMemory",
                "NtAllocateVirtualMemoryEx",
                "NtCreateThread",
                "NtCreateThreadEx",
                "NtCreateUserProcess",
                "NtFreeVirtualMemory",
                "NtLoadDriver",
                "NtMapViewOfSection",
                "NtOpenProcess",
                "NtProtectVirtualMemory",
                "NtQueueApcThread",
                "NtQueueApcThreadEx",
                "NtResumeThread",
                "NtSetContextThread",
                "NtSetInformationProcess",
                "NtSuspendThread",
                "NtUnloadDriver",
                "NtWriteVirtualMemory"
            };

            int i = 0;
            foreach(byte result in results)
            {
                if(result == 1) hookedFunctions.Add(functions[i]);
                i++;
            }

            byte[] data = null;


            return hookedFunctions;
        }

        public static string ByteArrayToFormattedString(byte[] array)
        {
            StringBuilder formatted = new StringBuilder(BitConverter.ToString(array).Replace("-", ", 0x"));
            formatted.Insert(0, "0x");

            for (int i = 0; i < (formatted.Length / 9998); i++)
            {
                int insertPosition = i * 9998;
                formatted.Insert(insertPosition, Environment.NewLine);
            }

            return formatted.ToString();
            
        }


    }
}
