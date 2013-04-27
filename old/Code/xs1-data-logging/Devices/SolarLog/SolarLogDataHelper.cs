using System;
using System.Text;
using System.Net;

namespace xs1_data_logging
{
	public static class SolarLogDataHelper
	{
		public static SolarLogDataSet UpdateSolarLog(String URL, ConsoleOutputLogger COL)
		{
			SolarLogDataSet Output = new SolarLogDataSet();;

			// create a web client and get the data
			String fullURL = "http://"+URL+"/min_cur.js?nocache";

			WebClient client = new WebClient ();

			try
			{
				String SolarLogValue = client.DownloadString(fullURL);

				// hurray, we got a string!
				// let's parse it!

				String[] LbL = SolarLogValue.Replace("\r","").Split(new char[] {'\n'});

				foreach(String _line in LbL)
				{
					#region Pac
					if (_line.StartsWith("var Pac="))
					{
						Output.Pac = Convert.ToInt32(_line.Replace("var Pac=",""));
					}
					#endregion

					#region aPdc
					if (_line.StartsWith("var aPdc="))
					{
						String firstpart_removed = _line.Replace("var aPdc=new Array(","");
						Output.aPdc = Convert.ToInt32( firstpart_removed.Remove(firstpart_removed.IndexOf(',')));
					}
					#endregion
				}
			}
			catch(Exception e)
			{
				COL.WriteLine("SolarLog Exception: "+e.Message);
				return null;
			}

			return Output;
		}
	}
}

