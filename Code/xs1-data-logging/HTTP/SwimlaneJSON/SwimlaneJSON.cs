using System;

namespace xs1_data_logging
{
	public class Lane
	{
		public int id { get; set; }
		public string label { get; set; }
	}
	
	public class Item
	{
		public int id { get; set; }
		public int lane { get; set; }
		public string start { get; set; }
		public string end { get; set; }
		public string @class { get; set; }
		public string desc { get; set; }
	}
	
	public class RootObject
	{
		public List<Lane> lanes { get; set; }
		public List<Item> items { get; set; }
	}
}

