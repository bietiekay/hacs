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

namespace Telekom.Common.Model
{
    internal static class JsonExtensions
    {
        public static string ToJsonString(this DateTime? dateTime)
        {
            if (dateTime.HasValue) {
                DateTime value = dateTime.Value;

                // Service expecteds times in UTC, so convert if not already done
                if (value.Kind == DateTimeKind.Local)
                    value = value.ToUniversalTime();

                return value.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z";
            }
            else {
                return null;
            }
        }

        public static DateTime? ToNullableDateTime(this string json)
        {
            return string.IsNullOrEmpty(json) ? null : (DateTime?)DateTime.Parse(json);
        }
    }
}
