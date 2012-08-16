using System;
using System.Net.Sockets;   // TCP-streaming
using System.Threading;     // the sleeping part...
using System.Text;


namespace MAXDebug
{
	class MainClass
	{
		private static bool keepRunning = true;

		public static void Main (string[] args)
		{
			ConsoleOutputLogger.verbose = true;
			ConsoleOutputLogger.writeLogfile = true;

			Console.WriteLine ("ELV MAX! Debug Tool version 1 (C) Daniel Kirstenpfad 2012");

			// not enough paramteres given, display help
			if (args.Length < 2)
			{
				Console.WriteLine("Syntax:");
				Console.WriteLine();
				Console.WriteLine("\tmaxdebug <hostname/ip> <port (e.g. 62910)> [commands]");
				Console.WriteLine();
				return;
			}

			// we obviously have enough paramteres, go on and try to connect
			TcpClient client = new TcpClient();
			client.Connect(args[0], Convert.ToInt32 (args[1]));
			NetworkStream stream = client.GetStream();
			// the read buffer (chosen quite big)
			byte[] myReadBuffer = new byte[4096*8];

			// to build the complete message
			StringBuilder myCompleteMessage = new StringBuilder();
			int numberOfBytesRead = 0;

			// Incoming message may be larger than the buffer size.
			do
			{
				myCompleteMessage = new StringBuilder();
				stream.ReadTimeout = 1000;
				try
				{
				numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
				myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

				Console.Write("RAW: "+myCompleteMessage);
				}
				catch(Exception)
				{
					keepRunning = false;
				}
				// sleep 100 msecs
				Thread.Sleep (100);
			}
			while(keepRunning);

			byte[] output = MAXEncodeDecode.Decode("VgIBAQpIb2JieWthbWVyADUIAQEANQhJRVEwMTA5MTI1DFRoZXJtb3N0YXQgMQEB");

			Console.WriteLine(output);

			stream.Close();
			client.Close();
		}
	}
}
