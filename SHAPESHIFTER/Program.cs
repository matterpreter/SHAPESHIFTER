using NDesk.Options;
using System;

namespace SHAPESHIFTER
{
    class Program
    {
        static readonly string banner = @"
  __       _  __  ___  __      ___ ___ ___ ___ __  
 (_ ` )_) /_) )_) )_  (_ ` )_)  )  )_   )  )_  )_) 
.__) ( ( / / /   (__ .__) ( ( _(_ (    (  (__ / \  
        ";

        static void Main(string[] args)
        {
            bool _help = false;
            string _host = "";
            int _port = 0;

            OptionSet options = new OptionSet()
            {
                { "host=", "IP address or FQDN of your SHAPESHIFTER server", v => _host = v },
                { "port=", "TCP port for the SHAPESHIFTER server", (int v) => _port = v },
                { "h|help",  "Show this message and exit", v => _help = v != null },
            };
            try
            {
                options.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.WriteLine("[-] Error while parsing arguments {0}", ex.Message);
                return;
            }

            if (_help)
            {
                PrintUsage(options);
                return;
            }

            if (_host != String.Empty && _port != 0)
            {
                Console.WriteLine(banner);
                Console.WriteLine("Host: {0}", _host);
                Console.WriteLine("Host: {0}", _port);
            }
            else
            {
                PrintUsage(options);
                return;
            }

            return;
        }

        static void PrintUsage(OptionSet o)
        {
            Console.WriteLine("Usage: SHAPESHIFTER.exe [options]");
            Console.WriteLine("\nOptions:");
            o.WriteOptionDescriptions(Console.Out);
            return;
        }
    }
}
