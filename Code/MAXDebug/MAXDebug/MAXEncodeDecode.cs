using System;

namespace MAXDebug
{
	public class ELVMAX_Metadata
	{
		public String CubeSerial;
		public String RFAdress;
		public Int32 FirmwareVersion;
	}

	public class ELVMAXData
	{
		public ELVMAX_Metadata Metadata;

	}

	/// <summary>
	/// this encodes/decodes messages from/to the ELV MAX! Cube. It's just base64.
	/// </summary>
	public static class MAXEncodeDecode
	{
	    static public string Encode(byte[] toEncode)
		{
			return Convert.ToBase64String(toEncode);
		}

	    static public byte[] Decode(string encodedData)
		{
			return System.Convert.FromBase64String(encodedData);
		}

		/// <summary>
		/// Decodes the message(s) and adds the extracted information to the overall ELVMAX Data package
		/// </summary>
		/// <param name='Data'>
		/// Data.
		/// </param>
		static public void DecodeMessage(ELVMAXData Data)
		{
			return;
		}
	}
}

