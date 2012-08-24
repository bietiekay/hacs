using System;
using System.Collections.Generic;

namespace MAXDebug
{
	public static class DiffHouse
	{
		/// <summary>
		/// This method calculates the difference between house1 and house2 - whereas house1 is the old house2 the new one
		/// </summary>
		public static List<IDeviceDiffSet> CalculateDifferences(Dictionary<String,IMAXDevice> DevicesHouse1, Dictionary<String,IMAXDevice> DevicesHouse2)
		{
			List<IDeviceDiffSet> Differences = new List<IDeviceDiffSet>();

			foreach(KeyValuePair<String,IMAXDevice> House1KVPair in DevicesHouse1)
			{
				// now we have a device from house 1 - we need to get that same device in house 2
				if (DevicesHouse2.ContainsKey(House1KVPair.Key))
				{
					// we got it
					IMAXDevice House2Device = DevicesHouse2[House1KVPair.Key];

					if (House1KVPair.Value.Type == DeviceTypes.HeatingThermostat)
					{
						// HeatingThermostat
						HeatingThermostatDiff Diff = null;

						HeatingThermostat Device1 = (HeatingThermostat)House1KVPair.Value;
						HeatingThermostat Device2 = (HeatingThermostat)House2Device;

						if (Device1.LowBattery != Device2.LowBattery)
						{
							if (Diff == null)
								Diff = new HeatingThermostatDiff(Device2.Name,Device2.AssociatedRoom.RoomID,Device2.AssociatedRoom.RoomName);

							if (Device2.LowBattery)
								Diff.LowBattery = BatteryStatus.lowbattery;
							else
								Diff.LowBattery = BatteryStatus.ok;
						}

						if (Device1.Mode != Device2.Mode)
						{
							if (Diff == null)
								Diff = new HeatingThermostatDiff(Device2.Name,Device2.AssociatedRoom.RoomID,Device2.AssociatedRoom.RoomName);

							Diff.Mode = Device2.Mode;
						}

						if (Device1.Temperature != Device2.Temperature)
						{
							if (Diff == null)
								Diff = new HeatingThermostatDiff(Device2.Name,Device2.AssociatedRoom.RoomID,Device2.AssociatedRoom.RoomName);

							Diff.Temperature = Device2.Temperature;
						}

						if (Diff != null)
						{
							Differences.Add(Diff);
						}
					}
					else
					if (House1KVPair.Value.Type == DeviceTypes.ShutterContact)
					{
						// ShutterContact
						ShutterContactDiff Diff = null;

						ShutterContact Device1 = (ShutterContact)House1KVPair.Value;
						ShutterContact Device2 = (ShutterContact)House2Device;

						if (Device1.LowBattery != Device2.LowBattery)
						{
							if (Diff == null)
								Diff = new ShutterContactDiff(Device2.Name,Device2.AssociatedRoom.RoomID,Device2.AssociatedRoom.RoomName);

							if (Device2.LowBattery)
								Diff.LowBattery = BatteryStatus.lowbattery;
							else
								Diff.LowBattery = BatteryStatus.ok;

						}

						if (Device1.ShutterState != Device2.ShutterState)
						{
							if (Diff == null)
								Diff = new ShutterContactDiff(Device2.Name,Device2.AssociatedRoom.RoomID,Device2.AssociatedRoom.RoomName);

							Diff.ShutterState = Device2.ShutterState;
						}

						if (Diff != null)
						{
							Differences.Add(Diff);
						}

					}


				}
			}

			return Differences;
		}
	}
}

