using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace hacs.Devices.MQTT.settings
{
    public class MiataruMQTTMapping
    {
        public string MiataruDeviceid { get; set; }
        public string MQTTContext { get; set; }
    }

    public class SolarlogMqttMapping
    {
        public string pac { get; set; }
        public string apdc { get; set; }
    }

    public class XS1MQTTMapping
    {
        public string XS1Context { get; set; }
        public string MQTTContext { get; set; }
    }

    public class ELVMAXMQTTMapping
    {
        public string ELVMAXContext { get; set; }
        public string MQTTContext { get; set; }
    }

    public class hacsMQTTmappings
    {
        public List<MiataruMQTTMapping> MiataruMQTTMapping { get; set; }
        public SolarlogMqttMapping SolarlogMqttMapping { get; set; }
        public List<XS1MQTTMapping> XS1MQTTMapping { get; set; }
        public List<ELVMAXMQTTMapping> ELVMAXMQTTMapping { get; set; }
    }

    public class MQTTMappingsConfiguration
    {
        public static hacsMQTTmappings MQTTMappingsFile = new hacsMQTTmappings();

        public static void ReadConfiguration(String ConfigurationFilename)
        {
            if (File.Exists(ConfigurationFilename))
            {
                String jsonfile = File.ReadAllText(ConfigurationFilename);
                MQTTMappingsFile = JsonConvert.DeserializeObject<hacsMQTTmappings>(jsonfile);
            }
        }
    }

}