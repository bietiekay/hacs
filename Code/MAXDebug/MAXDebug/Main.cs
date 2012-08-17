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
			Console.WriteLine();

			// not enough paramteres given, display help
			if (args.Length < 2)
			{
				Console.WriteLine("Syntax:");
				Console.WriteLine();
				Console.WriteLine("\tmaxdebug <hostname/ip> <port (e.g. 62910)> [commands]");
				Console.WriteLine();
				return;
			}
			ConsoleOutputLogger.LogToFile("--------------------------------------");
			// we obviously have enough paramteres, go on and try to connect
			TcpClient client = new TcpClient();
			client.Connect(args[0], Convert.ToInt32 (args[1]));
			NetworkStream stream = client.GetStream();
			// the read buffer (chosen quite big)
			byte[] myReadBuffer = new byte[4096*8];

			// to build the complete message
			StringBuilder myCompleteMessage = new StringBuilder();
			int numberOfBytesRead = 0;

			MAXEncodeDecode DecoderEncoder = new MAXEncodeDecode();

			// Incoming message may be larger than the buffer size.
			do
			{
				myCompleteMessage = new StringBuilder();
				stream.ReadTimeout = 1000;
				try
				{
					numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
					myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

//					Console.WriteLine("------RAW--------");
//					Console.Write(myCompleteMessage);
					IMaxData Message = DecoderEncoder.ProcessMessage(myCompleteMessage.ToString());
					if (Message != null)
					{
						//Console.WriteLine("------DEC--------");
						ConsoleOutputLogger.WriteLine(Message.ToString());
						ConsoleOutputLogger.WriteLine("");
					}
				}
				catch(Exception e)
				{
					//Console.WriteLine("Exception: "+e.Message);
					keepRunning = false;
				}
				// sleep 100 msecs
				Thread.Sleep (100);
			}
			while(keepRunning);

			// some writing
			if (args.Length > 2)
			{
				System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
				byte[] args_data_buffer = enc.GetBytes(args[2]+"\r\n");

				ConsoleOutputLogger.WriteLine("Sending Command: "+args[2]);

				stream.Write(args_data_buffer,0,args_data_buffer.Length);
				keepRunning = true;

				do
				{
					myCompleteMessage = new StringBuilder();
					stream.ReadTimeout = 1000;
					try
					{
						numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
						myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

					//					Console.WriteLine("------RAW--------");
					//					Console.Write(myCompleteMessage);
						IMaxData Message = DecoderEncoder.ProcessMessage(myCompleteMessage.ToString());
						if (Message != null)
						{
							//Console.WriteLine("------DEC--------");
							ConsoleOutputLogger.WriteLine(Message.ToString());
							ConsoleOutputLogger.WriteLine("");
						}
					}
					catch(Exception e)
					{
						//Console.WriteLine("Exception: "+e.Message);
						keepRunning = false;
					}
					// sleep 100 msecs
					Thread.Sleep (100);
				}
				while(keepRunning);
				
			}

			stream.Close();
			client.Close();

		}
	}
}
