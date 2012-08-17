using System;

namespace MAXDebug
{
	/// <summary>
	/// this encodes/decodes messages from/to the ELV MAX! Cube. It's just base64.
	/// </summary>
	public class MAXEncodeDecode
	{
		public IMaxData ProcessMessage(String Message)
		{
			if (Message.Length < 2)
				throw new MAXException("Unable to process message: "+Message);

			// check what type of message we got and return the processed values

			if (Message.StartsWith("M:"))
				return new M_Message(Message);

			if (Message.StartsWith("H:"))
				return new H_Message(Message);

			if (Message.StartsWith("C:"))
				return new C_Message(Message);

			if (Message.StartsWith("L:"))
				return new L_Message(Message);

			//throw new NotImplementedException();
			return null;
		}
	}
}

