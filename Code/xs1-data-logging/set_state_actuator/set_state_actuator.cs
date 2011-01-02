using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace xs1_data_logging.set_state_actuator
{
    public class set_state_actuator_response
    {
        public Int32 version;
    }

    public class set_state_actuator
    {
        public set_state_actuator()
        {

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
