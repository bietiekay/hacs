using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

// TimerName DateTime_Start(YearDoesn'tMatter) TimerPeriodDurationDays Duration_Minutes_Start Duration_Minutes_End OperationMode SwitchName JitterYesNo MinimumOnTimeMinutes

namespace hacs
{
    public class ScriptingTimerElement
    {
        public String TimerName;
        public DateTime Start;
        public DateTime End;    
        public Int32 Duration_Start;
        public Int32 Duration_End;
        public scripting_timer_operation_modes OperationMode;
        public String SwitchName;
        public Boolean Jitter;
        public Int32 MinimumOnTime;

        // here comes some additional information which is needed to hold a status when calculating timings
        public DateTime LastTimeSwitchedOn;
        public DateTime LastTimeSwitchedOff;       
    }

    public class ScriptingTimerConfiguration
    {
        public static List<ScriptingTimerElement> ScriptingTimerActions = new List<ScriptingTimerElement>();

        public static void ReadConfiguration(String Configfilename)
        {
            if (File.Exists(Configfilename))
            {
                // get all lines from the 
                String[] TimerConfigFileContent = File.ReadAllLines(Configfilename);
                Int32 LineNumber = 0;

                foreach (String LineElement in TimerConfigFileContent)
                {
                    String[] TokenizedLine = LineElement.Split(new char[1] { ' ' });
                    LineNumber++;

                    if (!LineElement.StartsWith("#"))
                    {

                        ScriptingTimerElement NewElement = new ScriptingTimerElement();

                        if (TokenizedLine.Length == 9)
                        {
                            NewElement.TimerName = TokenizedLine[0];
                            
                            if (!DateTime.TryParse(TokenizedLine[1].Replace('_', ' '), out NewElement.Start))
                                throw (new Exception("Scripting Timers Configuration File - Error in line " + LineNumber + " - Could not parse Start DateTime"));
                            if (!DateTime.TryParse(TokenizedLine[2].Replace('_', ' '), out NewElement.End))
                                throw (new Exception("Scripting Timers Configuration File - Error in line " + LineNumber + " - Could not parse End DateTime"));

                            NewElement.Duration_Start = Convert.ToInt32(TokenizedLine[3]);
                            NewElement.Duration_End = Convert.ToInt32(TokenizedLine[4]);

                            if (TokenizedLine[5].ToUpper() == "ONOFF")
                                NewElement.OperationMode = scripting_timer_operation_modes.OnOff;

                            NewElement.SwitchName = TokenizedLine[6];

                            if (TokenizedLine[7].ToUpper() == "YES")
                                NewElement.Jitter = true;
                            else
                                NewElement.Jitter = false;

                            NewElement.MinimumOnTime = Convert.ToInt32(TokenizedLine[8]);

                            ScriptingTimerActions.Add(NewElement);
                        }
                        else
                            throw (new Exception("Scripting Timer Configuration File - Error in line " + LineNumber));
                    }
                }
            }
            else
            {
                throw (new Exception("Scripting Timer Configuration File not found!"));
            }
        }

    }
}
