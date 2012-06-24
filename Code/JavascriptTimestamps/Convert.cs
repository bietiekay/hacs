using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaScriptTimeStampExtension
{
    public static class Extensions
    {
        public static Int64 JavaScriptTimestamp(this DateTime myDateTime)
        {
            DateTime unixstarted = new DateTime(1970, 1, 1);  // reference date
            return (Convert.ToInt64((myDateTime - unixstarted).TotalSeconds * 1000 ));
        }

        public static DateTime ParseJavaScriptTimestamp(Int64 JavaScriptTimestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(JavaScriptTimestamp / 1000);
        }
    }
}
