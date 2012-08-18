using System;
using System.Text;

namespace MAXDebug
{
	public class Device
	{
		public DeviceTypes Type;
		public Int32 RFAddress;
		public String SerialNumber;
		public String Name;

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("\tDeviceType: "+Type.ToString());
			sb.AppendLine("\tDeviceName: "+Name);
			sb.AppendLine("\tSerialNumber: "+SerialNumber);
			sb.AppendLine("\tRFAddress: "+RFAddress);

			return sb.ToString();
		}

		public Device()
		{
			Type = DeviceTypes.Invalid;
		}

	}
}

