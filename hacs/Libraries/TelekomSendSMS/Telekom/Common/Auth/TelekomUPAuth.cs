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
using System.Net;
using Telekom.Common.WebRequest;

namespace Telekom.Common.Auth
{
    /// <summary>
    /// Authentication to Telekom services with username and password
    /// </summary>
    public class TelekomUPAuth : TelekomAuth
    {
        /// <summary>
        /// Url of STS service. Can be overwritten if needed.
        /// </summary>
        public static string Url = "https://sts.idm.telekom.com/rest-v1/tokens/odg";

        private string username;
        private string password;

        /// <summary>
        /// Authentication with account credentials (new session)
        /// </summary>
        /// <param name="username">Telekom user name</param>
        /// <param name="password">Telekom password</param>
        public TelekomUPAuth(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// Authentication with account credentials (continue session)
        /// </summary>
        /// <param name="username">Telekom user name</param>
        /// <param name="password">Telekom password</param>
        /// <param name="accessToken">Saved access token for Telekom services</param>
        /// <param name="accessTokenValidUntil">Saved validity of the access token</param>
        public TelekomUPAuth(string username, string password, string accessToken, DateTime accessTokenValidUntil)
            : base(accessToken, accessTokenValidUntil)
        {
            this.username = username;
            this.password = password;
        }        

        /// <summary>
        /// Extract token information from HTTP response
        /// </summary>
        /// <param name="response">response from web request</param>
        private void ParseTokenResponse(WebResponse response)
        {
            string responseData;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseData = reader.ReadToEnd();
            }
            // Extract token from response
            int tokenStart = responseData.IndexOf("token=");
            if (tokenStart >= 0)
            {
                string tokenData = responseData.Substring(tokenStart + 6);
                DateTime tokenExpires = DateTime.Parse(response.Headers["Expires"]);

                AccessToken = tokenData;
                AccessTokenValidUntil = tokenExpires;
            }
        }

        /// <summary>
        /// Prepare a web request to STS server
        /// </summary>
        /// <returns></returns>
        private TelekomWebRequest CreateTokenRequest()
        {
            TelekomWebRequest request = new TelekomWebRequest(Url, HttpMethod.GET);
            request.Credentials = new NetworkCredential(username, password);
            request.Accept = "text/plain";
            return request;
        }

        /// <summary>
        /// Request an access token for Telekom services.
        /// This method can both be used to retrieve the initial token and to fetch
        /// a new one if the existing has expired. After completion, use HasValidToken()
        /// to check if the call succeeded.
        /// </summary>
        public void RequestAccessToken()
        {
            WebResponse response = CreateTokenRequest().ExecuteRaw();
            if (response != null)
            {
                ParseTokenResponse(response);
            }
        }

        /// <summary>
        /// Request access token asynchronously.
        /// @see RequestAccessToken
        /// </summary>
        /// <param name="callback">method to call if the operation finished</param>
        /// <see cref="RequestAccessToken()">For more detail see synchronous variant</see>
        public void RequestAccessToken(Action callback)
        {
            CreateTokenRequest().ExecuteRawAsync(delegate(WebResponse response)
            {
                if (response != null)
                {
                    ParseTokenResponse(response);
                }
                callback();
            });
        }
    }
}
