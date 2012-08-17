using System;

namespace MAXDebug
{
	public static class Base64
	{
		#region Base64 encoding / decoding
	    public static string Encode(byte[] toEncode)
		{
			return Convert.ToBase64String(toEncode);
		}

	    public static byte[] Decode(string encodedData)
		{
			return System.Convert.FromBase64String(encodedData);
		}
		#endregion	
	}
}

