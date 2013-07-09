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

namespace Telekom.Common.Model
{
    /// <summary>
    /// Common status result of SMS operations
    /// </summary>
    [DataContract]
    public class SmsResponseStatusValues
    {
        /// <summary>
        /// Message ID of the Exception received
        /// </summary>
        [DataMember(Name = "messageId")]
        public String messageId { get; set; }

        /// <summary>
        /// Text of the Exception received
        /// </summary>
        [DataMember(Name = "text")]
        public String text { get; set; }

        /// <summary>
        /// variables of the Exception received
        /// </summary>
        [DataMember(Name = "variables")]
        public String[] variables { get; set; }
    }
}
