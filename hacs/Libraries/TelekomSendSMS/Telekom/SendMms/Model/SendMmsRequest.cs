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
using Telekom.Common.Model.Validation;
using System.IO;

namespace Telekom.SendMms.Model
{
    /// <summary>
    /// Parameters to send an MMS
    /// </summary>
    public class SendMmsRequest : TelekomRequest
    {
        /// <summary>
        /// Comma separated phone numbers as expected by the web service
        /// </summary>
        /// <returns></returns>
        [Required]
        internal string Number { get; set; }

        /// <summary>
        /// Phone Number(s) of SMS receiver(s)
        /// </summary>
        [NoHttpParameter]
        public ICollection<string> Numbers
        {
            get
            {
                return (Number != null) ? Number.Split(',') : null;
            }
            set
            {
                Number = (value.Count > 0) ? string.Join(",", value.ToArray()) : null;
            }
        }

        /// <summary>
        /// Subject of the MMS
        /// </summary>
        [Required]
        public String Subject { get; set; }

        /// <summary>
        /// Message to send
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Base64 encoded attachment actually sent
        /// </summary>
        internal String Attachment { get; set; }

        /// <summary>
        /// Set the attachment data as a stream
        /// </summary>
        [NoHttpParameter]
        public Stream AttachmentAsStream
        {
            set
            {
                // Stream -> byte array
                byte[] buff = new byte[value.Length];
                value.Read(buff, 0, buff.Length);
                AttachmentAsBytes = buff;
            }
        }

        /// <summary>
        /// Set the attachment data as an array of bytes
        /// </summary>
        [NoHttpParameter]
        public byte[] AttachmentAsBytes
        {
            set
            {
                // byte array -> base64 string
                Attachment = Convert.ToBase64String(value);
            }
        }

        /// <summary>
        /// Filename of the attachment
        /// </summary>
        public String Filename { get; set; }

        /// <summary>
        /// Content-Type of the attachment
        /// </summary>
        public String ContentType { get; set; }

        /// <summary>
        /// Sender, as shown at the receiver
        /// </summary>
        public String Originator { get; set; }

        /// <summary>
        /// Account-ID of the sub account which should be billed for this service call
        /// </summary>
        public String Account { get; set; }


        internal override void EnforceRequiredFields()
        {
            base.EnforceRequiredFields();

            if ((Message == null) && (Attachment == null))
                throw new ArgumentException("Either Message or Attachment must be specified");

            if ((Attachment != null) && (string.IsNullOrEmpty(ContentType) || string.IsNullOrEmpty(Filename)))
                throw new ArgumentException("Sending an attachment requires specifying a ContentType and a Filename");
        }
    }
}
