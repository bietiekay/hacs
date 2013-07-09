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
    /// Common data in all responses from Telekom service
    /// </summary>
    [DataContract]
    public class SmsResponse : TelekomResponse
    {
        /// <summary>
        /// Returned error status of an operation
        /// </summary>
        [DataMember(Name = "requestError")]
        public SmsResponseStatus requestError { get; set; }

        /// <summary>
        /// Returned successful status of an query report operation
        /// </summary>
        [DataMember(Name = "deliveryInfoList")]
        public SmsResponseStatus deliveryInfoList { get; set; }

        /// <summary>
        /// Returned successful status of an notification subscription operation
        /// </summary>
        [DataMember(Name = "deliveryReceiptSubscription")]
        public SmsResponseStatus deliveryReceiptSubscription { get; set; }

        /// <summary>
        /// Returned successful status of an notification subscription operation
        /// </summary>
        [DataMember(Name = "inboundSMSMessageList")]
        public SmsResponseStatus inboundSMSMessageList { get; set; }

        /// <summary>
        /// Returned successful status of an notification subscription for an sms receive operation
        /// </summary>
        [DataMember(Name = "subscription")]
        public SmsResponseStatus subscription { get; set; }

        /// <summary>
        /// Checks if this status represents a successful response
        /// </summary>
        new public bool Success
        {
            get
            {
                return (requestError == null);
            }
        }

    }
}
