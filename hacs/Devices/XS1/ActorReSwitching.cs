using System;
using System.Collections.Generic;
using System.Threading;
using hacs.xs1.configuration;

namespace hacs
{	
	public class ActorReswitching
	{
		private bool _Shutdown = false;
        private XS1Configuration XS1_Configuration = null;
        private List<String> TemporaryBlacklist = null;
        private List<String> OnWaitOffList = null;
        private ConsoleOutputLogger ConsoleOutputLogger;

        public ActorReswitching(XS1Configuration configuration, ConsoleOutputLogger Logger, List<String> BlackList, List<String> OWOList)
		{
            XS1_Configuration = configuration;
            TemporaryBlacklist = BlackList;
            OnWaitOffList = OWOList;
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
            ConsoleOutputLogger.WriteLine("Starting Actor Re-Switching.");
            DateTime LastCheckpoint = DateTime.Now;
            TimeSpan SinceLastCheckpoint = new TimeSpan();
            while (!_Shutdown)
            {
                // we start by
                // checking how much time passed since we were here the last time
                SinceLastCheckpoint = DateTime.Now - LastCheckpoint;
                //Console.WriteLine(SinceLastCheckpoint.TotalMinutes);
                // now we can do something every minute or so
                if (SinceLastCheckpoint.TotalMinutes >= hacs.Properties.Settings.Default.SwitchAgainCheckpointMinutes)
                {
                    //ConsoleOutputLogger.WriteLine("Sensor Checkpoint");
                    LastCheckpoint = DateTime.Now;
                    #region Switch Actors again with same state
                    // go through all the known actor status codes and try to send
                    // them again to the actor one after another
                    
                    // TODO: this try is just here to handle enum exceptions just in case, introduce locking!
                    try
                    {
                        foreach (current_actor_status status in KnownActorStates.KnownActorStatuses.Values)
                        {
                            // if this actor was switched within the last configured minutes we switch it again to the exact same
                            // state, just to make sure that they were successfully switched (just ON/OFF states)
                            if ((DateTime.Now - status.LastUpdate).TotalMinutes <= hacs.Properties.Settings.Default.SwitchAgainTimeWindowMinutes)
                            {
                                bool ignorethisone = false;
                                lock(OnWaitOffList)
                                {
                                    if (OnWaitOffList.Contains(status.ActorName))
                                    {
                                        //Console.WriteLine(status.ActorName+" is on the ignorelist");
                                        ignorethisone = true;
                                    }
                                }
                                
                                if (!ignorethisone)
                                {
                                    ConsoleOutputLogger.WriteLine("Switching again actor " + status.ActorName + "(" + (DateTime.Now - status.LastUpdate).TotalMinutes + ")");
                                    // yes, within the last given number of minutes
                                    set_state_actuator.set_state_actuator ssa = new set_state_actuator.set_state_actuator();
                                    #region ON state
                                    if (status.Status == actor_status.On)
                                    {
                                        // set on temporary blacklist
                                        lock (TemporaryBlacklist)
                                        {
                                            TemporaryBlacklist.Add(status.ActorName);
                                        }
                                        ssa.SetStateActuatorPreset(hacs.Properties.Settings.Default.XS1, hacs.Properties.Settings.Default.Username, hacs.Properties.Settings.Default.Password, status.ActorName, "ON", XS1_Configuration);
                                    }
                                    #endregion

                                    #region OFF state
                                    if (status.Status == actor_status.Off)
                                    {
                                        // set on temporary blacklist
                                        lock (TemporaryBlacklist)
                                        {
                                            TemporaryBlacklist.Add(status.ActorName);
                                        }
                                        ssa.SetStateActuatorPreset(hacs.Properties.Settings.Default.XS1, hacs.Properties.Settings.Default.Username, hacs.Properties.Settings.Default.Password, status.ActorName, "OFF", XS1_Configuration);
                                    }
                                    #endregion
                                }
                            }
                            Thread.Sleep(2000);
                        }
                    }
                    catch(Exception)
                    {
                    }
                    #endregion
                }


				Thread.Sleep(10000); // just check every 10 seconds...
			}
		}
	}
}

