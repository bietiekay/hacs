using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using hacs.xs1;
using sones.storage;
using System.Threading;
using HTTP;
using hacs.xs1.configuration;
using System.Collections.Concurrent;
//using xs1_data_logging.set_state_actuator;


namespace xs1_data_logging
{
    public class LoggingThread
    {
        public String ServerName;
        public String UserName;
        public String Password;
        TinyOnDiskStorage actor_data_store = null;
        TinyOnDiskStorage sensor_data_store = null;
        TinyOnDiskStorage unknown_data_store = null;
        XS1Configuration XS1_Configuration = null;
        Int32 ConfigurationCacheMinutes;
        public Boolean AcceptingCommands = false;
        public List<String> TemporaryBlacklist = new List<string>();
        public List<String> OnWaitOffLIst = new List<string>();
        public ConsoleOutputLogger ConsoleOutputLogger;
		public ConcurrentQueue<XS1_DataObject> XS1_DataQueue;	// use a thread safe list like structure to hold the messages coming in from the XS1
        bool Shutdown = false;

		#region Ctor
        public LoggingThread(String _ServerName, ConsoleOutputLogger Logger,TinyOnDiskStorage _actor_store, TinyOnDiskStorage _sensor_store, TinyOnDiskStorage _unknown_store, String _Username, String _Password, Int32 _ConfigurationCacheMinutes)
        {
            actor_data_store = _actor_store;
            sensor_data_store = _sensor_store;
            unknown_data_store = _unknown_store;
            ServerName = _ServerName;
            UserName = _Username;
            Password = _Password;
            ConfigurationCacheMinutes = _ConfigurationCacheMinutes;
            ConsoleOutputLogger = Logger;
			XS1_DataQueue = new ConcurrentQueue<XS1_DataObject>();
        }
		#endregion

