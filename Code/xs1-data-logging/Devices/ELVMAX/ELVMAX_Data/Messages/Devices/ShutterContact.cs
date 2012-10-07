using System;
using System.Text;

namespace xs1_data_logging
{
	public class ShutterContact : IMAXDevice
	{
		private DeviceTypes _Type;
		private String _RFAddress;
		private String _SerialNumber;
		private String _Name;
		private Room _Room;
		private DateTime lastUpdate;

		private bool lowBattery;
		private bool panelLock;
		private bool gatewayOK;
		private bool error;
		private bool valid;
		private bool isAnswer;
		private bool linkError;
		private ShutterContactModes shutterState;

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("\tDeviceType: "+Type.ToString());
			sb.AppendLine("\tDeviceName: "+Name);
			sb.AppendLine("\tSerialNumber: "+SerialNumber);
			sb.AppendLine("\tRFAddress: "+RFAddress);
			sb.AppendLine("\tLowBattery: "+LowBattery);
			sb.AppendLine("\tisAnwer: "+IsAnswer);
			sb.AppendLine("\tpanelLock: "+PanelLock);
			sb.AppendLine("\tgatewayOK: "+GatewayOK);
			sb.AppendLine("\terror: "+Error);
			sb.AppendLine("\tvalid: "+Valid);
			sb.AppendLine("\tLinkError: "+LinkError);
			sb.AppendLine("\tShutterState: "+ShutterState);

			return sb.ToString();
		}

		public ShutterContact()
		{
			_Type = DeviceTypes.ShutterContact;
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

		#region ShutterContact specific properties

		public ShutterContactModes ShutterState {
			get {
				return shutterState;
			}
			set {
				lastUpdate = DateTime.Now;
				shutterState = value;
			}
		}
		public bool LinkError {
			get {
				return linkError;
			}
			set {
				lastUpdate = DateTime.Now;
				linkError = value;
			}
		}

		public bool IsAnswer {
			get {
				return isAnswer;
			}
			set {
				lastUpdate = DateTime.Now;
				isAnswer = value;
			}
		}

		public bool Valid {
			get {
				return valid;
			}
			set {
				lastUpdate = DateTime.Now;
				valid = value;
			}
		}

		public bool Error {
			get {
				return error;
			}
			set {
				lastUpdate = DateTime.Now;
				error = value;
			}
		}

		public bool GatewayOK {
			get {
				return gatewayOK;
			}
			set {
				lastUpdate = DateTime.Now;
				gatewayOK = value;
			}
		}

		public bool PanelLock {
			get {
				return panelLock;
			}
			set {
				lastUpdate = DateTime.Now;
				panelLock = value;
			}
		}
		public bool LowBattery {
			get {
				return lowBattery;
			}
			set {
				lastUpdate = DateTime.Now;
				lowBattery = value;
			}
		}
		#endregion

	}
}

