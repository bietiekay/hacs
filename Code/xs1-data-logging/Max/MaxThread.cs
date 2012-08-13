/*
 * This is the thread that handles the connection to (at least one) ELV MAX! Cube base controller for ELV Max! home automation
 * hardware.
 * 
 * more information here: http://www.elv.de/max-funk-heizungsregler-system.html
 * 
 * At the moment this is a whitespace file which will hold the implementation of the MAX! Cube interface in the future - at
 * least when I got my sample :-) * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Threading;

namespace xs1_data_logging
{	
	public class MaxThread
	{
		private bool _Shutdown = false;
        private ConsoleOutputLogger ConsoleOutputLogger;

		public MaxThread (ConsoleOutputLogger Logger)
		{
            ConsoleOutputLogger = Logger;
		}

		public void Shutdown()
		{
			_Shutdown = true;
		}
			
		/// <summary>
		/// Run this instance.
		/// </summary>
        public void Run()
        {
            ConsoleOutputLogger.WriteLine("Starting MAX! handling thread.");
			TimeSpan temp = new TimeSpan();
            while (!_Shutdown)
            {
                try
                {
					// do something
                }
                catch(Exception)
                {
                }

				Thread.Sleep(1000); // just check every 1 seconds...
			}
		}
	}
}

