using System;

namespace MAXDebug
{
	public enum ELVMessageType
	{
		undefined,
		H,
		M,
		C,
		L
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

		static public ELVMessageType WhichMessageType(String Message)
		{
			if (Message.StartsWith("H:"))
				return ELVMessageType.H;

			if (Message.StartsWith("M:"))
				return ELVMessageType.M;

			if (Message.StartsWith("C:"))
				return ELVMessageType.C;

			if (Message.StartsWith("L:"))
				return ELVMessageType.L;


			return ELVMessageType.undefined;
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

