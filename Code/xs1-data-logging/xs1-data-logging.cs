/*
* h.a.c.s (home automation control server) - http://github.com/bietiekay/hacs
* Copyright (C) 2010 Daniel Kirstenpfad
*
* hacs is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* hacs is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with hacs. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using sones.storage;
using hacs.xs1;
using System.Threading;
using System.Configuration;

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
            ConsoleOutputLogger.WriteLine("(C) 2010-2011 Daniel Kirstenpfad - http://github.com/bietiekay/hacs");
            #endregion
			
			ScriptingConfigurationSection section = (ScriptingConfigurationSection)ConfigurationManager.GetSection("scriptingactorsettings");			
			
            ConsoleOutputLogger.writeLogfile = true;

            TinyOnDiskStorage actor_data_store = new TinyOnDiskStorage("actor-data", false);
            TinyOnDiskStorage sensor_data_store = new TinyOnDiskStorage("sensor-data", false);
            TinyOnDiskStorage unknown_data_store = new TinyOnDiskStorage("unknown-data", false);

            List<Thread> LoggingThreads = new List<Thread>();

            //foreach (String _Server in Properties.Settings.Default)
            //{
                ConsoleOutputLogger.WriteLineToScreenOnly("Starting Logging for Server: " + Properties.Settings.Default.XS1);
                LoggingThread _Thread = new LoggingThread(Properties.Settings.Default.XS1, actor_data_store, sensor_data_store, unknown_data_store,Properties.Settings.Default.Username,Properties.Settings.Default.Password,Properties.Settings.Default.ConfigurationCacheMinutes);
                Thread LoggingThread = new Thread(new ThreadStart(_Thread.Run));

                LoggingThreads.Add(LoggingThread);

                LoggingThread.Start();

            //}


        }
    }
}
