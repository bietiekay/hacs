using System;
using sones.storage;

namespace xs1_data_logging
{
	public class Geolocation
	{
		private TinyOnDiskStorage latitude_data;
		private ConsoleOutputLogger ConsoleOutputLogger_;
		
		public Geolocation(TinyOnDiskStorage sensor_data_storage, ConsoleOutputLogger Logger)
		{
			latitude_data = sensor_data_storage;
			ConsoleOutputLogger_ = Logger;
		}

		public String GenerateJSON_LastEntry(String ObjectName)
		{
			if (latitude_data == null)
				return "";

			// todo: output current location for ObjectName here !!!

			return "";
		}
	}
}

