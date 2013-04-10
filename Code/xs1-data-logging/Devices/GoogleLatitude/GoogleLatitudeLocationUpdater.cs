using System;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace xs1_data_logging
{
	public static class GoogleLatitudeLocationUpdater
	{
		public static GoogleLatitudeDataObject UpdateLatitudeLocation(String AccountName, String LatitudeID, ConsoleOutputLogger COL)
		{
			GoogleLatitudeLocationData LocationData;
			GoogleLatitudeDataObject Output = null;

			try
			{
				// create a web client and get the data
                String fullURL = "http://www.google.com/latitude/apps/badge/api?user=" + LatitudeID + "&type=json";
				WebClient client = new WebClient ();

				String Value = client.DownloadString(fullURL);

				// hurray, we got a string!
				// let's parse it!

				LocationData = JsonConvert.DeserializeObject<GoogleLatitudeLocationData>(Value);			

				LatitudeProperties props = LocationData.features[0].properties;
				Geometry geo = LocationData.features[0].geometry;

				Output = new GoogleLatitudeDataObject(AccountName,LatitudeID,props.timeStamp,props.reverseGeocode,geo.coordinates[0],geo.coordinates[1],props.accuracyInMeters);
			}
			catch(Exception e)
			{
				COL.WriteLine("Google Latitude Exception: "+e.Message);
				return null;
			}

			return Output;
		}
	}
}

