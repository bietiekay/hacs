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
using System.Runtime.Serialization.Json;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using Telekom.Common.Model;

namespace Telekom.Common.WebRequest
{
    internal class JsonSerializer<T>
    {
        public static T Deserialize(Stream dataStream)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            #region Workaround
            // Workaround: Before the call is established, the VoiceCall backend returns an object instead of an
            // expected value type           
            string data = new StreamReader(dataStream, Encoding.UTF8).ReadToEnd();
            data = data.Replace("{\"@nil\":\"true\"}", "null");
            dataStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            #endregion
            if (dataStream.Length == 0)
                return default(T);
            return (T)serializer.ReadObject(dataStream);
        }

        public static Stream Serialize(T data)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, data);
            stream.Position = 0; // Rewind to prepare for reading
            return stream;
        }
    }
}
