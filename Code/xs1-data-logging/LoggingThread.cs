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
//using xs1_data_logging.set_state_actuator;


namespace xs1_data_logging
{
    public class LoggingThread
    {
        public String ServerName;
        public String UserName;
        public String Password;
        TinyOnDiskStorage actor_data_store = null;
        TinyOnDiskStorage sensor_data_store = null;
        TinyOnDiskStorage unknown_data_store = null;
        XS1Configuration XS1_Configuration = null;
        Int32 ConfigurationCacheMinutes;
        public Boolean AcceptingCommands = false;
        public List<String> TemporaryBlacklist = new List<string>();
        public List<String> OnWaitOffLIst = new List<string>();
        public ConsoleOutputLogger ConsoleOutputLogger;
        bool Shutdown = false;

        public LoggingThread(String _ServerName, ConsoleOutputLogger Logger,TinyOnDiskStorage _actor_store, TinyOnDiskStorage _sensor_store, TinyOnDiskStorage _unknown_store, String _Username, String _Password, Int32 _ConfigurationCacheMinutes)
        {
            actor_data_store = _actor_store;
            sensor_data_store = _sensor_store;
            unknown_data_store = _unknown_store;
            ServerName = _ServerName;
            UserName = _Username;
            Password = _Password;
            ConfigurationCacheMinutes = _ConfigurationCacheMinutes;
            ConsoleOutputLogger = Logger;
            //KnownActorStatuses = new Dictionary<String,current_actor_status>();
        }

