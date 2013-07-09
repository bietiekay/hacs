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
using System.ServiceModel.Channels;

//! Authentication methods
namespace Telekom.Common.Auth
{
    /// <summary>
    /// Base class for all authentication methods.
    /// Provides the access token for Telekom services to SDK clients.
    /// </summary>
    public abstract class TelekomAuth
    {
        /// <summary>
        /// Start a new authentication
        /// </summary>
        public TelekomAuth()
        {
        }

        /// <summary>
        /// Continue an already authenticated session
        /// </summary>
        /// <param name="accessToken">Stored token</param>
        /// <param name="accessTokenValidUntil">Stored token expiration date</param>
        public TelekomAuth(String accessToken, DateTime accessTokenValidUntil)
        {
            AccessToken = accessToken;
            AccessTokenValidUntil = accessTokenValidUntil;
        }

        /// <summary>
        /// Returns or sets the current authentication token.
        /// Null if no token has been fetched or authentication has failed.
        /// </summary>
        public string AccessToken { get; protected set; }

        /// <summary>
        /// Expiration date of the current access token.
        /// </summary>
        public DateTime? AccessTokenValidUntil { get; protected set; }

        /// <summary>
        /// Checks if there is an access token (whether valid or not)
        /// </summary>
        /// <returns>if there is a stored access token</returns>
        public bool HasToken()
        {
            return !string.IsNullOrEmpty(AccessToken);
        }

        /// <summary>
        /// Returns if we have a valid (not expired) access token.
        /// This call tries to refresh the token, if authentication mode supports it.
        /// </summary>
        /// <returns>if valid token is present after method call</returns>
        public virtual bool HasValidToken()
        {
            return HasToken() && (DateTime.Now <= AccessTokenValidUntil);
        }

        /// <summary>
        /// Returns if we have a valid (not expired) access token.
        /// This call tries to refresh the token, if authentication mode supports it.
        /// </summary>
        /// <param name="callback">Method to invoke with the result</param>
        public virtual void HasValidToken(Action<bool> callback)
        {
            bool result = HasValidToken();
            callback(result);
        }
    }
}
