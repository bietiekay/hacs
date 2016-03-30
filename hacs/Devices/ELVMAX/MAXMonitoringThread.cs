using System;
using System.Net.Sockets;   // TCP-streaming
using System.Threading;     // the sleeping part...
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace hacs
{
	public class MAXMonitoringThread
	{
		private String Hostname;
		private Int32 Port;
		public House theHouse;
		private DateTime LastReStoring;
		private Dictionary<String,IMAXDevice> previousHouse;
		private static bool keepRunning = true;
		public bool running = true;
		private Int32 MAXUpdateTime;
		private ConsoleOutputLogger ConsoleOutputLogger;
		private ConcurrentQueue<IDeviceDiffSet> iQueue;

		public MAXMonitoringThread(String _Hostname, Int32 _Port, ConsoleOutputLogger COL, ConcurrentQueue<IDeviceDiffSet> EventQueue, Int32 UpdateTime = 10000)
		{
			Hostname = _Hostname;
			Port = _Port;
			MAXUpdateTime = UpdateTime;
			ConsoleOutputLogger = COL;
			iQueue = EventQueue;
			LastReStoring = DateTime.Now;
		}

		// this is the ELV MAX! Cube monitoring script
		public void Run()
        {
			while(running)
			{
				TcpClient client;
				NetworkStream stream;
                Dictionary<String, IMAXDevice> currentHouse = new Dictionary<string,IMAXDevice>();

				#region Update House
				try
				{
					// now fill that with the initial handshake data...

					#region Initial connect and retrieving everything we get from the cube
					// network connect and first initialization
					client = new TcpClient();
					client.Connect(Hostname,Port);
					stream = client.GetStream();

					// the read buffer (chosen quite big)
					byte[] myReadBuffer = new byte[4096*8];
					List<String> Messages = new List<string>();
					
					// to build the complete message
					StringBuilder myCompleteMessage = new StringBuilder();
					int numberOfBytesRead = 0;
					
					MAXEncodeDecode DecoderEncoder = new MAXEncodeDecode();
					keepRunning = true;
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
						catch(Exception)
						{
							previousHouse = null;
							// everything to start
							theHouse = new House();

							keepRunning = false;
						}
					}
					while(keepRunning);
					#endregion

					ConsoleOutputLogger.WriteLine("ELV MAX Cube connected...");

					#region preprocess
					List<String> PreProcessedMessages = new List<string>();
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
					#endregion

					#region process
					// Analyze and Output Messages, feed them to the decoder
					foreach(String _Message in PreProcessedMessages)
					{
						//IMAXMessage Message = DecoderEncoder.ProcessMessage(_Message.ToString(), theHouse);
						//							if (Message != null)
						//							{
						//								ConsoleOutputLogger.WriteLine(_Message.ToString());
						//								ConsoleOutputLogger.WriteLine(Message.ToString());
						//								ConsoleOutputLogger.WriteLine("");
						//							}

						DecoderEncoder.ProcessMessage(_Message.ToString(), theHouse);
					}
					#endregion

					//while(running)
					//{
						// when we are here, we got a filled "theHouse" which does contain first hand L message information
						previousHouse = theHouse.GetAllDevicesInADictionary();

						#region send L: request and get all the feedback
						System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
						byte[] args_data_buffer = enc.GetBytes("l:\r\n");
						Messages = new List<string>();
						keepRunning = true;
						// Incoming message may be larger than the buffer size.
                        stream.Write(args_data_buffer, 0, args_data_buffer.Length);
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
							catch(Exception)
							{
								keepRunning = false;
							}
						}
						while(keepRunning);
						#endregion
						
						#region preprocess
						PreProcessedMessages = new List<string>();
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
						#endregion
						
						#region process
						// Analyze and Output Messages, feed them to the decoder
						foreach(String _Message in PreProcessedMessages)
						{
							//IMAXMessage Message = DecoderEncoder.ProcessMessage(_Message.ToString(), theHouse);
							//							if (Message != null)
							//							{
							//								ConsoleOutputLogger.WriteLine(_Message.ToString());
							//								ConsoleOutputLogger.WriteLine(Message.ToString());
							//								ConsoleOutputLogger.WriteLine("");
							//							}
                            currentHouse = new Dictionary<string, IMAXDevice>();
							DecoderEncoder.ProcessLMessage(_Message.ToString(), theHouse, currentHouse);
						}
						#endregion

						#region Diff the house
						if (previousHouse != null)
						{
							// only if we already got two houses in here...
							List<IDeviceDiffSet> differences = DiffHouse.CalculateDifferences(previousHouse,currentHouse);
							if (differences.Count != 0)
							{
								#region enqueue the difference-sets into the data queue
								foreach(IDeviceDiffSet _difference in differences)
								{
									iQueue.Enqueue(_difference);
								}
								#endregion
							}
						}
						#endregion
                        theHouse.UpdateDevices(currentHouse);

						// update appropriate devices and in given intervals output non updated
						TimeSpan _lastUpdate = DateTime.Now-LastReStoring;
						
						// auto-update every n ... minutes
						if (_lastUpdate.TotalSeconds > Properties.Settings.Default.ELVMAXSensorReStoringSec)
						{
							LastReStoring = DateTime.Now;

							foreach(IMAXDevice _device in currentHouse.Values)
							{

								#region Heating Thermostat
								if (_device.Type == DeviceTypes.HeatingThermostat)
								{
									HeatingThermostat _heating = (HeatingThermostat)_device;
									HeatingThermostatDiff _queueable = new HeatingThermostatDiff(_device.Name,_device.AssociatedRoom.RoomID,_device.AssociatedRoom.RoomName);

									if (_heating.LowBattery)
										_queueable.LowBattery = BatteryStatus.lowbattery;
									else
										_queueable.LowBattery = BatteryStatus.ok;

									_queueable.Mode = _heating.Mode;
									_queueable.Temperature = _heating.Temperature;

									if (_queueable.Temperature != 0)
										iQueue.Enqueue(_queueable);
								}
								#endregion

								#region ShutterContact
								if (_device.Type == DeviceTypes.ShutterContact)
								{
									ShutterContact _shutter = (ShutterContact)_device;
									ShutterContactDiff _queueable = new ShutterContactDiff(_device.Name,_device.AssociatedRoom.RoomID,_device.AssociatedRoom.RoomName);

									if (_shutter.LowBattery)
										_queueable.LowBattery = BatteryStatus.lowbattery;
									else
										_queueable.LowBattery = BatteryStatus.ok;

									_queueable.ShutterState = _shutter.ShutterState;
									iQueue.Enqueue(_queueable);
								}
								#endregion
							}
						}
                        stream.Close();
                        ConsoleOutputLogger.WriteLine("ELV MAX Cube disconnect...");
						Thread.Sleep (MAXUpdateTime);
					//}
				}
				catch(Exception e)
				{
					ConsoleOutputLogger.WriteLine(e.Message);
                    Thread.Sleep(Properties.Settings.Default.ELVMAXReconnectTimeMsec);

					ConsoleOutputLogger.WriteLine("ELV MAX Cube reconnect...");
					//stream.Close();
					//client.Close();
				}
				#endregion

			}
		}

	}
}

