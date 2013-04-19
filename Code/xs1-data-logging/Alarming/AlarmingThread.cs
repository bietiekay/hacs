using System;
using System.Threading;
using System.Collections.Concurrent;
using sones.storage;
using hacs.xs1;
using SMS77;

namespace xs1_data_logging
{
	/// <summary>
	/// Here all the alarming takes place - you configure this in the AlarmingConfiguration.config file in finest JSON notation
	/// 
	/// Alarming works like that:
	/// 
	/// An alarm consists of the following ingredients:
	/// 	- activators
	/// 		- these are the sensor values that need to match for the alarm to go off if multiple are defined here only
	/// 		  one of them needs to match correctly for the alarm to be activated
	/// 		- an activator has 
	/// 			- a type (temperature, hygrometer, remotecontrol, dooropen, windowopen, pwr_consump, pwr_peak, waterdetector, 
	/// 			  shutter_contact, heating_thermostat, Ping, Pac, aPdc
	/// 			- a device (XS1, ELVMAX, GoogleLatitude, SolarLog, NetworkMonitor)
	/// 			- a name (freely given... case sensitive!
	/// 			- a value
	/// 			- a comparison type (==,<=,>=)
	/// 	- sensorchecks
	/// 		- these are the sensor values that need to match 100% for the alarm to go off after being activated by the activators
	/// 	- actorchecks
	/// 		- these are the actor values that need to match 100% for the alarm to go off after being activated by the activators
	/// 	- smsrecipients
	/// 		- Since Alarming currently only knows about SMS this basically is the list of phone numbers who shall receive a SMS
	/// </summary>
	public class AlarmingThread
	{
		public bool Shutdown = false;
		private ConsoleOutputLogger ConsoleOutputLogger;
		private ConcurrentQueue<IAlarmingEvent> Alarming_Queue; // use a thread safe list like structure to hold the event's going to be sent to alarming
		private SMS77Gateway SMSGateway;

		public AlarmingThread(ConsoleOutputLogger Logger, ConcurrentQueue<IAlarmingEvent> _AlarmQueue, TinyOnDiskStorage sensor_data, TinyOnDiskStorage actor_data, TinyOnDiskStorage latitude_data)
		{
			ConsoleOutputLogger = Logger;
			Alarming_Queue = _AlarmQueue;
			SMSGateway = new SMS77Gateway(Properties.Settings.Default.AlarmingSMS77Username,Properties.Settings.Default.AlarmingSMS77Password);
		}