        public void Run()
        {
            // initialize XS1 Configuration
            XS1_Configuration = new XS1Configuration(ConfigurationCacheMinutes);

            HttpServer httpServer = new HttpServer(Properties.Settings.Default.HTTPPort,Properties.Settings.Default.HTTPIP,Properties.Settings.Default.HTTPDocumentRoot,sensor_data_store,XS1_Configuration, ConsoleOutputLogger);
            Thread http_server_thread = new Thread(new ThreadStart(httpServer.listen));
            http_server_thread.Start();

            SensorCheck Sensorcheck = new SensorCheck(ConsoleOutputLogger);
            Thread SensorCheckThread = new Thread(new ThreadStart(Sensorcheck.Run));
			SensorCheckThread.Start();
            ActorReswitching ActorReSwitch_ = new ActorReswitching(XS1_Configuration, ConsoleOutputLogger, TemporaryBlacklist,OnWaitOffLIst);
            Thread ActorReswitchThread = new Thread(new ThreadStart(ActorReSwitch_.Run));
            ActorReswitchThread.Start();

            while (!Shutdown)
            {
                try
                {
                    byte[] buf = new byte[8192];

                    String HacsURL = "http://" + ServerName + "/control?callback=cname&cmd=subscribe&format=tsv";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(HacsURL);
                    request.Timeout = 60000;
                    request.Credentials = new NetworkCredential(UserName,Password);

                    String _UsernameAndPassword = UserName+ ":" + Password;
                    Uri _URI = new Uri(HacsURL);
                    
                    CredentialCache _CredentialCache = new CredentialCache();
                    _CredentialCache.Remove(_URI, "Basic");
                    _CredentialCache.Add(_URI, "Basic", new NetworkCredential(UserName, Password));
                    String _AuthorizationHeader = "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(_UsernameAndPassword));

                    request.Headers.Add("Authorization", _AuthorizationHeader);

                    // execute the request
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        AcceptingCommands = true;
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
                                lock(actor_data_store)
                                {
                                    actor_data_store.Write(dataobject.Serialize());
                                }

                                lock(KnownActorStates.KnownActorStatuses)
                                {
                                    bool usethisactor = true;
                                    // check if this actor is on temporary blacklist (like when it was handled)
                                    lock (TemporaryBlacklist)
                                    {
                                        if (TemporaryBlacklist.Contains(dataobject.Name))
                                            usethisactor = false;
                                        TemporaryBlacklist.Remove(dataobject.Name);
                                    }

                                    if (usethisactor)
                                    {
                                        if (KnownActorStates.KnownActorStatuses.ContainsKey(dataobject.Name))
                                        {
                                            // is contained
                                            if (dataobject.Value == 100)
                                                KnownActorStates.KnownActorStatuses[dataobject.Name] = new current_actor_status(dataobject.Name, actor_status.On);
                                            else
                                                KnownActorStates.KnownActorStatuses[dataobject.Name] = new current_actor_status(dataobject.Name, actor_status.Off);
                                        }
                                        else
                                        {
                                            if (dataobject.Value == 100)
                                                KnownActorStates.KnownActorStatuses.Add(dataobject.Name, new current_actor_status(dataobject.Name, actor_status.On));
                                            else
                                                KnownActorStates.KnownActorStatuses.Add(dataobject.Name, new current_actor_status(dataobject.Name, actor_status.Off));
                                        }
                                    }
                                    else
                                        ConsoleOutputLogger.WriteLine("Actor "+dataobject.Name+" is on the blacklist (ActorReSwitching) and therefore is ignored this time.");
                                }
                            }
                            else
                                if (dataobject.Type == ObjectTypes.Sensor)
                                {
                                    lock(sensor_data_store)
                                    {
                                        sensor_data_store.Write(dataobject.Serialize());
                                    }
                                    // update the sensor in the sensor check
                                    Sensorcheck.UpdateSensor(dataobject.Name);

                                    // check if this sensor is something we should act uppon
                                    foreach (ScriptingActorElement Element in ScriptingActorConfiguration.ScriptingActorActions)
                                    {
                                        if (dataobject.Name == Element.SensorToWatchName)
                                        {
                                            if (dataobject.Value == Element.SensorValue)
                                            { 
                                                // obviously there is a ScriptingActorConfiguration entry
                                                // so we execute the actor preset

                                                set_state_actuator.set_state_actuator ssa = new set_state_actuator.set_state_actuator();
                                                ConsoleOutputLogger.WriteLineToScreenOnly("detected actor scripting action on sensor "+Element.SensorToWatchName+" - "+Element.ActorToSwitchName+" to "+Element.ActionToRunName);
                                                
                                                // check what action is going to happen now...
                                                if (Element.ActionToRunName == actor_status.On)
                                                {
                                                    ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "ON", XS1_Configuration);
                                                }

                                                if (Element.ActionToRunName == actor_status.Off)
                                                {
                                                    ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "OFF", XS1_Configuration);
                                                    
                                                    // remove from OnWaitOffList
                                                    lock (OnWaitOffLIst)
                                                    {
                                                        if (OnWaitOffLIst.Contains(Element.ActorToSwitchName))
                                                            OnWaitOffLIst.Remove(Element.ActorToSwitchName);
                                                    }
                                                }

                                                if (Element.ActionToRunName == actor_status.OnOff)
                                                {
                                                    // look for the current status in the known actors table
                                                    lock(KnownActorStates.KnownActorStatuses)
                                                    {
                                                        if (KnownActorStates.KnownActorStatuses.ContainsKey(Element.ActorToSwitchName))
                                                        {
                                                            current_actor_status Status = KnownActorStates.KnownActorStatuses[Element.ActorToSwitchName];
                                                            if (Status.Status == actor_status.On)
                                                                ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "OFF", XS1_Configuration);
                                                            else
                                                                if (Status.Status == actor_status.Off)
                                                                    ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "ON", XS1_Configuration);
                                                        }
                                                        else
                                                            ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "ON", XS1_Configuration);
                                                    }
                                                }
                                                if (Element.ActionToRunName == actor_status.OnWaitOff)
                                                {
                                                    lock (OnWaitOffLIst)
                                                    {
                                                        ConsoleOutputLogger.WriteLine("Adding " + Element.ActorToSwitchName + " to ActorReSwitching OnWaitOff List");
                                                        OnWaitOffLIst.Add(Element.ActorToSwitchName);
                                                    }
                                                    ssa.SetStateActuatorPreset(xs1_data_logging.Properties.Settings.Default.XS1, xs1_data_logging.Properties.Settings.Default.Username, xs1_data_logging.Properties.Settings.Default.Password, Element.ActorToSwitchName, "ON_WAIT_OFF", XS1_Configuration);
                                                }
                                            }
                                        }
                                    }
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
                catch (Exception)
                {                   
                    //ConsoleOutputLogger.WriteLineToScreenOnly("Reconnecting...");
                    AcceptingCommands = false;
                    Thread.Sleep(1);
                }
            }


        }
    }
}
