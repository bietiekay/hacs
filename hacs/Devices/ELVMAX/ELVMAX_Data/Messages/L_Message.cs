using System;
using System.Text;
using System.Collections.Generic;

namespace hacs
{
	// This reponse contains real-time information about the devices.
	public class L_Message : IMAXMessage
	{
		#region Message specific data
		public List<IMAXDevice> DevicesInThisMessage;
		public byte[] RawMessageDecoded;
		#endregion

		#region ToString override
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("L-Message:");
					
//			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
//			sb.AppendLine("ASCII: "+enc.GetString(RawMessageDecoded));
//			sb.Append("RAW: ");
//
//			foreach(byte _b in RawMessageDecoded)
//			{
//				sb.Append(_b);
//				sb.Append(" ");
//			}
			foreach(IMAXDevice _device in DevicesInThisMessage)
			{
				switch(_device.Type)
				{
				case DeviceTypes.HeatingThermostat:
					sb.Append(((HeatingThermostat)_device).ToString());
					break;
				case DeviceTypes.HeatingThermostatPlus:
					sb.Append(((HeatingThermostatPlus)_device).ToString());
					break;
				case DeviceTypes.PushButton:
					sb.Append(((PushButton)_device).ToString());
					break;
				case DeviceTypes.ShutterContact:
					sb.Append(((ShutterContact)_device).ToString());
					break;
				case DeviceTypes.WallMountedThermostat:
					sb.Append(((WallMountedThermostat)_device).ToString());
					break;
				default:
				break;
				}
				sb.AppendLine();
			}

			return sb.ToString();
		}
		#endregion

		#region IMaxData implementation
		public MAXMessageType MessageType 
		{
			get 
			{
				return MAXMessageType.C;
			}
		}
		#endregion

