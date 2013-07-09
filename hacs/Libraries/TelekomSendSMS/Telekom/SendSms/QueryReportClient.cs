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

namespace Telekom.SendSms
{
    /// <summary>
    /// Wrapper for Telekom Send SMS Query Report service
    /// </summary>
    public class QueryReportClient : TelekomClient
    {
        /// <summary>
        /// URL Path to Send SMS services. Can be overridden if necessary.
        /// {0} is replaced by selected environment
        /// </summary>
        public static string ServicePath = "/plone/sms/rest/{0}/smsmessaging/v1";

        private String reportId { get; set;}

        /// <summary>
        /// Constructs a Send SMS API Query Report client with specified authentication method and environment.
        /// </summary>
        /// <param name="authentication">Authentication instance</param>
        /// <param name="serviceEnvironment">Environment used for this client's service invocations</param>
        public QueryReportClient(TelekomAuth authentication, ServiceEnvironment serviceEnvironment)
            : base(authentication, serviceEnvironment, ServicePath)
        {
        }

        /// <summary>
        /// Queries the given reportId from an SMS
        /// </summary>
        /// <param name="request">prepared request to use for the query</param>
        /// <param name="reportId">reportId of the message to be queried</param>
        public SmsResponse queryReport(QueryReportRequest request, String reportId)
        {
            this.reportId = reportId;
            return CreateQueryReportRequest(request).Execute();
        }

        private TelekomJsonWebRequest<SmsResponse> CreateQueryReportRequest(QueryReportRequest request)
        {
            EnsureRequestValid(request);

            string uri = ServiceBaseUrl + "/outbound/{0}/requests/{1}/deliveryInfos";
            uri = string.Format(uri, Uri.EscapeDataString(request.senderAddress), Uri.EscapeDataString(this.reportId));

            return CreateAuthenticatedRequest<SmsResponse>(uri, HttpMethod.GET);
        }
    }
}
