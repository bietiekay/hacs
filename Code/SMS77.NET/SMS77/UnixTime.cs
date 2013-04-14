using System;
using System.Collections.Generic;
using System.Text;

namespace UnixTime
{
	public static class Extensions
	{
		public static Int64 UnixTimestamp(this DateTime myDateTime)
		{
			DateTime unixstarted = new DateTime(1970, 1, 1); // reference date
			return (Convert.ToInt64((myDateTime - unixstarted).TotalSeconds * 1000));
		}
		
		public static Int64 UnixTimestampNonMsec(this DateTime myDateTime)
		{
			DateTime unixstarted = new DateTime(1970, 1, 1); // reference date
			return (Convert.ToInt64((myDateTime.ToUniversalTime() - unixstarted).TotalSeconds));
		}
		
		
		public static DateTime ParseUnixTimestamp(Int64 UnixTimestamp)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return origin.AddSeconds(UnixTimestamp / 1000);
		}
	}
}