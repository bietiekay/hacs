using System;
using sones.Storage.Serializer;

namespace xs1_data_logging
{
	public class HeatingThermostatDiff : IDeviceDiffSet, IFastSerialize
	{
		private DeviceTypes _DeviceType;
		private String _DeviceName;
		private Int32 _RoomID;
		private String _RoomName;
		private DateTime _Creation;

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
			_Creation = DateTime.Now;
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

		#region overrides
		public override string ToString ()
		{
			return RoomID+" - "+DeviceType+" - "+RoomName+"-"+DeviceName+" - "+lowBattery+" - "+Temperature;
			//return "\t"+lowBattery+"\t"+mode+"\t"+temperature;
			//return string.Format ("[HeatingThermostatDiff: DeviceType={0}, DeviceName={1}, RoomID={2}, RoomName={3}, Mode={4}, Temperature={5}, LowBattery={6}]", DeviceType, DeviceName, RoomID, RoomName, Mode, Temperature, LowBattery);
		}
		#endregion

		#region IFastSerialize implementation
		byte[] IFastSerialize.Serialize ()
		{
			throw new NotImplementedException ();
		}
		void IFastSerialize.Deserialize (byte[] Data)
		{
			throw new NotImplementedException ();
		}
		#endregion

		#region IAlarmingEvent implementation
		
		public AlarmingEventType AlarmingType ()
		{
			return AlarmingEventType.ELVMAXEvent;
		}
		
		public string AlarmingName ()
		{
			return _DeviceName;
		}
		
		public DateTime AlarmingCreated ()
		{
			return _Creation;
		}
		
		#endregion

	}
}

