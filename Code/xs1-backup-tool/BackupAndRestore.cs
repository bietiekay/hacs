/*
* h.a.c.s (home automation control server) - http://github.com/bietiekay/hacs
* Copyright (C) 2010 Daniel Kirstenpfad
*
* hacs is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* hacs is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with hacs. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace xs1_backup_tool
{
    /// <summary>
    /// backups an EzControl XS1 device
    /// </summary>
    public static class BackupAndRestore
    {
        /// <summary>
        /// Backup functionality
        /// </summary>
        /// <param name="XS1ServerURL">the name or ip of the xs1 device, e.g. 192.168.1.242</param>
        /// <param name="Username">the username of the administrative User, e.g. Administrator</param>
        /// <param name="Password">the password of the administrative User</param>
        /// <param name="filename">the filename of the xs1 backup file</param>
        public static bool backup(String XS1ServerURL, String Username, String Password, String filename)
        {
            // basically what this does it retrieves a list of all sensors, actors and timers and then
            // iterates through that list and retrieves all sensor, actor and timer configurations
            // retrieve xs1 configuration: http://hacs.fritz.box/control?callback=xs_config&cmd=get_config_info
            // retrieve sensor configuration: http://hacs.fritz.box/control?callback=sensor_1_config&cmd=get_config_sensor&number=1

            // create and open the backup file
            StreamWriter backupfile = new StreamWriter(filename, true);

            // get the number of sensors, actors, timers from the XS1 configuration
            try
            {
                String _UsernameAndPassword = Username + ":" + Password;
                String _AuthorizationHeader = "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(_UsernameAndPassword));

                #region get the XS1 device configuration
                WebRequest wrGetURL;
                Console.WriteLine("Retrieving XS1 device configuration...");
                wrGetURL = WebRequest.Create("http://"+XS1ServerURL+"/control?user="+Username+"&pwd="+Password+"&callback=xs1_config&cmd=get_config_info");

                wrGetURL.Credentials = new NetworkCredential(Username, Password);
                wrGetURL.Headers.Add("Authorization", _AuthorizationHeader);
                
                String xs1_config_json = new StreamReader(wrGetURL.GetResponse().GetResponseStream()).ReadToEnd();

                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = 20000000;
                
                // remove the javascript callback/definitions
                xs1_config_json = xs1_config_json.Replace("xs1_config(", "");
                xs1_config_json = xs1_config_json.Remove(xs1_config_json.Length - 4, 4);
                
                // deserialize the XS1 configuration json stream
                xs1_config XS1Config = ser.Deserialize<xs1_config>(xs1_config_json);

                // write the xs1_config_json string to the backup file...
                backupfile.WriteLine("### get_config_info ###");
                backupfile.WriteLine(xs1_config_json);
                backupfile.WriteLine("--- get_config_info ---");
                backupfile.Flush();
                #endregion

                #region Check if configuration is correct
                if (XS1Config.version != 15)
                {
                    Console.WriteLine();
                    Console.WriteLine("The version of the XS1 API differs from the version this tool was written for! Please note");
                    Console.WriteLine("that you are using this tool at your own risk!");
                    Console.WriteLine();
                }
                #endregion

                #region get all sensor configurations
                Console.Write("Backing up sensor configuration...");
                for(Int32 i=1;i<=XS1Config.info.maxsensors;i++)
                {
                    Console.Write(".");

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?user=" + Username + "&pwd=" + Password + "&callback=sensor_config&cmd=get_config_sensor&number=" + i);
                    wrGetURL.Credentials = new NetworkCredential(Username, Password);
                    wrGetURL.Headers.Add("Authorization", _AuthorizationHeader);
                    
                    String sensor_config_json  = new StreamReader(wrGetURL.GetResponse().GetResponseStream()).ReadToEnd();

                    // remove the javascript callback/definitions
                    sensor_config_json = sensor_config_json.Replace("sensor_config(", "");
                    sensor_config_json = sensor_config_json.Remove(sensor_config_json.Length - 4, 4);
                    backupfile.WriteLine("### get_config_sensor ###");
                    backupfile.WriteLine(sensor_config_json);
                    backupfile.WriteLine("--- get_config_sensor ---");
                    backupfile.Flush();
                }
                Console.WriteLine("done");
                #endregion

                #region get all actor configurations
                Console.Write("Backing up actor configuration...");
                for (Int32 i = 1; i <= XS1Config.info.maxactuators; i++)
                {
                    Console.Write(".");

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?user=" + Username + "&pwd=" + Password + "&callback=actor_config&cmd=get_config_actuator&number=" + i);
                    wrGetURL.Credentials = new NetworkCredential(Username, Password);
                    wrGetURL.Headers.Add("Authorization", _AuthorizationHeader);

                    String actuator_config_json = new StreamReader(wrGetURL.GetResponse().GetResponseStream()).ReadToEnd();

                    // remove the javascript callback/definitions
                    actuator_config_json = actuator_config_json.Replace("actor_config(", "");
                    actuator_config_json = actuator_config_json.Remove(actuator_config_json.Length - 4, 4);
                    backupfile.WriteLine("### get_config_actuator ###");
                    backupfile.WriteLine(actuator_config_json);
                    backupfile.WriteLine("--- get_config_actuator ---");
                    backupfile.Flush();
                }
                Console.WriteLine("done");
                #endregion

                #region get all timer configurations
                Console.Write("Backing up timer configuration...");
                for (Int32 i = 1; i <= XS1Config.info.maxtimers; i++)
                {
                    Console.Write(".");

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?user=" + Username + "&pwd=" + Password + "&callback=timer_config&cmd=get_config_timer&number=" + i);
                    wrGetURL.Credentials = new NetworkCredential(Username, Password);
                    wrGetURL.Headers.Add("Authorization", _AuthorizationHeader);

                    String timer_config_json = new StreamReader(wrGetURL.GetResponse().GetResponseStream()).ReadToEnd();

                    // remove the javascript callback/definitions
                    timer_config_json = timer_config_json.Replace("timer_config(", "");
                    timer_config_json = timer_config_json.Remove(timer_config_json.Length - 4, 4);
                    backupfile.WriteLine("### get_config_timer ###");
                    backupfile.WriteLine(timer_config_json);
                    backupfile.WriteLine("--- get_config_timer ---");
                    backupfile.Flush();
                }
                Console.WriteLine("done");
                #endregion

                #region get all script configurations
                Console.Write("Backing up script configuration...");
                for (Int32 i = 1; i <= XS1Config.info.maxscripts; i++)
                {
                    Console.Write(".");

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?user=" + Username + "&pwd=" + Password + "&callback=script_config&cmd=get_config_script&number=" + i);
                    wrGetURL.Credentials = new NetworkCredential(Username, Password);
                    wrGetURL.Headers.Add("Authorization", _AuthorizationHeader);

                    String script_config_json = new StreamReader(wrGetURL.GetResponse().GetResponseStream()).ReadToEnd();

                    // remove the javascript callback/definitions
                    script_config_json = script_config_json.Replace("script_config(", "");
                    script_config_json = script_config_json.Remove(script_config_json.Length - 4, 4);
                    backupfile.WriteLine("### get_config_script ###");
                    backupfile.WriteLine(script_config_json);
                    backupfile.WriteLine("--- get_config_script ---");
                    backupfile.Flush();
                }
                Console.WriteLine("done");
                #endregion

                #region get all room configurations
                Console.Write("Backing up room configuration...");
                for (Int32 i = 1; i <= XS1Config.info.maxrooms; i++)
                {
                    Console.Write(".");

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?user=" + Username + "&pwd=" + Password + "&callback=room_config&cmd=get_config_room&number=" + i);
                    wrGetURL.Credentials = new NetworkCredential(Username, Password);
                    wrGetURL.Headers.Add("Authorization", _AuthorizationHeader);

                    String room_config_json = new StreamReader(wrGetURL.GetResponse().GetResponseStream()).ReadToEnd();

                    // remove the javascript callback/definitions
                    room_config_json = room_config_json.Replace("room_config(", "");
                    room_config_json = room_config_json.Remove(room_config_json.Length - 4, 4);
                    backupfile.WriteLine("### get_config_room ###");
                    backupfile.WriteLine(room_config_json);
                    backupfile.WriteLine("--- get_config_room ---");
                    backupfile.Flush();
                }
                Console.WriteLine("done");
                #endregion

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return false;
            }
            
        }

        /// <summary>
        /// Restore functionality
        /// </summary>
        /// <param name="XS1ServerURL">the name or ip of the xs1 device, e.g. 192.168.1.242</param>
        /// <param name="Username">the username of the administrative User, e.g. Administrator</param>
        /// <param name="Password">the password of the administrative User</param>
        /// <param name="filename">the filename of the xs1 backup file</param>
        public static bool restore(String XS1ServerURL, String Username, String Password, String filename)
        {
            try
            {
                bool Option_A = false;  // sending
                bool Option_B = false;  // receiving
                bool Option_C = false;  // scripting
                bool Option_D = false;  // memory card

                bool found = false;
                // open the backup file
                StreamReader backupfile = new StreamReader(filename);

                #region get the XS1 device configuration
                WebRequest wrGetURL;
                Console.WriteLine("Retrieving XS1 device configuration...");
                wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?user=" + Username + "&pwd=" + Password + "&callback=xs1_config&cmd=get_config_info");
                String xs1_config_json = new StreamReader(wrGetURL.GetResponse().GetResponseStream()).ReadToEnd();

                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = 20000000;

                // remove the javascript callback/definitions
                xs1_config_json = xs1_config_json.Replace("xs1_config(", "");
                xs1_config_json = xs1_config_json.Remove(xs1_config_json.Length - 4, 4);

                // deserialize the XS1 configuration json stream
                xs1_config XS1Config = ser.Deserialize<xs1_config>(xs1_config_json);

                #region check which options are available for restore on this XS1 device
                foreach (String _feature in XS1Config.info.features)
                {
                    switch(_feature)
                    {
                        case "A":
                            Option_A = true;
                            break;
                        case "B":
                            Option_B = true;
                            break;
                        case "C":
                            Option_C = true;
                            break;
                        case "D":
                            Option_D = true;
                            break;
                        default:
                            Console.WriteLine("Unknown featureset detected! Please update the backup tool!");
                            return false;
                    }

                }
                #endregion 
                #endregion

                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = backupfile.ReadLine()) != null)
                {
                    #region Restore Sensor, Actor, Timer and Script configurations
                    switch (line)
                    {
                        case "### get_config_sensor ###":
                            if (Option_B)
                            {
                                #region Get the configuration content
                                found = false;
                                StringBuilder SensorData = new StringBuilder();

                                while ((line == backupfile.ReadLine()) != null)
                                {
                                    if (line == "--- get_config_sensor ---")
                                    {
                                        found = true;
                                        break;
                                    }
                                    else
                                    {
                                        SensorData.AppendLine(line);
                                    }
                                }

                                if (!found)
                                {
                                    Console.WriteLine("Invalid backup file! Aborting!");
                                    return false;
                                }
                                #endregion

                                // write sensor configuration
                                if (!WriteSensorConfiguration(XS1ServerURL, Username, Password, SensorData.ToString(),XS1Config.version))
                                {
                                    return false;
                                }
                            }
                            break;
                        case "### get_config_actor ###":
                            if (Option_A)
                            {
                                #region Get the configuration content
                                found = false;
                                StringBuilder ActorData = new StringBuilder();

                                while ((line == backupfile.ReadLine()) != null)
                                {
                                    if (line == "--- get_config_actor ---")
                                    {
                                        found = true;
                                        break;
                                    }
                                    else
                                    {
                                        ActorData.AppendLine(line);
                                    }
                                }

                                if (!found)
                                {
                                    Console.WriteLine("Invalid backup file! Aborting!");
                                    return false;
                                }
                                #endregion

                                // write actor configuration
                                if (!WriteActorConfiguration(XS1ServerURL, Username, Password, ActorData.ToString(),XS1Config.version))
                                {
                                    return false;
                                }
                            }
                            break;
                        case "### get_config_script ###":
                            if (Option_C)
                            {
                                #region Get the configuration content
                                found = false;
                                StringBuilder ScriptData = new StringBuilder();

                                while ((line == backupfile.ReadLine()) != null)
                                {
                                    if (line == "--- get_config_script ---")
                                    {
                                        found = true;
                                        break;
                                    }
                                    else
                                    {
                                        ScriptData.AppendLine(line);
                                    }
                                }

                                if (!found)
                                {
                                    Console.WriteLine("Invalid backup file! Aborting!");
                                    return false;
                                }
                                #endregion

                                // write script configuration
                                if (!WriteScriptConfiguration(XS1ServerURL, Username, Password, ScriptData.ToString()))
                                {
                                    return false;
                                }
                            }
                            break;
                        case "### get_config_timer ###":
                            if (Option_A)
                            {
                                #region Get the configuration content
                                found = false;
                                StringBuilder TimerData = new StringBuilder();

                                while ((line == backupfile.ReadLine()) != null)
                                {
                                    if (line == "--- get_config_timer ---")
                                    {
                                        found = true;
                                        break;
                                    }
                                    else
                                    {
                                        TimerData.AppendLine(line);
                                    }
                                }

                                if (!found)
                                {
                                    Console.WriteLine("Invalid backup file! Aborting!");
                                    return false;
                                }
                                #endregion

                                // write timer configuration
                                if (!WriteScriptConfiguration(XS1ServerURL, Username, Password, TimerData.ToString()))
                                {
                                    return false;
                                }
                            }
                            break;
                        case "### get_config_room ###":
                            #region Get the configuration content
                            found = false;
                            StringBuilder RoomData = new StringBuilder();

                            while ((line == backupfile.ReadLine()) != null)
                            {
                                if (line == "--- get_config_room ---")
                                {
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    RoomData.AppendLine(line);
                                }
                            }

                            if (!found)
                            {
                                Console.WriteLine("Invalid backup file! Aborting!");
                                return false;
                            }
                            #endregion

                            // write timer configuration
                            if (!WriteRoomConfiguration(XS1ServerURL, Username, Password, RoomData.ToString()))
                            {
                                return false;
                            }
                            break;

                        default:
                            Console.WriteLine("Invalid backup file! Aborting!");
                            return false;
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return false;
            }
            
            return true;
        }

        #region Restore helper methods
        private static bool WriteSensorConfiguration(String XS1ServerURL, String Username, String Password, String SensorData, Int32 ProtocolVersion)
        {
            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = 20000000;

                // deserialize the json data stream
                config_sensor sensorconfiguration = ser.Deserialize<config_sensor>(SensorData);

                if (ProtocolVersion != sensorconfiguration.version)
                {
                    Console.WriteLine("Wrong protocol version! Aborting!");
                    return false;
                }

                Console.Write("Writing sensor configuration for sensor " + sensorconfiguration.sensor.name+"...");

                WebRequest wrGetURL;

                #region build query url
                StringBuilder RequestURL = new StringBuilder();
                RequestURL.Append("http://" + XS1ServerURL + "/control?user=" + Username + "&pwd=" + Password);
                RequestURL.Append("&number=" + sensorconfiguration.sensor.number);
                RequestURL.Append("&system=" + sensorconfiguration.sensor.system);
                RequestURL.Append("&type=" + sensorconfiguration.sensor.type);
                RequestURL.Append("&name=" + sensorconfiguration.sensor.name);
                RequestURL.Append("&address=" + sensorconfiguration.sensor.address);
                RequestURL.Append("&initvalue=");
                RequestURL.Append("&factor=" + sensorconfiguration.sensor.factor);
                RequestURL.Append("&offset=" + sensorconfiguration.sensor.offset);
                RequestURL.Append("&log=" + sensorconfiguration.sensor.log);
                RequestURL.Append("&hc1=" + sensorconfiguration.sensor.hc1);
                RequestURL.Append("&hc2=" + sensorconfiguration.sensor.hc2);
                RequestURL.Append("&address=" + sensorconfiguration.sensor.address);
                RequestURL.Append("&offset=" + sensorconfiguration.sensor.offset);
                RequestURL.Append("&factor=" + sensorconfiguration.sensor.factor);
                RequestURL.Append("&room=" + sensorconfiguration.sensor.room);
                RequestURL.Append("&x=" + sensorconfiguration.sensor.x);
                RequestURL.Append("&y=" + sensorconfiguration.sensor.y);
                RequestURL.Append("&z=" + sensorconfiguration.sensor.z);
                RequestURL.Append("&log=" + sensorconfiguration.sensor.log);
                RequestURL.Append("&callback=cname");
                #endregion

                wrGetURL = WebRequest.Create(RequestURL.ToString());
                String xs1_config_json = new StreamReader(wrGetURL.GetResponse().GetResponseStream()).ReadToEnd();
                // Todo: add check if correct set

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return false;
            }
            return true;
        }

        private static bool WriteActorConfiguration(String XS1ServerURL, String Username, String Password, String ActorData, Int32 ProtocolVersion)
        {
            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = 20000000;

                // deserialize the json data stream
                config_actuator actuatorconfiguration = ser.Deserialize<config_actuator>(ActorData);

                if (ProtocolVersion != actuatorconfiguration.version)
                {
                    Console.WriteLine("Wrong protocol version! Aborting!");
                    return false;
                }

                Console.Write("Writing actuator configuration for acturator " + actuatorconfiguration.actuator.name + "...");

                WebRequest wrGetURL;

                #region build query url
                StringBuilder RequestURL = new StringBuilder();
                RequestURL.Append("http://" + XS1ServerURL + "/control?user=" + Username + "&pwd=" + Password);
                RequestURL.Append("&number=" + actuatorconfiguration.actuator.number);
                RequestURL.Append("&id=" + actuatorconfiguration.actuator.id);
                RequestURL.Append("&name=" + actuatorconfiguration.actuator.name);
                RequestURL.Append("&system=" + actuatorconfiguration.actuator.system);
                RequestURL.Append("&type=" + actuatorconfiguration.actuator.type);
                RequestURL.Append("&hc1=" + actuatorconfiguration.actuator.hc1);
                RequestURL.Append("&hc2=" + actuatorconfiguration.actuator.hc2);
                RequestURL.Append("&address=" + actuatorconfiguration.actuator.address);

                // Todo: Add Actuator functions

                RequestURL.Append("&callback=cname");
                #endregion

                wrGetURL = WebRequest.Create(RequestURL.ToString());
                String xs1_config_json = new StreamReader(wrGetURL.GetResponse().GetResponseStream()).ReadToEnd();
                // Todo: add check if correct set

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return false;
            }
            return true;
        }

        private static bool WriteTimerConfiguration(String XS1ServerURL, String Username, String Password, String TimerData)
        {
            return false;
        }

        private static bool WriteRoomConfiguration(String XS1ServerURL, String Username, String Password, String RoomData)
        {
            return false;
        }

        private static bool WriteScriptConfiguration(String XS1ServerURL, String Username, String Password, String ScriptData)
        {
            return false;
        }
        #endregion
    }

}
