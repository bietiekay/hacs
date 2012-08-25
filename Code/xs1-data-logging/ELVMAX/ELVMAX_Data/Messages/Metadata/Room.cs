using System;
using System.Collections.Generic;
using System.Text;

namespace xs1_data_logging
{
	public class Room
	{
		public String RoomName;
		public Byte RoomID;
		public String RFAddress;
		public List<IMAXDevice> Devices;
		public House AssociatedHouse;

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

		public Room (House _House)
		{
			AssociatedHouse = _House;
			Devices = new List<IMAXDevice>();
		}
	}
}

