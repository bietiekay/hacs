using System;
using System.Collections.Generic;
using sones.Storage;
using hacs.xs1;
using sones.storage;

namespace xs1_data_logging
{
	/// <summary>
	/// Data cache - this class holds a configureable sized cache for deserialized and.
	/// </summary>
	public class DataCache
	{
		private Dictionary<long,XS1_DataObject> Cache;
		private List<long> HouseKeepingList;
		private long MaximumNoOfCacheItems;
		private TinyOnDiskStorage sensor_data;

		public DataCache (long MaximumNumberOfCacheItems, TinyOnDiskStorage storage)
		{
			HouseKeepingList = new List<long>();
			Cache = new Dictionary<long, XS1_DataObject>();
			MaximumNoOfCacheItems = MaximumNumberOfCacheItems;
			sensor_data = storage;
		}

		private void AddToCache(OnDiscAdress adress,XS1_DataObject data)
		{
            //Console.WriteLine("Adding to cache "+data.Name+" "+adress.End+" Cache: "+HouseKeepingList.Count);

            lock(Cache)
			{
				// it's already in here
				//if (Cache.ContainsKey(adress.End))
				//	return;
				Cache.Add(adress.End,data);
				HouseKeepingList.Add(adress.End);

				if (HouseKeepingList.Count > MaximumNoOfCacheItems)
				{
                    //Console.WriteLine("Removing from Cache");
					// remove from cache
					Cache.Remove(HouseKeepingList[0]);
					// remove from housekeepinglist
					HouseKeepingList.RemoveAt(0);
				}
			}
		}

		public XS1_DataObject ReadFromCache(OnDiscAdress adress)
		{
			lock(Cache)
			{
				if (!Cache.ContainsKey(adress.End))
				{
                    XS1_DataObject dataobject = new XS1_DataObject();
					dataobject.Deserialize(sensor_data.Read(adress));
                    //Console.WriteLine("Read from Disk " + dataobject.Name + " " + adress.End);
					// add to cache
					AddToCache(adress,dataobject);
					return dataobject;
				}
				else
                {
                    XS1_DataObject dataobject = Cache[adress.End];
                    //Console.WriteLine("Read from Cache " + dataobject.Name + " " + adress.End);
					return dataobject;
                }
			}
            return null;
		}
	}
}

