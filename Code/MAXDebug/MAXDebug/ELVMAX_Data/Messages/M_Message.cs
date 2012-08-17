using System;
using System.Text;

namespace MAXDebug
{
	// The M response contains information about additional data, like the Rooms that are defined, 
	// the Devices and the names they were given, and how the rooms and Devices are linked to each other.
	public class M_Message : IMaxData
	{
		#region Message specific data
		public Int32 Index;
		public Int32 Count;
		public byte[] RawMessageDecoded;
		#endregion

		#region ToString override
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("M-Message:");

			sb.AppendLine("Index: "+Index);
			sb.AppendLine("Count: "+Count);

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
				return MAXMessageType.M;
			}
		}
		#endregion

		// initializes this class and processes the given Input Message and fills the Message Fields
		public M_Message (String RAW_Message)
		{
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
			}
			else
				throw new MAXException("Unable to process M Message. Not enough content.");

		}
	}
}

