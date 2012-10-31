using System;

namespace SolarLogDebug
{
	public class SolarLogDataSet
	{
		public DateTime DateAndTime;
		public Int32 Pac;
		public Int32 aPdc;
		public float InverterEfficiency;

		public SolarLogDataSet()
		{
			DateAndTime = DateTime.Now;
		}
	}
}

