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
using System.Collections.Generic;
using Telekom.SendSms.Model;
using Telekom.Common.Model;

namespace TelekomSendSMS                                                                                                                                                                                   
{
	public class TelekomSendSMSGateway
	{
		/// <summary>
		/// Your DeveloperGarden account credentials
		/// </summary>
		private string clientId = "";
		private string clientSecret = "";
		private string scope = "DC0QX4UK"; // this is Global SMS API

		/// <summary>
		/// Number to send the SMS to.
		/// Please note that the SDK call allows to specify multiple numbers.
		/// </summary>
		private String targetNumber;

		/// <summary>
		/// Sender, as shown at the receiver
		/// </summary>
		private string senderAddress;

		/// <summary>
		/// Account-ID of the sub account to be billed. Null to use your main account.
		/// Can be set in Developer Center.
		/// </summary>
		private string subAccountId = null;

		private TelekomOAuth2Auth authentication;

		#region Ctor
        public TelekomSendSMSGateway(String _clientId, String _clientSecret)
		{
			clientId = _clientId;
			clientSecret = _clientSecret;

			//Console.Write("Requesting auth token...");
			//! [setproxy]
			//TelekomConfig.Proxy = new WebProxy("<yourproxy>", 1234);
			//! [setproxy]

			//! [upauth]
			authentication = new TelekomOAuth2Auth(clientId, clientSecret, scope);

			authentication.RequestAccessToken();
			if (!authentication.HasValidToken())
				throw new Exception("Authentication error.");
			//! [upauth]


		}
		#endregion

		#region Send a SMS
		public void Send(String _targetNumber,String Message, String _sender, ServiceEnvironment SMSType = ServiceEnvironment.Premium)
		{
			targetNumber = "tel:"+_targetNumber;
			senderAddress = "tel:"+_sender;

			//! [client]
			SendSmsClient client = new SendSmsClient(authentication, SMSType);

			//! [prepare]
			List<String> receiverNumbers = new List<String>();
			receiverNumbers.Add(targetNumber);

			SendSmsRequest request = new SendSmsRequest();
			request.Numbers = receiverNumbers;
			request.Message = Message;
			request.SenderAddress = senderAddress;
			request.SMSType = OutboundSMSType.TEXT;
			request.Account = subAccountId;
			//! [prepare]

			//Console.Write("Sending SMS...");
			//! [send]
			SmsResponse response = client.SendSms(request);
			if (!response.Success)
				throw new Exception(string.Format("error {0}: {1} - {2}",
				                                  response.requestError.policyException.messageId,
				                                  response.requestError.policyException.text.Substring(0, response.requestError.policyException.text.Length - 2),
				                                  response.requestError.policyException.variables[0]));
			//! [send]

			//Console.WriteLine("ok");

			//Console.WriteLine("End of demo. Press Enter to exit.");
			//Console.ReadLine();

		}
		#endregion
	}

}                                                                                                                                                                                                   

