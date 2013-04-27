using System;
using System.Net;
using System.Threading;
using System.Collections;
using Org.Mentalis.Network;
using System.Collections.Generic;

namespace Org.Mentalis.Network 
{

	public enum ICMP_Status
	{
		TimeOut,
		Success
	}

	public class ICMP_PingResult
	{
		public DateTime TimeOfMeasurement;
		public ICMP_Status Status;
		public List<double> RoundtripMS;
		public double AverageRoundtripMS
		{
			get 
			{
				Int32 count = RoundtripMS.Count;
				double ReturnValue = 0;

				if (count == 0)
					return 0;

				foreach(double _value in RoundtripMS)
				{
					ReturnValue += _value;
				}
				ReturnValue = ReturnValue / count;

				return ReturnValue;
			}
		}
		public IPAddress hostIP;

		public ICMP_PingResult()
		{
			RoundtripMS = new List<double>();
			TimeOfMeasurement = DateTime.Now;
		}
	}

	public class ICMP
	{
		public ICMP_PingResult Ping(String host, Int32 count = 4, Int32 timeout = 1000) 
		{
			bool infinite = false;
			ICMP_PingResult Result = new ICMP_PingResult();

			// Resolve the specified IP address
			IPAddress hostIP;
			try 
			{
				hostIP = Dns.Resolve(host).AddressList[0];
			} 
			catch (Exception e)
			{
				throw new Exception("ICMP_Ping: Unable to resolve the specified hostname.",e);
			}

			Result.hostIP = hostIP;

			// Start pinging
			try {
				Icmp icmp = new Icmp(hostIP);
				TimeSpan ret;
				while(infinite || count > 0) 
				{
					try 
					{
						ret = icmp.Ping(timeout);
						if (ret.Equals(TimeSpan.MaxValue))
							Result.Status = ICMP_Status.TimeOut;
						else
						{
							Result.Status = ICMP_Status.Success;
							Result.RoundtripMS.Add(ret.TotalMilliseconds);
						}
						if (1000 - ret.TotalMilliseconds > 0)
							Thread.Sleep(1000 - (int)ret.TotalMilliseconds);
					} 
					catch (Exception e) 
					{
						throw new Exception("Network Error",e);
					}
					if (!infinite)
						count--;
				}
			} catch {
				throw new Exception("Error while pinging the specified host.");
			}

			return Result;
		}
	}
}