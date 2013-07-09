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

namespace Telekom.SmsValidation.Model
{
    /// <summary>
    /// Parameters for a call to send the validation keyword
    /// </summary>
    public class SendValidationKeywordRequest : TelekomRequest
    {
        /// <summary>
        /// Number to send the keyword to
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// The accompanying message that should be sent with the validation code.
        /// This message must contain two placeholders as shown in the following example:
        /// "The keyword for validating your phone number with example.com is #key# and is valid until #validUntil#."
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Originator of the message as shown on receiver's phone
        /// </summary>
        public string Originator { get; set; }

        /// <summary>
        /// Sub-account to bill. If omitted, the main account is selected.
        /// </summary>
        public string Account { get; set; }

        internal override void EnforceRequiredFields()
        {
            base.EnforceRequiredFields();

            if (!string.IsNullOrEmpty(Message))
            {
                if (!Message.Contains("#key#"))
                    throw new ArgumentException("Message does not contain placeholder #key#");

                if (!Message.Contains("#validUntil#"))
                    throw new ArgumentException("Message does not contain placeholder #validUntil#");
            }
        }
    }
}
