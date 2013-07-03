/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Serialization;

namespace sones.Storage
{
    public class OnDiscAdress : IFastSerialize
    {
        public Int64 CreationTime;
        public Int64 Start;
        public Int64 End;
		public object cachedDeserializedObject;

		public object CachedDeserializedObject 
		{
			get 
			{
				return cachedDeserializedObject;
			}
			set 
			{
				cachedDeserializedObject = value;
			}
		}

        public OnDiscAdress()
        {
            CreationTime = DateTime.Now.Ticks;
        }

        #region IFastSerialize Members
        public byte[] Serialize()
        {
            SerializationWriter writer = new SerializationWriter();

            writer.WriteObject(CreationTime);
            writer.WriteObject(Start);
            writer.WriteObject(End);

            // align it...
            byte[] Output = new byte[33];
            writer.ToArray().CopyTo(Output, 0);

            return Output;
        }

        public void Deserialize(byte[] Data)
        {
            SerializationReader reader = new SerializationReader(Data);

            CreationTime = (Int64)reader.ReadObject();
            Start = (Int64)reader.ReadObject();
            End = (Int64)reader.ReadObject();
        }

        #endregion
    }
}
