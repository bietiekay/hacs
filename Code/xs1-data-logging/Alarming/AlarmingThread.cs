using System;
using System.Threading;
using System.Collections.Concurrent;
using sones.storage;

namespace xs1_data_logging
{
	public class AlarmingThread
	{
		public bool Shutdown = false;
		private ConsoleOutputLogger ConsoleOutputLogger;
		private ConcurrentQueue<IAlarmingEvent> Alarming_Queue; // use a thread safe list like structure to hold the event's going to be sent to alarming

		public AlarmingThread(ConsoleOutputLogger Logger, ConcurrentQueue<IAlarmingEvent> _AlarmQueue, TinyOnDiskStorage sensor_data, TinyOnDiskStorage actor_data, TinyOnDiskStorage latitude_data)
		{
			ConsoleOutputLogger = Logger;
			Alarming_Queue = _AlarmQueue;
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
						#region XS1 Events
						if (dataobject.AlarmingType() == AlarmingEventType.XS1Event)
						{
						}
						#endregion

						#region ELVMAX Events
						if (dataobject.AlarmingType() == AlarmingEventType.ELVMAXEvent)
						{
						}
						#endregion

						#region SolarLog Events
						if (dataobject.AlarmingType() == AlarmingEventType.SolarLogEvent)
						{
						}
						#endregion

						#region Network Monitor Events
						if (dataobject.AlarmingType() == AlarmingEventType.NetworkingEvent)
						{
						}
						#endregion

						#region Google Latitude Events
						if (dataobject.AlarmingType() == AlarmingEventType.GoogleLatitudeEvent)
						{
						}
						#endregion
					}

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

