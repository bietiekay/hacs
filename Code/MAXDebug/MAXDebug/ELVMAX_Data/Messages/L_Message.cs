using System;
using System.Text;
using System.Collections.Generic;

namespace MAXDebug
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

				String binValueData1 = Convert.ToString(Int32.Parse(Data1.ToString(),System.Globalization.NumberStyles.HexNumber),2);
				binValueData1 = binValueData1.PadLeft(8, '0');
				String binValueData2 = Convert.ToString(Int32.Parse(Data2.ToString(),System.Globalization.NumberStyles.HexNumber),2);
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

					if (foundDevice.Type == DeviceTypes.HeatingThermostat)
					{
						HeatingThermostat KnownDevice = (HeatingThermostat)foundDevice;
						// hurray, we've got a device we know how to handle B-)
						((HeatingThermostat)foundDevice).Temperature = array[Cursor]/2;
						Cursor++;
					}
				}
			}
		}
	}
}

