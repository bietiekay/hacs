// This file is part of the Telekom .NET SDK
// Copyright 2010 Deutsche Telekom AG
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Telekom.Common;
using Telekom.Common.Auth;
using Telekom.Common.Model;
using Telekom.SendSms.Model;
using Telekom.Common.WebRequest;
using Newtonsoft.Json;
using System.IO;

namespace Telekom.SendSms
{
    /// <summary>
    /// Wrapper for Telekom Send SMS Query Report service
    /// </summary>
    public class NotificationSubscribeClient : TelekomClient
    {
        /// <summary>
        /// URL Path to Send SMS services. Can be overridden if necessary.
        /// {0} is replaced by selected environment
        /// </summary>
        public static String ServicePath = "/plone/sms/rest/{0}/smsmessaging/v1";

        /// <summary>
        /// Constructs a Send SMS API Query Report client with specified authentication method and environment.
        /// </summary>
        /// <param name="authentication">Authentication instance</param>
        /// <param name="serviceEnvironment">Environment used for this client's service invocations</param>
        public NotificationSubscribeClient(TelekomAuth authentication, ServiceEnvironment serviceEnvironment)
            : base(authentication, serviceEnvironment, ServicePath)
        {
        }

        /// <summary>
        /// Queries the given reportId from an SMS
        /// </summary>
        /// <param name="request">prepared request to use for the query</param>
        public SmsResponse subscribeNotifications(NotificationSubscribeRequest request)
        {
		    EnsureRequestValid(request);

		    string uri = ServiceBaseUrl + "/outbound/{0}/subscriptions";
		
		    uri = string.Format(uri, Uri.EscapeDataString(request.senderAddress));

		    return CreateAuthenticatedJsonRequest<SmsResponse>(uri, request).Execute();
	    }

        private TelekomJsonWebRequest<SmsResponse> CreateAuthenticatedJsonRequest<ResponseType>(string uri, NotificationSubscribeRequest request)
        {
            TelekomJsonWebRequest<SmsResponse> webRequest = CreateAuthenticatedRequest<SmsResponse>(uri, HttpMethod.POST);

            Dictionary<string, Dictionary<string, object>> jsonRequest = new Dictionary<string, Dictionary<string, object>>();

            //---------------------------------------------------------------------------------------------------------------
            Dictionary<string, object> val = new Dictionary<string, object>();

            Dictionary<string, string> helper = new Dictionary<string, string>();

            helper.Add("notifyURL", request.notifyURL);
            if (request.callbackData != null)
            {
                helper.Add("callbackData", request.callbackData);
            }
            val.Add("callbackReference", helper);

            jsonRequest.Add("deliveryReceiptSubscription", val);

            JsonSerializer serializer = new JsonSerializer();

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, jsonRequest);
            writer.Flush();
            sw.Flush();
            ms.Position = 0;
            ms.Flush();

            webRequest.SetRawContent(ms, "application/json");

            return webRequest;
        }
    }
}
