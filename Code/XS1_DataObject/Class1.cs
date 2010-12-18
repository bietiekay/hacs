using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class XS1_DataObject
    {
        public String Name;
        public ObjectTypes Type;
        public String TypeName;
        public DateTime Timecode;
        public Int32 XS1ObjectID;
        public Double Value;

        public XS1_DataObject()
        {
        }

        public XS1_DataObject(String _Name, ObjectTypes _Type, String _TypeName, DateTime _Timecode, Int32 _XS1ObjectID, Double _Value)
        {
            Name = _Name;
            Type = _Type;
            TypeName = _TypeName;
            Timecode = _Timecode;
            XS1ObjectID = _XS1ObjectID;
            Value = _Value;
        }
    }
}