        public void Run()
        {
            // initialize XS1 Configuration
            XS1_Configuration = new XS1Configuration(ConfigurationCacheMinutes);

			// Start integrated HTTP Server
            HttpServer httpServer = new HttpServer(Properties.Settings.Default.HTTPPort,Properties.Settings.Default.HTTPIP,Properties.Settings.Default.HTTPDocumentRoot,sensor_data_store,XS1_Configuration, ConsoleOutputLogger);
            Thread http_server_thread = new Thread(new ThreadStart(httpServer.listen));
            http_server_thread.Start();

			// Start Sensor-Check Thread
            SensorCheck Sensorcheck = new SensorCheck(ConsoleOutputLogger);
            Thread SensorCheckThread = new Thread(new ThreadStart(Sensorcheck.Run));
			SensorCheckThread.Start();

			// Start Actor Re-Switching Thread
            ActorReswitching ActorReSwitch_ = new ActorReswitching(XS1_Configuration, ConsoleOutputLogger, TemporaryBlacklist,OnWaitOffLIst);
            Thread ActorReswitchThread = new Thread(new ThreadStart(ActorReSwitch_.Run));
            ActorReswitchThread.Start();

			// Start the ELVMax Thread
			// Todo: Add configurability of this startup
			MAXMonitoringThread ELVMax = new MAXMonitoringThread(Properties.Settings.Default.ELVMAXIP,Properties.Settings.Default.ELVMAXPort,ConsoleOutputLogger,Properties.Settings.Default.ELVMAXUpdateIntervalMsec);
			Thread ELVMaxThread = new Thread(new ThreadStart(ELVMax.Run));
			ELVMaxThread.Start();

			XS1MonitoringThread XS1 = new XS1MonitoringThread(ServerName,ConsoleOutputLogger,UserName,Password,XS1_DataQueue);
			Thread XS1Thread = new Thread(new ThreadStart(XS1.Run));
			XS1Thread.Start();

            while (!Shutdown)
            {
                try
                {
					#region Handle XS1 events
					XS1_DataObject dataobject;
					if (XS1_DataQueue.TryDequeue(out dataobject))
					{
						if (dataobject.Type == ObjectTypes.Actor)
                        {
                            lock(actor_data_store)
                            {
                                actor_data_store.Write(dataobject.Serialize());
                            }

                            lock(KnownActorStates.KnownActorStatuses)
                            {
                                bool usethisactor = true;
                                // check if this actor is on temporary blacklist (like when it was handled)
                                lock (TemporaryBlacklist)
                                {
                                    if (TemporaryBlacklist.Contains(dataobject.Name))
                                        usethisactor = false;
                                    TemporaryBlacklist.Remove(dataobject.Name);
                                }

                                if (usethisactor)
                                {
                                    if (KnownActorStates.KnownActorStatuses.ContainsKey(dataobject.Name))
                                    {
                                        // is contained
                                        if (dataobject.Value == 100)
                                            KnownActorStates.KnownActorStatuses[dataobject.Name] = new current_actor_status(dataobject.Name, actor_status.On);
                                        else
                                            KnownActorStates.KnownActorStatuses[dataobject.Name] = new current_actor_status(dataobject.Name, actor_status.Off);
                                    }
                                    else
                                    {
                                        if (dataobject.Value == 100)
                                            KnownActorStates.KnownActorStatuses.Add(dataobject.Name, new current_actor_status(dataobject.Name, actor_status.On));
                                        else
                                            KnownActorStates.KnownActorStatuses.Add(dataobject.Name, new current_actor_status(dataobject.Name, actor_status.Off));
                                    }
                                }
                                else
                                    ConsoleOutputLogger.WriteLine("Actor "+dataobject.Name+" is on the blacklist (ActorReSwitching) and therefore is ignored this time.");
                            }
                        }
                        else
                            if (dataobject.Type == ObjectTypes.Sensor)
                            {
                                lock(sensor_data_store)
                                {
                                    sensor_data_store.Write(dataobject.Serialize());
                                }
                                // update the sensor in the sensor check
                                Sensorcheck.UpdateSensor(dataobject.Name);

                                // check if this sensor is something we should act uppon
                                foreach (ScriptingActorElement Element in ScriptingActorConfiguration.ScriptingActorActions)
                                {
                                    if (dataobject.Name == Element.SensorToWatchName)
                                    {
                                        if (dataobject.Value == Element.SensorValue)
                                        { 
                                            // obviously there is a ScriptingActorConfiguration entry
                                            // so we execute the actor preset

                                            set_state_actuator.set_state_actuator ssa = new set_state_actuator.set_state_actuator();
                                            ConsoleOutputLogger.WriteLineToScreenOnly("detected actor scripting action on sensor "+Element.SensorToWatchName+" - "+Element.ActorToSwitchName+" to "+Element.ActionToRunName);
                                            
                                            // check what action is going to happen now...
                                            if (Element.ActionToRunName == actor_status.On)
                                            {
                                                ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "ON", XS1_Configuration);
                                            }

                                            if (Element.ActionToRunName == actor_status.Off)
                                            {
                                                ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "OFF", XS1_Configuration);
                                                
                                                // remove from OnWaitOffList
                                                lock (OnWaitOffLIst)
                                                {
                                                    if (OnWaitOffLIst.Contains(Element.ActorToSwitchName))
                                                        OnWaitOffLIst.Remove(Element.ActorToSwitchName);
                                                }
                                            }

                                            if (Element.ActionToRunName == actor_status.OnOff)
                                            {
                                                // look for the current status in the known actors table
                                                lock(KnownActorStates.KnownActorStatuses)
                                                {
                                                    if (KnownActorStates.KnownActorStatuses.ContainsKey(Element.ActorToSwitchName))
                                                    {
                                                        current_actor_status Status = KnownActorStates.KnownActorStatuses[Element.ActorToSwitchName];
                                                        if (Status.Status == actor_status.On)
                                                            ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "OFF", XS1_Configuration);
                                                        else
                                                            if (Status.Status == actor_status.Off)
                                                                ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "ON", XS1_Configuration);
                                                    }
                                                    else
                                                        ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "ON", XS1_Configuration);
                                                }
                                            }
                                            if (Element.ActionToRunName == actor_status.OnWaitOff)
                                            {
                                                lock (OnWaitOffLIst)
                                                {
                                                    ConsoleOutputLogger.WriteLine("Adding " + Element.ActorToSwitchName + " to ActorReSwitching OnWaitOff List");
                                                    OnWaitOffLIst.Add(Element.ActorToSwitchName);
                                                }
                                                ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "ON_WAIT_OFF", XS1_Configuration);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                                if (dataobject.Type == ObjectTypes.Unknown)
                                {
                                    unknown_data_store.Write(dataobject.Serialize());
                                }

                        ConsoleOutputLogger.WriteLine(ServerName+" - "+dataobject.OriginalXS1Statement);
					}
					#endregion
                }
                catch (Exception)
                {                   
                    AcceptingCommands = false;
                    Thread.Sleep(1);
                }
            }

			Thread.Sleep (200);	// ... msecs period to wait for new input...
        }
    }
}
