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
using Telekom.Common.Model;
using System.Runtime.Serialization;

namespace Telekom.Common.Auth.OAuth2
{
    /// <summary>
    /// Service response containing access token and optionally a refresh token
    /// </summary>
    [DataContract]
    public class AccessTokenResponse
    {
        /// <summary>
        /// Type of the received token
        /// </summary>
        [DataMember(Name="token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        [DataMember(Name="access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token (not always present)
        /// </summary>
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// The time the access token expires (in seconds from now)
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }
    }
}
