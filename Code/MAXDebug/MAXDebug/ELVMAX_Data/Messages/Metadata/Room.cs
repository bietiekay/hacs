using System;
using System.Collections.Generic;

namespace MAXDebug
{
	public class Room
	{
		public String RoomName;
		public Byte RoomID;
		public Int32 RFAddress;
		public List<Device> Devices;

		public Room ()
		{
			Devices = new List<Device>();
		}
	}
}

