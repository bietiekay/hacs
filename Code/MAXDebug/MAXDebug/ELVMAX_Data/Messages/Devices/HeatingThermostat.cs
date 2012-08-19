using System;
using System.Text;

namespace MAXDebug
{
	public class HeatingThermostat : IMAXDevice
	{
		private DeviceTypes _Type;
		private String _RFAddress;
		private String _SerialNumber;
		private String _Name;
		private Room _Room;

		private double temperature;
		private DateTime validUntil;
		private bool lowBattery;
		private bool panelLock;
		private bool gatewayOK;
		private bool error;
		private bool valid;
		private bool isAnswer;
		private bool linkError;
		private ThermostatModes mode;

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("\tDeviceType: "+Type.ToString());
			sb.AppendLine("\tDeviceName: "+Name);
			sb.AppendLine("\tSerialNumber: "+SerialNumber);
			sb.AppendLine("\tRFAddress: "+RFAddress);
			sb.AppendLine("\tTemperature: "+Temperature);
			sb.AppendLine("\tLowBattery: "+LowBattery);
			sb.AppendLine("\tisAnwer: "+IsAnswer);
			sb.AppendLine("\tpanelLock: "+PanelLock);
			sb.AppendLine("\tgatewayOK: "+GatewayOK);
			sb.AppendLine("\terror: "+Error);
			sb.AppendLine("\tvalid: "+Valid);
			sb.AppendLine("\tLinkError: "+LinkError);
			sb.AppendLine("\tMode: "+Mode);

			return sb.ToString();
		}

		public HeatingThermostat(Room _ARoom)
		{
			_Type = DeviceTypes.HeatingThermostat;
			_Room = _ARoom;
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
				_RFAddress = value;
			}
		}

		public string SerialNumber {
			get {
				return _SerialNumber;
			}
			set {
				_SerialNumber = value;
			}
		}

		public string Name {
			get {
				return _Name;
			}
			set {
				_Name = value;
			}
		}

		public Room AssociatedRoom
 		{
			get {
				return _Room;
			}
		}

		#endregion

		#region HeatingThermostat specific properties
		public ThermostatModes Mode {
			get {
				return mode;
			}
			set {
				mode = value;
			}
		}

		public bool LinkError {
			get {
				return linkError;
			}
			set {
				linkError = value;
			}
		}

		public bool IsAnswer {
			get {
				return isAnswer;
			}
			set {
				isAnswer = value;
			}
		}

		public bool Valid {
			get {
				return valid;
			}
			set {
				valid = value;
			}
		}

		public bool Error {
			get {
				return error;
			}
			set {
				error = value;
			}
		}

		public bool GatewayOK {
			get {
				return gatewayOK;
			}
			set {
				gatewayOK = value;
			}
		}

		public bool PanelLock {
			get {
				return panelLock;
			}
			set {
				panelLock = value;
			}
		}
		public double Temperature {
			get {
				return temperature;
			}
			set {
				temperature = value;
			}
		}

		public bool LowBattery {
			get {
				return lowBattery;
			}
			set {
				lowBattery = value;
			}
		}
		public DateTime ValidUntil {
			get {
				return validUntil;
			}
			set {
				validUntil = value;
			}
		}
		#endregion
	}
}

