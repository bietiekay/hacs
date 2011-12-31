using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.storage;
using sones.Storage;
using hacs.xs1;
using JavaScriptTimeStampExtension;

namespace xs1_data_logging.JSONHandlers
{
    /// <summary>
    /// this simple class creates JSON Output according to previously stored sensor logs
    /// </summary>
    public class JSONData
    {
        private TinyOnDiskStorage sensor_data;

        public JSONData(TinyOnDiskStorage sensor_data_storage)
        {
            sensor_data = sensor_data_storage;
        }
		
        /// <summary>
        /// generates JSON dataset from sensor data
        /// </summary>
        /// <returns></returns>
        public String GenerateDataJSONOutputWithoutInterpolation(ObjectTypes DataType, String ObjectTypeName, String ObjectName, DateTime StartDateTime, DateTime EndDateTime)
        {
            /* Example:
             * 
             * {    label: 'Europe (EU27)',
             *       data: [[1999, 3.0], [2000, 3.9], [2001, 2.0], [2002, 1.2], [2003, 1.3], [2004, 2.5], [2005, 2.0], [2006, 3.1], [2007, 2.9], [2008, 0.9]]
             * }
             * 
             * */

            StringBuilder Output = new StringBuilder();

            Output.Append("{ label: '"+ObjectName+"', data: [");
            bool firstdataset = true;
			UInt64 SerializerCounter = 0;
			UInt64 OutputCounter = 0;
			double LastValue = double.NaN;
			
			// TODO: there should be an appropriate caching algorithm in the sensor data... 

            lock (sensor_data.InMemoryIndex)
            {
                foreach (OnDiscAdress ondisc in sensor_data.InMemoryIndex)
                {
                    if (ondisc.CreationTime >= StartDateTime.Ticks)
                    {
                        if (ondisc.CreationTime <= EndDateTime.Ticks)
                        {
                            XS1_DataObject dataobject = new XS1_DataObject();

                            dataobject.Deserialize(sensor_data.Read(ondisc));
                            SerializerCounter++;

                            if (dataobject.Type == DataType)
                            {
                                if (dataobject.TypeName == ObjectTypeName)
                                {
                                    if (dataobject.Name == ObjectName)
                                    {
                                        if (!firstdataset)
                                            Output.Append(",");
                                        else
                                            firstdataset = false;

                                        if (LastValue == double.NaN)
                                        {
                                            LastValue = dataobject.Value;
                                            Output.Append("[");
                                            Output.Append(dataobject.Timecode.JavaScriptTimestamp());
                                            Output.Append(",");
                                            Output.Append(dataobject.Value.ToString().Replace(',', '.'));
                                            Output.Append("]");
                                            OutputCounter++;
                                        }
                                        else
                                        {
                                            if (LastValue != dataobject.Value)
                                            {
                                                Output.Append("[");
                                                Output.Append(dataobject.Timecode.JavaScriptTimestamp());
                                                Output.Append(",");
                                                Output.Append(dataobject.Value.ToString().Replace(',', '.'));
                                                Output.Append("]");
                                                OutputCounter++;
                                            }
                                            LastValue = dataobject.Value;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            Output.Append("]}");
			
			ConsoleOutputLogger.WriteLineToScreenOnly("Generated JSON Dataset with "+SerializerCounter+" Elements and outputted "+OutputCounter+" Elements.");

            return Output.ToString();
        }
		
		
		/// <summary>
        /// generates JSON dataset from sensor data
        /// </summary>
        /// <returns></returns>
        public String GenerateDataJSONOutput(ObjectTypes DataType, String ObjectTypeName, String ObjectName, DateTime StartDateTime, DateTime EndDateTime)
        {
            /* Example:
             * 
             * {    label: 'Europe (EU27)',
             *       data: [[1999, 3.0], [2000, 3.9], [2001, 2.0], [2002, 1.2], [2003, 1.3], [2004, 2.5], [2005, 2.0], [2006, 3.1], [2007, 2.9], [2008, 0.9]]
             * }
             * 
             * */

            StringBuilder Output = new StringBuilder();

            Output.Append("{ label: '"+ObjectName+"', data: [");
            bool firstdataset = true;
			UInt64 SerializerCounter = 0;
			
			// TODO: there should be an appropriate caching algorithm in the sensor data... 
            lock (sensor_data.InMemoryIndex)
            {
                foreach (OnDiscAdress ondisc in sensor_data.InMemoryIndex)
                {
                    if (ondisc.CreationTime >= StartDateTime.Ticks)
                    {
                        if (ondisc.CreationTime <= EndDateTime.Ticks)
                        {
                            XS1_DataObject dataobject = new XS1_DataObject();

                            dataobject.Deserialize(sensor_data.Read(ondisc));
                            SerializerCounter++;

                            if (dataobject.Type == DataType)
                            {
                                if (dataobject.TypeName == ObjectTypeName)
                                {
                                    if (dataobject.Name == ObjectName)
                                    {
                                        if (!firstdataset)
                                            Output.Append(",");
                                        else
                                            firstdataset = false;

                                        Output.Append("[");
                                        Output.Append(dataobject.Timecode.JavaScriptTimestamp());
                                        Output.Append(",");
                                        Output.Append(dataobject.Value.ToString().Replace(',', '.'));
                                        Output.Append("]");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Output.Append("]}");
			
			ConsoleOutputLogger.WriteLineToScreenOnly("Generated JSON Dataset with "+SerializerCounter+" Elements");

            return Output.ToString();
        }

        public String GeneratePowerSensorJSONOutput(PowerSensorOutputs OutputType, String ObjectName, DateTime StartDateTime, DateTime EndDateTime)
        {
            StringBuilder Output = new StringBuilder();
            StringBuilder Output2 = new StringBuilder();


            // TODO: there should be an appropriate caching algorithm in the sensor data... 

            if (OutputType == PowerSensorOutputs.HourkWh)
            {
                #region Hour kWh
                Output.Append("{ label: '" + ObjectName + "', data: [");
                UInt64 SerializerCounter = 0;

                lock (sensor_data.InMemoryIndex)
                {
                    foreach (OnDiscAdress ondisc in sensor_data.InMemoryIndex)
                    {
                        if (ondisc.CreationTime >= StartDateTime.Ticks)
                        {
                            if (ondisc.CreationTime <= EndDateTime.Ticks)
                            {
                                XS1_DataObject dataobject = new XS1_DataObject();

                                dataobject.Deserialize(sensor_data.Read(ondisc));
                                SerializerCounter++;

                                if (dataobject.Type == ObjectTypes.Sensor)
                                {
                                    if (dataobject.TypeName == "pwr_consump")
                                    {
                                        if (dataobject.Name == ObjectName)
                                        {
                                            Output2.Clear();
                                            Output2.Append("[");
                                            Output2.Append(dataobject.Timecode.JavaScriptTimestamp());
                                            Output2.Append(",");
                                            Output2.Append(dataobject.Value.ToString().Replace(',', '.'));
                                            Output2.Append("]");
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                Output.Append(Output2.ToString());
                #endregion
            }

            if (OutputType == PowerSensorOutputs.HourPeakkWh)
            {
                #region Hour Peak kWh
                Output.Append("{ label: '" + ObjectName + "', data: [");
                UInt64 SerializerCounter = 0;

                lock (sensor_data.InMemoryIndex)
                {
                    foreach (OnDiscAdress ondisc in sensor_data.InMemoryIndex)
                    {
                        if (ondisc.CreationTime >= StartDateTime.Ticks)
                        {
                            if (ondisc.CreationTime <= EndDateTime.Ticks)
                            {
                                XS1_DataObject dataobject = new XS1_DataObject();

                                dataobject.Deserialize(sensor_data.Read(ondisc));
                                SerializerCounter++;

                                if (dataobject.Type == ObjectTypes.Sensor)
                                {
                                    if (dataobject.TypeName == "pwr_peak")
                                    {
                                        if (dataobject.Name == ObjectName)
                                        {
                                            Output2.Clear();
                                            Output2.Append("[");
                                            Output2.Append(dataobject.Timecode.JavaScriptTimestamp());
                                            Output2.Append(",");
                                            Output2.Append(dataobject.Value.ToString().Replace(',', '.'));
                                            Output2.Append("]");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Output.Append(Output2.ToString());
                #endregion
            }

            if (OutputType == PowerSensorOutputs.CalculatedkWhCounterTotal)
            {
                #region Calculated kWh Counter (based on last known manual reading)
                DateTime ManualMeasurementDate = DateTime.MinValue;
                Double ManualMeasurementValue = Double.MinValue;
                UInt64 SerializerCounter = 0;

                // find the right sensor manual reading...
                foreach (PowerConsumptionSensor _manual_reading in PowerSensorConfiguration.PowerConsumptionSensors)
                {
                    Output.Append("{ label: '" + ObjectName + "', data: [");


                    if (_manual_reading.PowerSensorName == ObjectName)
                    {
                        ManualMeasurementDate = _manual_reading.InitialPowerSensorDate;
                        ManualMeasurementValue = _manual_reading.InitialPowerSensorValue;

                        // here comes the fun
                        StartDateTime = ManualMeasurementDate;
                        Double PowerSensorCalculatedValue = ManualMeasurementValue;
                        DateTime CurrentHourStart = StartDateTime;
                        Double CurrentHourMeanValue = Double.MinValue;
                        #region the lock
                        lock (sensor_data.InMemoryIndex)
                        {
                            foreach (OnDiscAdress ondisc in sensor_data.InMemoryIndex)
                            {
                                if (ondisc.CreationTime >= StartDateTime.Ticks)
                                {
                                    if (ondisc.CreationTime <= EndDateTime.Ticks)
                                    {
                                        XS1_DataObject dataobject = new XS1_DataObject();

                                        dataobject.Deserialize(sensor_data.Read(ondisc));
                                        SerializerCounter++;

                                        if (dataobject.Type == ObjectTypes.Sensor)
                                        {
                                            if (dataobject.TypeName == "pwr_consump")
                                            {
                                                if (dataobject.Name == ObjectName)
                                                {
                                                    // okay, we got the right sensor data element type with the right name... 

                                                    // calculate the time difference between hour start and current data object
                                                    TimeSpan ts = new TimeSpan(dataobject.Timecode.Ticks - CurrentHourStart.Ticks);

                                                    if (ts.TotalMinutes >= 60)
                                                    {
                                                        // we have a full hour...add to the calculated value and reset hour values
                                                        CurrentHourStart = dataobject.Timecode;
                                                        PowerSensorCalculatedValue += CurrentHourMeanValue / 1000;
                                                        //Console.WriteLine(" -> " + PowerSensorCalculatedValue + " : "+CurrentHourMeanValue + "("+dataobject.Timecode.ToShortDateString()+")");
                                                        CurrentHourMeanValue = Double.MinValue;
                                                    }
                                                    else
                                                    {
                                                        if (CurrentHourMeanValue == Double.MinValue)
                                                            CurrentHourMeanValue = dataobject.Value;
                                                        else
                                                        {
                                                            CurrentHourMeanValue = (CurrentHourMeanValue + dataobject.Value) / 2;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        // add the corrector value
                        PowerSensorCalculatedValue = PowerSensorCalculatedValue + _manual_reading.Corrector;

                        Output.Append("[");
                        Output.Append(DateTime.Now.JavaScriptTimestamp());
                        Output.Append(",");
                        Output.Append(PowerSensorCalculatedValue.ToString().Replace(',', '.'));
                        Output.Append("]");                    
                    }
                }

                if (ManualMeasurementValue == Double.MinValue)
                {
                    Console.WriteLine("No manual measurement configuration could be found for sensor "+ObjectName);
                }
               #endregion
            }

            Output.Append("]}");

            //ConsoleOutputLogger.WriteLineToScreenOnly("Generated JSON Dataset with " + SerializerCounter + " Elements");

            return Output.ToString();
        }
    }
}
