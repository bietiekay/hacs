using System;
using System.Collections.Generic;
using System.Text;

namespace MAXDebug
{
	public class Room
	{
		public String RoomName;
		public Byte RoomID;
		public Int32 RFAddress;
		public List<Device> Devices;

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("RoomName: "+RoomName);
			sb.AppendLine("RoomID: "+RoomID);
			sb.AppendLine("RFAddress: "+RFAddress);
			sb.AppendLine("Devices:");

			foreach(Device _device in Devices)
			{
				sb.Append(_device.ToString());
				sb.AppendLine();
			}

			return sb.ToString();
		}

		public Room ()
		{
			Devices = new List<Device>();
		}
	}
}

