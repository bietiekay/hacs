using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using System.Net;
using System.Threading;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace hacs.Devices.MQTTBinding
{
    public class MQTT_Handling
    {
        public bool Shutdown = false;
        private ConsoleOutputLogger ConsoleOutputLogger;
        private MqttClient client;
        private String clientId;

        #region Constructor
        public MQTT_Handling(ConsoleOutputLogger Logger)
        {
            // create client instance 
            client = new MqttClient(IPAddress.Parse(MQTTBindingConfiguration.MQTTBindingConfigFile.mqtt_broker_host),MQTTBindingConfiguration.MQTTBindingConfigFile.mqtt_broker_port,false,null,null,MqttSslProtocols.None);
            clientId = Guid.NewGuid().ToString();
            ConsoleOutputLogger = Logger;
            // connect
            try
            {
                client.Connect(clientId);

                if (client.IsConnected)
                    ConsoleOutputLogger.WriteLine("MQTT Binding: Successfully connected!");
                else
                    ConsoleOutputLogger.WriteLine("MQTT Binding: Connection Problem - could not connect!");
            }
            catch(Exception e)
            {
                ConsoleOutputLogger.WriteLine("MQTT Binding Exception - " + e.Message);
            }
            
        }
        #endregion

        #region Thread

        public void Run()
        {
            ConsoleOutputLogger.WriteLine("MQTT Binding Thread started");

            while (!Shutdown)
            {
                Thread.Sleep(100);
            }
        }
        #endregion

        public void MQTT_Handle_Sensor(xs1.XS1_DataObject XS1SensorData)
        {
            ConsoleOutputLogger.WriteLineToScreenOnly("MQTT-Binding-SensorInput: " + XS1SensorData.Name);

            foreach (MqttSensorMapping sensormap in MQTTBindingConfiguration.MQTTBindingConfigFile.mqtt_sensor_mapping)
            {
                if (sensormap.hacs == XS1SensorData.Name)
                {
                    // we have a valid mapping
                    if (!client.IsConnected)
                        client.Connect(clientId);
                        
                    client.Publish(sensormap.mqtt, Encoding.UTF8.GetBytes(Convert.ToString(XS1SensorData.Value)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                    ConsoleOutputLogger.WriteLine("MQTT-Publish: " + sensormap.mqtt + " - " + XS1SensorData.Value);

                }
            }

        }

        public void MQTT_Handle_Actor(xs1.XS1_DataObject XS1ActorData)
        {
            //ConsoleOutputLogger.WriteLineToScreenOnly("MQTT-Binding-ActorInput: " + XS1ActorData.Name);
            foreach (MqttActorMapping actormap in MQTTBindingConfiguration.MQTTBindingConfigFile.mqtt_actor_mapping)
            {
                if (actormap.hacs == XS1ActorData.Name)
                {
                    // we have a valid mapping
                    if (!client.IsConnected)
                        client.Connect(clientId);

                    client.Publish(actormap.mqtt, Encoding.UTF8.GetBytes(Convert.ToString(XS1ActorData.Value)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                    ConsoleOutputLogger.WriteLine("MQTT-Publish: " + actormap.mqtt + " - " + XS1ActorData.Value);
                }
            }

        }

    }
}
