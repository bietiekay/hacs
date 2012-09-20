using System;
using System.Text;
using Newtonsoft.Json;

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
			StringBuilder sb = new StringBuilder();

			ActorJSON_Root deserializedSensors = JsonConvert.DeserializeObject<ActorJSON_Root>(XS1_get_list_actuators_response);

			return sb.ToString();
		}

		/// <summary>
		/// Inject into an existing get_list_sensors JSON output of an ezcontrol XS1 device
		/// </summary>
		public static String Inject_get_list_sensors(String XS1_get_list_sensor_response, House ELVMAXHouse)
		{
			StringBuilder sb = new StringBuilder();

			SensorJSON_Root deserializedSensors = JsonConvert.DeserializeObject<SensorJSON_Root>(XS1_get_list_sensor_response);

			return sb.ToString();
		}

	}
}

