using System;

namespace hacs
{
	public interface IAlarmingEvent
	{
		AlarmingEventType AlarmingType();
		String AlarmingName();
		DateTime AlarmingCreated();
	}
}

