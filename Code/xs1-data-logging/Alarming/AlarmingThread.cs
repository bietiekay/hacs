using System;
using System.Threading;

namespace xs1_data_logging
{
	public class AlarmingThread
	{
		public bool Shutdown = false;
		private ConsoleOutputLogger ConsoleOutputLogger;
		
		public AlarmingThread(ConsoleOutputLogger Logger)
		{
			ConsoleOutputLogger = Logger;
		}

		public void Run()
		{
			ConsoleOutputLogger.WriteLine("Alarming Thread started");
			while (!Shutdown)
			{
				try
				{
				}
				catch (Exception e)
				{                   
					Thread.Sleep(100);
				}
				
				Thread.Sleep(1);
			}
			
			
		}

	}
}

