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

namespace Telekom.Common
{
    /// <summary>
    /// Available environments
    /// </summary>
    public enum ServiceEnvironment
    {
        /// <summary>
        /// Productive / live environment
        /// </summary>
        Production,
        /// <summary>
        /// Productive / live environment basic (SendSMS)
	    /// </summary>
        Budget,
        /// <summary>
        /// Productive / live environment premium (SendSMS)
        /// </summary>
        Premium,
        /// <summary>
        /// Sandbox environment
        /// </summary>
        Sandbox,
        /// <summary>
        /// Environment for tests
        /// </summary>
        Mock
    }

    /// <summary>
    /// HTTP Method for REST requests
    /// </summary>
    internal enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

}
