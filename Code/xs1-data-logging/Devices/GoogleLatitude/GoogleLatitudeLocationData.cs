using System;
using System.Collections.Generic;

namespace xs1_data_logging
{
	#region JSON

	public class Geometry
	{
		public string type { get; set; }
		public List<double> coordinates { get; set; }
	}
	
	public class LatitudeProperties
	{
		public string id { get; set; }
		public int accuracyInMeters { get; set; }
		public long timeStamp { get; set; }
		public string reverseGeocode { get; set; }
		public string photoUrl { get; set; }
		public int photoWidth { get; set; }
		public int photoHeight { get; set; }
		public string placardUrl { get; set; }
		public int placardWidth { get; set; }
		public int placardHeight { get; set; }
	}
	
	public class Feature
	{
		public string type { get; set; }
		public Geometry geometry { get; set; }
		public LatitudeProperties properties { get; set; }
	}
	
	public class GoogleLatitudeLocationData
	{
		public string type { get; set; }
		public List<Feature> features { get; set; }
	}

	#endregion
}

