using System;

namespace hacs
{
	public class ShutterContactDiff : IDeviceDiffSet
	{
		private DeviceTypes _DeviceType;
		private String _DeviceName;
		private Int32 _RoomID;
		private String _RoomName;
		private DateTime _Creation;

		#region device specific diff information
		private ShutterContactModes state;
		private BatteryStatus lowBattery;
		#endregion

		public ShutterContactDiff (String Name, Int32 Room_ID, String Room_Name)
		{
			_DeviceType = DeviceTypes.ShutterContact;
			_DeviceName = Name;
			_RoomID = Room_ID;
			_RoomName = Room_Name;
			lowBattery = BatteryStatus.unchanged;
			state = ShutterContactModes.unchanged;
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

		public ShutterContactModes ShutterState {
			get {
				return state;
			}
			set {
				state = value;
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
	
		public override string ToString ()
		{
			return RoomID+" - "+DeviceType+" - "+RoomName+"-"+DeviceName+" - "+lowBattery+" - "+ShutterState;
		}		

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

