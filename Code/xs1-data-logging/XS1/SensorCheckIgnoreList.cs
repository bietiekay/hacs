/// <summary>
/// This file holds the logic to interact with the simple actor scripting configuration
/// </summary>
using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;

namespace xs1_data_logging
{
    public static class SensorCheckIgnoreConfiguration
    {
        public static List<String> SensorCheckIgnoreList = new List<String>();

        public static void ReadConfiguration(String Configfilename)
        {
            if (File.Exists(Configfilename))
            {
                // get all lines from the 
                String[] ConfigFileContent = File.ReadAllLines(Configfilename);
                Int32 LineNumber = 0;

                foreach(String LineElement in ConfigFileContent)
                {
                    LineNumber++;

                    if (!LineElement.StartsWith("#"))
                    { 
                            SensorCheckIgnoreList.Add(LineElement);
                    }
                }
            }
            else
            {
                // we simply don't ignore anything...
            }
        }

    }
}


