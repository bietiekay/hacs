using System;
using System.Text;

namespace hacs
{
	public class PushButton : IMAXDevice
	{
		private DeviceTypes _Type;
		private String _RFAddress;
		private String _SerialNumber;
		private String _Name;
		private Room _Room;
		private DateTime lastUpdate;

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("\tDeviceType: "+Type.ToString());
			sb.AppendLine("\tDeviceName: "+Name);
			sb.AppendLine("\tSerialNumber: "+SerialNumber);
			sb.AppendLine("\tRFAddress: "+RFAddress);

			return sb.ToString();
		}

		public PushButton()
		{
			_Type = DeviceTypes.PushButton;
			lastUpdate = DateTime.Now;
		}		

		#region IMAXDevice implementation
		public DeviceTypes Type {
			get {
				return _Type;
			}
		}

		public string RFAddress {
			get {
				return _RFAddress;
			}
			set {
				lastUpdate = DateTime.Now;
				_RFAddress = value;
			}
		}

		public string SerialNumber {
			get {
				return _SerialNumber;
			}
			set {
				lastUpdate = DateTime.Now;
				_SerialNumber = value;
			}
		}

		public string Name {
			get {
				return _Name;
			}
			set {
				lastUpdate = DateTime.Now;
				_Name = value;
			}
		}

		public Room AssociatedRoom
 		{
			get {
				return _Room;
			}
			set {
				lastUpdate = DateTime.Now;
				_Room = value;
			}
		}

		public DateTime LastUpdate {
			get {
				return lastUpdate;
			}
		}
		#endregion


	}
}

