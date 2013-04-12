﻿using System;
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
		TinyOnDiskStorage latitude_data_store = null;
        XS1Configuration XS1_Configuration = null;
        Int32 ConfigurationCacheMinutes;
        public Boolean AcceptingCommands = false;
        public List<String> TemporaryBlacklist = new List<string>();
        public List<String> OnWaitOffLIst = new List<string>();
        public ConsoleOutputLogger ConsoleOutputLogger;
		private ConcurrentQueue<XS1_DataObject> XS1_DataQueue;	// use a thread safe list like structure to hold the messages coming in from the XS1
		private ConcurrentQueue<IDeviceDiffSet> MAX_DataQueue;  // use a thread safe list like structure to hold the messages coming in from the ELV MAX
		private ConcurrentQueue<SolarLogDataSet> SolarLog_DataQueue; // use a thread safe list like structure to hold the messages coming in from the SolarLog
		private ConcurrentQueue<NetworkMonitoringDataSet> NetworkMonitor_Queue; // use a thread safe list like structure to hold the messages coming in from the NetworkMonitor
        bool Shutdown = false;

		#region Ctor
        public LoggingThread(String _ServerName, ConsoleOutputLogger Logger,TinyOnDiskStorage _actor_store, TinyOnDiskStorage _sensor_store, TinyOnDiskStorage _unknown_store, TinyOnDiskStorage _latitude_store, String _Username, String _Password, Int32 _ConfigurationCacheMinutes)
        {
            actor_data_store = _actor_store;
            sensor_data_store = _sensor_store;
            unknown_data_store = _unknown_store;
			latitude_data_store = _latitude_store;
            ServerName = _ServerName;
            UserName = _Username;
            Password = _Password;
            ConfigurationCacheMinutes = _ConfigurationCacheMinutes;
            ConsoleOutputLogger = Logger;
			XS1_DataQueue = new ConcurrentQueue<XS1_DataObject>();
			MAX_DataQueue = new ConcurrentQueue<IDeviceDiffSet>();
            SolarLog_DataQueue = new ConcurrentQueue<SolarLogDataSet>();
            NetworkMonitor_Queue = new ConcurrentQueue<NetworkMonitoringDataSet>();
        }
		#endregion

        public void Run()
        {
            // initialize XS1 Configuration
            XS1_Configuration = new XS1Configuration(ConfigurationCacheMinutes);
			MAXMonitoringThread ELVMax = null;
			SolarLogMonitoringThread SolarLog = null;

			// Start Sensor-Check Thread
            SensorCheck Sensorcheck = new SensorCheck(ConsoleOutputLogger);
            Thread SensorCheckThread = new Thread(new ThreadStart(Sensorcheck.Run));
			SensorCheckThread.Start();

			// Start Actor Re-Switching Thread
            ActorReswitching ActorReSwitch_ = new ActorReswitching(XS1_Configuration, ConsoleOutputLogger, TemporaryBlacklist,OnWaitOffLIst);
            Thread ActorReswitchThread = new Thread(new ThreadStart(ActorReSwitch_.Run));
            ActorReswitchThread.Start();

			// Start the SolarLog Thread (if enabled)
			// TODO: make it switchable / configurable
			if (Properties.Settings.Default.SolarLogEnabled)
			{
				SolarLog = new SolarLogMonitoringThread(Properties.Settings.Default.SolarLogURLDomain,ConsoleOutputLogger,SolarLog_DataQueue,Properties.Settings.Default.SolarLogUpdateIntervalMsec);
				Thread SolarLogThread = new Thread(new ThreadStart(SolarLog.Run));
				SolarLogThread.Start();
			}

			// Start the ELVMax Thread
			if (Properties.Settings.Default.ELVMAXEnabled)
			{
				ELVMax = new MAXMonitoringThread(Properties.Settings.Default.ELVMAXIP,Properties.Settings.Default.ELVMAXPort,ConsoleOutputLogger,MAX_DataQueue,Properties.Settings.Default.ELVMAXUpdateIntervalMsec);
				Thread ELVMaxThread = new Thread(new ThreadStart(ELVMax.Run));
				ELVMaxThread.Start();
			}

			XS1MonitoringThread XS1 = new XS1MonitoringThread(ServerName,ConsoleOutputLogger,UserName,Password,XS1_DataQueue);
			Thread XS1Thread = new Thread(new ThreadStart(XS1.Run));
			XS1Thread.Start();

            // Start integrated HTTP Server
            HttpServer httpServer = new HttpServer(Properties.Settings.Default.HTTPPort, Properties.Settings.Default.HTTPIP, Properties.Settings.Default.HTTPDocumentRoot, sensor_data_store, latitude_data_store, XS1_Configuration, ConsoleOutputLogger, ELVMax);
            Thread http_server_thread = new Thread(new ThreadStart(httpServer.listen));
            http_server_thread.Start();

			// Start Service Monitorng thread
            if (Properties.Settings.Default.NetworkMonitorEnabled)
            {
			    NetworkMonitoring monitor = new NetworkMonitoring(ConsoleOutputLogger,NetworkMonitor_Queue,Properties.Settings.Default.NetworkMonitorUpdateIntervalMsec);
			    Thread monitorThread = new Thread(new ThreadStart(monitor.Run));
			    monitorThread.Start();
            }

			// Start Alarming thread
            if (Properties.Settings.Default.AlarmingEnabled)
            {
			    AlarmingThread alarmThread = new AlarmingThread(ConsoleOutputLogger);
			    Thread alarming_thread = new Thread(new ThreadStart(alarmThread.Run));
			    alarming_thread.Start();
            }

			// Start Google Latitude Thread
			if (Properties.Settings.Default.GoogleLatitudeEnabled)
            {
				GoogleLatitudeThread googleLatitudeThread = new GoogleLatitudeThread(ConsoleOutputLogger,latitude_data_store,Properties.Settings.Default.GoogleLatitudeUpdateTime);
			    Thread googleLatitude_Thread = new Thread(new ThreadStart(googleLatitudeThread.Run));
			    googleLatitude_Thread.Start();
            }

	        while (!Shutdown)
            {
                try
                {
					#region Handle XS1 events
					XS1_DataObject dataobject = null;
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
									// this actor action did not result in any action hacs made - so we try to make the best out of it
									#region Scripting Handling
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
												ConsoleOutputLogger.WriteLineToScreenOnly("detected actor scripting action on actor "+Element.SensorToWatchName+" - "+Element.ActorToSwitchName+" to "+Element.ActionToRunName);
												
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
									#endregion


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

					#region Handle MAX events
					// the strategy for MAX events is quite easy: emulate XS1 events and stuff the XS1 queue with those faked events
					// that takes care of the storage and the
					if (Properties.Settings.Default.ELVMAXEnabled)
					{
						IDeviceDiffSet max_dataobject = null;

						if(MAX_DataQueue.TryDequeue(out max_dataobject))
					   	{
							StringBuilder sb = new StringBuilder();

							sb.Append("S\t"+max_dataobject.DeviceName+"\t"+max_dataobject.DeviceType);

							if (max_dataobject.DeviceType == DeviceTypes.HeatingThermostat)
							{
								HeatingThermostatDiff _heating = (HeatingThermostatDiff)max_dataobject;

								// this is what is different on the heating thermostats
								//ConsoleOutputLogger.WriteLine(_heating.ToString());

								// first the temperature data
								XS1_DataObject maxdataobject = new XS1_DataObject(Properties.Settings.Default.ELVMAXIP,_heating.RoomName+"-"+_heating.DeviceName,ObjectTypes.Sensor,"heating_thermostat",DateTime.Now,_heating.RoomID,_heating.Temperature,_heating.ToString());
								SensorCheckIgnoreConfiguration.AddToIgnoreList(maxdataobject.Name);
								XS1_DataQueue.Enqueue(maxdataobject);

								// then the low battery if exists
								if (_heating.LowBattery == BatteryStatus.lowbattery)
								{
									XS1_DataObject lowbatteryobject = new XS1_DataObject(Properties.Settings.Default.ELVMAXIP,_heating.RoomName+"-"+_heating.DeviceName,ObjectTypes.Sensor,"low_battery",DateTime.Now,_heating.RoomID,_heating.Temperature,_heating.ToString()+", LowBattery");
									SensorCheckIgnoreConfiguration.AddToIgnoreList(lowbatteryobject.Name);
									XS1_DataQueue.Enqueue(lowbatteryobject);
								}

								if (_heating.Mode == ThermostatModes.boost)
								{
									XS1_DataObject boostmodeobject = new XS1_DataObject(Properties.Settings.Default.ELVMAXIP,_heating.RoomName+"-"+_heating.DeviceName,ObjectTypes.Sensor,"boost",DateTime.Now,_heating.RoomID,_heating.Temperature, _heating.ToString()+", Boost");
									SensorCheckIgnoreConfiguration.AddToIgnoreList(boostmodeobject.Name);
									XS1_DataQueue.Enqueue(boostmodeobject);
								}
							}

							if (max_dataobject.DeviceType == DeviceTypes.ShutterContact)
							{
								ShutterContactDiff _shutter = (ShutterContactDiff)max_dataobject;

								// this is what is different on the ShutterContacts
								//ConsoleOutputLogger.WriteLine(_shutter.ToString());

								// first the open/close status
								if (_shutter.ShutterState == ShutterContactModes.open)
								{
									XS1_DataObject maxdataobject = new XS1_DataObject(Properties.Settings.Default.ELVMAXIP,_shutter.RoomName+"-"+_shutter.DeviceName,ObjectTypes.Sensor,"shutter_contact",DateTime.Now,_shutter.RoomID,1.0,_shutter.ToString());
									SensorCheckIgnoreConfiguration.AddToIgnoreList(maxdataobject.Name);
									XS1_DataQueue.Enqueue(maxdataobject);
								}
								else
								{
									XS1_DataObject maxdataobject = new XS1_DataObject(Properties.Settings.Default.ELVMAXIP,_shutter.RoomName+"-"+_shutter.DeviceName,ObjectTypes.Sensor,"shutter_contact",DateTime.Now,_shutter.RoomID,0.0,_shutter.ToString());
									SensorCheckIgnoreConfiguration.AddToIgnoreList(maxdataobject.Name);
									XS1_DataQueue.Enqueue(maxdataobject);
								}

								// then the low battery if exists
								if (_shutter.LowBattery == BatteryStatus.lowbattery)
								{
									if (_shutter.ShutterState == ShutterContactModes.open)
									{
										XS1_DataObject lowbatteryobject = new XS1_DataObject(Properties.Settings.Default.ELVMAXIP,_shutter.RoomName+"-"+_shutter.DeviceName,ObjectTypes.Sensor,"low_battery",DateTime.Now,_shutter.RoomID,1.0,_shutter.ToString()+",LowBattery");
										SensorCheckIgnoreConfiguration.AddToIgnoreList(lowbatteryobject.Name);
										XS1_DataQueue.Enqueue(lowbatteryobject);
									}
									else
									{
										XS1_DataObject lowbatteryobject = new XS1_DataObject(Properties.Settings.Default.ELVMAXIP,_shutter.RoomName+"-"+_shutter.DeviceName,ObjectTypes.Sensor,"low_battery",DateTime.Now,_shutter.RoomID,0.0,_shutter.ToString()+",LowBattery");
										SensorCheckIgnoreConfiguration.AddToIgnoreList(lowbatteryobject.Name);
										XS1_DataQueue.Enqueue(lowbatteryobject);
									}
								}

							}
						}
					}
					#endregion

					#region Handle SolarLog events
					if (Properties.Settings.Default.SolarLogEnabled)
					{
						SolarLogDataSet solarlog_dataobject = null;
						
						if(SolarLog_DataQueue.TryDequeue(out solarlog_dataobject))
						{
							// Pac
                            XS1_DataQueue.Enqueue(new XS1_DataObject(Properties.Settings.Default.SolarLogURLDomain, "Pac", ObjectTypes.Sensor, "Pac", solarlog_dataobject.DateAndTime, 1, solarlog_dataobject.Pac, "solarlog," + Properties.Settings.Default.SolarLogURLDomain + ",Pac," + solarlog_dataobject.Pac + "," + solarlog_dataobject.DateAndTime.Ticks));
							// aPdc
                            XS1_DataQueue.Enqueue(new XS1_DataObject(Properties.Settings.Default.SolarLogURLDomain, "aPdc", ObjectTypes.Sensor, "aPdc", solarlog_dataobject.DateAndTime, 1, solarlog_dataobject.aPdc, "solarlog," + Properties.Settings.Default.SolarLogURLDomain + ",aPdc," + solarlog_dataobject.aPdc+","+solarlog_dataobject.DateAndTime.Ticks));
						}
					}
					#endregion

					#region Handle Network Monitor events
					if (Properties.Settings.Default.NetworkMonitorEnabled)
					{
						NetworkMonitoringDataSet networkmonitor_dataobject = null;
						
						if(NetworkMonitor_Queue.TryDequeue(out networkmonitor_dataobject))
						{
							if (networkmonitor_dataobject.Status == Org.Mentalis.Network.ICMP_Status.Success)
								XS1_DataQueue.Enqueue(new XS1_DataObject(networkmonitor_dataobject.Descriptor,networkmonitor_dataobject.HostnameIP,ObjectTypes.Sensor,"Ping",networkmonitor_dataobject.TimeOfMeasurement,2,1,"OnlineCheck,"+networkmonitor_dataobject.HostnameIP+","+networkmonitor_dataobject.Descriptor+",Online"));
							else
								XS1_DataQueue.Enqueue(new XS1_DataObject(networkmonitor_dataobject.Descriptor,networkmonitor_dataobject.HostnameIP,ObjectTypes.Sensor,"Ping",networkmonitor_dataobject.TimeOfMeasurement,2,0,"OnlineCheck,"+networkmonitor_dataobject.HostnameIP+","+networkmonitor_dataobject.Descriptor+",Offline"));
						}
					}
					#endregion
                }
                catch (Exception)
                {                   
                    AcceptingCommands = false;
                    //Thread.Sleep(1);
                }
                Thread.Sleep(1);
            }
			if (ELVMax != null)
				ELVMax.running = false;
			XS1.running = false;

			Thread.Sleep (200);	// ... msecs period to wait for new input...
        }
    }
}
