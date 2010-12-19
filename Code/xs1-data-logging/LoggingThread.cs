using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using hacs.xs1;
using sones.storage;
using System.Threading;

namespace xs1_data_logging
{
    public class LoggingThread
    {
        public String ServerName;
        TinyOnDiskStorage actor_data_store = null;
        TinyOnDiskStorage sensor_data_store = null;
        TinyOnDiskStorage unknown_data_store = null;

        bool Shutdown = false;

        public LoggingThread(String _ServerName, TinyOnDiskStorage _actor_store, TinyOnDiskStorage _sensor_store, TinyOnDiskStorage _unknown_store)
        {
            actor_data_store = _actor_store;
            sensor_data_store = _sensor_store;
            unknown_data_store = _unknown_store;
            ServerName = _ServerName;
        }

        public void Run()
        {
            while (!Shutdown)
            {
                try
                {
                    byte[] buf = new byte[8192];

                    String HacsURL = "http://" + ServerName + "/control?callback=cname&cmd=subscribe&format=tsv";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(HacsURL);
                    // execute the request
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ConsoleOutputLogger.WriteLineToScreenOnly("XS1 successfully connected!");
                    }
                    // we will read data via the response stream
                    Stream resStream = response.GetResponseStream();

                    string tempString = null;
                    int count = 0;

                    do
                    {
                        // fill the buffer with data
                        count = resStream.Read(buf, 0, buf.Length);

                        // make sure we read some data
                        if (count != 0)
                        {
                            // translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);
                            XS1_DataObject dataobject = HandleXS1_TSV.HandleValue(tempString);
                            dataobject.ServerName = ServerName;

                            if (dataobject.Type == ObjectTypes.Actor)
                            {
                                actor_data_store.Write(dataobject.Serialize());
                            }
                            else
                                if (dataobject.Type == ObjectTypes.Sensor)
                                {
                                    sensor_data_store.Write(dataobject.Serialize());
                                }
                                else
                                    if (dataobject.Type == ObjectTypes.Unknown)
                                    {
                                        unknown_data_store.Write(dataobject.Serialize());
                                    }

                            ConsoleOutputLogger.WriteLine(ServerName+" - "+tempString);

                        }
                    }
                    while (count > 0); // any more data to read?
                }
                catch (Exception e)
                {                   
                    //ConsoleOutputLogger.WriteLineToScreenOnly("Reconnecting...");
                    Thread.Sleep(1);
                }
            }


        }
    }
}
