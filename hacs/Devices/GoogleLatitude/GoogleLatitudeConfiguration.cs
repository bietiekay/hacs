using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace hacs
{
	#region google latitude configuration file json classes
	
	public class GoogleLatitudeAccount
	{
		public string Name { get; set; }
		public string GoogleLatitudeID { get; set; }
	}
	
	public class GoogleLatitudeConfigurationFile
	{
		public List<GoogleLatitudeAccount> GoogleLatitudeIDs { get; set; }
	}
	#endregion

	public class GoogleLatitudeConfiguration
	{
		public static GoogleLatitudeConfigurationFile GoogleLatitudeAccounts = new GoogleLatitudeConfigurationFile();

		public static void ReadConfiguration(String ConfigurationFilename)
		{
			if (File.Exists(ConfigurationFilename))
			{
				String jsonfile = File.ReadAllText(ConfigurationFilename);
                //Console.WriteLine(jsonfile);
				GoogleLatitudeAccounts = JsonConvert.DeserializeObject<GoogleLatitudeConfigurationFile>(jsonfile);
                //Console.WriteLine(GoogleLatitudeAccounts.GoogleLatitudeIDs.Count);
			}
		}
	}
}

