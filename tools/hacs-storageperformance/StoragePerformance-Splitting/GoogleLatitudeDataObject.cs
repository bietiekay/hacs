using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Serialization;

namespace xs1_data_logging
{
	// TODO: GeoHash!

    /// <summary>
    /// holds the values
    /// </summary>
    public class GoogleLatitudeDataObject : IFastSerialize
    {
		public String AccountName;
		public String LatitudeID;
		public Int64 Timecode;
		public String reverseGeocode;
		public Double Latitude;
		public Double Longitude;
		public Int32 AccuracyInMeters;

		public GoogleLatitudeDataObject()
        {
        }

		public GoogleLatitudeDataObject(String _AccountName, String _LatitudeID, Int64 _TimeCode, String _reverseGeocode, Double _Latitude, Double _Longitude, Int32 _AccuracyInMeters)
        {
			AccountName = _AccountName;
			LatitudeID = _LatitudeID;
			Timecode = _TimeCode;
			reverseGeocode = _reverseGeocode;
			Latitude = _Latitude;
			Longitude = _Longitude;
			AccuracyInMeters = _AccuracyInMeters;
        }


        #region IFastSerialize Members
        public byte[] Serialize()
        {
            SerializationWriter writer = new SerializationWriter();

            writer.WriteObject(AccountName);
			writer.WriteObject(LatitudeID);
            writer.WriteObject(Timecode);
			writer.WriteObject(reverseGeocode);
			writer.WriteObject(Latitude);
			writer.WriteObject(Longitude);
			writer.WriteObject(AccuracyInMeters);

            return writer.ToArray();
        }

        public void Deserialize(byte[] Data)
        {
            SerializationReader reader = new SerializationReader(Data);
            
			AccountName = (String)reader.ReadObject();
			LatitudeID = (String)reader.ReadObject();
			Timecode = (Int64)reader.ReadObject();
			reverseGeocode = (String)reader.ReadObject();
			Latitude = (Double)reader.ReadObject();
			Longitude = (Double)reader.ReadObject();
			AccuracyInMeters = (Int32)reader.ReadObject();

        }

        #endregion
    }
}
