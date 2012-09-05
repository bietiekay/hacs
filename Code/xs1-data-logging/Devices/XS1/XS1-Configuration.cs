using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace hacs.xs1.configuration
{
    /// <summary>
    /// this class holds all necessary information to query and deserialize XS1 configuration data
    /// </summary>
    public class XS1Configuration
    {
        private XS1ActuatorList ActuatorListCache;
        private DateTime LastActuatorListUpdated;
        private Int32 ConfigurationCacheMinutes;

        public XS1Configuration(Int32 _ConfigurationCacheMinutes)
        {
            ConfigurationCacheMinutes = _ConfigurationCacheMinutes;
        }

        public XS1ActuatorList getXS1ActuatorList(String XS1_URL, String Username, String Password)
        {
            // TODO: more error handling !!!

            // check if we already cached something
            if (ActuatorListCache != null)
            {
                if ((DateTime.Now - LastActuatorListUpdated).TotalMinutes < ConfigurationCacheMinutes)
                    return ActuatorListCache;
            }

            // now we got the parameters, we need to find out which actors and which functions shall be called
            WebRequest wrGetURL = WebRequest.Create("http://" + XS1_URL + "/control?user=" + Username + "&pwd=" + Password + "&callback=actorlist&cmd=get_list_actuators");

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
            actuator_config_json = actuator_config_json.Replace("actorlist(", "");
            actuator_config_json = actuator_config_json.Remove(actuator_config_json.Length - 4, 4);

            // deserialize the XS1 configuration json stream
            ActuatorListCache = ser.Deserialize<XS1ActuatorList>(actuator_config_json);

            LastActuatorListUpdated = DateTime.Now;

            return ActuatorListCache;
        }
    }
}
