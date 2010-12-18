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

/* IFastSerialize
 * (c) Daniel Kirstenpfad, 2007-2008
 * (c) Achim Friedland, 2008
 * 
 * This interface has to be implemented by any object that should
 * be serialized by the new FastSerializer.
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 *      Achim Friedland
 * 
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace PandoraDatabase.Storage.Serializer
{

    /// <summary>
    /// This interface has to be implemented by any object that should
    /// be serialized by the new FastSerializer.
    /// </summary>
    public interface IFastSerialize
    {
        /// <summary>
        /// This method serializes the implementing object
        /// </summary>
        /// <returns>the serialized data as byte array</returns>
        byte[] Serialize();

        /// <summary>
        /// This method deserializes the given data into the object that implements this interface
        /// </summary>
        void Deserialize(byte[] Data);

    }

}
