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
    internal abstract class HttpBodyParamWriter : IDisposable
    {
        protected StreamWriter writer;

        public HttpBodyParamWriter(Stream stream)
        {
            writer = new StreamWriter(stream);
        }

        public abstract void WriteParam(string name, string value);

        public abstract void WriteFile(string name, Stream value);

        public abstract string GetContentType();

        public virtual void Dispose()
        {
            writer.Flush();
            writer.Dispose();
        }
    }
}
