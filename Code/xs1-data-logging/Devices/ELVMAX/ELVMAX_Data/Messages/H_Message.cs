using System;
using System.Text;

namespace xs1_data_logging
{
	// The H response contains information about the Cube.
	public class H_Message : IMAXMessage
	{
		#region Message specific data
		public String MAXserialNumber;
		public String RFAddress;
		public Int32 FirmwareVersion;
		public String HTTPConnId;
		public DateTime CubeDateTime;
		#endregion

		#region ToString override
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("H-Message:");
			sb.AppendLine("Serial Number: "+MAXserialNumber);
			sb.AppendLine("RF Address: "+RFAddress);
			sb.AppendLine("Firmware Version: "+FirmwareVersion);
			sb.AppendLine("HTTPConnId: "+HTTPConnId);
			sb.AppendLine("DateTime: "+CubeDateTime.ToLongDateString()+" "+CubeDateTime.ToLongTimeString());
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

		#region Decode Date and Time
		public DateTime DecodeDateTime(String CubeDate, String CubeTime)
		{
			/// Example:
			/// CubeDate: 0c0812
			/// CubeTime: 1505

			Int32 Day = Int32.Parse(CubeDate.Substring (4,2),System.Globalization.NumberStyles.HexNumber);
			Int32 Month = Int32.Parse(CubeDate.Substring(2,2),System.Globalization.NumberStyles.HexNumber);
			Int32 Year = Int32.Parse(CubeDate.Substring(0,2),System.Globalization.NumberStyles.HexNumber)+2000;

			Int32 Hour = Int32.Parse(CubeTime.Substring(0,2),System.Globalization.NumberStyles.HexNumber);
			Int32 Minute = Int32.Parse(CubeTime.Substring(2,2),System.Globalization.NumberStyles.HexNumber);

			DateTime time = new DateTime(Year,Month,Day,Hour,Minute,0);

			return time;
		}
		#endregion

		// initializes this class and processdes the given Input Message and fills the Message Fields
		public H_Message (String RAW_Message, House _House)
		{
			if (RAW_Message.Length < 2)
				throw new MAXException("Unable to process the RAW Message.");

			if (!RAW_Message.StartsWith("H:"))
				throw new MAXException("Unable to process the RAW Message. Not a H Message.");

			String[] SplittedRAWMessage = RAW_Message.Remove(0,2).Split(new char[1] { ',' });

			if (SplittedRAWMessage.Length >= 3)
			{
				MAXserialNumber = SplittedRAWMessage[0];
				RFAddress = SplittedRAWMessage[1];//Int32.Parse(SplittedRAWMessage[1],System.Globalization.NumberStyles.HexNumber);
				FirmwareVersion = Int32.Parse(SplittedRAWMessage[2],System.Globalization.NumberStyles.HexNumber);
				HTTPConnId = SplittedRAWMessage[4];
				CubeDateTime = DecodeDateTime(SplittedRAWMessage[7],SplittedRAWMessage[8]);

				_House.CubeInformation = this;
			}
			else
				throw new MAXException("Unable to process H Message. Not enough content.");

		}
	}
}

