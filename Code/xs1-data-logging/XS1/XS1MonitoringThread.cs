using System;
using System.Net.Sockets;   // TCP-streaming
using System.Threading;     // the sleeping part...
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using hacs.xs1;
using System.Net;
using System.IO;

namespace xs1_data_logging
{
	public class XS1MonitoringThread
	{
		private String ServerName;
		private String UserName;
		private String Password;
		public ConsoleOutputLogger ConsoleOutputLogger;
		public bool running = true;
		private ConcurrentQueue<XS1_DataObject> iQueue;
		public Boolean AcceptingCommands = false;

		public XS1MonitoringThread(String _ServerName, ConsoleOutputLogger Logger, String _Username, String _Password, ConcurrentQueue<XS1_DataObject> _Queue)
		{
			ServerName = _ServerName;
			UserName = _Username;
			Password = _Password;
			ConsoleOutputLogger = Logger;
			iQueue = _Queue;
		}

		// this is providing us with XS1 data objects in result of XS1 events
		public void Run()
        {
			while(running)
			{
				try
				{
					byte[] buf = new byte[8192];
					
					String HacsURL = "http://" + ServerName + "/control?callback=cname&cmd=subscribe&format=tsv";
					
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(HacsURL);
					request.Timeout = 60000;
					request.Credentials = new NetworkCredential(UserName,Password);
					
					String _UsernameAndPassword = UserName+ ":" + Password;
					Uri _URI = new Uri(HacsURL);
					
					CredentialCache _CredentialCache = new CredentialCache();
					_CredentialCache.Remove(_URI, "Basic");
					_CredentialCache.Add(_URI, "Basic", new NetworkCredential(UserName, Password));
					String _AuthorizationHeader = "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(_UsernameAndPassword));
					
					request.Headers.Add("Authorization", _AuthorizationHeader);
					
					// execute the request
					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					if (response.StatusCode == HttpStatusCode.OK)
					{
						AcceptingCommands = true;
						ConsoleOutputLogger.WriteLineToScreenOnly("XS1 successfully connected!");
					}
					// we will read data via the response stream
					Stream resStream = response.GetResponseStream();
					
					string tempString = null;
					int count = 0;
					
					do
					{
						#region XS1 Receiving and Queue stuffing
						// fill the buffer with data
						count = resStream.Read(buf, 0, buf.Length);
						
						// make sure we read some data
						if (count != 0)
						{
							// translate from bytes to ASCII text
							tempString = Encoding.ASCII.GetString(buf, 0, count);
							XS1_DataObject dataobject = HandleXS1_TSV.HandleValue(tempString);
							dataobject.ServerName = ServerName;
							dataobject.OriginalXS1Statement = tempString;

							iQueue.Enqueue(dataobject);	// add it to the queue
						}					
						#endregion
					}
					while (count > 0); // any more data to read?
				}
				catch (Exception)
				{                   
					AcceptingCommands = false;
					Thread.Sleep(1);
				}

			}
		}

	}
}