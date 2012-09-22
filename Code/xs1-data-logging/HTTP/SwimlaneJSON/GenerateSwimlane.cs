using System;
using sones.storage;

namespace xs1_data_logging
{
	public static class GenerateSwimlane
	{
		public static string Generate(MAXMonitoringThread ELVMAX,TinyOnDiskStorage SensorDataStore)
		{
			SwimLaneRootObject _root = new SwimLaneRootObject();

			_root.items = new System.Collections.Generic.List<ItemJSON>();
			_root.lanes = new System.Collections.Generic.List<LaneJSON>();

			#region fill the lanes
			// we need to have the data here
			// we shall have a cache (it's private for the other json methods... what about it?)
			// we need to have a selector which sensors need to be outputted...

			#endregion


			return "";
		}
	}
}

