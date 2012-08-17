using System;
using System.Text;

namespace MAXDebug
{
	// The H response contains information about the Cube.
	public class H_Message : IMaxData
	{
		#region Message specific data
		public String MAXserialNumber;
		public Int32 RFAdress;
		public Int32 FirmwareVersion;
		#endregion

		#region ToString override
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("H-Message:");
			sb.AppendLine("Serial Number: "+MAXserialNumber);
			sb.AppendLine("RF Adress: "+RFAdress);
			sb.Append("Firmware Version: "+FirmwareVersion);

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
		public H_Message (String RAW_Message)
		{
			if (RAW_Message.Length < 2)
				throw new MAXException("Unable to process the RAW Message.");

			if (!RAW_Message.StartsWith("H:"))
				throw new MAXException("Unable to process the RAW Message. Not a H Message.");

			String[] SplittedRAWMessage = RAW_Message.Remove(0,2).Split(new char[1] { ',' });

			if (SplittedRAWMessage.Length >= 3)
			{
				MAXserialNumber = SplittedRAWMessage[0];
				RFAdress = Int32.Parse(SplittedRAWMessage[1],System.Globalization.NumberStyles.HexNumber);
				FirmwareVersion = Int32.Parse(SplittedRAWMessage[2],System.Globalization.NumberStyles.HexNumber);
			}
			else
				throw new MAXException("Unable to process H Message. Not enough content.");

		}
	}
}

