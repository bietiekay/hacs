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
    public class SmsResponseStatus
    {
        /// <summary>
        /// A policyException occured while sending an SMS
        /// </summary>
        [DataMember(Name = "policyException")]
        public SmsResponseStatusValues policyException { get; set; }

        /// <summary>
        /// A serviceException occured while sending an SMS
        /// </summary>
        [DataMember(Name = "serviceException")]
        public SmsResponseStatusValues serviceException { get; set; }

        /// <summary>
        /// A list of deliveryInfos applied when querying sms status
        /// </summary>
        [DataMember(Name = "deliveryInfo")]
        public SmsResponseDeliveryValues[] deliveryInfo { get; set; }

        /// <summary>
        /// A list of deliveryInfos applied when querying sms status
        /// </summary>
        [DataMember(Name = "subscription")]
        public SmsResponseSubscriptionValues subscription { get; set; }

        /// <summary>
        /// A list of deliveryInfos applied when querying sms status
        /// </summary>
        [DataMember(Name = "inboundSMSMessage")]
        public SmsResponseInboundSmsMessageValues[] inboundSMSMessage { get; set; }

        /// <summary>
        /// The resourceURL borrowed from the request
        /// </summary>
        [DataMember(Name = "resourceURL")]
        public String resourceURL { get; set; }

        /// <summary>
        /// The criteria borrowed from the request
        /// </summary>
        [DataMember(Name = "criteria")]
        public String criteria { get; set; }

        /// <summary>
        /// The destinationAddress borrowed from the request
        /// </summary>
        [DataMember(Name = "destinationAddress")]
        public String destinationAddress { get; set; }

        /// <summary>
        /// The notificationFormat borrowed from the request
        /// </summary>
        [DataMember(Name = "notificationFormat")]
        public String notificationFormat { get; set; }

        /// <summary>
        /// The clientCorrelator borrowed from the request
        /// </summary>
        [DataMember(Name = "clientCorrelator")]
        public String clientCorrelator { get; set; }

        /// <summary>
        /// The account borrowed from the request
        /// </summary>
        [DataMember(Name = "account")]
        public String account { get; set; }

        /// <summary>
        /// The numberOfMessagesInThisBatch borrowed from the request
        /// </summary>
        [DataMember(Name = "numberOfMessagesInThisBatch")]
        public String numberOfMessagesInThisBatch { get; set; }

        /// <summary>
        /// The totalNumberOfPendingMessages borrowed from the request
        /// </summary>
        [DataMember(Name = "totalNumberOfPendingMessages")]
        public String totalNumberOfPendingMessages { get; set; }

        /// <summary>
        /// callbackReference borrowed from the request
        /// </summary>
        [DataMember(Name = "callbackReference")]
        public SmsResponseSubscriptionValues callbackReference { get; set; }
    }
}
