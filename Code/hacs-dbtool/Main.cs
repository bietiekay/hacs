using System;
using sones.storage;

namespace hacsdbtool
{
	class MainClass
	{
		#region HelpMessage
		public static void HelpMessage()
		{
			Console.WriteLine("This tool backups and restores the configuration of an EzControl XS1 device.");
			Console.WriteLine();
			Console.WriteLine("Usage:");
			Console.WriteLine();
			Console.WriteLine("\t-sl\tthis parameter lists all available sensors and a short statistic on it");
			Console.WriteLine();
		}
		#endregion

		#region list all sensors
		static void SensorList (String filename)
		{
			// try to open it for reading...
			Console.Write("Opening "+filename+" data-store for reading...");
			TinyOnDiskStorage data_store = new TinyOnDiskStorage(filename, false,100000);
			Console.WriteLine("done");
		}
		#endregion

		static void Main(string[] args)
		{
			Console.WriteLine("hacs database tool - part of the h.a.c.s toolkit");
			Console.WriteLine("(C) 2013 Daniel Kirstenpfad - http://technology-ninja.com");
			Console.WriteLine();
			
			#region Syntax Handling
			// check for enough parameters
			if (args.Length > 4)
			{
				switch (args[1])
				{
				case "-sl":
					SensorList(args[0]);
					break;
				default:
					HelpMessage();
					return;
				}
			}
			else
			{
				HelpMessage();
				return;
			}
		#endregion			
		}
	}
}
