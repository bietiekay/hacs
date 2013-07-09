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

namespace Telekom.SmsValidation.Model
{
    /// <summary>
    /// Result of a call to list the validated numbers
    /// </summary>
    [DataContract]
    public class GetValidatedNumbersResponse : TelekomResponse
    {
        /// <summary>
        /// List of validated numbers
        /// </summary>
        [DataMember(Name = "numbers")]
        public List<ValidatedNumber> Numbers { get; set; }
    }
}
