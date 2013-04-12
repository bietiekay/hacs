using System;
using System.Collections.Generic;

namespace xs1_data_logging
{
	
	public class GeoJSON_PointFeature_Properties
	{
		public string popupContent { get; set; }
	}
	
	public class GeoJSON_PointFeature_Geometry
	{
		public string type { get; set; }
		public List<double> coordinates { get; set; }
	}
	
	public class GeoJSON_PointFeature_Feature
	{
		public string type { get; set; }
		public GeoJSON_PointFeature_Properties properties { get; set; }
		public GeoJSON_PointFeature_Geometry geometry { get; set; }
	}
	
	public class GeoJSON_PointFeature_Root
	{
		public string type { get; set; }
		public List<GeoJSON_PointFeature_Feature> features { get; set; }
	}
}

