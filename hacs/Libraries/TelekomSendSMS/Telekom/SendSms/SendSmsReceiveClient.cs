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
using Telekom.SendSms.Model;
using Telekom.Common.Auth;
using Telekom.Common;
using Telekom.Common.Model;
using Telekom.Common.WebRequest;

namespace Telekom.SendSms
{
    /// <summary>
    /// Wrapper for Telekom Send SMS receive service
    /// </summary>
    public class SendSmsReceiveClient : TelekomClient
    {

        /// <summary>
        /// URL Path to Send SMS services. Can be overridden if necessary.
        /// {0} is replaced by selected environment
        /// </summary>
        public static string ServicePath = "/plone/sms/rest/{0}/smsmessaging/v1";


        /// <summary>
        /// Constructs a Send SMS API client with specified authentication method and environment.
        /// </summary>
        /// <param name="authentication">Authentication instance</param>
        /// <param name="environment">Environment used for this client's service invocations</param>
        public SendSmsReceiveClient(TelekomAuth authentication, ServiceEnvironment serviceEnvironment)
            : base(authentication, serviceEnvironment, ServicePath)
        {
        }

        private TelekomJsonWebRequest<SmsResponse> CreateSendSmsReceiveWebRequest(SendSmsReceiveRequest request)
        {
            EnsureRequestValid(request);

            string uri = ServiceBaseUrl + "/inbound/registrations/{0}/messages";
            uri = string.Format(uri, Uri.EscapeDataString(request.registrationId));
            //return CreateAuthenticatedRequest<TelekomResponse>(uri, HttpMethod.POST, request);
            return CreateAuthenticatedJsonRequest<SmsResponse>(uri, request);
        }

        private TelekomJsonWebRequest<SmsResponse> CreateAuthenticatedJsonRequest<ResponseType>(string uri, SendSmsReceiveRequest request)
        {
            TelekomJsonWebRequest<SmsResponse> webRequest = CreateAuthenticatedRequest<SmsResponse>(uri, HttpMethod.GET, request);

            return webRequest;
        }

        /// <summary>Send an SMS</summary>
        /// <param name="request">Parameter object</param> 
        /// <returns>Result of this operation</returns>
        public SmsResponse SendSmsReceive(SendSmsReceiveRequest request)
        {
            return CreateSendSmsReceiveWebRequest(request).Execute();
        }

        /// <summary>Send an SMS (asynchronously)</summary>
        /// <param name="request">Parameter object</param> 
        /// <param name="callback">Handler to invoke after completion</param>
        public void SendSmsReceive(SendSmsReceiveRequest request, Action<TelekomResponse> callback)
        {
            CreateSendSmsReceiveWebRequest(request).ExecuteAsync(a => callback(a));
        }
    }
}
