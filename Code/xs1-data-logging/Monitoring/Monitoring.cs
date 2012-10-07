using System;
using System.Net.Sockets;   // TCP-streaming
using System.Threading;     // the sleeping part...
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace xs1_data_logging
{
	public class MonitoringThread
	{
		public bool running = true;
		private ConsoleOutputLogger ConsoleOutputLogger;

		public MonitoringThread(ConsoleOutputLogger COL)
		{
			ConsoleOutputLogger = COL;
		}

		// this is the service monitoring script
		public void Run()
        {
			while(running)
			{
				try
				{
					while(running)
					{
						Thread.Sleep (100);
					}
				}
				catch(Exception e)
				{
					ConsoleOutputLogger.WriteLine(e.Message);
				}
			}
		}
	}
}

