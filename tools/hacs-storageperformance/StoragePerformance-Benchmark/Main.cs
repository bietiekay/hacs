using System;
using sones.storage;
using hacs.xs1;
using System.Collections.Generic;
using xs1_data_logging;
using System.IO;

namespace hacsdbtool
{
	class MainClass
	{
		static void Main(string[] args)
		{
			Console.WriteLine("hacs storage performance tuning project: benchmark");
			Console.WriteLine("(C) 2013 Daniel Kirstenpfad - http://technology-ninja.com");
			Console.WriteLine();

			#region Generating benchmark data set
			Directory.CreateDirectory("benchmark-data");
			TinyOnDiskStorage benchmark_storage = new TinyOnDiskStorage("benchmark-data" + Path.DirectorySeparatorChar + "benchmark-data", false, 5000000);
			Console.WriteLine("Initialized benchmark-data storage: "+benchmark_storage.InMemoryIndex.Count);

			Console.Write("Generating data: ");

			int number = 1000000;

			int CursorLeft = Console.CursorLeft;
			int CursorTop = Console.CursorTop;

			for (int i=0;i<=number;i++)
			{
				Console.SetCursorPosition(CursorLeft,CursorTop);
				Console.WriteLine(i+"/"+number);

				// now generating the file...
				XS1_DataObject data = new XS1_DataObject("ServerName","Name",ObjectTypes.Actor,"TypeName",DateTime.Now,i,i);

				benchmark_storage.Write(data.Serialize());
			}
			Console.WriteLine();


			#endregion
		}
	}
}
