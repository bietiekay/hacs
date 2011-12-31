/// <summary>
/// This file holds the logic to interact with the power consumption sensor configuration
/// </summary>
using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;

namespace xs1_data_logging
{
    public class PowerConsumptionSensor
    {
        public String PowerSensorName;
        public DateTime InitialPowerSensorDate;
        public Double InitialPowerSensorValue;
        public Double Corrector;
    }

    public static class PowerSensorConfiguration
    {
        public static List<PowerConsumptionSensor> PowerConsumptionSensors = new List<PowerConsumptionSensor>();

        public static void ReadConfiguration(String Configfilename)
        {
            if (File.Exists(Configfilename))
            {
                // get all lines from the 
                String[] PowerSensorConfigFileContent = File.ReadAllLines(Configfilename);
                Int32 LineNumber = 0;

                foreach (String LineElement in PowerSensorConfigFileContent)
                {
                    
                    String[] TokenizedLine = LineElement.Split(new char[1] { ' ' });
                    LineNumber++;

                    if (!LineElement.StartsWith("#"))
                    {

                        PowerConsumptionSensor NewElement = new PowerConsumptionSensor();

                        if (TokenizedLine.Length == 4)
                        {
                            NewElement.PowerSensorName = TokenizedLine[0];
                            NewElement.InitialPowerSensorValue = Convert.ToDouble(TokenizedLine[2]);
                            NewElement.Corrector = Convert.ToDouble(TokenizedLine[3]);

                            if (DateTime.TryParse(TokenizedLine[1].Replace('_',' '), out NewElement.InitialPowerSensorDate))
                            {
                                PowerConsumptionSensors.Add(NewElement);
                            }
                            else
                                throw (new Exception("Power Sensor Configuration File - Error in line "+LineNumber+" - Could not parse DateTime"));
                        }
                        else
                            throw (new Exception("Power Sensor Configuration File - Error in line "+LineNumber));
                    }
                }
            }
            else
            {
                throw (new Exception("Power Sensor Configuration File not found!"));
            }
        }

    }
}


