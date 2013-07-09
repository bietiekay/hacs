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

namespace Telekom.Common
{
    internal class UriHelper
    {
        /// <summary>
        /// Escape an URI string
        /// </summary>
        /// <param name="uriPart">string to encode</param>
        /// <returns>encoded string</returns>
        public static String EscapeUriString(String uriPart)
        {
            return Uri.EscapeUriString(uriPart);
        }

        private static bool IsUnreservedChar(byte b)
        {
            // RFC 3986
            // unreserved  = ALPHA / DIGIT / "-" / "." / "_" / "~"
            return ((b >= 'a' && b <= 'z') || (b >= 'A' && b <= 'Z') ||
                (b >= '0' && b <= '9') || (b == '-') || (b == '.') || (b == '_') || (b == '~'));
        }

        private static String EscapeDataChar(byte b)
        {
            return String.Format("%{0:X2}", b);
        }

        /// <summary>
        /// Escape an URI data string
        /// .NET Framework's Uri.EscapeDataString cannot be used because of it's length limitation
        /// </summary>
        /// <param name="uriPart">string to encode</param>
        /// <returns>encoded string</returns>
        public static String EscapeDataString(String uriPart)
        {
            StringBuilder result = new StringBuilder();

            byte[] partAsBytes = Encoding.UTF8.GetBytes(uriPart);
            foreach (byte b in partAsBytes) {
                if (IsUnreservedChar(b)) {
                    result.Append((char)b);
                }
                else {
                    result.Append(EscapeDataChar(b));
                }
            }

            return result.ToString();
        }

    }
}
