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
using System.IO;
using sones.Storage.Serializer;
using sones.Storage;

namespace sones.storage
{
    public class TinyOnDiskStorage
    {
        private FileStream DatabaseFile;
        private FileStream DatabaseIndexFile;

        public List<OnDiscAdress> InMemoryIndex = null;

        public TinyOnDiskStorage(String DatabaseFilename, bool createNew)
        {
            if (createNew)
            {
                DatabaseFile = new FileStream(DatabaseFilename + ".data", FileMode.CreateNew);
                DatabaseIndexFile = new FileStream(DatabaseFilename + ".idx", FileMode.CreateNew);
                InMemoryIndex = new List<OnDiscAdress>();
            }
            else
            {
                if (File.Exists(DatabaseFilename + ".data"))
                {
                    // open the file
                    DatabaseFile = new FileStream(DatabaseFilename + ".data", FileMode.Open);
                    DatabaseIndexFile = new FileStream(DatabaseFilename + ".idx", FileMode.Open);

                    // read everything into the index...
                    ReadCompleteIndexFromDiskIntoMemory();
                }
                else
                {
                    DatabaseFile = new FileStream(DatabaseFilename + ".data", FileMode.CreateNew);
                    DatabaseIndexFile = new FileStream(DatabaseFilename + ".idx", FileMode.CreateNew);
                    InMemoryIndex = new List<OnDiscAdress>();
                }
            }
        }

        public void Shutdown()
        {
            DatabaseFile.Flush();
            DatabaseIndexFile.Flush();
            DatabaseFile.Close();
            DatabaseIndexFile.Close();
            InMemoryIndex = null;
        }

        public void Write(byte[] Data)
        {
            lock (DatabaseFile)
            {
                OnDiscAdress adress = WriteToDatabase(Data);

                WriteToIndex(adress);
            }
        }

        public byte[] Read(OnDiscAdress Adress)
        {
            byte[] Readin;

            lock (DatabaseFile)
            {
                // seek to the position
                DatabaseFile.Seek(Adress.Start, SeekOrigin.Begin);
                Readin = new byte[Adress.End - Adress.Start];
                // read it in
                DatabaseFile.Read(Readin, 0, Readin.Length);
            }
            return Readin;
        }

        private void WriteToIndex(OnDiscAdress Adresspattern)
        {
            lock (DatabaseIndexFile)
            {
                // seek to the end...
                DatabaseIndexFile.Seek(DatabaseIndexFile.Length, SeekOrigin.Begin);

                byte[] ToWrite = Adresspattern.Serialize();

                DatabaseIndexFile.Write(ToWrite, 0, ToWrite.Length);

                DatabaseIndexFile.Flush();
            }
        }

        public void ReadCompleteIndexFromDiskIntoMemory()
        {
            lock (DatabaseIndexFile)
            {
                DatabaseIndexFile.Seek(0, SeekOrigin.Begin);

                long currentPosition = 0;

                byte[] _SerializedData;
                OnDiscAdress _deserializedAdress;
                InMemoryIndex = new List<OnDiscAdress>();

                try
                {
                    while (currentPosition != DatabaseIndexFile.Length)
                    {
                        // read through the index...
                        // one OnDiskAdress is 33 bytes
                        _SerializedData = new byte[33];

                        DatabaseIndexFile.Read(_SerializedData, 0, 33);
                        _deserializedAdress = new OnDiscAdress();
                        _deserializedAdress.Deserialize(_SerializedData);

                        InMemoryIndex.Add(_deserializedAdress);
                        currentPosition = currentPosition + 33;

                        /// HACK
                        //if (InMemoryIndex.Count > 50000)
                        //    break;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void FlushIndexInMemory()
        {
            InMemoryIndex.Clear();
            InMemoryIndex = null;
        }

        private OnDiscAdress WriteToDatabase(byte[] ToWrite)
        {
            OnDiscAdress OutputAdress = new OnDiscAdress();

            lock (DatabaseFile)
            {
                // seek to the end...
                DatabaseFile.Seek(DatabaseFile.Length, SeekOrigin.Begin);
                OutputAdress.Start = DatabaseFile.Position;
                DatabaseFile.Write(ToWrite, 0, ToWrite.Length);
                OutputAdress.End = DatabaseFile.Position;

                DatabaseFile.Flush();
            }

            return OutputAdress;
        }


    }
}
