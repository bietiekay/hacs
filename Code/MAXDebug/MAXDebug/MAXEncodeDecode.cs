using System;

namespace MAXDebug
{
	/// <summary>
	/// this encodes/decodes messages from/to the ELV MAX! Cube. It's just base64.
	/// </summary>
	public static class MAXEncodeDecode
	{
	    static public string Encode(string toEncode)
		{
			byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
			string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

	    static public string Decode(string encodedData)
		{
			byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
			string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
			return returnValue;
		}

	}
}

