using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using sones.storage;
using hacs.xs1;
using System.Threading;

namespace xs1_data_logging
{
    /// <summary>
    /// this small tool logs the data from one or many ezcontrol xs1 devices
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            #region ConsoleOutputLogger
            ConsoleOutputLogger.verbose = true;
            ConsoleOutputLogger.writeLogfile = false;
            #endregion

            #region Logo
            ConsoleOutputLogger.WriteLine("EzControl XS1 Data Logger " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            ConsoleOutputLogger.WriteLine("(C) 2010 Daniel Kirstenpfad - http://github.com/bietiekay/hacs");
            #endregion

            ConsoleOutputLogger.writeLogfile = true;

            TinyOnDiskStorage actor_data_store = new TinyOnDiskStorage("actor-data", false);
            TinyOnDiskStorage sensor_data_store = new TinyOnDiskStorage("sensor-data", false);
            TinyOnDiskStorage unknown_data_store = new TinyOnDiskStorage("unknown-data", false);

            List<Thread> LoggingThreads = new List<Thread>();

            foreach (String _Server in Properties.Settings.Default.XS1Servers)
            {
                ConsoleOutputLogger.WriteLineToScreenOnly("Starting Logging for Server: " + _Server);
                LoggingThread _Thread = new LoggingThread(_Server, actor_data_store, sensor_data_store, unknown_data_store);
                Thread LoggingThread = new Thread(new ThreadStart(_Thread.Run));

                LoggingThreads.Add(LoggingThread);

                LoggingThread.Start();

            }


        }
    }
}
