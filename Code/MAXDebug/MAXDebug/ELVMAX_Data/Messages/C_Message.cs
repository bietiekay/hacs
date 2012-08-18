using System;
using System.Text;

namespace MAXDebug
{
	// The C response contains information about the configuration of a device.
	public class C_Message : IMaxData
	{
		#region Message specific data
		public Int32 RFAdress;
		public byte[] RawMessageDecoded;
		#endregion

		#region ToString override
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("C-Message:");

			sb.AppendLine("RF Address: "+RFAdress);

			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
			sb.AppendLine("ASCII: "+enc.GetString(RawMessageDecoded));
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
		public C_Message (String RAW_Message)
		{
			if (RAW_Message.Length < 2)
				throw new MAXException("Unable to process the RAW Message.");

			if (!RAW_Message.StartsWith("C:"))
				throw new MAXException("Unable to process the RAW Message. Not a C Message.");

			String[] SplittedRAWMessage = RAW_Message.Remove(0,2).Split(new char[1] { ',' });

			if (SplittedRAWMessage.Length >= 2)
			{
				RFAdress = Int32.Parse(SplittedRAWMessage[0],System.Globalization.NumberStyles.HexNumber);
				RawMessageDecoded = Base64.Decode(SplittedRAWMessage[1]);
			}
			else
				throw new MAXException("Unable to process C Message. Not enough content.");

		}
	}
}

