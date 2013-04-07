using System;
using System.Collections.Generic;

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
	}
	
	public class Sensorcheck
	{
		public string name { get; set; }
		public string value { get; set; }
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
	}
	
	#endregion

	public class Alarming
	{
		public Alarming ()
		{
		}
	}
}

