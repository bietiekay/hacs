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
using System.IO;

using Telekom.Common.Auth;
using Telekom.Common;
using Telekom.Common.Model;
using Telekom.Common.WebRequest;
using Telekom.SendMms.Model;

//! Send MMS allows to send multimedia messages (MMS) to recipients in national mobile networks.
namespace Telekom.SendMms
{
    /// <summary>
    /// Wrapper for Telekom Send MMS service
    /// </summary>
    public class SendMmsClient : TelekomClient
    {
        /// <summary>
        /// URL Path to Send MMS services. Can be overridden if neccesary.
        /// {0} is replaced by selected environment
        /// </summary>
        public static string ServicePath = "/plone/odg-mms/rest/{0}";

        /// <summary>
        /// Constructs a Send SMS API client with specified authentication method and environment.
        /// </summary>
        /// <param name="authentication">Authentication instance</param>
        /// <param name="environment">Environment used for this client's service invocations</param>
        public SendMmsClient(TelekomAuth authentication, ServiceEnvironment environment)
            : base(authentication, environment, ServicePath)
        {
        }

        private TelekomJsonWebRequest<TelekomResponse> CreateSendMmsWebRequest(SendMmsRequest request)
        {
            EnsureRequestValid(request);

            string uri = ServiceBaseUrl + "/sendMMS";
            return CreateAuthenticatedRequest<TelekomResponse>(uri, HttpMethod.POST, request);
        }

        /// <summary>Send a MMS</summary>
        /// <param name="request">Parameter object</param> 
        /// <returns>Result of this operation</returns>
        public TelekomResponse SendMms(SendMmsRequest request)
        {
            return CreateSendMmsWebRequest(request).Execute();
        }

        /// <summary>Send a MMS (asynchronously)</summary>
        /// <param name="request">Parameter object</param> 
        /// <param name="callback">Handler to invoke after completion</param>
        public void SendMms(SendMmsRequest request, Action<TelekomResponse> callback)
        {
            CreateSendMmsWebRequest(request).ExecuteAsync(a => callback(a));
        }

    }
}
