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
using Telekom.Common.Auth;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Net;
using Telekom.Common.Model;
using Telekom.Common.WebRequest;

//! Provides core functionality for Telekom service
namespace Telekom.Common
{
    /// <summary>
    /// Base class for all Telekom REST SDK clients
    /// </summary>
    public abstract class TelekomClient
    {
        /// <summary>
        /// Address of Telekom API server.
        /// </summary>
        public static string TelekomBaseUrl = "https://gateway.developer.telekom.com";

        /// <summary>
        /// Base URL to the concrete service.
        /// </summary>
        protected string ServiceBaseUrl { get; private set; }

        /// <summary>
        /// Authentication object that provides us with an access token for the requests
        /// </summary>
        protected TelekomAuth authentication;    

        internal TelekomClient(TelekomAuth authentication, ServiceEnvironment environment,
            string baseUrlTemplate)
        {
            if (authentication == null)
            {
                throw new ArgumentNullException("authentication");
            }

            this.authentication = authentication;

            // Replace the placeholder for environment in baseUrlTemplate
            string environmentString = Enum.GetName(typeof(ServiceEnvironment), environment).ToLower();
            ServiceBaseUrl = TelekomBaseUrl + string.Format(baseUrlTemplate, environmentString);
        }

        internal void EnsureRequestValid(TelekomRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            request.EnforceRequiredFields();
        }

        /// <summary>
        /// Create a web request with access token
        /// </summary>
        /// <typeparam name="ResponseType">Type which the response will be deserialized into</typeparam>
        /// <param name="uri">Target URI of the request</param>
        /// <param name="method">HTTP method of the request</param>
        /// <returns>configured web request with added header fields</returns>
        internal virtual TelekomJsonWebRequest<ResponseType> CreateAuthenticatedRequest<ResponseType>(string uri, HttpMethod method)
            where ResponseType : TelekomResponse
        {
            return CreateAuthenticatedRequest<ResponseType>(uri, method, null);
        }

        /// <summary>
        /// Create a web request with access token and serialized parameters
        /// </summary>
        /// <typeparam name="ResponseType">Type which the response will be deserialized into</typeparam>
        /// <param name="uri">Target URI of the request</param>
        /// <param name="method">HTTP method of the request</param>
        /// <param name="requestParams">Object containing request parameters</param>
        /// <returns>configured web request with added header fields</returns>
        internal virtual TelekomJsonWebRequest<ResponseType> CreateAuthenticatedRequest<ResponseType>(string uri, HttpMethod method, TelekomRequest requestParams)
        {
            if (!authentication.HasToken())
                throw new InvalidOperationException("No access token fetched.");

            var request = new TelekomJsonWebRequest<ResponseType>(uri, method);
            request.AuthHeader = "OAuth realm=\"developergarden.com\", oauth_token=\"" + authentication.AccessToken + "\"";
            if (requestParams != null)
                requestParams.BuildRequestParameters(request);
            return request;
        }
    }
}
