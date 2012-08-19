using System;
using System.Collections.Generic;
using System.Text;

namespace MAXDebug
{
	public class Room
	{
		public String RoomName;
		public Byte RoomID;
		public String RFAddress;
		public List<IMAXDevice> Devices;

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("RoomName: "+RoomName);
			sb.AppendLine("RoomID: "+RoomID);
			sb.AppendLine("RFAddress: "+RFAddress);
			sb.AppendLine("Devices:");

			foreach(IMAXDevice _device in Devices)
			{
				sb.Append(_device.ToString());
				sb.AppendLine();
			}

			return sb.ToString();
		}

		public Room ()
		{
			Devices = new List<IMAXDevice>();
		}
	}
}

