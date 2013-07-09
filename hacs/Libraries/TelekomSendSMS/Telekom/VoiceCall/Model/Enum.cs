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

namespace Telekom.VoiceCall.Model
{
    /// <summary>
    /// State of a participant of a voice call
    /// </summary>
    public enum ParticipantStatus
    {
        /// <summary>
        /// Idle
        /// </summary>
        Idle,
        /// <summary>
        /// Connecting to participant
        /// </summary>
        Connecting,
        /// <summary>
        /// Participant's phone is ringing
        /// </summary>
        Ringing,
        /// <summary>
        /// Connected to participant
        /// </summary>
        Connected,
        /// <summary>
        /// Currently disconnecting the participant
        /// </summary>
        Disconnecting,
        /// <summary>
        /// The participant is disconnected
        /// </summary>
        Disconnected
    }

    /// <summary>
    /// Reasons for ending of connection
    /// </summary>
    public enum ParticipantEndReason : int
    {
        /// <summary>
        /// Call not yet ended.
        /// </summary>
        NotEndedYet = 0,
        /// <summary>
        /// The participant is engaged.
        /// </summary>
        Engaged = 1,
        /// <summary>
        /// The participiant has rejected the call
        /// </summary>
        Rejected = 2,
        /// <summary>
        /// The participant could not be reached or has not accepted the call.
        /// </summary>
        NotAvailable = 3,
        /// <summary>
        /// The participant could not be reached due to barriers.
        /// </summary>
        Barriers = 4,
        /// <summary>
        /// An internal error has occurred
        /// </summary>
        InternalError = 99
    }
}
