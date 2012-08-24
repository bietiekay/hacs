using System;
using System.Threading;

namespace MAXDebug
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ConsoleOutputLogger.verbose = true;
			ConsoleOutputLogger.writeLogfile = true;

			Console.WriteLine ("ELV MAX! Debug Tool version 1 (C) Daniel Kirstenpfad 2012");
			Console.WriteLine();

			// not enough paramteres given, display help
			if (args.Length < 2)
			{
				Console.WriteLine("Syntax:");
				Console.WriteLine();
				Console.WriteLine("\tmaxdebug <hostname/ip> <port (e.g. 62910)> [commands]");
				Console.WriteLine();
				return;
			}
			ConsoleOutputLogger.LogToFile("--------------------------------------");

			MAXMonitoringThread _Thread = new MAXMonitoringThread(args[0], Convert.ToInt32 (args[1]),1000);
            Thread MAXMonitoring = new Thread(new ThreadStart(_Thread.Run));

			MAXMonitoring.Start();

			while(_Thread.running)
			{
				Thread.Sleep(100);
			}
		}
	}
}
