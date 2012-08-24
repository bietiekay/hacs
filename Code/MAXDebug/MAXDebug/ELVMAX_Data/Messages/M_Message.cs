using System;
using System.Text;
using System.Collections.Generic;

namespace MAXDebug
{
	// The M response contains information about additional data, like the Rooms that are defined, 
	// the Devices and the names they were given, and how the rooms and Devices are linked to each other.
	public class M_Message : IMAXMessage
	{
		#region Message specific data
		public Int32 Index;
		public Int32 Count;
		public byte[] RawMessageDecoded;
		private House thisHouse;
		#endregion

		#region ToString override
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			foreach(Room _room in thisHouse.Rooms)
			{
				sb.Append(_room.ToString());
			}

			return sb.ToString();
		}
		#endregion

		#region IMaxData implementation
		public MAXMessageType MessageType 
		{
			get 
			{
				return MAXMessageType.M;
			}
		}
		#endregion

		// initializes this class and processes the given Input Message and fills the Message Fields
		public M_Message (String RAW_Message, House _House)
		{
			thisHouse = _House;
			if (RAW_Message.Length < 2)
				throw new MAXException("Unable to process the RAW Message.");

			if (!RAW_Message.StartsWith("M:"))
				throw new MAXException("Unable to process the RAW Message. Not a M Message.");

			String[] SplittedRAWMessage = RAW_Message.Remove(0,2).Split(new char[1] { ',' });

			if (SplittedRAWMessage.Length >= 3)
			{
				Index = Int32.Parse(SplittedRAWMessage[0],System.Globalization.NumberStyles.HexNumber);
				Count = Int32.Parse(SplittedRAWMessage[1],System.Globalization.NumberStyles.HexNumber);
				RawMessageDecoded = Base64.Decode(SplittedRAWMessage[2]);

//				System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
//				Console.WriteLine(enc.GetString (RawMessageDecoded));
//				StringBuilder hexlist = new StringBuilder();
//
//				foreach(Byte _byte in RawMessageDecoded)
//				{
//					hexlist.Append(_byte.ToString()+" ");
//				}

				Int32 Cursor = 2;

				// now go deeper
				Byte RoomCount = RawMessageDecoded[Cursor];
				Cursor++;

				#region Rooms
				// go through every room
				for(byte roomnumber=1;roomnumber<=RoomCount;roomnumber++)
				{
					Room newRoom = new Room(thisHouse);

					newRoom.RoomID = RawMessageDecoded[Cursor];
					Cursor++;

					Byte RoomNameLength = RawMessageDecoded[Cursor];
					Cursor++;

					byte[] RoomName = new byte[RoomNameLength];

					for(Byte j=0;j<=RoomNameLength-1;j++)
					{
						//RoomName.Append((char)RawMessageDecoded[Cursor]);
						RoomName[j] = RawMessageDecoded[Cursor];
						Cursor++;
					}
					newRoom.RoomName = System.Text.Encoding.UTF8.GetString(RoomName);

					StringBuilder RFAddress_Buffer = new StringBuilder();
					for(Byte j=0;j<=3-1;j++)
					{
						RFAddress_Buffer.Append(RawMessageDecoded[Cursor]);
						Cursor++;
					}
					newRoom.RFAddress = RFAddress_Buffer.ToString();//Int32.Parse(RFAddress_Buffer.ToString(),System.Globalization.NumberStyles.HexNumber);

					_House.Rooms.Add(newRoom);
				}

				#region Devices
				//newRoom.RFAddress = Int32.Parse(RFAddress.ToString(),System.Globalization.NumberStyles.HexNumber);
				Byte DeviceCount = RawMessageDecoded[Cursor];
				Cursor++;
				// go through all the devices in here
				for(byte devicenumber=1;devicenumber<=DeviceCount;devicenumber++)
				{
					// read in the device
					IMAXDevice newDevice = new UnknownDevice();

					#region Determine DeviceType
					Byte DevType = RawMessageDecoded[Cursor];
					Cursor++;
					
					switch(DevType)
					{
						case 1: 
							newDevice = new HeatingThermostat();
					        break;
					    case 2:
					        newDevice = new HeatingThermostatPlus();
					        break;
					    case 3:
					        newDevice = new WallMountedThermostat();
					        break;
					    case 4:
					        newDevice = new ShutterContact();
					        break;
					    case 5:
					        newDevice = new PushButton();
					        break;
					    default:
					        break;
					}
					#endregion

					StringBuilder DeviceRFAddress = new StringBuilder();
					for(Byte j=0;j<=3-1;j++)
					{
						DeviceRFAddress.Append(RawMessageDecoded[Cursor]);
						Cursor++;
					}
					newDevice.RFAddress = DeviceRFAddress.ToString();//Int32.Parse(DeviceRFAddress.ToString(),System.Globalization.NumberStyles.HexNumber);

					StringBuilder DeviceSerialNumber = new StringBuilder();
					for(Byte j=0;j<=10-1;j++)
					{
						DeviceSerialNumber.Append((char)RawMessageDecoded[Cursor]);
						Cursor++;
					}
					newDevice.SerialNumber = DeviceSerialNumber.ToString();

					Byte DeviceNameLength = RawMessageDecoded[Cursor];
					Cursor++;

					byte[] DeviceName = new byte[DeviceNameLength];

					for(Byte j=0;j<=DeviceNameLength-1;j++)
					{
						DeviceName[j] = RawMessageDecoded[Cursor];
						Cursor++;
					}
					newDevice.Name = System.Text.Encoding.UTF8.GetString(DeviceName);

					Byte RoomID = RawMessageDecoded[Cursor];
					Cursor++;

					// add the device to the room
					foreach(Room newRoom in _House.Rooms)
					{
						if (newRoom.RoomID == RoomID)
						{
							newDevice.AssociatedRoom = newRoom;
							newRoom.Devices.Add(newDevice);
							break;
						}
					}
				}
				#endregion
				// add this Room to the M_Message-Structure
				#endregion
			}
			else
				throw new MAXException("Unable to process M Message. Not enough content.");

		}
	}
}

