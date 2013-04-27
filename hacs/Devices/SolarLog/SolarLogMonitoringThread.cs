using System;
using System.Net.Sockets;   // TCP-streaming
using System.Threading;     // the sleeping part...
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace hacs
{
	public class SolarLogMonitoringThread
	{
		private String URL;
		public bool running = true;
		private Int32 SolarLogUpdateTime;
		private ConsoleOutputLogger ConsoleOutputLogger;
		private ConcurrentQueue<SolarLogDataSet> iQueue;
		private SolarLogDataSet LastDataSet = null;

		public SolarLogMonitoringThread(String _URL, ConsoleOutputLogger COL, ConcurrentQueue<SolarLogDataSet> EventQueue, Int32 UpdateTime = 10000)
		{
			URL = _URL;
			SolarLogUpdateTime = UpdateTime;
			ConsoleOutputLogger = COL;
			iQueue = EventQueue;
		}

		// this is the ELV MAX! Cube monitoring script
		public void Run()
        {
			ConsoleOutputLogger.WriteLine("Starting up SolarLog data gathering...");
			while(running)
			{
				SolarLogDataSet data = SolarLogDataHelper.UpdateSolarLog(URL,ConsoleOutputLogger);

				if (data != null)
				{
					if (LastDataSet == null)
					{
						LastDataSet = data;
						iQueue.Enqueue(data);
					}
					else
					{
						bool QueueIt = false;

						if (data.Pac != LastDataSet.Pac)
							QueueIt = true;

						if (data.aPdc != LastDataSet.aPdc)
							QueueIt = true;

						// if the last data set is older than 5 minutes, definitly queue this one...
						TimeSpan Diff = DateTime.Now-LastDataSet.DateAndTime;
						//if (Diff.TotalMinutes > Properties.Settings.Default.AutomatedSensorCheck_ResponseTimeWindow)
						if (Diff.TotalMinutes > 5)
							QueueIt = true;

						if (QueueIt)
                        {
							iQueue.Enqueue(data);
                            LastDataSet = data;
                        }
					}
				}

				Thread.Sleep (SolarLogUpdateTime);
			}
		}
	}
}

