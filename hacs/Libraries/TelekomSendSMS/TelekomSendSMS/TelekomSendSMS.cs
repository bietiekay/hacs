// This file is based partially on the Telekom .NET SDK
// Copyright 2010 Deutsche Telekom AG
// Changes done after Commit 7a860c1cfb488909e7d4dec87f9633839997c982 (C) Daniel Kirstenpfad 2013
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;                                                                                                                                                                                       
//using TelekomSendSMSGateway;                                                                                                                                                                                        
using Telekom.Common.Auth;
using Telekom.SendSms;
using Telekom.Common;

namespace TelekomSendSMS                                                                                                                                                                                   
{
	class SendSMS
	{
		/// <summary>
		/// Your DeveloperGarden account credentials
		/// </summary>
		static string clientId = "";
		static string clientSecret = "";
		static string scope = "DC0QX4UK"; // this is Global SMS API

		/// <summary>
		/// Number to send the SMS to.
		/// Please note that the SDK call allows to specify multiple numbers.
		/// </summary>
		static string targetNumber = "tel:+49xxxx";

		/// <summary>
		/// Sender, as shown at the receiver
		/// </summary>
		static string senderAddress = "tel:+49xxxx";

		/// <summary>
		/// Account-ID of the sub account to be billed. Null to use your main account.
		/// Can be set in Developer Center.
		/// </summary>
		static string subAccountId = null;

		#region Ctor
		public SendSMS()
		{
		}
		#endregion

		#region Send a SMS
		public void Send()
		{
			//Console.Write("Requesting auth token...");

			//! [setproxy]
			//TelekomConfig.Proxy = new WebProxy("<yourproxy>", 1234);
			//! [setproxy]

			//! [upauth]
			TelekomOAuth2Auth authentication = new TelekomOAuth2Auth(clientId, clientSecret, scope);

			authentication.RequestAccessToken();
			if (!authentication.HasValidToken())
				throw new Exception("Authentication error.");
			//! [upauth]

			//Console.WriteLine("done");

			//! [client]
			SendSmsClient client = new SendSmsClient(authentication, ServiceEnvironment.Premium);

		}
		#endregion
	}

}                                                                                                                                                                                                   

