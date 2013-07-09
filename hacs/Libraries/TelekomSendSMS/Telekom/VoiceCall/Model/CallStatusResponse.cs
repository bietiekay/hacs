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
using System.Runtime.Serialization;

namespace Telekom.VoiceCall.Model
{
    /// <summary>
    /// Response of a call to query the call status
    /// </summary>
    [DataContract]
    public class CallStatusResponse : TelekomResponse
    {   
        [DataMember(Name = "stateA")]
        internal String StateAInternal { get; set; }

        /// <summary>
        /// State of the first participant
        /// </summary>
        public ParticipantStatus StateA
        {
            get
            {
                return (ParticipantStatus)Enum.Parse(typeof(ParticipantStatus), StateAInternal, true);
            }
        }

        [DataMember(Name = "stateB")]
        internal String StateBInternal { get; set; }

        /// <summary>
        /// State of the second participant
        /// </summary>
        public ParticipantStatus StateB
        {
            get
            {
                return (ParticipantStatus)Enum.Parse(typeof(ParticipantStatus), StateBInternal, true);
            }
        }

        /// <summary>
        /// Duration of the call with the first participant in seconds.
        /// A value of -1 means that the call has not yet started.
        /// </summary>
        [DataMember(Name = "connectionTimeA")]
        public int ConnectionTimeA { get; set; }

        /// <summary>
        /// Duration of the call with the second participant in seconds.
        /// A value of -1 means that the call has not yet started.
        /// </summary>
        [DataMember(Name = "connectionTimeB")]
        public int ConnectionTimeB { get; set; }

        [DataMember(Name = "reasonA")]
        internal String ReasonAInternal { get; set; }

        /// <summary>
        /// Reason for ending the connection to the first participant
        /// </summary>
        public ParticipantEndReason ReasonA
        {
            get
            {
                return (ParticipantEndReason)int.Parse(ReasonAInternal);
            }
        }

        [DataMember(Name = "reasonB")]
        internal String ReasonBInternal { get; set; }

        /// <summary>
        /// Reason for ending the connection to the second participant
        /// </summary>
        public ParticipantEndReason ReasonB
        {
            get
            {
                return (ParticipantEndReason)int.Parse(ReasonBInternal);
            }
        }

        /// <summary>
        /// Human-Readable description of ReasonA
        /// </summary>
        [DataMember(Name = "descriptionA")]
        public String DescriptionA { get; set; }

        /// <summary>
        /// Human-Readable description of ReasonB
        /// </summary>
        [DataMember(Name = "descriptionB")]
        public String DescriptionB { get; set; }

        /// <summary>
        /// Phone number of the second participant, that was called.
        /// </summary>
        [DataMember(Name = "be164")]
        public String Be164 { get; set; }

        /// <summary>
        /// Index of the phone number of the second participant (B), that was called.
        /// 0 means the first B party phone number was called,
        /// 1 means the second B party phone number was called etc.,
        /// -1 if not connected
        /// </summary>
        [DataMember(Name = "bindex")]
        public int Bindex { get; set; }

    }
}
