using System;
using System.Collections.Generic;

namespace hacs
{
	public class FunctionJSON
	{
		public string type { get; set; }
		public string dsc { get; set; }
	}
	
	public class ActuatorJSON
	{
		public string name { get; set; }
		public int id { get; set; }
		public string type { get; set; }
		public double value { get; set; }
		public Int64 utime { get; set; }
		public string unit { get; set; }
		public List<FunctionJSON> function { get; set; }
	}
	
	public class ActorJSON_Root
	{
		public int version { get; set; }
		public string type { get; set; }
		public int utc_offset { get; set; }
		public string dst { get; set; }
		public List<ActuatorJSON> actuator { get; set; }
	}
}