		public L_Message(String RAW_Message, House _theHouse, Dictionary<String,IMAXDevice> OutputDeviceList)
		{
			DevicesInThisMessage = new List<IMAXDevice>();
			
			if (RAW_Message.Length < 2)
				throw new MAXException("Unable to process the RAW Message.");
			
			if (!RAW_Message.StartsWith("L:"))
				throw new MAXException("Unable to process the RAW Message. Not a L Message.");
			
			RawMessageDecoded = Base64.Decode(RAW_Message.Remove(0,2));
			
			// Tokenize RAW Message
			List<byte[]> Tokenized = TokenizeMessage.Tokenize(RawMessageDecoded);
			
			foreach(byte[] array in Tokenized)
			{
				StringBuilder sb = new StringBuilder();
				
				for(int i=0;i<=2;i++)
				{
					sb.Append(array[i]);
				}
				// get data 1 and data 2 out
				// on position 5,6
				byte Data1 = array[4];
				byte Data2 = array[5];
				
				String binValueData1 = Convert.ToString(Data1,2);
				binValueData1 = binValueData1.PadLeft(8, '0');
				String binValueData2 = Convert.ToString(Data2,2);
				binValueData2 = binValueData2.PadLeft(8, '0');
				
				Int32 Cursor = 7;	// the current position, skipping ?1,
				
				String RFAddress = sb.ToString();
				
				#region look for this RF Adress in the House's device list
                List<IMAXDevice> AllDevices = _theHouse.GetAllDevices();
                IMAXDevice foundDevice = null;
				foreach(IMAXDevice _device in AllDevices)
				{
					if (_device.RFAddress == RFAddress)
					{
                        if (_device.Type == DeviceTypes.HeatingThermostat)
                        {
                            foundDevice = new HeatingThermostat();
                            foundDevice.AssociatedRoom = _device.AssociatedRoom;
                            foundDevice.Name = _device.Name;
                            foundDevice.RFAddress = _device.RFAddress;
                            foundDevice.SerialNumber = _device.SerialNumber;
                        }
                        if (_device.Type == DeviceTypes.ShutterContact)
                        {
                            foundDevice = new ShutterContact();
                            foundDevice.AssociatedRoom = _device.AssociatedRoom;
                            foundDevice.Name = _device.Name;
                            foundDevice.RFAddress = _device.RFAddress;
                            foundDevice.SerialNumber = _device.SerialNumber;
                        }

						break;
					}
				}
				#endregion
				
				if (foundDevice != null)
				{
					// remove the device from the house to add it later again...
					DevicesInThisMessage.Add(foundDevice);
					
					#region HeatingThermostat
					if (foundDevice.Type == DeviceTypes.HeatingThermostat)
					{
						HeatingThermostat KnownDevice = (HeatingThermostat)foundDevice;
						
						#region get all those flags out of Data1 and Data2
						
						#region Valid
						if (binValueData1[3] == '1')
							KnownDevice.Valid = true;
						else
							KnownDevice.Valid = false;
						#endregion
						
						#region Error
						if (binValueData1[4] == '1')
							KnownDevice.Error = true;
						else
							KnownDevice.Error = false;
						#endregion
						
						#region IsAnswer
						if (binValueData1[5] == '1')
							KnownDevice.IsAnswer = true;
						else
							KnownDevice.IsAnswer = false;
						#endregion
						
						#region LowBattery
						if (binValueData2[0] == '1')
							KnownDevice.LowBattery = true;
						else
							KnownDevice.LowBattery = false;
						#endregion
						
						#region LinkError
						if (binValueData2[1] == '1')
							KnownDevice.LinkError = true;
						else
							KnownDevice.LinkError = false;
						#endregion
						
						#region PanelLock
						if (binValueData2[2] == '1')
							KnownDevice.PanelLock = true;
						else
							KnownDevice.PanelLock = false;
						#endregion
						
						#region GatewayOK
						if (binValueData2[3] == '1')
							KnownDevice.GatewayOK = true;
						else
							KnownDevice.GatewayOK = false;
						#endregion
						
						#region Mode
						String ModeValue = binValueData2[6]+""+binValueData2[7];
						
						switch(ModeValue)
						{
						case "00":
							KnownDevice.Mode = ThermostatModes.automatic;
							break;
						case "01":
							KnownDevice.Mode = ThermostatModes.manual;
							break;
						case "10":
							KnownDevice.Mode = ThermostatModes.vacation;
							break;
						case "11":
							KnownDevice.Mode = ThermostatModes.boost;
							break;	
						default:
							break;
						}
						#endregion
						
						#endregion
						
						// hurray, we've got a device we know how to handle B-)
						((HeatingThermostat)foundDevice).Temperature = array[Cursor]/2;
						Cursor++;

                        OutputDeviceList.Add(KnownDevice.SerialNumber,KnownDevice);
					}
					#endregion
					
					#region ShutterContact
					if (foundDevice.Type == DeviceTypes.ShutterContact)
					{
						ShutterContact KnownDevice = (ShutterContact)foundDevice;
						
						#region get all those flags out of Data1 and Data2
						
						#region Valid
						if (binValueData1[3] == '1')
							KnownDevice.Valid = true;
						else
							KnownDevice.Valid = false;
						#endregion
						
						#region Error
						if (binValueData1[4] == '1')
							KnownDevice.Error = true;
						else
							KnownDevice.Error = false;
						#endregion
						
						#region IsAnswer
						if (binValueData1[5] == '1')
							KnownDevice.IsAnswer = true;
						else
							KnownDevice.IsAnswer = false;
						#endregion
						
						#region LowBattery
						if (binValueData2[0] == '1')
							KnownDevice.LowBattery = true;
						else
							KnownDevice.LowBattery = false;
						#endregion
						
						#region LinkError
						if (binValueData2[1] == '1')
							KnownDevice.LinkError = true;
						else
							KnownDevice.LinkError = false;
						#endregion
						
						#region PanelLock
						if (binValueData2[2] == '1')
							KnownDevice.PanelLock = true;
						else
							KnownDevice.PanelLock = false;
						#endregion
						
						#region GatewayOK
						if (binValueData2[3] == '1')
							KnownDevice.GatewayOK = true;
						else
							KnownDevice.GatewayOK = false;
						#endregion
						
						#region Mode
						String ModeValue = binValueData2[6]+""+binValueData2[7];
						
						switch(ModeValue)
						{
						case "00":
							KnownDevice.ShutterState = ShutterContactModes.closed;
							break;
						case "10":
							KnownDevice.ShutterState = ShutterContactModes.open;
							break;
						default:
							break;
						}
						#endregion
						
						#endregion

                        OutputDeviceList.Add(KnownDevice.SerialNumber, KnownDevice);
					}
					#endregion
				}
			}
		}

