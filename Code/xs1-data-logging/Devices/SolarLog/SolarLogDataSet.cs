using System;

namespace xs1_data_logging
{
	public class SolarLogDataSet
	{
		public DateTime DateAndTime;
		public Int32 Pac;
		public Int32 aPdc;

		public SolarLogDataSet()
		{
			DateAndTime = DateTime.Now;
			Pac = 0;
			aPdc = 0;
		}
	}
}

