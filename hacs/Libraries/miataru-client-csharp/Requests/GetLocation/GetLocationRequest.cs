using System;
using System.Collections.Generic;

namespace miataruclientcsharp
{

	public class MiataruGetLocation
	{
		public string Device { get; set; }

		public MiataruGetLocation(String DeviceID)
		{
			Device = DeviceID;
		}
	}

	public class GetLocationRequest
	{
		public List<MiataruGetLocation> MiataruGetLocation { get; set; }

		public GetLocationRequest(String DeviceID)
		{
			MiataruGetLocation = new List<MiataruGetLocation> ();
			MiataruGetLocation.Add (new MiataruGetLocation (DeviceID));
		}
	}
}

