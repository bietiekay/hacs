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
        public String GenerateDataJSONOutput(ObjectTypes DataType, String ObjectTypeName, String ObjectName)
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

            foreach (OnDiscAdress ondisc in sensor_data.InMemoryIndex)
            {
                XS1_DataObject dataobject = new XS1_DataObject();

                dataobject.Deserialize(sensor_data.Read(ondisc));

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

            Output.Append("]}");


            return Output.ToString();
        }
    }
}
