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
                #region get the XS1 device configuration
                WebRequest wrGetURL;
                Console.WriteLine("Retrieving XS1 device configuration...");
                wrGetURL = WebRequest.Create("http://"+XS1ServerURL+"/control?callback=xs1_config&cmd=get_config_info");
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

                #region get all sensor configurations
                Console.Write("Backing up sensor configuration...");
                for(Int32 i=1;i<=XS1Config.info.maxsensors;i++)
                {
                    Console.Write(".");

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?callback=sensor_config&cmd=get_config_sensor&number="+i);
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

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?callback=actor_config&cmd=get_config_actuator&number=" + i);
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

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?callback=timer_config&cmd=get_config_timer&number=" + i);
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

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?callback=script_config&cmd=get_config_script&number=" + i);
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

                    wrGetURL = WebRequest.Create("http://" + XS1ServerURL + "/control?callback=room_config&cmd=get_config_room&number=" + i);
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
            // create and open the backup file
            StreamReader backupfile = new StreamReader(filename);

            throw new NotImplementedException("Implement restore");
        }

    }

}
