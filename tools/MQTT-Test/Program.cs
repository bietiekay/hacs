using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading;

namespace MQTT_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // create client instance 
            MqttClient client = new MqttClient(IPAddress.Parse("192.168.178.30"));

            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);

            Random r = new Random((int)DateTime.Now.Ticks);
            //string strValue = Convert.ToString(23.00);
            double stuff = 0;
            while (true)
            {
                // publish a message on "/home/temperature" topic with QoS 2
                stuff = r.NextDouble() * 100;
                client.Publish("sensors/temperature/dht22", Encoding.UTF8.GetBytes(Convert.ToString(stuff)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                Console.WriteLine(stuff);
                Thread.Sleep(1000);
            }
        }
    }
}
