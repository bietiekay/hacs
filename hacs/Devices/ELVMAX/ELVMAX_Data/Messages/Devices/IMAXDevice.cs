using System;

namespace hacs
{
	public interface IMAXDevice
	{
		DeviceTypes Type { get;}
		String RFAddress { get; set;}
		String SerialNumber { get; set;}
		String Name { get; set;}
		Room AssociatedRoom { get; set;}
		DateTime LastUpdate { get;}
	}
}

