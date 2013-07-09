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
using System.Reflection;
using Telekom.Common.Model.Validation;
using System.Runtime.Serialization;
using Telekom.Common.WebRequest;
using System.IO;

namespace Telekom.Common.Model
{
    /// <summary>
    /// Base class for all request objects
    /// </summary>
    [DataContract]
    public abstract class TelekomRequest
    {
        /// <summary>
        /// Uses reflection to infer parameters from the request object and
        /// adds them to a web request
        /// </summary>
        /// <param name="webRequest">web request to receive parameters</param>
        internal void BuildRequestParameters(TelekomWebRequest webRequest)
        {
            PropertyInfo[] properties = this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                HttpParameterAttribute httpParamAttribute = property
                    .GetCustomAttributes(typeof(HttpParameterAttribute), false)
                    .SingleOrDefault() as HttpParameterAttribute;

                NoHttpParameterAttribute noHttpParamAttribute = property
                    .GetCustomAttributes(typeof(NoHttpParameterAttribute), false)
                    .SingleOrDefault() as NoHttpParameterAttribute;

                // Check for opt-out serialization attribute
                if (noHttpParamAttribute == null)
                {
                    string parameterName;
                    // Name overwritten?
                    if (httpParamAttribute != null)
                    {
                        parameterName = httpParamAttribute.Name;
                    }
                    else
                    {
                        // Infer parameter name from property and change the
                        // first letter to lower-case: OwnerId -> ownerId
                        parameterName = property.Name[0].ToString().ToLower()
                            + property.Name.Substring(1);
                    }
                    object value = property.GetValue(this, null);

                    // Only add defined values
                    if (value != null)
                    {
                        // Determin actual type to allow specific serialization
                        Type actualType = Nullable.GetUnderlyingType(property.PropertyType);
                        if (actualType == null)
                            actualType = property.PropertyType;

                        // Replace enum value if EnumMember attribute is specified
                        if (actualType.IsEnum)
                        {
                            FieldInfo enumFieldInfo = actualType.GetField(value.ToString());
                            EnumMemberAttribute enumMemberAttrib =
                                enumFieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                                .SingleOrDefault()
                                as EnumMemberAttribute;

                            // Enum value overwritten with EnumMember attribute?
                            if (enumMemberAttrib != null)
                            {
                                value = enumMemberAttrib.Value;
                            }
                            else
                            {
                                value = (int)enumFieldInfo.GetValue(property);
                            }
                        }

                        // Convert bool to string
                        if (actualType.Equals(typeof(bool)))
                        {
                            value = (bool)value ? "true" : "false";
                        }

                        // Convert DateTime to the expected format
                        if (actualType.Equals(typeof(DateTime)))
                        {
                            // Note: Since the service returns times in UTC that is converted to local time
                            // by .NET, we automatically convert it the other way round
                            value = ((DateTime)value)
                                .ToUniversalTime()
                                .ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                            // Add UTC suffix
                            value += "Z";
                        }

                        if (actualType.Equals(typeof(Stream)))
                        {
                            webRequest.AddParameter(parameterName, (Stream)value);
                        }
                        else
                        {
                            webRequest.AddParameter(parameterName, value.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks all validation properties and throws an exception
        /// if there is a problem
        /// </summary>
        internal virtual void EnforceRequiredFields()
        {
            PropertyInfo[] properties = this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                object[] validationAttributes = property.GetCustomAttributes(typeof(ValidationAttribute), false);
                if (validationAttributes != null)
                {
                    for (int i = 0; i < validationAttributes.GetLength(0); i++)
                    {
                        ValidationAttribute rule = validationAttributes[i] as ValidationAttribute;
                        // Rule throws an exception if not valid
                        rule.EnforceValid(property.GetValue(this, null), property.Name);
                    }
                }
            }
        }
    }
}
