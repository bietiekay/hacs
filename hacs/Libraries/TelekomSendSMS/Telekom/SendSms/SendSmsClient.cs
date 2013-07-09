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

using Telekom.SendSms.Model;
using Telekom.Common.Auth;
using Telekom.Common;
using Telekom.Common.Model;
using Telekom.Common.WebRequest;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

//! Use Send SMS to send text messages to the national and international fixed and mobile networks. A single text message can contain up to 765 characters.
namespace Telekom.SendSms
{
    /// <summary>
    /// Wrapper for Telekom Send SMS service
    /// </summary>
    public class SendSmsClient : TelekomClient
    {
        /// <summary>
        /// URL Path to Send SMS services. Can be overridden if necessary.
        /// {0} is replaced by selected environment
        /// </summary>
        public static string ServicePath = "/plone/sms/rest/{0}/smsmessaging/v1";

        /// <summary>
        /// Constructs a Send SMS API client with specified authentication method and environment.
        /// </summary>
        /// <param name="authentication">Authentication instance</param>
        /// <param name="environment">Environment used for this client's service invocations</param>
        public SendSmsClient(TelekomAuth authentication, ServiceEnvironment environment)
            : base(authentication, environment, ServicePath)
        {
        }

        private TelekomJsonWebRequest<SmsResponse> CreateSendSmsWebRequest(SendSmsRequest request)
        {
            EnsureRequestValid(request);

            string uri = ServiceBaseUrl + "/outbound/{0}/requests";
            uri = string.Format(uri, Uri.EscapeDataString(request.SenderAddress));
            //return CreateAuthenticatedRequest<TelekomResponse>(uri, HttpMethod.POST, request);
            return CreateAuthenticatedJsonRequest<SmsResponse>(uri, request);
        }

        private TelekomJsonWebRequest<SmsResponse> CreateAuthenticatedJsonRequest<ResponseType>(string uri, SendSmsRequest request)
        {
            TelekomJsonWebRequest<SmsResponse> webRequest = CreateAuthenticatedRequest<SmsResponse>(uri, HttpMethod.POST);

            Dictionary<string, Dictionary<string, object>> jsonRequest = new Dictionary<string, Dictionary<string, object>>();

            //---------------------------------------------------------------------------------------------------------------
            Dictionary<string, object> val = new Dictionary<string, object>();

            val.Add("address", request.Address);
            Dictionary<string, object> helper = new Dictionary<string, object>();

            String key = "";
            String key2 = "";
            if (request.SMSType.Equals(OutboundSMSType.TEXT))
            {
                key = "outboundSMSTextMessage";
                key2 = "message";
            }
            else if (request.SMSType.Equals(OutboundSMSType.BINARY))
            {
                key = "outboundSMSBinaryMessage";
                key2 = "message";
            }
            else if (request.SMSType.Equals(OutboundSMSType.FLASH))
            {
                key = "outboundSMSFlashMessage";
                key2 = "flashMessage";
            }
            helper.Add(key2, request.Message);
            val.Add(key, helper);

            val.Add("senderAddress", request.SenderAddress);

            if (request.SenderName != null)
            {
                val.Add("senderName", request.SenderName);
            }
            if (request.Account != null)
            {
                val.Add("account", request.Account);
            }
            helper = new Dictionary<string, object>();
            if (request.CallbackData != null || request.NotifyURL != null)
            {
                if (request.CallbackData != null)
                {
                    helper.Add("callbackData", request.CallbackData);
                }
                if (request.NotifyURL != null)
                {
                    helper.Add("notifyURL", request.NotifyURL);
                }
                val.Add("receiptRequest", helper);
            }

            if (request.Encoding != null)
            {
                if (request.Encoding.Equals(OutboundEncoding.GSM))
                {
                    val.Add("outboundEncoding", "7bitGSM");
                }
                else if (request.Encoding.Equals(OutboundEncoding.UCS))
                {
                    val.Add("outboundEncoding", "16bitUCS2");
                }
            }

            if (request.ClientCorrelator != null)
            {
                val.Add("clientCorrelator", request.ClientCorrelator);
            }

            jsonRequest.Add("outboundSMSMessageRequest", val);

            JsonSerializer serializer = new JsonSerializer();

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, jsonRequest);
            writer.Flush();
            sw.Flush();
            ms.Position = 0;
            ms.Flush();

            webRequest.SetRawContent(ms, "application/json");

            //var sr = new StreamReader(ms);
            //var myStr = sr.ReadToEnd();
            //Console.WriteLine(myStr);
            
            return webRequest;
        }

        /// <summary>Send an SMS</summary>
        /// <param name="request">Parameter object</param> 
        /// <returns>Result of this operation</returns>
        public SmsResponse SendSms(SendSmsRequest request)
        {
            return CreateSendSmsWebRequest(request).Execute();
        }

        /// <summary>Send an SMS (asynchronously)</summary>
        /// <param name="request">Parameter object</param> 
        /// <param name="callback">Handler to invoke after completion</param>
        public void SendSms(SendSmsRequest request, Action<TelekomResponse> callback)
        {
            CreateSendSmsWebRequest(request).ExecuteAsync(a => callback(a));
        }
    }
}
