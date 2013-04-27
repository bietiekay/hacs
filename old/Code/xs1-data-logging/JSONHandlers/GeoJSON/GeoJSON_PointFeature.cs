using System;
using System.Collections.Generic;

namespace xs1_data_logging
{
	
	public class GeoJSON_PointFeature_Properties
	{
		public string popupContent { get; set; }

		public GeoJSON_PointFeature_Properties(String Content)
		{
			popupContent = Content;
		}
	}
	
	public class GeoJSON_PointFeature_Geometry
	{
		public string type { get; set; }
		public List<double> coordinates { get; set; }

		public GeoJSON_PointFeature_Geometry(double Latitude, double Longitude)
		{
			coordinates = new List<double>();
			coordinates.Add(Latitude);
			coordinates.Add(Longitude);
			type = "Point";
		}
	}
	
	public class GeoJSON_PointFeature_Feature
	{
		public string type { get; set; }
		public GeoJSON_PointFeature_Properties properties { get; set; }
		public GeoJSON_PointFeature_Geometry geometry { get; set; }

		public GeoJSON_PointFeature_Feature(double Latitude, double Longitude, String Content)
		{
			type = "Feature";
			properties = new GeoJSON_PointFeature_Properties(Content);
			geometry = new GeoJSON_PointFeature_Geometry(Latitude,Longitude);
		}
	}
	
	public class GeoJSON_PointFeature_Root
	{
		public string type { get; set; }
		public List<GeoJSON_PointFeature_Feature> features { get; set; }

		public GeoJSON_PointFeature_Root()
		{
			features = new List<GeoJSON_PointFeature_Feature>();
		}
	}
}

