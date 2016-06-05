using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hacs.Devices.MQTTBinding
{

    public class MqttActorMapping
    {
        public string hacs { get; set; }
        public string mqtt { get; set; }
    }

    public class MqttSensorMapping
    {
        public string hacs { get; set; }
        public string mqtt { get; set; }
    }

    public class MQTTBindingRoot
    {
        public string mqtt_broker_host { get; set; }
        public Int32 mqtt_broker_port { get; set; }
        public string mqtt_qos_level { get; set; }
        public List<MqttActorMapping> mqtt_actor_mapping { get; set; }
        public List<MqttSensorMapping> mqtt_sensor_mapping { get; set; }
    }

}
