using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace xs1_data_logging
{
	#region google latitude configuration file json classes
	
	public class GoogleLatitudeAccount
	{
		public string Name { get; set; }
		public string GoogleLatitudeID { get; set; }
	}
	
	public class GoogleLatitudeConfigurationFile
	{
		public List<GoogleLatitudeAccount> GoogleLatitudeAccounts { get; set; }
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
				GoogleLatitudeAccounts = JsonConvert.DeserializeObject<GoogleLatitudeConfigurationFile>(jsonfile);				
			}
		}
	}
}