		// initializes this class and processes the given Input Message and fills the Message Fields
		public L_Message (String RAW_Message, House _House)
		{
			DevicesInThisMessage = new List<IMAXDevice>();

			if (RAW_Message.Length < 2)
				throw new MAXException("Unable to process the RAW Message.");

			if (!RAW_Message.StartsWith("L:"))
				throw new MAXException("Unable to process the RAW Message. Not a L Message.");

			RawMessageDecoded = Base64.Decode(RAW_Message.Remove(0,2));

			// Tokenize RAW Message
			List<byte[]> Tokenized = TokenizeMessage.Tokenize(RawMessageDecoded);

			foreach(byte[] array in Tokenized)
			{
				StringBuilder sb = new StringBuilder();

				for(int i=0;i<=2;i++)
				{
					sb.Append(array[i]);
				}
				// get data 1 and data 2 out
				// on position 5,6
				byte Data1 = array[4];
				byte Data2 = array[5];

				String binValueData1 = Convert.ToString(Data1,2);
				binValueData1 = binValueData1.PadLeft(8, '0');
				String binValueData2 = Convert.ToString(Data2,2);
				binValueData2 = binValueData2.PadLeft(8, '0');

				Int32 Cursor = 7;	// the current position, skipping ?1,

				String RFAddress = sb.ToString();

				#region look for this RF Adress in the House's device list
				List<IMAXDevice> AllDevices = _House.GetAllDevices();
				IMAXDevice foundDevice = null;
				foreach(IMAXDevice _device in AllDevices)
				{
					if (_device.RFAddress == RFAddress)
					{
						foundDevice = _device;
						break;
					}
				}
				#endregion

				if (foundDevice != null)
				{
					DevicesInThisMessage.Add(foundDevice);

					#region HeatingThermostat
					if (foundDevice.Type == DeviceTypes.HeatingThermostat)
					{
						HeatingThermostat KnownDevice = (HeatingThermostat)foundDevice;
		
						#region get all those flags out of Data1 and Data2

						#region Valid
						if (binValueData1[3] == '1')
							KnownDevice.Valid = true;
						else
							KnownDevice.Valid = false;
						#endregion

						#region Error
						if (binValueData1[4] == '1')
							KnownDevice.Error = true;
						else
							KnownDevice.Error = false;
						#endregion

						#region IsAnswer
						if (binValueData1[5] == '1')
							KnownDevice.IsAnswer = true;
						else
							KnownDevice.IsAnswer = false;
						#endregion

						#region LowBattery
						if (binValueData2[0] == '1')
							KnownDevice.LowBattery = true;
						else
							KnownDevice.LowBattery = false;
						#endregion

						#region LinkError
						if (binValueData2[1] == '1')
							KnownDevice.LinkError = true;
						else
							KnownDevice.LinkError = false;
						#endregion

						#region PanelLock
						if (binValueData2[2] == '1')
							KnownDevice.PanelLock = true;
						else
							KnownDevice.PanelLock = false;
						#endregion

						#region GatewayOK
						if (binValueData2[3] == '1')
							KnownDevice.GatewayOK = true;
						else
							KnownDevice.GatewayOK = false;
						#endregion

						#region Mode
						String ModeValue = binValueData2[6]+""+binValueData2[7];

						switch(ModeValue)
						{
							case "00":
								KnownDevice.Mode = ThermostatModes.automatic;
							break;
							case "01":
								KnownDevice.Mode = ThermostatModes.manual;
							break;
							case "10":
								KnownDevice.Mode = ThermostatModes.vacation;
							break;
							case "11":
								KnownDevice.Mode = ThermostatModes.boost;
							break;	
							default:
							break;
						}
						#endregion

						#endregion

						// hurray, we've got a device we know how to handle B-)
						((HeatingThermostat)foundDevice).Temperature = array[Cursor]/2;
						Cursor++;
					}
					#endregion

					#region ShutterContact
					if (foundDevice.Type == DeviceTypes.ShutterContact)
					{
						ShutterContact KnownDevice = (ShutterContact)foundDevice;
		
						#region get all those flags out of Data1 and Data2

						#region Valid
						if (binValueData1[3] == '1')
							KnownDevice.Valid = true;
						else
							KnownDevice.Valid = false;
						#endregion

						#region Error
						if (binValueData1[4] == '1')
							KnownDevice.Error = true;
						else
							KnownDevice.Error = false;
						#endregion

						#region IsAnswer
						if (binValueData1[5] == '1')
							KnownDevice.IsAnswer = true;
						else
							KnownDevice.IsAnswer = false;
						#endregion

						#region LowBattery
						if (binValueData2[0] == '1')
							KnownDevice.LowBattery = true;
						else
							KnownDevice.LowBattery = false;
						#endregion

						#region LinkError
						if (binValueData2[1] == '1')
							KnownDevice.LinkError = true;
						else
							KnownDevice.LinkError = false;
						#endregion

						#region PanelLock
						if (binValueData2[2] == '1')
							KnownDevice.PanelLock = true;
						else
							KnownDevice.PanelLock = false;
						#endregion

						#region GatewayOK
						if (binValueData2[3] == '1')
							KnownDevice.GatewayOK = true;
						else
							KnownDevice.GatewayOK = false;
						#endregion

						#region Mode
						String ModeValue = binValueData2[6]+""+binValueData2[7];

						switch(ModeValue)
						{
							case "00":
								KnownDevice.ShutterState = ShutterContactModes.closed;
							break;
							case "10":
								KnownDevice.ShutterState = ShutterContactModes.open;
							break;
							default:
							break;
						}
						#endregion

						#endregion

					}
					#endregion
				}
			}
		}
	}
}

