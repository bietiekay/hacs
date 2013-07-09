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
using System.Net;
using Telekom.Common.Model;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace Telekom.Common.WebRequest
{
    /// <summary>
    /// Web Request whose response will be deserialized to the specified ResponseType.
    /// Response format is JSON, ResponseType class must be decorated as DataContract.
    /// </summary>
    /// <typeparam name="ResponseType">Type to deserialize into</typeparam>
    internal class TelekomJsonWebRequest<ResponseType> : TelekomWebRequest
    {
        public TelekomJsonWebRequest(string uri, HttpMethod method) : base(uri, method) { }

        public ResponseType Execute()
        {
            WebResponse response = base.ExecuteRaw();
            Stream responseStream = response.GetResponseStream();
            
            return JsonSerializer<ResponseType>.Deserialize(responseStream);
        }

        public ResponseType ExecuteCustom(Dictionary<string, string> additionalHeaders)
        {
            this.additionalHeaders = additionalHeaders;

            WebResponse response = base.ExecuteRaw(true);
            Stream responseStream = response.GetResponseStream();

            return JsonSerializer<ResponseType>.Deserialize(responseStream);
        }

        public void ExecuteAsync(Action<ResponseType> callback)
        {
            base.ExecuteRawAsync(delegate (WebResponse response) {
                Stream responseStream = response.GetResponseStream();
                ResponseType responseObject = JsonSerializer<ResponseType>.Deserialize(responseStream);
                callback(responseObject);
            });
        }

    }
}
