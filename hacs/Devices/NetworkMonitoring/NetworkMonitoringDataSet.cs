using System;
using Org.Mentalis.Network;

namespace hacs
{
	public class NetworkMonitoringDataSet : IAlarmingEvent
	{
		public DateTime TimeOfMeasurement;
		public ICMP_Status Status;
		public double AverageRoundtripMS;
		public String HostnameIP;
		public String Descriptor;

		public NetworkMonitoringDataSet()
		{
			TimeOfMeasurement = DateTime.Now;
		}

		#region IAlarmingEvent implementation

		public AlarmingEventType AlarmingType ()
		{
			return AlarmingEventType.NetworkingEvent;
			HostnameIP = "";
		}

		public string AlarmingName ()
		{
			if (HostnameIP == "")
				return "Ping";
			else
				return HostnameIP;

		}

		public DateTime AlarmingCreated ()
		{
			return TimeOfMeasurement;
		}

		#endregion
	}
}

