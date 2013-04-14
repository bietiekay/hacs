using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Storage.Serializer;
using xs1_data_logging;

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
    public class XS1_DataObject : IFastSerialize,IAlarmingEvent
    {
        public String ServerName;
        public String Name;
        public ObjectTypes Type;
        public String TypeName;
        public DateTime Timecode;
        public Int32 XS1ObjectID;
        public Double Value;
		public String OriginalXS1Statement;
		private DateTime Creation;
		public Boolean IgnoreForAlarming;

        public XS1_DataObject()
        {
        }

		public XS1_DataObject(String _ServerName, String _Name, ObjectTypes _Type, String _TypeName, DateTime _Timecode, Int32 _XS1ObjectID, Double _Value, Boolean _IgnoreForAlarming = false)
        {
            ServerName = _ServerName;
            Name = _Name;
            Type = _Type;
            TypeName = _TypeName;
            Timecode = _Timecode;
            XS1ObjectID = _XS1ObjectID;
            Value = _Value;
			IgnoreForAlarming = _IgnoreForAlarming;
        }

        public XS1_DataObject(String _ServerName, String _Name, ObjectTypes _Type, String _TypeName, DateTime _Timecode, Int32 _XS1ObjectID, Double _Value,String OriginalStatement, Boolean _IgnoreForAlarming = false)
        {
            ServerName = _ServerName;
            Name = _Name;
            Type = _Type;
            TypeName = _TypeName;
            Timecode = _Timecode;
            XS1ObjectID = _XS1ObjectID;
            Value = _Value;
            OriginalXS1Statement = OriginalStatement;
			Creation = DateTime.Now;
			IgnoreForAlarming = _IgnoreForAlarming;
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

		#region IAlarmingEvent implementation

		public AlarmingEventType AlarmingType ()
		{
			return AlarmingEventType.XS1Event;
		}

		public string AlarmingName ()
		{
			return Name;
		}

		public DateTime AlarmingCreated ()
		{
			return Creation;
		}

		#endregion
    }
}
