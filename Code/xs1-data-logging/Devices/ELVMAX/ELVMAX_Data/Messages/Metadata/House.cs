using System;
using System.Collections.Generic;

namespace xs1_data_logging
{
	public class House
	{
		public String Name;
		public List<Room> Rooms;
		public H_Message CubeInformation;

		public House ()
		{
			Rooms = new List<Room>();
		}

		/// <summary>
		/// Gets all devices.
		/// </summary>
		/// <returns>
		/// The all devices.
		/// </returns>
		public List<IMAXDevice> GetAllDevices()
		{
			List<IMAXDevice> Devices = new List<IMAXDevice>();
			foreach(Room _room in Rooms)
			{
				foreach(IMAXDevice _Device in _room.Devices)
				{
					Devices.Add(_Device);
				}
			}

			return Devices;
		}

		public Dictionary<String,IMAXDevice> GetAllDevicesInADictionary()
		{
			Dictionary<String,IMAXDevice> Devices = new Dictionary<string, IMAXDevice>();
			foreach(Room _room in Rooms)
			{
				foreach(IMAXDevice _Device in _room.Devices)
				{
					Devices.Add(_Device.SerialNumber,_Device);
				}
			}

			return Devices;
		}

		public bool RemoveDevice(IMAXDevice _device)
		{
			foreach(Room _room in Rooms)
			{
				IMAXDevice _removedevice = null;
				foreach(IMAXDevice _roomdevice in _room.Devices)
				{
					if (_roomdevice.RFAddress == _device.RFAddress)
					{
						_removedevice = _roomdevice;
						break;
					}
				}
				if (_removedevice != null)
				{
					_room.Devices.Remove(_removedevice);
					return true;
				}
			}
			
			return false;
		}
	}
}

