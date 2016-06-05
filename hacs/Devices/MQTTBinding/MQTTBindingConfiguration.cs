using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using hacs.Devices.MQTTBinding;

namespace hacs
{
	public class MQTTBindingConfiguration
	{
        public static MQTTBindingRoot MQTTBindingConfigFile = new MQTTBindingRoot();

		public static void ReadConfiguration(String ConfigurationFilename)
		{
			if (File.Exists(ConfigurationFilename))
			{
				String jsonfile = File.ReadAllText(ConfigurationFilename);
                MQTTBindingConfigFile = JsonConvert.DeserializeObject<MQTTBindingRoot>(jsonfile);
			}
		}
	}
}

