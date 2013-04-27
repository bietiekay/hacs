using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using hacs.xs1.configuration;

namespace hacs.set_state_actuator
{
    public class set_state_actuator_response
    {
        public Int32 version;
    }

    public class set_state_actuator
    {
        public set_state_actuator() {   }

        public String SetStateActuatorPreset(String XS1_URL, String Username, String Password, String actorname, String preset, XS1Configuration XS1_Configuration)
        {
            String Output = "";
            // get the XS1 Actuator List to find the ID and the Preset ID
            XS1ActuatorList actuatorlist = XS1_Configuration.getXS1ActuatorList(hacs.Properties.Settings.Default.XS1, hacs.Properties.Settings.Default.Username, hacs.Properties.Settings.Default.Password);

            bool foundatleastoneactuator = false;

            Int32 foundActorID = 0;
            Int32 foundPresetID = 0;

            foreach (XS1Actuator _actuator in actuatorlist.actuator)
            {
                foundActorID++;
                if (_actuator.name.ToUpper() == actorname.ToUpper())
                {
                    //foundActorID = _actuator.id;

                    bool foundpreset = false;

                    foreach (actuator_function actorfunction in _actuator.function)
                    {
                        foundPresetID++;

                        if (actorfunction.type.ToUpper() == preset.ToUpper())
                        {
                            foundpreset = true;
                            break;
                        }
                    }

                    #region doing real stuff
                    if (foundpreset)
                    {
                        if (foundActorID != 0)
                        {
                            // so we obviously got the actor and the preset id... now lets do the call
                            set_state_actuator ssa = new set_state_actuator();
                            Output = ssa.SetStateActuatorPreset(hacs.Properties.Settings.Default.XS1, hacs.Properties.Settings.Default.Username, hacs.Properties.Settings.Default.Password, foundActorID, foundPresetID);
                            foundatleastoneactuator = true;
                            break;
                        }
                    }
                    #endregion
                }
            }

            return Output;
        }

        public String SetStateActuatorPreset(String XS1_URL, String Username, String Password, Int32 ActuatorID, Int32 PresetID)
        {
            // TODO: more error handling !!!

            WebRequest wrGetURL = WebRequest.Create("http://" + XS1_URL + "/control?user=" + Username + "&pwd=" + Password + "&callback=setstate&cmd=set_state_actuator&number="+ActuatorID+"&function="+PresetID);

            String _UsernameAndPassword = Username + ":" + Password;
            String _AuthorizationHeader = "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(_UsernameAndPassword));

            wrGetURL.Credentials = new NetworkCredential(Username, Password);
            wrGetURL.Headers.Add("Authorization", _AuthorizationHeader);
            HttpWebResponse response = (HttpWebResponse)wrGetURL.GetResponse();
            // check for eventual errors
            if (response.StatusCode != HttpStatusCode.OK)
            {
                // TODO: refactor to correct http response codes
                return null;
            }
            // we will read data via the response stream
            String actuator_config_json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = 20000000;

            // remove the javascript callback/definitions
            actuator_config_json = actuator_config_json.Replace("setstate(", "");
            actuator_config_json = actuator_config_json.Remove(actuator_config_json.Length - 4, 4);


            return "";
        }
    }
}
