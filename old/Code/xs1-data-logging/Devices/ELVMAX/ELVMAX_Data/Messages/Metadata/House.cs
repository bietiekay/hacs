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
				foreach(IMAXDevice _Device in _room.Devices.Values)
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
				foreach(IMAXDevice _Device in _room.Devices.Values)
				{
					Devices.Add(_Device.SerialNumber,_Device);
				}
			}

			return Devices;
		}

		public bool UpdateDevices(Dictionary<string, IMAXDevice> _devices)
		{
			foreach(Room _room in Rooms)
			{
                foreach(IMAXDevice _device in _devices.Values)
                {
                    if (_room.Devices.ContainsKey(_device.SerialNumber))
                    {
                        _room.Devices.Remove(_device.SerialNumber);
                        _device.AssociatedRoom = _room;
                        _room.Devices.Add(_device.SerialNumber,_device);
                    }
			    }
            }
			
			return false;
		}
	}
}

