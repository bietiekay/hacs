using System;
using System.Net.Sockets;   // TCP-streaming
using System.Threading;     // the sleeping part...
using System.Text;
using System.Collections.Generic;

namespace MAXDebug
{
	public class MAXMonitoringThread
	{
		private String Hostname;
		private Int32 Port;
		public House thisHouse;
		private static bool keepRunning = true;

		public MAXMonitoringThread(String _Hostname, Int32 _Port)
		{
			Hostname = _Hostname;
			Port = _Port;
			thisHouse = new House();
		}

		// this is the ELV MAX! Cube monitoring script
		public void Run()
        {
			// we obviously have enough paramteres, go on and try to connect
			TcpClient client = new TcpClient();
			client.Connect(Hostname,Port);
			NetworkStream stream = client.GetStream();

			// the read buffer (chosen quite big)
			byte[] myReadBuffer = new byte[4096*8];
			List<String> Messages = new List<string>();

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

					Messages.Add(myCompleteMessage.ToString());
				}
				catch(Exception e)
				{
					//Console.WriteLine("Exception: "+e.Message);
					keepRunning = false;
				}
			}
			while(keepRunning);

			List<String> PreProcessedMessages = new List<string>();
			// preprocess
			foreach(String _Message in Messages)
			{
				if (_Message.Remove(_Message.Length-2).Contains("\r\n"))
				{
					String[] PMessages = _Message.Remove(_Message.Length-2).Split(new char[1] { '\n' },StringSplitOptions.RemoveEmptyEntries);
					foreach(String pmessage in PMessages)
					{
						PreProcessedMessages.Add(pmessage.Replace("\r","")+"\r\n");
					}
				}
				else
					PreProcessedMessages.Add(_Message);
			}			

			// Analyze and Output Messages
			foreach(String _Message in PreProcessedMessages)
			{
				IMAXMessage Message = DecoderEncoder.ProcessMessage(_Message.ToString(), thisHouse);
				if (Message != null)
				{
					ConsoleOutputLogger.WriteLine(_Message.ToString());
					ConsoleOutputLogger.WriteLine(Message.ToString());
					ConsoleOutputLogger.WriteLine("");
				}
			}
			// now we got the house information, try to get into the 
			if (args.Length > 2)
			{
				System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
				byte[] args_data_buffer = enc.GetBytes(args[2]+"\r\n");

				ConsoleOutputLogger.WriteLine("Sending Command: "+args[2]);

				stream.Write(args_data_buffer,0,args_data_buffer.Length);
				keepRunning = true;
				Messages = new List<string>();

				do
				{
					myCompleteMessage = new StringBuilder();
					stream.ReadTimeout = 1000;
					try
					{
						numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
						myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
						Messages.Add(myCompleteMessage.ToString());
					}
					catch(Exception e)
					{
						//jConsole.WriteLine("Exception: "+e.Message);
						keepRunning = false;
					}
				}
				while(keepRunning);

				PreProcessedMessages = new List<string>();
				// preprocess
				foreach(String _Message in Messages)
				{
					if (_Message.Remove(_Message.Length-2).Contains("\r\n"))
					{
						String[] PMessages = _Message.Remove(_Message.Length-2).Split(new char[1] { '\n' },StringSplitOptions.RemoveEmptyEntries);
						foreach(String pmessage in PMessages)
						{
							PreProcessedMessages.Add(pmessage.Replace("\r","")+"\r\n");
						}
					}
					else
						PreProcessedMessages.Add(_Message);
				}			

				foreach(String _Message in PreProcessedMessages)
				{
					IMAXMessage Message = DecoderEncoder.ProcessMessage(_Message,thisHouse);
					if (Message != null)
					{
						ConsoleOutputLogger.WriteLine(Message.ToString());
						ConsoleOutputLogger.LogToFile("");
					}
				}
			}

			stream.Close();
			client.Close();

				}
	}
}

