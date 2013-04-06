using System;
using sones.storage;
using hacs.xs1;
using sones.Storage;
using System.Collections.Generic;

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

			Dictionary<String,XS1_DataObject> Sensors = new Dictionary<string, XS1_DataObject>();

			foreach (OnDiscAdress ondisc in data_store.InMemoryIndex)
			{
				XS1_DataObject dataobject = new XS1_DataObject();
				dataobject.Deserialize(data_store.Read(ondisc));
				if (!Sensors.ContainsKey(dataobject.Name))
					Sensors.Add(dataobject.Name,dataobject);
				//Console.WriteLine(dataobject.Timecode.ToLongTimeString()+";"+dataobject.Timecode.ToShortDateString()+";"+dataobject.Name+";"+dataobject.Type+";"+dataobject.TypeName+";"+dataobject.Value+";"+dataobject.OriginalXS1Statement);
			}

			foreach(XS1_DataObject dataobject in Sensors.Values)
			{
				Console.WriteLine(dataobject.Name+";"+dataobject.Type+";"+dataobject.TypeName+";"+dataobject.Value+";"+dataobject.OriginalXS1Statement);
			}

		}
		#endregion

		static void Main(string[] args)
		{
			Console.WriteLine("hacs database tool - part of the h.a.c.s toolkit");
			Console.WriteLine("(C) 2013 Daniel Kirstenpfad - http://technology-ninja.com");
			Console.WriteLine();

			#region Syntax Handling
			// check for enough parameters
			if (args.Length > 0)
			{
				switch (args[0])
				{
					case "-sl":
						SensorList(args[1]);
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
