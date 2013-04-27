using System;
using System.Collections.Generic;

namespace hacs
{
	public class SensorJSON
	{
		public string name { get; set; }
		public int id { get; set; }
		public string type { get; set; }
		public double value { get; set; }
		public Int64 utime { get; set; }
		public string unit { get; set; }
	}
	
	public class SensorJSON_Root
	{
		public int version { get; set; }
		public string type { get; set; }
		public int utc_offset { get; set; }
		public string dst { get; set; }
		public List<SensorJSON> sensor { get; set; }
	}
}

