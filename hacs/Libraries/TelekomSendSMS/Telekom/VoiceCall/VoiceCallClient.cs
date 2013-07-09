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
using Telekom.VoiceCall.Model;
using Telekom.Common.WebRequest;
using Telekom.Common.Model;

//! Voice Call connects two callers over the fixed or mobile network. When using the service, the requested call numbers are entered and the connection between the two callers is set up automatically.
namespace Telekom.VoiceCall
{
    /// <summary>
    /// Wrapper for Telekom Voice Call service
    /// </summary>
    public class VoiceCallClient : TelekomClient
    {
        /// <summary>
        /// URL Path to Voice call services. Can be overridden if necessary.
        /// {0} is replaced by selected environment
        /// </summary>
        public static string ServicePath = "/plone/odg-voicecall/rest/{0}";

        /// <summary>
        /// Constructs a Voice Call API client with specified authentication method and environment.
        /// </summary>
        /// <param name="authentication">Authentication instance</param>
        /// <param name="environment">Environment used for this client's service invocations</param>
        public VoiceCallClient(TelekomAuth authentication, ServiceEnvironment environment)
            : base(authentication, environment, ServicePath)
        {
        }

        #region NewCall
        private TelekomJsonWebRequest<NewCallResponse> CreateNewCallWebRequest(NewCallRequest request)
        {
            EnsureRequestValid(request);

            string uri = ServiceBaseUrl + "/call";

            return CreateAuthenticatedRequest<NewCallResponse>(uri, HttpMethod.POST, request);
        }

        /// <summary>
        /// Start a new call
        /// </summary>
        /// <param name="request">Call parameters</param>
        /// <returns>Service call response</returns>
        public NewCallResponse NewCall(NewCallRequest request)
        {
            return CreateNewCallWebRequest(request).Execute();
        }

        /// <summary>
        /// Start a new call (async)
        /// </summary>
        /// <param name="request">Call parameters</param>
        /// <param name="callback">Handler to invoke after completion</param>
        public void NewCall(NewCallRequest request, Action<NewCallResponse> callback)
        {
            CreateNewCallWebRequest(request).ExecuteAsync(a => callback(a));
        }
        #endregion

        #region CallStatus
        private TelekomJsonWebRequest<CallStatusResponse> CreateCallStatusWebRequest(CallStatusRequest request)
        {
            EnsureRequestValid(request);

            string uri = ServiceBaseUrl
                + string.Format("/call/{0}", Uri.EscapeUriString(request.SessionId));

            return CreateAuthenticatedRequest<CallStatusResponse>(uri, HttpMethod.GET, request);
        }

        /// <summary>
        /// Query the status of a call
        /// </summary>
        /// <param name="request">Call parameters</param>
        /// <returns>Service call response</returns>
        public CallStatusResponse CallStatus(CallStatusRequest request)
        {
            return CreateCallStatusWebRequest(request).Execute();
        }

        /// <summary>
        /// Query the status of a call (async)
        /// </summary>
        /// <param name="request">Call parameters</param>
        /// <param name="callback">Handler to invoke after completion</param>
        public void CallStatus(CallStatusRequest request, Action<CallStatusResponse> callback)
        {
            CreateCallStatusWebRequest(request).ExecuteAsync(a => callback(a));
        }
        #endregion

        #region TeardownCall
        private TelekomJsonWebRequest<TelekomResponse> CreateTeardownCallWebRequest(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                throw new ArgumentNullException("sessionId");

            string uri = ServiceBaseUrl
                + string.Format("/call/{0}", Uri.EscapeUriString(sessionId));

            return CreateAuthenticatedRequest<TelekomResponse>(uri, HttpMethod.DELETE);
        }

        /// <summary>
        /// End a call
        /// </summary>
        /// <param name="sessionId">session ID of the call to end</param>
        /// <returns>Service call response</returns>
        public TelekomResponse TeardownCall(string sessionId)
        {
            return CreateTeardownCallWebRequest(sessionId).Execute();
        }

        /// <summary>
        /// End a call
        /// </summary>
        /// <param name="sessionId">session ID of the call to end</param>
        /// <param name="callback">Handler to invoke after completion</param>
        public void TeardownCall(string sessionId, Action<TelekomResponse> callback)
        {
            CreateTeardownCallWebRequest(sessionId).ExecuteAsync(a => callback(a));
        }
        #endregion

    }
}
