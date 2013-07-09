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

namespace Telekom.VoiceCall.Model
{
    /// <summary>
    /// Parameters for starting a new call
    /// </summary>
    public class NewCallRequest : TelekomRequest
    {
        /// <summary>
        /// Phone number of the first participant
        /// </summary>
        [Required]
        public string Anumber { get; set; }

        /// <summary>
        /// Comma separated phone numbers of the second participant(s).
        /// </summary>
        [Required]
        public string Bnumber
        {
            get
            {
                return ((Bnumbers != null) && (Bnumbers.Count > 0)) ? string.Join(",", Bnumbers.ToArray()) : null;
            }
            set
            {
                Bnumbers = value.Split(',').ToList();
            }
        }

        /// <summary>
        /// Phone number(s) of the second participant(s). Provide multiple numbers to create a sequential call.
        /// </summary>
        [NoHttpParameter]
        public ICollection<string> Bnumbers { get; set; }

        /// <summary>
        /// Duration in seconds after which the Voice Call service disconnects a call without receiving a callStatus request.
        /// A value of 0 means that there is no callStatus request needed to maintain the connection.
        /// </summary>
        public int? Expiration { get; set; }

        /// <summary>
        /// Maximum duration of the connection in seconds.
        /// Note: The system limit also applies.
        /// </summary>
        public int? Maxduration { get; set; }

        /// <summary>
        /// The time in seconds after which the next phone number in the list of b-numbers is called during a sequential call.
        /// </summary>
        public int? Maxwait { get; set; }

        /// <summary>
        /// Phone number suppression with the first participant.
        /// </summary>
        public bool? Aprivacy { get; set; }

        /// <summary>
        /// Phone number suppression with the second participant.
        /// </summary>
        public bool? Bprivacy { get; set; }

        /// <summary>
        /// Account ID of the sub-account that should be billed for the service use. If the parameter is not specified, the main account is billed.
        /// </summary>
        public string Account { get; set; }

    }
}
