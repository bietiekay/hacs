using System;
using System.Threading;     // the sleeping part...
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Org.Mentalis.Network;	// the icmp ping part

namespace xs1_data_logging
{
	public class NetworkMonitoringThread
	{
		private String URL;
		public bool running = true;
		private Int32 NetworkMonitorUpdateTime;
		private ConsoleOutputLogger ConsoleOutputLogger;
		private ConcurrentQueue<NetworkMonitoringDataSet> iQueue;

		public NetworkMonitoringThread(ConsoleOutputLogger COL, ConcurrentQueue<NetworkMonitoringDataSet> EventQueue, Int32 UpdateTime = 10000)
		{
			NetworkMonitorUpdateTime = UpdateTime;
			ConsoleOutputLogger = COL;
			iQueue = EventQueue;
		}

		// this is the network monitoring script
		public void Run()
        {
			ConsoleOutputLogger.WriteLine("Starting up the Network Monitoring...");
			while(running)
			{
				Thread.Sleep (NetworkMonitorUpdateTime);
			}
		}
	}
}

