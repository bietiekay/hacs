using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

// State object for receiving data from remote device.
using MAXDebug;

namespace MAXDebug
{
	public class StateObject 
	{
	    // Client socket.
	    public Socket workSocket = null;
	    // Size of receive buffer.
	    public const int BufferSize = 256;
	    // Receive buffer.
	    public byte[] buffer = new byte[BufferSize];
	    // Received data string.
	    public StringBuilder sb = new StringBuilder();
		public bool LiveRun = false;
	}

	public class AsynchronousClient 
	{
	    // ManualResetEvent instances signal completion.
	    private static ManualResetEvent connectDone = 
	        new ManualResetEvent(false);
	    private static ManualResetEvent sendDone = 
	        new ManualResetEvent(false);
	    private static ManualResetEvent receiveDone = 
	        new ManualResetEvent(false);

	    // The response from the remote device.
	    private static String response = String.Empty;

	    private static void StartClient(String[] args) 
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

	        // Connect to a remote device.
	        try {
	            // Establish the remote endpoint for the socket.
	            // The name of the 
	            // remote device is "host.contoso.com".
	            IPHostEntry ipHostInfo = Dns.Resolve(args[0]);
	            IPAddress ipAddress = ipHostInfo.AddressList[0];
	            IPEndPoint remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32 (args[1]));

	            // Create a TCP/IP socket.
	            Socket client = new Socket(AddressFamily.InterNetwork,
	                SocketType.Stream, ProtocolType.Tcp);

	            // Connect to the remote endpoint.
	            client.BeginConnect( remoteEP, 
	                new AsyncCallback(ConnectCallback), client);
	            connectDone.WaitOne();

				client.ReceiveTimeout = 1000;

	            // Receive the response from the remote device.
	            Receive(client);
				receiveDone.WaitOne();

				#region execute command from commandline
				if (args.Length > 2)
				{
					// Send test data to the remote device.
	            	Send(client,args[2]+"\r\n");
	            	sendDone.WaitOne();

		            // Receive the response from the remote device.
		            Receive(client);
					receiveDone.WaitOne();
				}
				#endregion

				// Release the socket.
		        client.Shutdown(SocketShutdown.Both);
		        client.Close();

				// take the response and split it into it's lines
				String[] SplittedResponse = response.Replace("\r","").Split(new char[1] { '\n' },StringSplitOptions.RemoveEmptyEntries);
				MAXEncodeDecode DecoderEncoder = new MAXEncodeDecode();

				// Analyze and Output Messages
				foreach(String _Message in SplittedResponse)
				{
					IMaxData Message = DecoderEncoder.ProcessMessage(_Message.ToString());
					if (Message != null)
					{
						ConsoleOutputLogger.WriteLine(Message.ToString());
						ConsoleOutputLogger.WriteLine("");
					}
				}

	        } catch (Exception e) {
	            Console.WriteLine(e.ToString());
	        }
	    }

	    private static void ConnectCallback(IAsyncResult ar) {
	        try {
	            // Retrieve the socket from the state object.
	            Socket client = (Socket) ar.AsyncState;

	            // Complete the connection.
	            client.EndConnect(ar);

	            Console.WriteLine("Socket connected to {0}",
	                client.RemoteEndPoint.ToString());

	            // Signal that the connection has been made.
	            connectDone.Set();
	        } catch (Exception e) {
	            Console.WriteLine(e.ToString());
	        }
	    }

	    private static void Receive(Socket client) {
	        try {
	            // Create the state object.
	            StateObject state = new StateObject();
	            state.workSocket = client;

	            // Begin receiving the data from the remote device.
	            client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
	                new AsyncCallback(ReceiveCallback), state);
	        } catch (Exception e) {
	            Console.WriteLine(e.ToString());
	        }
	    }

	    private static void ReceiveCallback( IAsyncResult ar ) {
	        try {
	            // Retrieve the state object and the client socket 
	            // from the asynchronous state object.
	            StateObject state = (StateObject) ar.AsyncState;
	            Socket client = state.workSocket;

	            // Read data from the remote device.
	            int bytesRead = client.EndReceive(ar);

	            if (bytesRead > 0) {

	                // There might be more data, so store the data received so far.
		            state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));
					if (!state.LiveRun)
					{
						if (state.sb.ToString().Contains("L:"))
						{
							response = state.sb.ToString();
						    receiveDone.Set();
							return;
						}
					}
	                // Get the rest of the data.
	                client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,
	                    new AsyncCallback(ReceiveCallback), state);
	            } else {
	                // All the data has arrived; put it in response.
	                if (state.sb.Length > 1) {
	                    response = state.sb.ToString();
	                }
	                // Signal that all bytes have been received.
	                receiveDone.Set();
	            }
	        } catch (Exception e) {
	            Console.WriteLine(e.ToString());
	        }
	    }

	    private static void Send(Socket client, String data) {
	        // Convert the string data to byte data using ASCII encoding.
	        byte[] byteData = Encoding.ASCII.GetBytes(data);

	        // Begin sending the data to the remote device.
	        client.BeginSend(byteData, 0, byteData.Length, 0,
	            new AsyncCallback(SendCallback), client);
	    }

	    private static void SendCallback(IAsyncResult ar) {
	        try {
	            // Retrieve the socket from the state object.
	            Socket client = (Socket) ar.AsyncState;

	            // Complete sending the data to the remote device.
	            int bytesSent = client.EndSend(ar);
	            Console.WriteLine("Sent {0} bytes to server.", bytesSent);

	            // Signal that all bytes have been sent.
	            sendDone.Set();
	        } catch (Exception e) {
	            Console.WriteLine(e.ToString());
	        }
	    }
	    
	    public static int Main(String[] args) {
	        StartClient(args);
	        return 0;
	    }
	}
}