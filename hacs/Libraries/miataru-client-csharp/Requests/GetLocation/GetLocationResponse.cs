using System;
using System.Collections.Generic;


namespace miataruclientcsharp
{
	public class MiataruLocation
	{
		public string Device { get; set; }
		public string Timestamp { get; set; }
		public string Longitude { get; set; }
		public string Latitude { get; set; }
		public string HorizontalAccuracy { get; set; }
	}

	public class MiataruNoLocation
	{
		public string Device { get; set; }
	}

	public class GetLocationResponse
	{
		public List<MiataruLocation> MiataruLocation { get; set; }
		public List<MiataruNoLocation> MiataruNoLocation { get; set; }
	}
}

