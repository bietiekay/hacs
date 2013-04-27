using System;

namespace hacs
{
	public interface IDeviceDiffSet : IAlarmingEvent
	{
		DeviceTypes DeviceType { get;}
		String DeviceName { get;}
		Int32 RoomID { get;}
		String RoomName { get;}
	}
}