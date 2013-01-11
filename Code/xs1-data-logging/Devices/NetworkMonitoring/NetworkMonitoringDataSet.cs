using System;
using Org.Mentalis.Network;

namespace xs1_data_logging
{
	public class NetworkMonitoringDataSet
	{
		public DateTime TimeOfMeasurement;
		public ICMP_Status Status;
		public double AverageRoundtripMS;
		public String HostnameIP;
		public String Descriptor;

		public NetworkMonitoringDataSet()
		{

		}
	}
}

