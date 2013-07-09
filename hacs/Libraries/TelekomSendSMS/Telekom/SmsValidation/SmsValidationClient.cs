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
using System.IO;

using Telekom.Common.Auth;
using Telekom.Common;
using Telekom.Common.Model;
using Telekom.Common.WebRequest;
using Telekom.SmsValidation.Model;

//! Validate phone numbers to use them as originator in SMS or MMS messages
namespace Telekom.SmsValidation
{
    /// <summary>
    /// Wrapper for Telekom SMS Validation service
    /// </summary>
    public class SmsValidationClient : TelekomClient
    {
        /// <summary>
        /// URL Path to SMS Validation services. Can be overridden if neccesary.
        /// {0} is replaced by selected environment
        /// </summary>
        public static string ServicePath = "/plone/odg-sms-validation/rest/{0}";

        /// <summary>
        /// Constructs a SMS Validation API client with specified authentication method and environment.
        /// </summary>
        /// <param name="authentication">Authentication instance</param>
        /// <param name="environment">Environment used for this client's service invocations</param>
        public SmsValidationClient(TelekomAuth authentication, ServiceEnvironment environment)
            : base(authentication, environment, ServicePath)
        {
        }

        #region SendValidationKeyword
        private TelekomJsonWebRequest<TelekomResponse> CreateSendValidationKeywordWebRequest(SendValidationKeywordRequest request)
        {
            EnsureRequestValid(request);

            string uri = ServiceBaseUrl + "/send";
            return CreateAuthenticatedRequest<TelekomResponse>(uri, HttpMethod.POST, request);
        }

        /// <summary>Request to send a validation keyword to a phone number.</summary>
        /// <param name="request">Parameter object</param> 
        /// <returns>Result of this operation</returns>
        public TelekomResponse SendValidationKeyword(SendValidationKeywordRequest request)
        {
            return CreateSendValidationKeywordWebRequest(request).Execute();
        }

        /// <summary>Request to send a validation keyword to a phone number (async).</summary>
        /// <param name="request">Parameter object</param> 
        /// <param name="callback">Handler to invoke after completion</param>
        public void SendValidationKeyword(SendValidationKeywordRequest request, Action<TelekomResponse> callback)
        {
            CreateSendValidationKeywordWebRequest(request).ExecuteAsync(a => callback(a));
        }
        #endregion

        #region Validate
        private TelekomJsonWebRequest<TelekomResponse> CreateValidateWebRequest(ValidateRequest request)
        {
            EnsureRequestValid(request);

            string uri = ServiceBaseUrl
                + string.Format("/validatednumbers/{0}", request.Number);

            return CreateAuthenticatedRequest<TelekomResponse>(uri, HttpMethod.POST, request);
        }

        /// <summary>Return the received validation code to complete phone number validation.</summary>
        /// <param name="request">Parameter object</param> 
        /// <returns>Result of this operation</returns>
        public TelekomResponse Validate(ValidateRequest request)
        {
            return CreateValidateWebRequest(request).Execute();
        }

        /// <summary>Return the received validation code to complete phone number validation (async).</summary>
        /// <param name="request">Parameter object</param> 
        /// <param name="callback">Handler to invoke after completion</param>
        public void Validate(ValidateRequest request, Action<TelekomResponse> callback)
        {
            CreateValidateWebRequest(request).ExecuteAsync(a => callback(a));
        }
        #endregion

        #region Invalidate
        private TelekomJsonWebRequest<TelekomResponse> CreateInvalidateWebRequest(string number)
        {
            if (string.IsNullOrEmpty(number))
                throw new ArgumentNullException("number");

            string uri = ServiceBaseUrl
                + string.Format("/validatednumbers/{0}", number);

            return CreateAuthenticatedRequest<TelekomResponse>(uri, HttpMethod.DELETE);
        }

        /// <summary>Revoke a number's validation state</summary>
        /// <param name="number">Number to invalidate</param> 
        /// <returns>Result of this operation</returns>
        public TelekomResponse Invalidate(string number)
        {
            return CreateInvalidateWebRequest(number).Execute();
        }

        /// <summary>Revoke a number's validation state (async)</summary>
        /// <param name="number">Number to invalidate</param> 
        /// <param name="callback">Handler to invoke after completion</param>
        public void Invalidate(string number, Action<TelekomResponse> callback)
        {
            CreateInvalidateWebRequest(number).ExecuteAsync(a => callback(a));
        }
        #endregion

        #region GetValidatedNumbers
        private TelekomJsonWebRequest<GetValidatedNumbersResponse> CreateGetValidatedNumbersRequest()
        {
            string uri = ServiceBaseUrl + "/validatednumbers";

            return CreateAuthenticatedRequest<GetValidatedNumbersResponse>(uri, HttpMethod.GET);
        }

        /// <summary>Retrieve a list of validated numbers</summary>
        /// <returns>Result of this operation</returns>
        public GetValidatedNumbersResponse GetValidatedNumbers()
        {
            return CreateGetValidatedNumbersRequest().Execute();
        }

        /// <summary>Retrieve a list of validated numbers (async)</summary>
        /// <param name="callback">Handler to invoke after completion</param>
        public void GetValidatedNumbers(Action<GetValidatedNumbersResponse> callback)
        {
            CreateGetValidatedNumbersRequest().ExecuteAsync(a => callback(a));
        }
        #endregion

    }
}
