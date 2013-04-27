using System;
using System.Text;
using UnixTime;
using System.Web;
using System.Net;
using System.IO;

namespace SMS77
{
	public enum SMSType
	{
		basicplus,
		quality,
		festnetz,
		flash
	}
	
	public class SMS77Gateway
	{
		private String pUsername;
		private String pPassword;
		private String pSMS77Gateway;

		/// <summary>
		/// Initializes a new instance of the <see cref="SMS77.SMS77Gateway"/> class.
		/// </summary>
		/// <param name="Username">the SMS77 Username.</param>
		/// <param name="Password">the SMS77 Password (preferably the MD5 hashed one).</param>
		public SMS77Gateway(String Username, String Password, String SMSGatewayURL = "https://gateway.sms77.de/")
		{
			pUsername = Username;
			pPassword = Password;
			pSMS77Gateway = SMSGatewayURL;
		}

		public void SendSMS(String RecipientNum, String Text, String SenderNum, bool TestOnlyDebug = false, bool Status = false, SMSType SMSType = SMSType.basicplus)
		{
			UriBuilder URL = new UriBuilder(pSMS77Gateway);

			var kvp = HttpUtility.ParseQueryString(string.Empty);
			kvp["u"] = pUsername;
			kvp["p"] = pPassword;
			kvp["to"] = RecipientNum;
			kvp["text"] = Text;
			kvp["type"] = SMSType.ToString("G");

			if (TestOnlyDebug)
				kvp["debug"]="1";
			if (Status)
				kvp["status"]="1";

			URL.Query = kvp.ToString();

			//Console.WriteLine(URL.ToString ());

            bool success = false;
            Int32 tryCounter = 0;
            Int32 MaxTries = 10;

            while (!success)
            {
                tryCounter++;
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(URL.ToString());

                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();

                StreamReader objReader = new StreamReader(objStream);

                string sLine = "";
                int i = 0;

                while (sLine != null)
                {
                    i++;
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                    {
                        if (sLine.Contains("100"))
                            success = true;
                    }
                }

                if (tryCounter == MaxTries)
                {
                    return;
                }
            }
		}
	}
}

