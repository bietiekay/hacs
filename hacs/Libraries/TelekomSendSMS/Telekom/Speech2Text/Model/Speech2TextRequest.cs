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
using System.Runtime.Serialization;
using System.IO;

namespace Telekom.Speech2Text.Model
{
    /// <summary>
    /// Request container of a speech2text transcription api call
    /// </summary>
    [DataContract]
    public class Speech2TextRequest
    {
        [DataMember(Name = "fileData")]
        public Stream FileData { get; set; }

        [DataMember(Name = "acceptTopic")]
        public String AcceptTopic { get; set; }

        [DataMember(Name = "language")]
        public String Language { get; set; }

        [DataMember(Name = "audioFileContentType")]
        public String AudioFileContentType { get; set; }

        [DataMember(Name = "account")]
        public String Account { get; set; }
    }
}
