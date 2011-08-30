/// <summary>
/// This file holds the logic to interact with the simple actor scripting configuration
/// </summary>
using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;

namespace xs1_data_logging
{
    public class ScriptingActorElement
    {
        public String SensorToWatchName;
        public Double SensorValue;
        public String ActorToSwitchName;
        public actor_status ActionToRunName;
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

                        if (TokenizedLine.Length == 4)
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

                            ScriptingActorActions.Add(NewElement);
                        }
                        else
                            throw (new Exception("Scripting Actor Configuration File - Error in line "+LineNumber));
                    }
                }
            }
            else
            {
                throw (new Exception("Scripting Actor Configuration File not found!"));
            }
        }

    }
}


