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

namespace hacs
{
    public class ScriptingTimerThread
    {
        public bool Shutdown = false;
        //private LoggingThread _LoggingThreadInstance;
        private ConsoleOutputLogger ConsoleOutputLogger;

        public ScriptingTimerThread(LoggingThread __LoggingThreadInstance, ConsoleOutputLogger Logger)
        {
            //_LoggingThreadInstance = __LoggingThreadInstance;
            ConsoleOutputLogger = Logger;
        }

        #region TimerOverdue
        private Boolean TimerOverdue(ScriptingTimerElement ScriptingTimer)
        {
            return false;
        }
        #endregion

        #region PowerOn
        private void PowerOn(ScriptingTimerElement ScriptingTimer)
        {
            ScriptingTimer.LastTimeSwitchedOn = DateTime.Now;
            ScriptingTimer.LastTimeSwitchedOff = DateTime.MinValue;

            // TODO: Power-On Code
            ConsoleOutputLogger.WriteLine("Power On "+ScriptingTimer.SwitchName+" by Timer " + ScriptingTimer.TimerName);
        }
        #endregion

        public void Run()
        {
            while (!Shutdown)
            {
                try
                {
                    // this is the main loop of all Scripting Timer Handling - so here is decided when something is to be switched on or off
                    // # TimerName DateTime_Start(YearDoesn'tMatter) DateTime_End(YearDoesn'tMatter) Duration_Minutes_Start Duration_Minutes_End OperationMode SwitchName JitterYesNo MinimumOnTimeMinutes   
                    foreach (ScriptingTimerElement ScriptingTimer in ScriptingTimerConfiguration.ScriptingTimerActions)
                    {
                        // we got one timer, now we decide if this one is overdue
                        if (TimerOverdue(ScriptingTimer))
                        {
                               
                        }

                    }
                }
                catch (Exception)
                {                   
                    Thread.Sleep(100);
                }

                Thread.Sleep(1);
            }


        }
    }
}
