using System;

namespace SolarLogDebug
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("SolarLog Revere-Engineering Sample");

			SolarLogDataSet data = SolarLog.UpdateSolarLog("solarlog.fritz.box");

			Console.WriteLine(data.DateAndTime.ToString());
		}
	}
}