		public void Run()
		{
			ConsoleOutputLogger.WriteLine("Alarming Thread started");
			while (!Shutdown)
			{
				try
				{
					IAlarmingEvent dataobject = null;
					if (Alarming_Queue.TryDequeue(out dataobject))
					{
						// we should get events from all sorts of devices here - let's take them and check if they
						// are eventually matching the activators - if they do we check against the other
						// data storages to follow up with the alarms...

						// check if we find this in the alarms...
						foreach(Alarm _alarm in AlarmingConfiguration.Alarms.Alarms)
						{
							foreach(Activator _activator in _alarm.activators)
							{
								// activator names are case sensitive!!!
								if (dataobject.AlarmingName() == _activator.name)
								{
									// we got a positive activator match here!
									// now check further circumstances...
									#region XS1 Events
									if ((_activator.device.ToUpper() == "XS1")&&(dataobject.AlarmingType() == AlarmingEventType.XS1Event))
									{
										// now we got an alarm triggering this activator and device...
										// check if the type matches...
										XS1_DataObject xs1_data = (XS1_DataObject)dataobject;

										if (_activator.type.ToUpper() == xs1_data.TypeName.ToUpper())
										{
											// for the value comparison convert the given value to a double...
											double comp_value = Convert.ToDouble(_activator.value);
											// it's the right typename...
											// now we gotta check for the right value there...
											Boolean alarm_activated = false;
											#region Activator Value Comparison
											switch(_activator.comparison)
											{
												case "==":
													if (comp_value == xs1_data.Value)
													{
														// hurray, everything matches! Activate this event!!!
														alarm_activated = true;
													}
													break;
												case "<=":
													if (comp_value <= xs1_data.Value)
													{
														// hurray, everything matches! Activate this event!!!
														alarm_activated = true;
													}
													break;
												case ">=":
													if (comp_value >= xs1_data.Value)
													{
														// hurray, everything matches! Activate this event!!!
														alarm_activated = true;
													}
													break;
											}
											#endregion

											#region do the sensor and actor checks...
											// TODO: this is very rough - needs to be worked out more later on... 
											// for the moment it is just a actor check - MORE HERE !!!!!!!!
											if (alarm_activated)
											{
												foreach(Actorcheck _actor in _alarm.actorchecks)
												{
													#region XS1 actors...
													if (_actor.device.ToUpper() == "XS1")
													{
														if (KnownActorStates.KnownActorStatuses.ContainsKey(_actor.name))
														{
															// there's an actor...
															current_actor_status status = (current_actor_status)KnownActorStates.KnownActorStatuses[_actor.name];
															// TODO: what about actor types!?

															if (_actor.value.ToUpper() == "ON")
															{
																// so it should be on...
																if (status.Status != actor_status.On)
																{
																	alarm_activated = false;
																}									
															}
															if (_actor.value.ToUpper() == "OFF")
															{
																// so it should be off...
																if (status.Status != actor_status.Off)
																{
																	alarm_activated = false;
																}									

															}
														}
													}
													#endregion
												}
											}
											#endregion

											if (alarm_activated)
											{
												ConsoleOutputLogger.WriteLine("!!!! ALARM - "+_alarm.name+" - ALARM !!!!");
												// send out the SMS...
												foreach(Smsrecipient recipient in _alarm.smsrecipients)
												{
													ConsoleOutputLogger.WriteLine("Sending Alarm SMS to "+recipient.number+" for alarm "+_alarm.name);
													SMSGateway.SendSMS(recipient.number,_alarm.message,Properties.Settings.Default.AlarmingSMS77SenderNumber);
												}
											}
										}
									}
									#endregion

									#region ELVMAX Events
									if ((_activator.device.ToUpper() == "ELVMAX")&&(dataobject.AlarmingType() == AlarmingEventType.ELVMAXEvent))
									{
                                        
                                        //ConsoleOutputLogger.WriteLine("ELVMAX: " + _activator.device);

										// now we got an alarm triggering this activator and device...
										// check if the type matches...
										IDeviceDiffSet diffset = (IDeviceDiffSet)dataobject;

                                        //ConsoleOutputLogger.WriteLine("ELVMAX: " + diffset.DeviceName);
                                        //ConsoleOutputLogger.WriteLine("ELVMAX: " + diffset.RoomName);
                                        //ConsoleOutputLogger.WriteLine("ELVMAX: " + _activator.type);
                                        //ConsoleOutputLogger.WriteLine("ELVMAX: " + _activator.name);


										// for now only shuttercontacts are interesting
										if ((_activator.type.ToUpper() == "SHUTTERCONTACT")&&(diffset.DeviceType == DeviceTypes.ShutterContact)&&
                                            (_activator.name == diffset.DeviceName))
										{
                                            //ConsoleOutputLogger.WriteLine("ELVMAX Shuttercontact");

											ShutterContactDiff shutterdiff = (ShutterContactDiff)diffset;

                                            //ConsoleOutputLogger.WriteLine("ELVMAX: "+shutterdiff.ToString());

											ShutterContactModes activatorstate = ShutterContactModes.unchanged;
											Boolean alarm_activated = false;

											if (_activator.value.ToUpper() == "OPEN")
												activatorstate = ShutterContactModes.open;

											if (_activator.value.ToUpper() == "CLOSED")
												activatorstate = ShutterContactModes.closed;

											if (activatorstate == shutterdiff.ShutterState)
												alarm_activated = true;

											#region do the sensor and actor checks...
											// TODO: this is very rough - needs to be worked out more later on... 
											// for the moment it is just a actor check - MORE HERE !!!!!!!!
											if (alarm_activated)
											{
												foreach(Actorcheck _actor in _alarm.actorchecks)
												{
													#region XS1 actors...
													if (_actor.device.ToUpper() == "XS1")
													{
														if (KnownActorStates.KnownActorStatuses.ContainsKey(_actor.name))
														{
															// there's an actor...
															current_actor_status status = (current_actor_status)KnownActorStates.KnownActorStatuses[_actor.name];
															// TODO: what about actor types!?
															
															if (_actor.value.ToUpper() == "ON")
															{
																// so it should be on...
																if (status.Status != actor_status.On)
																{
																	alarm_activated = false;
																}									
															}
															if (_actor.value.ToUpper() == "OFF")
															{
																// so it should be off...
																if (status.Status != actor_status.Off)
																{
																	alarm_activated = false;
																}									
																
															}
														}
													}
													#endregion
												}
											}
											#endregion
											
											if (alarm_activated)
											{
												ConsoleOutputLogger.WriteLine("!!!! ALARM - "+_alarm.name+" - ALARM !!!!");
												// send out the SMS...
												foreach(Smsrecipient recipient in _alarm.smsrecipients)
												{
													ConsoleOutputLogger.WriteLine("Sending Alarm SMS to "+recipient.number+" for alarm "+_alarm.name);
													SMSGateway.SendSMS(recipient.number,_alarm.message,Properties.Settings.Default.AlarmingSMS77SenderNumber);
												}
											}
											}
									}
									#endregion
									// TODO: ------------------------------
									#region SolarLog Events
									if ((_activator.device.ToUpper() == "SOLARLOG")&&(dataobject.AlarmingType() == AlarmingEventType.SolarLogEvent))
									{
										// now we got an alarm triggering this activator and device...
									}
									#endregion
									#region Network Monitor Events
									if ((_activator.device.ToUpper() == "NETWORKMONITOR")&&(dataobject.AlarmingType() == AlarmingEventType.NetworkingEvent))
									{
										// now we got an alarm triggering this activator and device...
									}
									#endregion
									#region Google Latitude Events
									if ((_activator.device.ToUpper() == "GOOGLELATITUDE")&&(dataobject.AlarmingType() == AlarmingEventType.GoogleLatitudeEvent))
									{
										// now we got an alarm triggering this activator and device...
									}
									#endregion
								}
							}
						}





					}

				}
				catch (Exception e)
				{                   
					Shutdown = true;
					ConsoleOutputLogger.WriteLine("Alarming Exception: "+e.Message);
					ConsoleOutputLogger.WriteLine("Stopping Alarming Execution!");
					Thread.Sleep(100);
				}
				
				Thread.Sleep(10);
			}
			
			
		}

	}
}

