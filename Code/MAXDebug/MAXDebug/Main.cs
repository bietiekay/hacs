using System;

namespace MAXDebug
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ConsoleOutputLogger.verbose = true;
			ConsoleOutputLogger.writeLogfile = true;

			Console.WriteLine ("ELV MAX! Debug Tool version 1 (C) Daniel Kirstenpfad 2012");

			// not enough paramteres given, display help
			if (args.Length < 2)
			{
				Console.WriteLine("Syntax:");
				Console.WriteLine();
				Console.WriteLine("\tmaxdebug <hostname/ip> <port (e.g. 62910)> [commands]");
				Console.WriteLine();
				return;
			}

			// we obviously have enough paramteres, go on and try to connect


		}
	}
}
