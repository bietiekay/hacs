using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace hacs
{
	#region miataru configuration file json classes
	
	public class MiataruAccount
	{
		public string Name { get; set; }
		public string MiataruDeviceID { get; set; }
		public string MiataruServerURL { get; set; }
	}
	
	public class MiataruAccountConfigurationFile
	{
		public List<MiataruAccount> MiataruAccounts { get; set; }
	}
	#endregion

	public class MiataruConfiguration
	{
		public static MiataruAccountConfigurationFile MiataruAccountConfigFile = new MiataruAccountConfigurationFile();

		public static void ReadConfiguration(String ConfigurationFilename)
		{
			if (File.Exists(ConfigurationFilename))
			{
				String jsonfile = File.ReadAllText(ConfigurationFilename);
                //Console.WriteLine(jsonfile);
				MiataruAccountConfigFile = JsonConvert.DeserializeObject<MiataruAccountConfigurationFile>(jsonfile);
                //Console.WriteLine(GoogleLatitudeAccounts.GoogleLatitudeIDs.Count);
			}
		}
	}
}

