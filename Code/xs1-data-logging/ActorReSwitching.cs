using System;
using System.Collections.Generic;
using System.Threading;
using hacs.xs1.configuration;

namespace xs1_data_logging
{	
	public class ActorReswitching
	{
		private bool _Shutdown = false;
        private XS1Configuration XS1_Configuration = null;
        private List<String> TemporaryBlacklist = null;
        private List<String> OnWaitOffList = null;

        public ActorReswitching(XS1Configuration configuration, List<String> BlackList, List<String> OWOList)
		{
            XS1_Configuration = configuration;
            TemporaryBlacklist = BlackList;
            OnWaitOffList = OWOList;
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
                if (SinceLastCheckpoint.TotalMinutes >= xs1_data_logging.Properties.Settings.Default.SwitchAgainCheckpointMinutes)
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
                            if ((DateTime.Now - status.LastUpdate).TotalMinutes <= xs1_data_logging.Properties.Settings.Default.SwitchAgainTimeWindowMinutes)
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
                                        ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, status.ActorName, "ON", XS1_Configuration);
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
                                        ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, status.ActorName, "OFF", XS1_Configuration);
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

