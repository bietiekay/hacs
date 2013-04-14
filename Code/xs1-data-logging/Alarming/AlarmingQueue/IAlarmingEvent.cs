using System;

namespace xs1_data_logging
{
	public interface IAlarmingEvent
	{
		AlarmingEventType AlarmingType();
		String AlarmingName();
		DateTime AlarmingCreated();
	}
}

