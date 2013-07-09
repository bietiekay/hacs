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
using System.Net;
using System.IO;
using Telekom.Common.Model;
using Telekom.Common.WebRequest.HttpParamWriter;
using System.Reflection;
using System.Runtime.Serialization;

namespace Telekom.Common.WebRequest
{
    internal class TelekomWebRequest
    {
        protected HttpWebRequest request;
        protected Dictionary<string, string> bodyParameter;
        protected Dictionary<string, Stream> attachedFiles;
        protected UriParameterBuilder uriParamBuilder;

        protected Dictionary<string, string> additionalHeaders;

        private Stream rawContent = null;
        private string rawContentType;

        public TelekomWebRequest(string uri, HttpMethod method)
        {
            Uri = uri;
            Method = method;

            UserAgent = TelekomConfig.UserAgent;
            Accept = "application/json";
            AuthHeader = null;
            Credentials = null;

            bodyParameter = new Dictionary<string, string>();
            attachedFiles = new Dictionary<string, Stream>();
            uriParamBuilder = new UriParameterBuilder();
        }

        protected string Uri { get; set; }

        protected HttpMethod Method { get; set; }

        public string UserAgent { get; set; }

        public string Accept { get; set; }

        public string AuthHeader { get; set; }

        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Adds a parameter to the request
        /// </summary>
        /// <param name="key">name of the parameter</param>
        /// <param name="value">value of the parameter</param>
        public void AddParameter(string key, string value)
        {
            if (Method == HttpMethod.GET)
            {
                uriParamBuilder.WriteParam(key, value);
            }
            else
            {
                bodyParameter.Add(key, value);
            }
        }

        public void AddParameter(string key, Stream stream)
        {
            if (Method == HttpMethod.GET)
                throw new InvalidOperationException("Cannot add files to GET request");

            attachedFiles.Add(key, stream);
        }

        /// <summary>
        /// Set raw content
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="contentType"></param>
        public void SetRawContent(Stream rawData, string contentType)
        {
            this.rawContent = rawData;
            this.rawContentType = contentType;
        }

        internal WebResponse ExecuteRaw()
        {
#if !WINDOWS_PHONE
            PrepareRequest();

            if (HasRequestBody())
            {
                Stream requestStream = request.GetRequestStream();
                WritePayload(requestStream);
            }
            try
            {
                return request.GetResponse();
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                // HTTP errors such as 404 or 403 throw an exception,
                // but actually could contain a valid response
                return e.Response;
            }
#else
            throw new InvalidOperationException("Synchronous calls not supported on Windows Phone");
#endif
        }

        internal WebResponse ExecuteRaw(Boolean custom)
        {
#if !WINDOWS_PHONE
            PrepareRequest();

            if (HasRequestBody())
            {
                if (custom)
                {
                    foreach (KeyValuePair<string, string> pair in additionalHeaders)
                    {
                        request.Headers.Add(pair.Key, pair.Value);
                    }
                }
                Stream requestStream = request.GetRequestStream();
                WritePayload(requestStream);
            }
            try
            {
                return request.GetResponse();
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                // HTTP errors such as 404 or 403 throw an exception,
                // but actually could contain a valid response
                return e.Response;
            }
#else
            throw new InvalidOperationException("Synchronous calls not supported on Windows Phone");
#endif
        }

        internal void ExecuteRawAsync(Action<WebResponse> callback)
        {           
            PrepareRequest();
            if (HasRequestBody())
            {
                request.BeginGetRequestStream(delegate(IAsyncResult streamResult)
                {
                    Stream requestStream = request.EndGetRequestStream(streamResult);
                    WritePayload(requestStream);

                    GetResponseAsync(callback);
                }, null);
            }
            else {
                GetResponseAsync(callback);
            }
        }

        private void GetResponseAsync(Action<WebResponse> callback)
        {
            request.BeginGetResponse(delegate(IAsyncResult responseResult)
            {
                WebResponse response;
                try
                {
                    response = request.EndGetResponse(responseResult);                    
                }
                catch (WebException e)
                {
                    // HTTP errors such as 404 or 403 throw an exception,
                    // but actually could contain a valid response
                    response = e.Response;
                }
                catch (Exception)
                {
                    response = null;
                }
                // Make sure that callback is called, even if we didn't have success
                callback(response);
            }, null);
        }

        private bool HasRequestBody()
        {
            return (rawContent != null) || (bodyParameter.Count > 0) || (attachedFiles.Count > 0);
        }
        
        private void PrepareRequest()
        {
            string uriWithParameters = Uri + uriParamBuilder.ToString();

            request = (HttpWebRequest)HttpWebRequest.Create(uriWithParameters);
            request.Method = System.Enum.GetName(typeof(HttpMethod), Method);            
            request.UserAgent = UserAgent;
            request.Accept = Accept;
            request.Credentials = Credentials;
            // Overwrite system proxy settings, if applicable
            if (TelekomConfig.Proxy != null)
            {
                request.Proxy = TelekomConfig.Proxy;
            }
            if (AuthHeader != null)
            {
                request.Headers[HttpRequestHeader.Authorization] = AuthHeader;
            }
        }

        private void WritePayload(Stream stream)
        {
            if (rawContent != null)
            {
                request.ContentType = rawContentType;

                //set stream position to 0 for reading from it.
                rawContent.Position = 0;

                // Copy stream
                byte[] buffer = new byte[32768];
                int read;
                while ((read = rawContent.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, read);
                }
                stream.Close();
            }            
            else if (attachedFiles.Count > 0)
            {
                // If files have to be atteched, use Form Multipart format
                using (var paramWriter = new FormMultipartWriter(stream))
                {
                    WriteBodyParamter(paramWriter);
                }
            }
            else
            {
                using (var paramWriter = new RequestStringWriter(stream))
                {
                    WriteBodyParamter(paramWriter);
                }
            }
        }

        private void WriteBodyParamter(HttpBodyParamWriter paramWriter)
        {
            request.ContentType = paramWriter.GetContentType();
            foreach (KeyValuePair<string, string> entry in bodyParameter)
            {
                paramWriter.WriteParam(entry.Key, entry.Value);
            }
            foreach (KeyValuePair<string, Stream> entry in attachedFiles)
            {
                paramWriter.WriteFile(entry.Key, entry.Value);
            }
        }

    }
}
