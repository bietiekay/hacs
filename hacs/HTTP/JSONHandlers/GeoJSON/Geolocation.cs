using System;
using sones.storage;
using sones.Storage;
using System.Linq;
using Newtonsoft.Json;

namespace hacs
{
	/*	public class Geolocation
	{
		private TinyOnDiskStorage latitude_data;
		//private ConsoleOutputLogger ConsoleOutputLogger_;
		
		public Geolocation(TinyOnDiskStorage sensor_data_storage, ConsoleOutputLogger Logger)
		{
			latitude_data = sensor_data_storage;
			//ConsoleOutputLogger_ = Logger;
		}

		private MiataruDataObject ReadFromCache(OnDiscAdress adress)
		{
			MiataruDataObject dataobject = new MiataruDataObject();
			
			object cacheditem = latitude_data.Cache.ReadFromCache(adress);
			if (cacheditem == null)
			{
				// not found in cache, read from disk and add to cache
				dataobject.Deserialize(latitude_data.Read(adress));
				latitude_data.Cache.AddToCache(adress,dataobject);
			}
			else
			{
				// found in cache, take it...
				dataobject = (MiataruDataObject)cacheditem;
			}
			
			return dataobject;
		}

		public String GenerateJSON_LastEntry(String ObjectName)
		{
			if (latitude_data == null)
				return "";

			MiataruDataObject pLatitudeValue = null;

			// todo: output current location for ObjectName here !!!
			lock (latitude_data.InMemoryIndex)
			{
				foreach (OnDiscAdress ondisc in latitude_data.InMemoryIndex.Reverse<OnDiscAdress>())
				{
					MiataruDataObject dataobject = ReadFromCache(ondisc);

					if (dataobject.AccountName.ToUpper() == ObjectName.ToUpper())
					{
						pLatitudeValue = dataobject;
						break;
					}
				}
			}

			if (pLatitudeValue != null)
			{
				GeoJSON_PointFeature_Root root = new GeoJSON_PointFeature_Root();

				GeoJSON_PointFeature_Feature feature = new GeoJSON_PointFeature_Feature(pLatitudeValue.Latitude,pLatitudeValue.Longitude,pLatitudeValue.AccountName+"@"+pLatitudeValue.reverseGeocode);
				root.features.Add(feature);

				return JsonConvert.SerializeObject(root);
			}
			else
				return "not found";
		}
	}*/
}