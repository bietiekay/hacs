using System;

namespace hacs
{
	public class SolarLogDataSet : IAlarmingEvent
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

		#region IAlarmingEvent implementation

		public AlarmingEventType AlarmingType ()
		{
			return AlarmingEventType.SolarLogEvent;
		}

		public string AlarmingName ()
		{
			return "SolarLog";
		}

		public DateTime AlarmingCreated ()
		{
			return DateAndTime;
		}

		#endregion
	}
}

