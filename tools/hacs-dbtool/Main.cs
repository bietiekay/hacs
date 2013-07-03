using System;
using sones.storage;
using hacs.xs1;
using sones.Storage;
using System.Collections.Generic;
using xs1_data_logging;

namespace hacsdbtool
{
	class MainClass
	{
		#region HelpMessage
		public static void HelpMessage()
		{
			Console.WriteLine("hacs database tool - part of the h.a.c.s toolkit");
			Console.WriteLine("(C) 2013 Daniel Kirstenpfad - http://technology-ninja.com");
			Console.WriteLine();

			Console.WriteLine("This tool backups and restores the configuration of an EzControl XS1 device.");
			Console.WriteLine();
			Console.WriteLine("Usage:");
			Console.WriteLine();
			Console.WriteLine("\t-sl\tthis parameter lists all available sensors and a short statistic on it");
			Console.WriteLine("\t-kml\tthis parameter exports all available latitude data for a certain accountname as kml");
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

		#region Export as KML
		static void ExportKML(String filename, String Accountname)
		{
			Console.WriteLine ("<?xml version=\"1.0\" encoding=\"UTF-8\"?><kml xmlns=\"http://www.opengis.net/kml/2.2\"><Document><name>" + Accountname + "</name><open>1</open>");
			Console.WriteLine ("<Style id=\"trailsstyle\">");
			Console.WriteLine ("<LineStyle><color>7f0000ff</color><width>6</width></LineStyle></Style><description>n/a</description>");
			
			// try to open it for reading...			
			TinyOnDiskStorage data_store = new TinyOnDiskStorage(filename, false,100000);
			Console.WriteLine("<Placemark><styleUrl>#trailsstyle</styleUrl><name>"+Accountname+"</name><LineString>;<tessellate>1</tessellate>");
			Console.WriteLine("<coordinates>");

			foreach (OnDiscAdress ondisc in data_store.InMemoryIndex)
			{
				GoogleLatitudeDataObject dataobject = new GoogleLatitudeDataObject();
				dataobject.Deserialize(data_store.Read(ondisc));
				//Int32 CurrentDay = -1;

				if (dataobject.AccountName == Accountname) 
				{
					Console.WriteLine(String.Format("{0:F6}#{1:F6}#{0:F6}",dataobject.Latitude, dataobject.Longitude,dataobject.AccuracyInMeters).Replace(",",".").Replace("#",","));
					// check which date it is...
/*					if (CurrentDay == -1)
					{	// the first time...
						Console.WriteLine("<Placemark><styleUrl>#trailsstyle</styleUrl><name>"+DateTime.FromBinary (dataobject.Timecode).ToString()+"</name><LineString>;<tessellate>1</tessellate>");
						Console.WriteLine("<coordinates>");

						CurrentDay = DateTime.FromBinary (dataobject.Timecode).Day;
					}

					if (DateTime.FromBinary (dataobject.Timecode).Day == CurrentDay) 
					{
						Console.WriteLine(String.Format("{0:F6},{1:F6},{0:F6}",dataobject.Latitude, dataobject.Longitude,dataobject.AccuracyInMeters));
					}
					else
					{
						Console.WriteLine("</coordinates></LineString></Placemark>");
						Console.WriteLine("<Placemark><styleUrl>#trailsstyle</styleUrl><name>"+DateTime.FromBinary (dataobject.Timecode).ToString()+"</name><LineString>;<tessellate>1</tessellate>");
						Console.WriteLine("<coordinates>");
						Console.WriteLine(String.Format("{0:F6},{1:F6},{0:F6}",dataobject.Latitude, dataobject.Longitude,dataobject.AccuracyInMeters));
						CurrentDay = DateTime.FromBinary (dataobject.Timecode).Day;
					}*/
				}
			}
			Console.WriteLine("</coordinates></LineString></Placemark></Document></kml>");
		}
		#endregion

		static void Main(string[] args)
		{
			#region Syntax Handling
			// check for enough parameters
			if (args.Length > 0)
			{
				switch (args[0])
				{
					case "-sl":
						SensorList(args[1]);
						break;
					case "-kml":
					ExportKML(args[1],args[2]);
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
