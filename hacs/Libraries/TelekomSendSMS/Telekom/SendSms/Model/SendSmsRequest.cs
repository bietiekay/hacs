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
using Telekom.Common.Model.Validation;
using Telekom.Common.WebRequest;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;

namespace Telekom.SendSms.Model
{
    /// <summary>
    /// Parameter object for SendSms call
    /// </summary>
    public class SendSmsRequest : TelekomRequest
    {
        /// <summary>
        /// Comma separated phone numbers of the recipient(s) as expected by the web service
        /// </summary>
        /// <returns></returns>
        [Required]
        public string[] Address
        {
            get
            {
                //return ((Numbers != null) && (Numbers.Count > 0)) ? string.Join(",", Numbers.ToArray()) : null;
                return Numbers.ToArray();
            }
            set
            {
                Numbers = value.ToList();
            }
        }

        /// <summary>
        /// Phone Number(s) of recipient(s)
        /// </summary>
        [NoHttpParameter]
        public ICollection<string> Numbers { get; set; }

        /// <summary>
        /// The message to send
        /// </summary>
        [Required]
        public String Message { get; set; }

        /// <summary>
        /// Sender, as shown at the receiver
        /// </summary>
        [Required]
        public String SenderAddress { get; set; }

        /// <summary>
        /// Sendername, as shown at the receiver
        /// </summary>
        public String SenderName { get; set; }

        /// <summary>
        /// Defines, if the SMS should be sent as flash SMS
        /// </summary>
        public OutboundSMSType SMSType { get; set; }

        /// <summary>
        /// Account-ID of the sub account which should be billed for this service call
        /// </summary>
        public String Account { get; set; }

        /// <summary>
        /// message identifier for notification
        /// </summary>
        public String CallbackData { get; set; }

        /// <summary>
        /// url to send the notification to
        /// </summary>
        public String NotifyURL { get; set; }

        /// <summary>
        /// specifies request sms character encoding
        /// </summary>
        public String Encoding { get; set; }

        /// <summary>
        /// identifies requests
        /// </summary>
        public String ClientCorrelator { get; set; }


    }
}
