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

namespace Telekom.SendSms.Model
{
    /// <summary>
    /// Parameter object for SendSms subscribe notifications call
    /// </summary>
    public class ReceiveNotificationSubscribeRequest : TelekomRequest
    {

        /// <summary>
        /// destinationAddress
        /// </summary>
        /// <returns></returns>
        [Required]
        public String destinationAddress { get; set; }

        /// <summary>
        /// callbackData
        /// </summary>
        /// <returns></returns>
        [Required]
        public String callbackData { get; set; }

        /// <summary>
        /// clientCorrelator
        /// </summary>
        /// <returns></returns>
        public String clientCorrelator { get; set; }

        /// <summary>
        /// notify URL
        /// </summary>
        /// <returns></returns>
        [Required]
        public String notifyURL { get; set; }

        /// <summary>
        /// criteria
        /// </summary>
        /// <returns></returns>
        public String criteria { get; set; }

        /// <summary>
        /// notificationFormat
        /// </summary>
        /// <returns></returns>
        public String notificationFormat { get; set; }

        /// <summary>
        /// account
        /// </summary>
        /// <returns></returns>
        public String account { get; set; }
    }
}
