using System;

namespace xs1_data_logging
{
	public interface IMAXDevice
	{
		DeviceTypes Type { get;}
		String RFAddress { get; set;}
		String SerialNumber { get; set;}
		String Name { get; set;}
		Room AssociatedRoom { get; set;}
	}
}

