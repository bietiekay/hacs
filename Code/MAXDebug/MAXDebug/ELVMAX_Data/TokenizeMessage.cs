using System;
using System.Collections.Generic;

namespace MAXDebug
{
	public static class TokenizeMessage
	{
		#region TokenizeLMessage
		/// this method extracts all the tokens out of one L message. This is a
		/// fairly simple process: get the first byte, out of that we get the number
		/// of bytes that belong to that token, read those bytes. If there are bytes
		/// left in the L message do it all over again.
		/// Example:
		/// 
		/// 6 0 37 41 9 18 16 11 2 115 104 71 18 24 100 42 0 0 0
		/// 
		/// Tokenized:
		/// 
		/// Token 1 (Length 6): 0 37 41 9 18 16
		/// Token 2 (Length 11): 2 115 104 71 18 24 100 42 0 0 0
		public static List<byte[]> Tokenize(byte[] DecodedRAWMessage)
		{
			Int32 Cursor = 0;	// this is the cursor that wanders through the message
			List<byte[]> ReturnValues = new List<byte[]>();

			while(Cursor != DecodedRAWMessage.Length)
			{
				// get first byte
				Byte NumberOfBytes = DecodedRAWMessage[Cursor];
				Cursor++;
				byte[] Token = new byte[NumberOfBytes];

				// now read exactly NumberOfBytes from DecodedRAWMessage
				for(Byte i=0;i<=NumberOfBytes-1;i++)
				{
					Token[i] = DecodedRAWMessage[Cursor];
					Cursor++;
				}
				ReturnValues.Add(Token);
			}		

			return ReturnValues;
		}
		#endregion
	}
}

