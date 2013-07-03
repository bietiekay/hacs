using System;
using System.Diagnostics;
using sones.storage;
using hacs.xs1;
using System.Collections.Generic;
using xs1_data_logging;
using System.IO;
using sones.Storage;

namespace hacsdbtool
{
	class MainClass
	{
		static void Main(string[] args)
		{
			Console.WriteLine("hacs storage performance tuning project: benchmark");
			Console.WriteLine("(C) 2013 Daniel Kirstenpfad - http://technology-ninja.com");
			Console.WriteLine();

			int number = 100000;
			int cachefactor = 1;
			Stopwatch sw = new Stopwatch();

			#region Generating benchmark data set
			if (Directory.Exists("benchmark-data"))
			{
				Directory.Delete("benchmark-data",true);
				Directory.CreateDirectory("benchmark-data");
			}
			else
				Directory.CreateDirectory("benchmark-data");
			sw.Start();
			TinyOnDiskStorage benchmark_storage = new TinyOnDiskStorage("benchmark-data" + Path.DirectorySeparatorChar + "benchmark-data", false, number/cachefactor);
			sw.Stop();
			Console.WriteLine("Initialized benchmark-data storage in "+sw.ElapsedMilliseconds+" ms");

			sw.Reset();

			Console.Write("Generating data ("+number+") - ");

			int CursorLeft = Console.CursorLeft;
			int CursorTop = Console.CursorTop;

			sw.Start ();
			for (int i=0;i<=number;i++)
			{
				//Console.SetCursorPosition(CursorLeft,CursorTop);
				//Console.Write(i+"/"+number);

				// now generating the file...
				XS1_DataObject data = new XS1_DataObject("ServerName","Name",ObjectTypes.Actor,"TypeName",DateTime.Now,i,i);

				benchmark_storage.Write(data.Serialize());
			}
			sw.Stop();
			Console.WriteLine("done in "+sw.ElapsedMilliseconds+" ms");

			sw.Reset();
			Console.WriteLine("Resetting...");

			benchmark_storage.Shutdown();

			sw.Start();
			benchmark_storage = new TinyOnDiskStorage("benchmark-data" + Path.DirectorySeparatorChar + "benchmark-data", false, number/2);
			sw.Stop();
			Console.WriteLine("Initialized benchmark-data storage in "+sw.ElapsedMilliseconds+" ms");
			sw.Reset();

			Console.Write("Reading ("+number+") - ");
			int o = 0;
			sw.Start();
			foreach(OnDiscAdress addr in benchmark_storage.InMemoryIndex)
			{
				XS1_DataObject data = new XS1_DataObject();
				data.Deserialize(benchmark_storage.Read(addr));
				if (data.XS1ObjectID != o)
					Console.WriteLine(data.Timecode.ToString());
				o++;
			}
			o--;
			sw.Stop();
			Console.WriteLine("done ("+o+") in "+sw.ElapsedMilliseconds+" ms");
			sw.Reset();
			#endregion
		}
	}
}
