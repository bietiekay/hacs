using System;
using System.Text;
using System.Net;

namespace SolarLogDebug
{
	public static class SolarLog
	{
		public static SolarLogDataSet UpdateSolarLog(String URL)
		{
			SolarLogDataSet Output = null;

			// create a web client and get the data
			String fullURL = "http://"+URL+"/min_cur.js?nocache";

			WebClient client = new WebClient ();

			try
			{
				String SolarLogValue = client.DownloadString(fullURL);

				// hurray, we got a string!
				// let's parse it!

				String[] LbL = SolarLogValue.Replace("\r","").Split(new char[] {'\n'});

				Output = new SolarLogDataSet();

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
					}
					#endregion
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("Exception: "+e.Message);
				return null;
			}

			return Output;
		}
	}
}

