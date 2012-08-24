using System;

namespace MAXDebug
{
	public class HeatingThermostatDiff : IDeviceDiffSet
	{
		private DeviceTypes _DeviceType;
		private String _DeviceName;
		private Int32 _RoomID;
		private String _RoomName;

		#region device specific diff information
		private ThermostatModes mode;
		private Double temperature;
		private BatteryStatus lowBattery;
		#endregion

		public HeatingThermostatDiff (String Name, Int32 Room_ID, String Room_Name)
		{
			_DeviceType = DeviceTypes.HeatingThermostat;
			_DeviceName = Name;
			_RoomID = Room_ID;
			_RoomName = Room_Name;
			lowBattery = BatteryStatus.unchanged;
			temperature = Double.NaN;
			mode = ThermostatModes.unchanged;
		}

		#region IDeviceDiffSet implementation
		public DeviceTypes DeviceType {
			get {
				return _DeviceType;
			}
		}

		public string DeviceName {
			get {
				return _DeviceName;
			}
		}

		public int RoomID {
			get {
				return _RoomID;
			}
		}

		public string RoomName {
			get {
				return _RoomName;
			}
		}
		#endregion		

		#region device specific diff properties
		public ThermostatModes Mode {
			get {
				return mode;
			}
			set {
				mode = value;
			}
		}

		public Double Temperature {
			get {
				return temperature;
			}
			set {
				temperature = value;
			}
		}

		public BatteryStatus LowBattery {
			get {
				return lowBattery;
			}
			set {
				lowBattery = value;
			}
		}
		#endregion
	}
}

