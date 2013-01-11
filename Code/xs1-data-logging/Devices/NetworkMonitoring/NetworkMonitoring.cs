using System;
using System.Threading;     // the sleeping part...
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Org.Mentalis.Network;	// the icmp ping part

namespace xs1_data_logging
{
	public class NetworkMonitoring
	{
		private String URL;
		public bool running = true;
		private Int32 NetworkMonitorUpdateTime;
		private ConsoleOutputLogger ConsoleOutputLogger;
		private ConcurrentQueue<NetworkMonitoringDataSet> iQueue;

		public NetworkMonitoring(ConsoleOutputLogger COL, ConcurrentQueue<NetworkMonitoringDataSet> EventQueue, Int32 UpdateTime = 10000)
		{
			NetworkMonitorUpdateTime = UpdateTime;
			ConsoleOutputLogger = COL;
			iQueue = EventQueue;
		}

		// this is the network monitoring script
		public void Run()
        {
			ConsoleOutputLogger.WriteLine("Starting up the Network Monitoring...");
			ICMP pinger = new ICMP();

			while(running)
			{
				foreach(NetworkMonitoringHost Host in NetworkMonitorConfiguration.NetworkHosts)
				{
					// start pinging around
					ICMP_PingResult result = null;
					try
					{
						result = pinger.Ping(Host.IPAdressOrHostname);
					}
					catch(Exception e)
					{
						//ConsoleOutputLogger.WriteLine("NetworkMonitor Exception: "+e.Message);
                        result = new ICMP_PingResult();
                        result.Status = ICMP_Status.TimeOut;
                        result.hostIP = null;
                    }

					// we got a result...
					if (result != null)
					{
						NetworkMonitoringDataSet ds_result = new NetworkMonitoringDataSet();

						ds_result.AverageRoundtripMS = result.AverageRoundtripMS;
						ds_result.Descriptor = Host.Descriptor;
						ds_result.HostnameIP = Host.IPAdressOrHostname;
						ds_result.Status = result.Status;
						ds_result.TimeOfMeasurement = result.TimeOfMeasurement;

						iQueue.Enqueue(ds_result);
					}
				}

				Thread.Sleep (NetworkMonitorUpdateTime);
			}
		}
	}
}

