using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace xs1_data_logging
{
	#region alarm configuration file json classes
	
	public class Smsrecipient
	{
		public string number { get; set; }
	}
	
	public class Activator
	{
		public string name { get; set; }
		public string value { get; set; }
		public string type { get; set; }
	}
	
	public class Sensorcheck
	{
		public string name { get; set; }
		public string value { get; set; }
		public string type { get; set; }
	}
	
	public class Actorcheck
	{
		public string name { get; set; }
		public string value { get; set; }
	}
	
	public class Alarm
	{
		public string name { get; set; }
		public string message { get; set; }
		public List<Smsrecipient> smsrecipients { get; set; }
		public List<Activator> activators { get; set; }
		public List<Sensorcheck> sensorchecks { get; set; }
		public List<Actorcheck> actorchecks { get; set; }
	}
	
	public class AlarmConfiguration
	{
		public List<Alarm> Alarms { get; set; }

		public AlarmConfiguration()
		{
			Alarms = new List<Alarm>();
		}
	}
	
	#endregion

	public class AlarmingConfiguration
	{
		public static AlarmConfiguration Alarms = new AlarmConfiguration();

		public static void ReadConfiguration(String ConfigurationFilename)
		{
			if (File.Exists(ConfigurationFilename))
			{
				String jsonfile = File.ReadAllText(ConfigurationFilename);
				Alarms = JsonConvert.DeserializeObject<AlarmConfiguration>(jsonfile);				
			}
		}
	}
}

