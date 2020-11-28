using System;

namespace Stage0
{
	class Program
	{
        public static string _host = "[SHAPESHIFTER_HOST]";
        public static string _port = "[SHAPESHIFTER_PORT]";
        static void Main()
		{
			Console.WriteLine("{0}:{1}", _host, _port);
		}
	}
}