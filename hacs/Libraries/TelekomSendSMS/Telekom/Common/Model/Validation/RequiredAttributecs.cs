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

namespace Telekom.Common.Model.Validation
{
    /// <summary>
    /// Defines a property as required, i.e. the property must not be NULL.
    /// Empty strings ("") are not allowed either.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    internal class RequiredAttribute : ValidationAttribute
    {
        internal override void EnforceValid(object value, string propertyName)
        {
            // Strings must not be null or ""
            if (value is string)
            {
                if (string.IsNullOrEmpty(value as string))
                {
                    throw new ArgumentNullException("Parameter " + propertyName + " is required");
                }
            }
            else if (value == null)
            {
                throw new ArgumentNullException("Parameter " + propertyName + " is required");
            }          
        }

    }
}
