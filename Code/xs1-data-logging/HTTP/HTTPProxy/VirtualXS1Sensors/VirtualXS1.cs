using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace xs1_data_logging
{
	/// <summary>
	/// the purpose of this class is to inject virtual sensor and actor data into an existing proxied ezControl XS1 device
	/// For example to display ELV MAX sensor and actor data alongside the XS1 sensor and actor data
	/// </summary>
	public static class VirtualXS1
	{
		/// <summary>
		/// Inject into an existing get_list_actuator JSON output of an ezcontrol XS1 device
		/// </summary>
		public static String Inject_get_list_actuators(String XS1_get_list_actuators_response, House ELVMAXHouse)
		{
            Int32 id = 65;
            Int32 numberofCharactersToDelete = XS1_get_list_actuators_response.IndexOf('(');

            String Start = XS1_get_list_actuators_response.Remove(numberofCharactersToDelete + 1);

            String Prepared = XS1_get_list_actuators_response.Remove(0, numberofCharactersToDelete + 1);
            Prepared = Prepared.Remove(Prepared.Length - 4, 4);

            ActorJSON_Root deserializedActors = JsonConvert.DeserializeObject<ActorJSON_Root>(Prepared);

            String SensorJSON = JsonConvert.SerializeObject(deserializedActors);
            SensorJSON = Start + SensorJSON + ")";

            return SensorJSON;
		}

		/// <summary>
		/// Inject into an existing get_list_sensors JSON output of an ezcontrol XS1 device
		/// </summary>
		public static String Inject_get_list_sensors(String XS1_get_list_sensor_response, House ELVMAXHouse)
		{
            Int32 id = 65;
            Int32 numberofCharactersToDelete = XS1_get_list_sensor_response.IndexOf('(');

            String Start = XS1_get_list_sensor_response.Remove(numberofCharactersToDelete + 1);

            String Prepared = XS1_get_list_sensor_response.Remove(0, numberofCharactersToDelete+1);
            Prepared = Prepared.Remove(Prepared.Length - 4, 4);

            SensorJSON_Root deserializedSensors = JsonConvert.DeserializeObject<SensorJSON_Root>(Prepared);

            List<IMAXDevice> devices = ELVMAXHouse.GetAllDevices();

            foreach(IMAXDevice _device in devices)
            {
                if (_device.Type == DeviceTypes.HeatingThermostat)
                {
                    HeatingThermostat heating = (HeatingThermostat)_device;

                    //heating.Temperature
                    SensorJSON _newsensor = new SensorJSON();

                    if (heating.Temperature == 4.0)
                    {
                        // this heatingthermostat is on "OFF"
                        _newsensor.id = id;
                        id++;
                        _newsensor.name = heating.Name;
                        _newsensor.type = "remotecontrol";
                        _newsensor.unit = "boolean";
                        _newsensor.value = 0.0;
                    }
                    else
                    {
                        //this is normal temperature
                        _newsensor.id = id;
                        id++;
                        _newsensor.name = heating.Name;
                        _newsensor.type = "temperature";
                        _newsensor.unit = "°C";
                        _newsensor.value = heating.Temperature;
                    }

                    deserializedSensors.sensor.Add(_newsensor);
                }

                if (_device.Type == DeviceTypes.ShutterContact)
                {
                    ShutterContact shutter = (ShutterContact)_device;

                    SensorJSON _newsensor = new SensorJSON();
                    _newsensor.id = id;
                    id++;
                    _newsensor.name = shutter.Name;
                    _newsensor.type = "dooropen";
                    _newsensor.unit = "boolean";
                    if (shutter.ShutterState == ShutterContactModes.open)
                        _newsensor.value = 1.0;
                    else
                        _newsensor.value = 0.0;

                    deserializedSensors.sensor.Add(_newsensor);
                }
            }

            String SensorJSON = JsonConvert.SerializeObject(deserializedSensors);

            SensorJSON = Start + SensorJSON + ")";

            return SensorJSON;
		}

	}
}

