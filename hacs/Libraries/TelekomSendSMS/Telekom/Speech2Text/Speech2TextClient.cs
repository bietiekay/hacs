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
using Telekom.Speech2Text.Model;
using Telekom.Common.Model;
using Telekom.Common.WebRequest;

namespace Telekom.Speech2Text
{
    /// <summary>
    /// Wrapper for Telekom Voice Call service
    /// </summary>
    public class Speech2TextClient : TelekomClient
    {

        /// <summary>
        /// URL Path to Voice call services. Can be overridden if necessary.
        /// {0} is replaced by selected environment
        /// </summary>
        public static string ServicePath = "/plone/speech2text/rest/v1";

        /// <summary>
        /// Constructs a Voice Call API client with specified authentication method and environment.
        /// </summary>
        /// <param name="authentication">Authentication instance</param>
        /// <param name="environment">Environment used for this client's service invocations</param>
        public Speech2TextClient(TelekomAuth authentication, ServiceEnvironment environment)
            : base(authentication, environment, ServicePath)
        {
        }

        #region discovery
        public Speech2TextResponse discovery(Speech2TextDiscoveryRequest request)
        {
            EnsureRequestValid(request);

            string uri = ServiceBaseUrl + "/discovery";

            return CreateAuthenticatedRequest<Speech2TextResponse>(uri, HttpMethod.GET, request).Execute();
        }
        #endregion

        #region transcription
        public Speech2TextResponse transcription(Speech2TextRequest request, ServiceEnvironment env)
        {
            //TODO: EnsureRequestValid(request);

            String uri = ServiceBaseUrl + "/{0}/transcriptions";
		
		    uri = string.Format(uri, Uri.EscapeDataString(Enum.GetName(typeof(ServiceEnvironment), env).ToLower()));

            Dictionary<string, string> headers = new Dictionary<string, string>();

		    headers.Add("Accept-Topic", request.AcceptTopic);
		    headers.Add("Accept-Language", request.Language);
		    headers.Add("Audio-File-Content-Type", request.AudioFileContentType);
		
		    return createAuthenticatedTranscriptionRequest<Speech2TextResponse>(uri, request)
				    .ExecuteCustom(headers);
        }
        #endregion

        /// <summary>
        /// Create a web request with access token and serialized parameters
        /// </summary>
        /// <typeparam name="ResponseType">Type which the response will be deserialized into</typeparam>
        /// <param name="uri">Target URI of the request</param>
        /// <param name="method">HTTP method of the request</param>
        /// <param name="requestParams">Object containing request parameters</param>
        /// <returns>configured web request with added header fields</returns>
        private TelekomJsonWebRequest<ResponseType> createAuthenticatedTranscriptionRequest<ResponseType>(string uri, Speech2TextRequest requestParams)
        {
            TelekomJsonWebRequest<ResponseType> webRequest = CreateAuthenticatedRequest<ResponseType>(uri, HttpMethod.POST, null);

            webRequest.AddParameter("file", requestParams.FileData);

            return webRequest;
        }
    }
}
