using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Storage.Serializer;

namespace hacs
{
	// TODO: GeoHash!

    /// <summary>
    /// holds the values
    /// </summary>
    public class MiataruDataObject : IFastSerialize
    {
		public String AccountName;
        public String DeviceID;
		public String Timestamp;
		public String Latitude;
		public String Longitude;
		public String AccuracyInMeters;

		public MiataruDataObject()
        {
        }

		public MiataruDataObject(String _AccountName, String _DeviceID, String _TimeStamp, String _Latitude, String _Longitude, String _AccuracyInMeters)
        {
			AccountName = _AccountName;
            DeviceID = _DeviceID;
			Timestamp = _TimeStamp;
			Latitude = _Latitude;
			Longitude = _Longitude;
			AccuracyInMeters = _AccuracyInMeters;
        }


        #region IFastSerialize Members
        public byte[] Serialize()
        {
            SerializationWriter writer = new SerializationWriter();

            writer.WriteObject(AccountName);
            writer.WriteObject(DeviceID);
			writer.WriteObject(Timestamp);
			writer.WriteObject(Latitude);
			writer.WriteObject(Longitude);
			writer.WriteObject(AccuracyInMeters);

            return writer.ToArray();
        }

        public void Deserialize(byte[] Data)
        {
            SerializationReader reader = new SerializationReader(Data);
            
			AccountName = (String)reader.ReadObject();
            DeviceID = (String)reader.ReadObject();
			Timestamp = (String)reader.ReadObject();
			Latitude = (String)reader.ReadObject();
			Longitude = (String)reader.ReadObject();
			AccuracyInMeters = (String)reader.ReadObject();
        }

        #endregion
    }
}
