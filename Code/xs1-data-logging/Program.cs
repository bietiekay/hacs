using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace xs1_data_logging
{
    /// <summary>
    /// this small tool logs the data from one or many ezcontrol xs1 devices
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            #region ConsoleOutputLogger
            ConsoleOutputLogger.verbose = true;
            ConsoleOutputLogger.writeLogfile = false;
            #endregion

            #region Logo
            ConsoleOutputLogger.WriteLine("EzControl XS1 Data Logger "+System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            ConsoleOutputLogger.WriteLine("(C) 2010 Daniel Kirstenpfad - http://github.com/bietiekay/hacs");
            #endregion 

            if (args.Length == 0)
            {
                ConsoleOutputLogger.WriteLine("Syntax:");
                ConsoleOutputLogger.WriteLine("        xs1-data-logging.exe <ip or cname of xs1>vc");

                return;
            }

            ConsoleOutputLogger.writeLogfile = true;

            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];


            String HacsURL = "http://"+args[0]+"/control?callback=cname&cmd=subscribe&format=tsv";

            while (true)
            {
                try
                {
                    //Thread.Sleep(100);
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

                            // continue building the string
                            ConsoleOutputLogger.WriteLine(tempString);
                        }
                    }
                    while (count > 0); // any more data to read?
                }
                catch (Exception e)
                {
                    ConsoleOutputLogger.WriteLineToScreenOnly("Reconnecting...");
                }
            }
        }
    }
}
