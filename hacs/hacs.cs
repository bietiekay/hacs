﻿/*
* h.a.c.s (home automation control server) - http://github.com/bietiekay/hacs
* Copyright (C) 2010-2012 Daniel Kirstenpfad
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

namespace hacs
{
    /// <summary>
    /// this small tool logs the data from one or many ezcontrol xs1 devices
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            #region ConsoleOutputLogger
            ConsoleOutputLogger ConsoleOutputLogger = new hacs.ConsoleOutputLogger();
            ConsoleOutputLogger.verbose = true;
            ConsoleOutputLogger.writeLogfile = false;
            #endregion

            #region Logo
            ConsoleOutputLogger.WriteLine("home automation control server " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            ConsoleOutputLogger.WriteLine("(C) 2010-2013 Daniel Kirstenpfad - http://github.com/bietiekay/hacs");
            #endregion
						
            ConsoleOutputLogger.writeLogfile = true;

            TinyOnDiskStorage actor_data_store = new TinyOnDiskStorage(Properties.Settings.Default.DataDirectory + Path.DirectorySeparatorChar + "actor-data", false, hacs.Properties.Settings.Default.DataObjectCacheSize);
			ConsoleOutputLogger.WriteLine("Initialized actor-data storage: "+actor_data_store.InMemoryIndex.Count);
            TinyOnDiskStorage sensor_data_store = new TinyOnDiskStorage(Properties.Settings.Default.DataDirectory + Path.DirectorySeparatorChar + "sensor-data", false, hacs.Properties.Settings.Default.DataObjectCacheSize);
			ConsoleOutputLogger.WriteLine("Initialized sensor-data storage: "+sensor_data_store.InMemoryIndex.Count);
            TinyOnDiskStorage unknown_data_store = new TinyOnDiskStorage(Properties.Settings.Default.DataDirectory + Path.DirectorySeparatorChar + "unknown-data", false, hacs.Properties.Settings.Default.DataObjectCacheSize);
			ConsoleOutputLogger.WriteLine("Initialized unknown-data storage: "+unknown_data_store.InMemoryIndex.Count);
			TinyOnDiskStorage miataru_data_store = null;

			if (hacs.Properties.Settings.Default.MiataruEnabled)
			{
				miataru_data_store = new TinyOnDiskStorage(Properties.Settings.Default.DataDirectory + Path.DirectorySeparatorChar + "miataru-data", false, hacs.Properties.Settings.Default.DataObjectCacheSize);
				ConsoleOutputLogger.WriteLine("Initialized Miataru storage: "+miataru_data_store.InMemoryIndex.Count);
			}


            //List<Thread> LoggingThreads = new List<Thread>();
            ScriptingTimerThread _ScriptingTimerThread;

            ScriptingActorConfiguration.ReadConfiguration(Properties.Settings.Default.ConfigurationDirectory + Path.DirectorySeparatorChar + Properties.Settings.Default.ScriptingActorConfigurationFilename);
            PowerSensorConfiguration.ReadConfiguration(Properties.Settings.Default.ConfigurationDirectory + Path.DirectorySeparatorChar + Properties.Settings.Default.PowerSensorConfigurationFilename);
            ScriptingTimerConfiguration.ReadConfiguration(Properties.Settings.Default.ConfigurationDirectory + Path.DirectorySeparatorChar + Properties.Settings.Default.ScriptingTimerConfigurationFilename);
            SensorCheckIgnoreConfiguration.ReadConfiguration(Properties.Settings.Default.ConfigurationDirectory + Path.DirectorySeparatorChar + Properties.Settings.Default.SensorCheckIgnoreFile);
            HTTPProxyConfiguration.ReadConfiguration(Properties.Settings.Default.ConfigurationDirectory + Path.DirectorySeparatorChar + Properties.Settings.Default.HTTPProxyConfigurationFilename);
			
            if (Properties.Settings.Default.AlarmingEnabled)
                AlarmingConfiguration.ReadConfiguration(Properties.Settings.Default.ConfigurationDirectory + Path.DirectorySeparatorChar + Properties.Settings.Default.AlarmingConfigurationFilename);

            if (Properties.Settings.Default.MiataruEnabled)
				MiataruConfiguration.ReadConfiguration(Properties.Settings.Default.ConfigurationDirectory + Path.DirectorySeparatorChar + Properties.Settings.Default.MiataruConfigurationFilename);

            if (Properties.Settings.Default.MQTTBindingEnabled)
                MQTTBindingConfiguration.ReadConfiguration(Properties.Settings.Default.ConfigurationDirectory + Path.DirectorySeparatorChar + Properties.Settings.Default.MQTTBindingConfigurationFile);

			#region add NetworkMonitor sensors to sensorcheckignore list
			if (Properties.Settings.Default.NetworkMonitorEnabled)
			{
                NetworkMonitorConfiguration.ReadConfiguration(Properties.Settings.Default.ConfigurationDirectory + Path.DirectorySeparatorChar + Properties.Settings.Default.NetworkMonitorConfigurationFilename);

				foreach(NetworkMonitoringHost Host in NetworkMonitorConfiguration.NetworkHosts)
				{
					SensorCheckIgnoreConfiguration.AddToIgnoreList(Host.IPAdressOrHostname);
				}
			}
			#endregion

            #region Logging and Actor Handling
            ConsoleOutputLogger.WriteLineToScreenOnly("Starting Logging for Server: " + Properties.Settings.Default.XS1);                        
			LoggingThread _Thread = new LoggingThread(Properties.Settings.Default.XS1, ConsoleOutputLogger, actor_data_store, sensor_data_store, unknown_data_store,miataru_data_store,Properties.Settings.Default.Username,Properties.Settings.Default.Password,Properties.Settings.Default.ConfigurationCacheMinutes);
            Thread LoggingThread = new Thread(new ThreadStart(_Thread.Run));
            LoggingThread.Start();
            #endregion

            #region Scripting Timer Handling
            ConsoleOutputLogger.WriteLineToScreenOnly("Starting Scripting Timer Handling.");
            _ScriptingTimerThread = new ScriptingTimerThread(_Thread,ConsoleOutputLogger);
            Thread ScriptingTThread = new Thread(new ThreadStart(_ScriptingTimerThread.Run));
            ScriptingTThread.Start();
            #endregion
        }
    }
}
