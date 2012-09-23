using System;
using sones.storage;
using sones.Storage;

namespace xs1_data_logging
{
	public static class GenerateSwimlane
	{
		public static string Generate(MAXMonitoringThread ELVMAX,TinyOnDiskStorage SensorDataStore, String ObjectName, String ObjectTypeName, DateTime StartDateTime, DateTime EndDateTime)
		{
			SwimLaneRootObject _root = new SwimLaneRootObject();

			_root.items = new System.Collections.Generic.List<ItemJSON>();
			_root.lanes = new System.Collections.Generic.List<LaneJSON>();

			#region fill the lanes
			// we need to have the data here
			// we shall have a cache (it's private for the other json methods... what about it?)
			// we need to have a selector which sensors need to be outputted...
			lock (SensorDataStore.InMemoryIndex)
			{
				foreach (OnDiscAdress ondisc in SensorDataStore.InMemoryIndex)
				{
					if (ondisc.CreationTime >= StartDateTime.Ticks)
					{
						if (ondisc.CreationTime <= EndDateTime.Ticks)
						{

						}
					}
				}
			}


			#endregion


			return "";
		}
	}
}

