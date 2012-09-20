using System;

namespace xs1_data_logging
{
	public class Sensor
	{
		public string name { get; set; }
		public int id { get; set; }
		public string type { get; set; }
		public double value { get; set; }
		public int utime { get; set; }
		public string unit { get; set; }
	}
	
	public class RootObject
	{
		public int version { get; set; }
		public string type { get; set; }
		public int utc_offset { get; set; }
		public string dst { get; set; }
		public List<Sensor> sensor { get; set; }
	}
}

