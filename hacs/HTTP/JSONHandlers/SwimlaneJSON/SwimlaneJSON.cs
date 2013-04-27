using System;
using System.Collections.Generic;

namespace hacs
{
	public class LaneJSON
	{
		public int id { get; set; }
		public string label { get; set; }
	}
	
	public class ItemJSON
	{
		public int id { get; set; }
		public int lane { get; set; }
		public string start { get; set; }
		public string end { get; set; }
		public string @class { get; set; }
		public string desc { get; set; }
	}
	
	public class SwimLaneRootObject
	{
		public List<LaneJSON> lanes { get; set; }
		public List<ItemJSON> items { get; set; }
	}
}

