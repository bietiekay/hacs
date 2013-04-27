using System;
using System.Collections.Generic;

namespace xs1_data_logging
{
	/// <summary>
	/// this encodes/decodes messages from/to the ELV MAX! Cube. It's just base64.
	/// </summary>
	public class MAXEncodeDecode
	{
        public IMAXMessage ProcessLMessage(String Message, House _House, Dictionary<String, IMAXDevice> _currentHouseDevices)
		{
			if (Message.Length < 2)
				throw new MAXException("Unable to process message: "+Message);

			// check what type of message we got and return the processed values

			if (Message.StartsWith("L:"))
                return new L_Message(Message, _House, _currentHouseDevices);

			//throw new NotImplementedException();
			return null;
		}

		public IMAXMessage ProcessMessage(String Message, House _House)
		{
			if (Message.Length < 2)
				throw new MAXException("Unable to process message: "+Message);
			
			// check what type of message we got and return the processed values
			
			if (Message.StartsWith("M:"))
				return new M_Message(Message, _House);
			
			if (Message.StartsWith("H:"))
				return new H_Message(Message, _House);
			
			//if (Message.StartsWith("C:"))
			//	return new C_Message(Message, _House);

			if (Message.StartsWith("L:"))
				return new L_Message(Message, _House);

			//throw new NotImplementedException();
			return null;
		}

	}
}

