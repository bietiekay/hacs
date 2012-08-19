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
			sb.Append("RAW: ");

			foreach(byte _b in RawMessageDecoded)
			{
				sb.Append(_b);
				sb.Append(" ");
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
			if (RAW_Message.Length < 2)
				throw new MAXException("Unable to process the RAW Message.");

			if (!RAW_Message.StartsWith("L:"))
				throw new MAXException("Unable to process the RAW Message. Not a L Message.");

			RawMessageDecoded = Base64.Decode(RAW_Message.Remove(0,2));

			// Tokenize RAW Message
			List<byte[]> Tokenized = TokenizeMessage.Tokenize(RawMessageDecoded);

			foreach(byte[] array in Tokenized)
			{
				byte Length = array[0];

				StringBuilder sb = new StringBuilder();

				for(int i=0;i<=2;i++)
				{
					sb.Append(array[i]);
				}

				String RFAddress = sb.ToString();

				// look for this RF Adress in the House's device list
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

				if (foundDevice != null)
				{
					// we've found a device, now fill it with happiness


				}
				else
				{
					// unknown device / RF Address
				}

			}


		}
	}
}

