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
    /// Common status result of SMS Retrieve operations
    /// </summary>
    [DataContract]
    public class SmsResponseInboundSmsMessageValues
    {
        /// <summary>
        /// dateTime
        /// </summary>
        [DataMember(Name = "dateTime")]
        public String dateTime { get; set; }

        /// <summary>
        /// destinationAddress
        /// </summary>
        [DataMember(Name = "destinationAddress")]
        public String destinationAddress { get; set; }

        /// <summary>
        /// messageId
        /// </summary>
        [DataMember(Name = "messageId")]
        public String messageId { get; set; }

        /// <summary>
        /// message
        /// </summary>
        [DataMember(Name = "message")]
        public String message { get; set; }

        /// <summary>
        /// resourceURL
        /// </summary>
        [DataMember(Name = "resourceURL")]
        public String resourceURL { get; set; }

        /// <summary>
        /// senderAddress
        /// </summary>
        [DataMember(Name = "senderAddress")]
        public String senderAddress { get; set; }
    }
}
