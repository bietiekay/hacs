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

namespace Telekom.Common.WebRequest.HttpParamWriter
{
    /// <summary>
    /// This class encodes the request body as a HTTP multi-part attachment
    /// </summary>
    internal class FormMultipartWriter : HttpBodyParamWriter
    {
        private string multipartBoundary;

        internal FormMultipartWriter(Stream stream)
            : base(stream)
        {
            multipartBoundary = "----" + Guid.NewGuid().ToString(); 
        }

        public override void WriteParam(string name, string value)
        {
            if (value != null)
            {
                writer.WriteLine("--" + multipartBoundary);
                writer.WriteLine("Content-Disposition: form-data; name=\"" + name + "\"");
                writer.WriteLine("Content-Type: text/plain; charset=UTF-8");
                writer.WriteLine("Content-Transfer-Encoding: 8bit");
                writer.WriteLine();
                writer.WriteLine(value);
            }
        }


        public override void WriteFile(string name, Stream value)
        {
            writer.WriteLine("--" + multipartBoundary);
            writer.WriteLine("Content-Disposition: form-data; name=\"" + name + "\"");
            writer.WriteLine("Content-Type: application/octet-stream; charset=ISO-8859-1");
            writer.WriteLine("Content-Transfer-Encoding: binary");
            writer.WriteLine();
            writer.Flush();

            // Dump whole file
            byte[] buffer = new byte[32768];
            int read;
            while ((read = value.Read(buffer, 0, buffer.Length)) > 0)
            {
                writer.BaseStream.Write(buffer, 0, read);
            }
            writer.WriteLine();
        }

        public override string GetContentType()
        {
            return "multipart/form-data; boundary=" + multipartBoundary;
        }

        public override void Dispose()
        {
            writer.WriteLine("--" + multipartBoundary + "--");
            base.Dispose();
        }

    }
}
