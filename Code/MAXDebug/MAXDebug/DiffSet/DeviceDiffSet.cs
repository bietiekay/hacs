using System;

namespace MAXDebug
{
	public interface IDeviceDiffSet
	{
		DeviceTypes DeviceType { get;}
		String DeviceName { get;}
		Int32 RoomID { get;}
		String RoomName { get;}
	}
}