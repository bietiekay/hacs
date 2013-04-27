using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Storage.Serializer;

namespace hacs.xs1
{
    /// <summary>
    /// holds the possible types of objects that can be accessed through the XS1
    /// </summary>
    public enum ObjectTypes
    {
        Sensor,
        Actor,
        Unknown
    }

    /// <summary>
    /// holds the values
    /// </summary>
    public class XS1_DataObject : IFastSerialize
    {
        public String ServerName;
        public String Name;
        public ObjectTypes Type;
        public String TypeName;
        public DateTime Timecode;
        public Int32 XS1ObjectID;
        public Double Value;
		public String OriginalXS1Statement;

        public XS1_DataObject()
        {
        }

        public XS1_DataObject(String _ServerName, String _Name, ObjectTypes _Type, String _TypeName, DateTime _Timecode, Int32 _XS1ObjectID, Double _Value)
        {
            ServerName = _ServerName;
            Name = _Name;
            Type = _Type;
            TypeName = _TypeName;
            Timecode = _Timecode;
            XS1ObjectID = _XS1ObjectID;
            Value = _Value;
        }

        public XS1_DataObject(String _ServerName, String _Name, ObjectTypes _Type, String _TypeName, DateTime _Timecode, Int32 _XS1ObjectID, Double _Value,String OriginalStatement)
        {
            ServerName = _ServerName;
            Name = _Name;
            Type = _Type;
            TypeName = _TypeName;
            Timecode = _Timecode;
            XS1ObjectID = _XS1ObjectID;
            Value = _Value;
            OriginalXS1Statement = OriginalStatement;
        }

        #region IFastSerialize Members
        public byte[] Serialize()
        {
            SerializationWriter writer = new SerializationWriter();

            writer.WriteObject(ServerName);
            writer.WriteObject(Name);
            if (Type == ObjectTypes.Actor)
                writer.WriteObject((byte)0);
            else
            if (Type == ObjectTypes.Sensor)
                writer.WriteObject((byte)1);
            else
            if (Type == ObjectTypes.Unknown)
                writer.WriteObject((byte)2);

            writer.WriteObject(TypeName);
            writer.WriteObject(Timecode.Ticks);
            writer.WriteObject(XS1ObjectID);
            writer.WriteObject(Value);

            return writer.ToArray();
        }

        public void Deserialize(byte[] Data)
        {
            SerializationReader reader = new SerializationReader(Data);
            
            ServerName = (String)reader.ReadObject();
            Name = (String)reader.ReadObject();
            byte _Type = (byte)reader.ReadObject();

            if (_Type == 0)
                Type = ObjectTypes.Actor;
            else
                if (_Type == 1)
                    Type = ObjectTypes.Sensor;
                else
                    if (_Type == 2)
                        Type = ObjectTypes.Unknown;


            TypeName = (String)reader.ReadObject();
            Timecode = new DateTime((Int64)reader.ReadObject());
            XS1ObjectID = (Int32)reader.ReadObject();
            Value = (Double)reader.ReadObject();
        }

        #endregion
    }
}
