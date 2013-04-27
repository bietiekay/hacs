using System;
using sones.storage;
using sones.Storage;
using hacs.xs1;

namespace hacs
{
	public static class GenerateSwimlane
	{
		private static XS1_DataObject ReadFromCache(TinyOnDiskStorage sensor_data, OnDiscAdress adress)
		{
			XS1_DataObject dataobject = null;
			
			object cacheditem = sensor_data.Cache.ReadFromCache(adress);
			if (cacheditem == null)
			{
				// not found in cache, read from disk and add to cache
				dataobject.Deserialize(sensor_data.Read(adress));
				sensor_data.Cache.AddToCache(adress,dataobject);
			}
			else
			{
				// found in cache, take it...
				dataobject = (XS1_DataObject)cacheditem;
			}
			
			return dataobject;
		}

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
							// we are in the right timespan
							// is this the right sensor?
							XS1_DataObject dataobject = ReadFromCache(SensorDataStore,ondisc);

							if (dataobject.TypeName == ObjectTypeName)
							{
								if (dataobject.Name == ObjectName)
								{
									// okay we got what we want...
								}
							}
						}
					}
				}
			}
			#endregion

			return "";
		}
	}
}

