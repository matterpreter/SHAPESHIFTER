using System;
using System.Net;
using System.Net.Sockets;


namespace SHAPESHIFTER
{
    class TcpServer
    {
        public static void ServerInit(string address, int port)
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
                Console.Write("[+] Server started on {0}:{1}...\n", ip, port);
                Console.WriteLine("Prepared to receive {0} bytes of data", bytes.Length);
                while (true)
                {
                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        string data = BitConverter.ToString(bytes);
                        //data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);


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
