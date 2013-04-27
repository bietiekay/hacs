/// <summary>
/// This file holds the logic to interact with the simple actor scripting configuration
/// </summary>
using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;

namespace hacs
{
    public class NetworkMonitoringHost
    {
        public String Descriptor;
        public String IPAdressOrHostname;
    }

    public static class NetworkMonitorConfiguration
    {
		public static List<NetworkMonitoringHost> NetworkHosts = new List<NetworkMonitoringHost>();

        public static void ReadConfiguration(String Configfilename)
        {
            if (File.Exists(Configfilename))
            {
                // get all lines from the 
                String[] ConfigFileContent = File.ReadAllLines(Configfilename);
                Int32 LineNumber = 0;

				foreach(String LineElement in ConfigFileContent)
                {
                    
                    String[] TokenizedLine = LineElement.Split(new char[1] { ' ' });
                    LineNumber++;

                    if (!LineElement.StartsWith("#"))
                    { 
						NetworkMonitoringHost NewElement = new NetworkMonitoringHost();

                        if (TokenizedLine.Length == 2)
                        { 
                            NewElement.IPAdressOrHostname = TokenizedLine[0];
							NewElement.Descriptor = TokenizedLine[1];

							NetworkHosts.Add(NewElement);
                        }
                        else
                            throw (new Exception("NetworkMonitoring Host Configuration File - Error in line "+LineNumber));
                    }
                }
            }
            else
            {
				throw (new Exception("NetworkMonitoring Host Configuration File  not found!"));
            }
        }

    }
}


