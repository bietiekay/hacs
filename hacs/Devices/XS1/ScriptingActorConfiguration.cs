/// <summary>
/// This file holds the logic to interact with the simple actor scripting configuration
/// </summary>
using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;

namespace hacs
{
    public class ScriptingActorElement
    {
        public String SensorToWatchName;
        public Double SensorValue;
        public String ActorToSwitchName;
        public actor_status ActionToRunName;
        public Int32 StartHour;
        public Int32 EndHour;

        public Boolean isCurrentlyWithinStartEndHours()
        {
            if (StartHour > EndHour)
            {
                if ((StartHour <= DateTime.Now.Hour) || (EndHour >= DateTime.Now.Hour))
                {
                    return true;
                }
            }
            else
            {
                if ((StartHour <= DateTime.Now.Hour) || (EndHour <= DateTime.Now.Hour))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class ScriptingActorConfiguration
    {
        public static List<ScriptingActorElement> ScriptingActorActions = new List<ScriptingActorElement>();

        public static void ReadConfiguration(String Configfilename)
        {
            if (File.Exists(Configfilename))
            {
                // get all lines from the 
                String[] ActorConfigFileContent = File.ReadAllLines(Configfilename);
                Int32 LineNumber = 0;

                foreach(String LineElement in ActorConfigFileContent)
                {
                    
                    String[] TokenizedLine = LineElement.Split(new char[1] { ' ' });
                    LineNumber++;

                    if (!LineElement.StartsWith("#"))
                    { 

                        ScriptingActorElement NewElement = new ScriptingActorElement();

                        if (TokenizedLine.Length == 5)
                        { 
                            NewElement.SensorToWatchName = TokenizedLine[0];
                            NewElement.SensorValue = Convert.ToDouble(TokenizedLine[1]);
                            NewElement.ActorToSwitchName = TokenizedLine[2];
                            if (TokenizedLine[3].ToUpper() == "ON")
                                NewElement.ActionToRunName = actor_status.On;
                            else
                                if (TokenizedLine[3].ToUpper() == "OFF")
                                    NewElement.ActionToRunName = actor_status.Off;
                                else
                                    if (TokenizedLine[3].ToUpper() == "ONOFF")
                                        NewElement.ActionToRunName = actor_status.OnOff;
                                    else
                                        if (TokenizedLine[3].ToUpper() == "ONWAITOFF")
                                            NewElement.ActionToRunName = actor_status.OnWaitOff;
										else
											if (TokenizedLine[3].ToUpper() == "URL")
												NewElement.ActionToRunName = actor_status.URL;

                            String[] FromToTime = TokenizedLine[4].Split(new char[1] { '-' });
                            NewElement.StartHour = Convert.ToInt32(FromToTime[0]);
                            NewElement.EndHour = Convert.ToInt32(FromToTime[1]);

							ScriptingActorActions.Add(NewElement);
                        }
                        else
                            throw (new Exception("Scripting Actor Configuration File - Error in line "+LineNumber));
                    }
                }
            }
            else
            {
                throw (new Exception("Scripting Actor Configuration File not found! (" + Configfilename+")"));
            }
        }

    }
}


