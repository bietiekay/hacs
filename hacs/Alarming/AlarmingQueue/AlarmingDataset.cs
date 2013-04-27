using System;

namespace hacs
{
	/// <summary>
	/// this is just the standard implementation for an alarming event - it just holds a couple of default information
	/// that help to determine to which object this should be casted...
	/// </summary>
	public class AlarmingDataSet : IAlarmingEvent
	{
		private String _Name;
		private AlarmingEventType _Type;
		private DateTime _created;

		public AlarmingDataSet (AlarmingEventType Type, String Name)
		{
			_Type = Type;
			_Name = Name;
			_created = DateTime.Now;
		}

		#region IAlarmingEvent implementation

		public AlarmingEventType AlarmingType ()
		{
			return _Type;
		}

		public string AlarmingName ()
		{
			return _Name;
		}

		public DateTime AlarmingCreated ()
		{
			return _created;
		}

		#endregion
	}
}

