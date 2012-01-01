using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using hacs.xs1;
using sones.storage;
using System.Threading;
using HTTP;
using hacs.xs1.configuration;

namespace xs1_data_logging
{
    public class ScriptingTimerThread
    {
        public bool Shutdown = false;
        private LoggingThread _LoggingThreadInstance;

        public ScriptingTimerThread(LoggingThread __LoggingThreadInstance)
        {
            _LoggingThreadInstance = __LoggingThreadInstance;
        }

        public void Run()
        {
            while (!Shutdown)
            {
                try
                {
                    
                }
                catch (Exception e)
                {                   
                    Thread.Sleep(1);
                }

                Thread.Sleep(1);
            }


        }
    }
}
