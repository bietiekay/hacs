using System;

namespace xs1_data_logging
{
	public interface IDeviceDiffSet
	{
		DeviceTypes DeviceType { get;}
		String DeviceName { get;}
		Int32 RoomID { get;}
		String RoomName { get;}
	}
}